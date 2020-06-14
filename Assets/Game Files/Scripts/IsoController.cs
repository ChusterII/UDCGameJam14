using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using MEC;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;


public class IsoController : MonoBehaviour
{
    [Header("Player Movement")]
    public float moveSpeed;
    public float characterHeight = 0.75f;
    
    [Header("Fireball Settings")]
    public float fireballSpeed;
    public GameObject fireballController;

    [Header("Floating Text")]
    public GameObject floatingTextObject;
    
    // --------------------------- Private variables start here ---------------------------
    private Vector3 _forward, _right; // Different from world axis forward and right because of camera
    private Camera _camera;
    private NavMeshAgent _agent;
    private Vector3 _velocity;
    private Vector3 _mousePosition;
    [HideInInspector]
    public bool movementEnabled = true;
    [HideInInspector]
    public bool mouseLookEnabled = true;
    public bool inputDisabled;
    private bool _isTeleporting;
    private bool _moveFireball;
    private bool _continueCoroutine;
    private ObjectPooler _objectPooler;
    private int _currentNavMeshLayer;
    private GameManager _gameManager;
    private PlayerController _playerController;
    private Animator _animator;
    private Vector2 _movement;
    private Rigidbody _rigidBody;
    private Quaternion _rotation;
    private GameObject _fireball;
    private TextMeshPro _floatingText;
    private AudioSource _audioSource;
    


    // Start is called before the first frame update
    void Start()
    {
      InitializeComponents();
      InitializeForwardVector();
      InitializeRightVector();
      InitializeParticles();

      Timing.RunCoroutine(_FireballController().CancelWith(gameObject));
    }

    // Update is called once per frame
    void Update()
    {
        MouseLook();
        ShootFireball();
        
        // Keeps the quad from turning
        transform.rotation = _rotation;
        
        // Keeps the fireballController with the player
        fireballController.transform.position = transform.position;
    }

    private void FixedUpdate()
    {
        Move();
    }

    
    // --------------------------- User-made methods start here ---------------------------
    private void Move()
    {
        if (movementEnabled)
        {
            // Store input values in a temporary Vector2
            _movement.x = Input.GetAxisRaw("Horizontal");
            _movement.y = Input.GetAxisRaw("Vertical");
            
            // Call the animator to play the respective animations based on the Blend Tree values
            _animator.SetFloat("Horizontal", _movement.x);
            _animator.SetFloat("Vertical", _movement.y);
            _animator.SetFloat("Speed", _movement.sqrMagnitude);
            
           // new forward and right movement directions since the camera is tilted in an angle.
            Vector3 horizontalMovement = _right * _movement.x;
            Vector3 verticalMovement = _forward * _movement.y;
            
            // Calculate our heading
            Vector3 heading = Vector3.Normalize(horizontalMovement + verticalMovement);

            // If we're standing still, don't move (DERP!)
            if (!heading.Equals(Vector3.zero))
            {   
                // If we're moving, point in the right direction!
                transform.forward = heading;
            }
            
            // Move towards heading using speed
            _rigidBody.MovePosition(_rigidBody.position + heading * (moveSpeed * Time.fixedDeltaTime));
        }
    }
    
    private void MouseLook()
    {
        if (mouseLookEnabled)
        {
            // Get a ray from the camera to wherever in the map
            Ray mousePos = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
        
            // Check if the ray hit something. Might have to add layers later.
            if (Physics.Raycast(mousePos, out hitInfo, Mathf.Infinity))
            {
                // Modify the point where the mouse hit so it's always looking at the same height as the player so he doesnt rotate down or up.
                Vector3 position = transform.position;
                Vector3 lookAtPoint = new Vector3(hitInfo.point.x, position.y, hitInfo.point.z);
            
                // Save the point where we're looking
                _mousePosition = lookAtPoint;

                // Make the fireballController Look (in secret!!)
                fireballController.transform.LookAt(lookAtPoint);
            }
        }
    }
    
    private void ShootFireball()
    {
        if (_moveFireball)
        {
            // Move the fireball towards the mouse position
            _fireball.transform.position = Vector3.MoveTowards(_fireball.transform.position, _mousePosition,
                fireballSpeed * Time.deltaTime);

            // Check if the position of the fireball and mouse position are approximately equal.
            if (Vector3.Distance(_fireball.transform.position, _mousePosition) < 0.001f)
            {
                // We reset the fireball's position to it's original one (parented withing the Player)
                _fireball.transform.position = transform.position;
                
                // Turn off this method
                _moveFireball = false;
                
                // Make the coroutine continue
                _continueCoroutine = true;
            }
        }
    }

   
    private int CheckClickedNavMeshLayer(Vector3 clickedPosition)
    {
        // We try to check 30 times for the navMesh, in case it¿ll hit somewhere else
        for (int i = 0; i < 30; i++)
        {
            // We make a sphere centered on the clicked position with a 2f radius for checking
            Vector3 randomPoint = clickedPosition + Random.insideUnitSphere * 2f;
            
            // If we hit a navmesh
            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
                // Get the index of the navMesh from the mask
                int index = IndexFromMask(hit.mask);
                return index;
            }
        }

        // Otherwise return -1
        return -1;
    }

    private int CheckCurrentNavMeshLayer()
    {
        // We get the mask where the agent is on at the current time
        _agent.SamplePathPosition(NavMesh.AllAreas, 0f, out NavMeshHit hit);
        
        // Get the index of the navMesh from the mask
        int index = IndexFromMask(hit.mask);
        return index;
    }
 
    private int IndexFromMask(int mask)
    {
        // Converts the mask format (32bit) to an index number.
        for (int i = 0; i < 32; ++i)
        {
            if ((1 << i) == mask)
                return i;
        }
        return -1;
    }


    private IEnumerator<float> _FireballController()
    {
        // Infinite loop to keep this method working all the time
        while (true)
        {
            // Checks the current Nav Mesh layer of the player
            _currentNavMeshLayer = CheckCurrentNavMeshLayer();

            // If we press the mouse button for moving
            if (Input.GetMouseButtonDown(0) && !_isTeleporting && !_gameManager.clickedOnFire && !inputDisabled)
            {
                // Get the Nav Mesh layer of where the player clicked
                int clickedNavMeshLayer = CheckClickedNavMeshLayer(_mousePosition);
                
                // Calculations for fireball cost. 
                float remainingFire = _playerController.playerCurrentFire - 1;

                // Don't let the player teleport to another mesh (or to somewhere weird like Australia) nor let him cast if out of power
                if (_currentNavMeshLayer == clickedNavMeshLayer && clickedNavMeshLayer != -1 && remainingFire >= 0)
                {
                    // Disable movement and mouselook since animations will play
                    EnablePlayerMovement(false);

                    // Wait one frame
                    yield return Timing.WaitForOneFrame;
                    
                    // Stop any movement animations
                    _animator.SetFloat("Speed", 0);
                    
                    // Wait one frame
                    yield return Timing.WaitForOneFrame;

                    // Play the animation
                    _animator.SetTrigger("Casting Fireball");

                    // Raise the teleporting flag!
                    _isTeleporting = true;
                    
                    // Substract fire power from the player
                    _playerController.SetFireValue(-1);

                    // Initial particle effects
                    SpawnInitialNova();
                    
                    // Play the casting sound
                    _audioSource.PlayOneShot(_playerController.castingSound, 0.7f);
                    
                    // Wait for initial animations to play
                    yield return Timing.WaitForSeconds(0.4f);
                    
                    // Make player invisible
                    _animator.SetBool("Character Visible", false);

                    // Spawn a fireball!!!
                    _fireball.SetActive(true);
                    
                    // Move the fireball towards the clicked point, this is done in Update
                    _moveFireball = true;
                    
                    // Play Fireball sound
                    _audioSource.PlayOneShot(_playerController.fireballMovingSound, 0.5f);
                    
                    // Wait for the fireball to finish it's movement
                    while (_continueCoroutine == false)
                    {
                        yield return Timing.WaitForOneFrame;
                    }

                    // The fireball arrived, we stop moving and turn off the fireball
                    _fireball.SetActive(false);
                    
                    // Reset the flag once we've continued
                    _continueCoroutine = false;

                    // Wait one frame
                    yield return Timing.WaitForOneFrame;

                    // Warp to where the mouse was pointing
                    _agent.Warp(_mousePosition);
                    
                    // Reset animation trigger
                    _animator.ResetTrigger("Casting Fireball");
                    
                    // Wait one frame
                    yield return Timing.WaitForOneFrame;

                    // Make the player visible again
                    _animator.SetBool("Character Visible", true);
                    
                    // Play the ending explosion
                    SpawnMediumExplosion();
                    
                    // Play the explosion sound
                    _audioSource.PlayOneShot(_playerController.explosionSound, 0.8f);

                    // Reenable movement
                    EnablePlayerMovement(true);
                    
                    // Lower the teleporting flag!
                    _isTeleporting = false;
                }
                else
                {
                    if (remainingFire < 0)
                    {
                        // We display a message
                        SetFloatingTextValue("I need fire...");
                        floatingTextObject.SetActive(true);

                        yield return Timing.WaitForSeconds(0.75f);
                        floatingTextObject.SetActive(false);
                        
                    }
                }
            }

            // Wait one frame, otherwise this will crash Unity!
            yield return Timing.WaitForOneFrame;
        }
    }

    private void SpawnMediumExplosion()
    {
        float height = transform.position.y - characterHeight;
        Vector3 spawnPosition = new Vector3(transform.position.x, height + 0.401f, transform.position.z);
        Quaternion spawnRotation = Quaternion.Euler(270, 0 , 0);
        
        // Spawn the particles from the object pooler
        _objectPooler.SpawnFromPool("Beam Medium Explosion", spawnPosition, spawnRotation);
        _objectPooler.SpawnFromPool("Circles Medium Explosion", spawnPosition, spawnRotation);
        _objectPooler.SpawnFromPool("Impact Medium Explosion", spawnPosition, spawnRotation);
        _objectPooler.SpawnFromPool("Particles Medium Explosion", spawnPosition, spawnRotation);
    }

    private void SpawnInitialNova()
    {
        // Get the spawn position and rotation for the explosion
        float height = transform.position.y - 0.75f;
        Vector3 spawnPosition = new Vector3(transform.position.x, height + 0.401f, transform.position.z);
        Quaternion spawnRotation = Quaternion.Euler(270, 0 , 0);

        _objectPooler.SpawnFromPool("Imploding Circle", spawnPosition, spawnRotation);
        _objectPooler.SpawnFromPool("Localized Explosion", spawnPosition, spawnRotation);
        _objectPooler.SpawnFromPool("Tiny Beam", spawnPosition, spawnRotation);
        _objectPooler.SpawnFromPool("Tiny Impact", spawnPosition, spawnRotation);
        _objectPooler.SpawnFromPool("Tiny Particles", spawnPosition, spawnRotation);
    }

    private void EnablePlayerMovement(bool value)
    {
        movementEnabled = value;
        mouseLookEnabled = value;
    }

    private void SetFloatingTextValue(string value)
    {
        _floatingText.text = value;
    }


    /// <summary>
    /// Initializations for variables on Start
    /// </summary>
    #region Initializations
    
    private void InitializeRightVector()
    {
        _right = Quaternion.Euler(new Vector3(0, 90, 0)) * _forward;
    }

    private void InitializeForwardVector()
    {
        _forward = _camera.transform.forward;
        _forward.y = 0f;
        _forward = Vector3.Normalize(_forward);
    }

    private void InitializeComponents()
    {
        _camera = Camera.main;
        _agent = GetComponent<NavMeshAgent>();
        _objectPooler = ObjectPooler.Instance;
        _gameManager = GameManager.Instance;
        _playerController = GetComponent<PlayerController>();
        _animator = GetComponent<Animator>();
        _fireball = fireballController.transform.GetChild(0).gameObject;
        _rigidBody = GetComponent<Rigidbody>();
        _rotation = Quaternion.Euler(0,45,0);
        _floatingText = floatingTextObject.GetComponent<TextMeshPro>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void InitializeParticles()
    {
        fireballController.transform.position = transform.position;
        _fireball.SetActive(false);
    }

    #endregion
}


