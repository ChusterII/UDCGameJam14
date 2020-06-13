using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using MEC;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;


public class Enemy : MonoBehaviour
{

    public Material[] bloodSplatsMaterials;
    public float enemyHeight = 0.25f;
    public float enemySpeed = 3f;
    public float attackingRange = 2f;

    // --------------------------- Private variables start here ---------------------------
    private ObjectPooler _objectPooler;
    private float _bloodPositionY;
    private GameObject _spawnedBlood;
    private Vector3 _forward, _right; // Different from world axis forward and right because of camera
    private Camera _camera;
    private NavMeshAgent _agent;
    private Quaternion _rotation;
    private Animator _animator;
    private GameManager _gameManager;
    private FieldOfView _fov;
    private bool _enableMovement = true;
    private bool _isAttacking;
    private bool _attackDisabled;


    // Start is called before the first frame update
    void Start()
    {
        InitializeComponents();
        InitializeForwardVector();
        InitializeRightVector();

        Timing.RunCoroutine(Attack());
    }


    // Update is called once per frame
    void Update()
    {
        // Keeps the quad from turning
        transform.rotation = _rotation;
        
        MoveTowardsTarget();
    }

    
    private IEnumerator<float> Attack()
    {

        while (true)
        {
            // If the player is in range and he enemy can see it
            if (_fov.visibleTargets.Count > 0 && !_attackDisabled)
            {
                // Assign the player as the target
                Transform target = _fov.visibleTargets[0];
            
                if (Vector3.Distance(transform.position, target.position) < attackingRange)
                {
                    // Set the flags for attacking
                    _isAttacking = true;
                    _enableMovement = false;
                    
                    // We stop the enemy and any movement animation
                    _animator.SetFloat("Speed", 0);
                    _agent.isStopped = true;

                    // Wait for the animator
                    yield return Timing.WaitForOneFrame;

                    // Get the direction towards the player
                    Vector3 targetDirection = (target.position - transform.position).normalized;
                    
                    // Get the general direction from the velocity
                    Vector3 direction = ExtrapolateDirection(targetDirection, 0.2f);

                    // Convert the local direction to a world direction, taking into consideration the camera angle.
                    Vector2 translatedDirection = TranslateVector(direction);
                    
                    // Set the animator variables for animations.
                    _animator.SetFloat("Horizontal", translatedDirection.x);
                    _animator.SetFloat("Vertical", translatedDirection.y);

                    // Player is in range, attack
                    _animator.SetTrigger("Attack");
                    
                    // Wait for animation to finish
                    yield return Timing.WaitForSeconds(0.4f);
                    
                    // If the player is still in range after the attack, player dies.
                    if (Vector3.Distance(transform.position, target.position) < attackingRange)
                    {
                        // Tell the game Manager that the player is dead and stop all movement and animations.
                        _gameManager.PlayerDied();
                        _enableMovement = false;
                        _animator.SetFloat("Speed", 0);
                        _attackDisabled = true;
                    }
                    
                    // Small delay before he starts walking again
                    yield return Timing.WaitForSeconds(0.75f);

                    // Not attacking anymore
                    _isAttacking = false;
                    _agent.isStopped = false;

                }
                else
                {
                    // Target not in range, move.
                    _enableMovement = true;
                }
            }

            yield return Timing.WaitForOneFrame;
        }
    }
    

    private void MoveTowardsTarget()
    {
        // If the player is in range and he enemy can see it
        if (_fov.visibleTargets.Count > 0 && _enableMovement && !_isAttacking)
        {
            // Assign the player as the target
            Transform target = _fov.visibleTargets[0];
            
            // Move towards the player
            _agent.destination = target.position;

            // Get agent's velocity, which includes direction
            Vector3 velocity = _agent.velocity;
            
            // Get the general direction from the velocity
            Vector3 direction = ExtrapolateDirection(velocity, 0.2f);

            // Convert the local direction to a world direction, taking into consideration the camera angle.
            Vector2 translatedDirection = TranslateVector(direction);

            // Set the animator variables for animations.
            _animator.SetFloat("Horizontal", translatedDirection.x);
            _animator.SetFloat("Vertical", translatedDirection.y);
            
            // Set the speed if moving
            float speed = _agent.velocity.sqrMagnitude;
            _animator.SetFloat("Speed", speed);
        }
    }

    private static Vector3 ExtrapolateDirection(Vector3 vector, float errorMargin)
    {
        if (vector.x > errorMargin)
        {
            vector.x = 1;
        }
        else
        {
            if (vector.x < -errorMargin)
            {
                vector.x = -1;
            }
            else
            {
                vector.x = 0;
            }
        }

        if (vector.z > errorMargin)
        {
            vector.z = 1;
        }
        else
        {
            if (vector.z < -errorMargin)
            {
                vector.z = -1;
            }
            else
            {
                vector.z = 0;
            }
        }

        return vector;
    }

    private Vector2 TranslateVector(Vector3 vector)
    {
        if ((int)vector.x == 1)
        {
            switch (vector.z)
            {
                case 1:
                    // This is (1,1), "UpRight". Translate to "Up"
                    return new Vector2(0,1);
                case 0:
                    // This is (1,0), "Right". Translate to "UpRight"
                    return new Vector2(1,1);
                case -1:
                    // This is (1,-1), "DownRight". Translate to "Right"
                    return new Vector2(1,0);
            }
        }
        else
        {
            if ((int)vector.x == 0)
            {
                switch (vector.z)
                {
                    case 1:
                        // This is (0,1), "Up". Translate to "UpLeft"
                        return new Vector2(-1,1);
                    case -1:
                        // This is (0,-1), "Down". Translate to "DownRight"
                        return new Vector2(1, -1);
                }
            }
            else
            {
                switch (vector.z)
                {
                    case 1:
                        // This is (-1,1), "UpLeft". Translate to "Left"
                        return new Vector2(-1, 0);
                    case 0:
                        // This is (-1,0), "Left". Translate to "DownLeft"
                        return new Vector2(-1, -1);
                    case -1:
                        // This is (-1,-1), "DownLeft". Translate to "Down"
                        return new Vector2(0,-1);
                }
            }
        }

        return new Vector2(0,0);
    }

    public void OnDeath()
    {
        // Height for the blood from the ground.
        _bloodPositionY = 0.01f;
        
        // Spawn a blood splatter quad on the ground
        Vector3 bloodPosition = new Vector3(transform.position.x, _bloodPositionY, transform.position.z);

        // Spawn a blood splat
        _spawnedBlood = _objectPooler.SpawnFromPool("Blood Splatter", bloodPosition, Quaternion.Euler(90, 0, 0));

        // Set the material for the blood at random from the array
        _spawnedBlood.GetComponent<Renderer>().material = PickRandomBloodMaterial();
        
        // Spawn the death effect
        _objectPooler.SpawnFromPool("Enemy Death Effect", transform.position, Quaternion.Euler(270, 0 , 0));
        
        // Queue the object back for reusage
        _objectPooler.poolDictionary["Enemy"].Enqueue(gameObject);
        
        //move the game object away //TODO: do something aobut the damn NavMeshAgent trying to access the object while disabled.
        transform.position = new Vector3(0, 13, 0);
        
        // Disable the object
        gameObject.SetActive(false);
    }

    private Material PickRandomBloodMaterial()
    {
        // Get a random number between 0 and the size of the array and return it
        int randomIndex = Random.Range(0, bloodSplatsMaterials.Length);
        return bloodSplatsMaterials[randomIndex];
    }

    private void InitializeComponents()
    {
        _camera = Camera.main;
        _objectPooler = ObjectPooler.Instance;
        _gameManager = GameManager.Instance;
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _rotation = Quaternion.Euler(0,45,0);
        _fov = GetComponent<FieldOfView>();
    }
    
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
}
