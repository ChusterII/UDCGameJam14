using System.Collections;
using System.Collections.Generic;
using MEC;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float playerMaxHealth = 100f;
    public float playerMaxFire = 100f;
    public float startingFire = 0f;
    
    [Tooltip("Negative number")]
    public float fireballCost = -10f;
    [HideInInspector]
    public float playerCurrentFire;

    private UIManager _uiManager;
    private IsoController _playerController;
    private ObjectPooler _objectPooler;
    private Animator _animator;


    // Start is called before the first frame update
    void Start()
    {
        _uiManager = UIManager.Instance;
        _objectPooler = ObjectPooler.Instance;
        _animator = GetComponent<Animator>();
        _playerController = GetComponent<IsoController>();
        
        // Set initial fire value
        playerCurrentFire = startingFire;
        
        // Disable all input
        _playerController.movementEnabled = false;
        _playerController.mouseLookEnabled = false;
        _playerController.inputDisabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetFireValue(float value)
    {
        // Increments the player's fire value within the player
        playerCurrentFire += value;
        
        // Sends the value so it's updated in the progress bar
        _uiManager.UpdateFireProgressBar(value);
    }

    public void PlayerDeath()
    {
        // Disable all input
        _playerController.movementEnabled = false;
        _playerController.mouseLookEnabled = false;
        _playerController.inputDisabled = true;
        
        // Spawn blood!
        _objectPooler.SpawnFromPool("Player Death Effect", transform.position, Quaternion.Euler(270, 0, 0));
        
        // Play death animation
        _animator.SetTrigger("IsDead");
    }

    public IEnumerator<float> StartingSequence()
    {
        // Wait a few for intro sequence
        yield return Timing.WaitForSeconds(3f);

        // Play get up animation
        _animator.SetTrigger("GetUp");
        
        // Wait for animation to finish
        yield return Timing.WaitForSeconds(2.5f);
        
        // Enable all input and start the game!
        _playerController.movementEnabled = true;
        _playerController.mouseLookEnabled = true;
        _playerController.inputDisabled = false;

    }
}
