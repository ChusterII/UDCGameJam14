using System.Collections;
using System.Collections.Generic;
using MEC;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float playerMaxHealth = 100f;
    public float playerMaxFire = 100f;
    public int startingFire = 0;

    public AudioClip fireballMovingSound;
    public AudioClip castingSound;
    public AudioClip explosionSound;
    public AudioClip dyingSound;
    
    
    [HideInInspector]
    public int playerCurrentFire;

    private UIManager _uiManager;
    private IsoController _playerController;
    private ObjectPooler _objectPooler;
    private Animator _animator;
    private AudioSource _audioSource;
    private Collider _collider;


    // Start is called before the first frame update
    void Start()
    {
        InitializeComponents();

        // Set initial fire value
        playerCurrentFire = startingFire;
        
        // Disable all input
        _playerController.movementEnabled = false;
        _playerController.mouseLookEnabled = false;
        _playerController.inputDisabled = true;
    }

    private void InitializeComponents()
    {
        _uiManager = UIManager.Instance;
        _objectPooler = ObjectPooler.Instance;
        _animator = GetComponent<Animator>();
        _playerController = GetComponent<IsoController>();
        _audioSource = GetComponent<AudioSource>();
        _collider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetFireValue(int value)
    {
        // Increments the player's fire value within the player
        playerCurrentFire += value;

        // Clamp the value
        if (playerCurrentFire < 0)
        {
            playerCurrentFire = 0;
        }

        if (playerCurrentFire > 5)
        {
            playerCurrentFire = 5;
        }
        
        // Sends the value so it's updated in the progress bar
        _uiManager.UpdateFireContainer(playerCurrentFire);
    }

    public void PlayerDeath()
    {
        // Disable all input
        _playerController.movementEnabled = false;
        _playerController.mouseLookEnabled = false;
        _playerController.inputDisabled = true;
        
        // Spawn blood!
        _objectPooler.SpawnFromPool("Player Death Effect", transform.position, Quaternion.Euler(270, 0, 0));
        
        // Play death sound
        _audioSource.PlayOneShot(dyingSound, 0.7f);

        // Disable the collider?
        _collider.enabled = false;
        
        // Play death animation
        _animator.SetTrigger("IsDead");
    }

    public IEnumerator<float> StartingSequence()
    {
        // Wait a few for intro sequence
        yield return Timing.WaitForSeconds(1.5f);

        // Play get up animation
        _animator.SetTrigger("GetUp");
        
        // Wait for animation to finish
        yield return Timing.WaitForSeconds(2f);
        
        // Enable all input and start the game!
        _playerController.movementEnabled = true;
        _playerController.mouseLookEnabled = true;
        _playerController.inputDisabled = false;

    }
}
