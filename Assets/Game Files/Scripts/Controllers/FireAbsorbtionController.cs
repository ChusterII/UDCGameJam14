using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireAbsorbtionController : MonoBehaviour
{

    public float moveSpeed = 10f;
    public float disableTime = 2f;

    
    
    private GameObject _player;
    private Transform _playerTransform;
    private PlayerController _playerController;
    private ObjectPooler _objectPooler;
    private AudioSource _audioSource;
    
    // Start is called before the first frame update
    void Start()
    {
        InitializeComponents();
    }

    // Update is called once per frame
    void Update()
    {
        MoveTowardsPlayer();
    }

    private void MoveTowardsPlayer()
    {
        transform.position =
            Vector3.MoveTowards(transform.position, _playerTransform.position, moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Spawn the absorb animation
        GameObject spawnedFireAbsorb =  _objectPooler.SpawnFromPool("Fire Absorb Effect", _playerTransform.position, Quaternion.Euler(270, 0 ,0));
        
        // Play absorb sound
        _audioSource.Play();
        
        // Set the animation to be parented within the player so it moves with the player
        spawnedFireAbsorb.transform.SetParent(_playerTransform);
        
        // Increases player's Fire by the amount specified in the inspector
        _playerController.SetFireValue(1);
        
        // Disable the trail in the time specified in the inspector.
        Invoke(nameof(DisableTrail), disableTime);
    }

    private void DisableTrail()
    {
        gameObject.SetActive(false);
    }
    
    private void InitializeComponents()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerTransform = _player.transform;
        _playerController = _player.GetComponent<PlayerController>();
        _objectPooler = ObjectPooler.Instance;
        _audioSource = GetComponent<AudioSource>();
    }
    
}
