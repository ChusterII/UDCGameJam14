using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireAbsorbtionController : MonoBehaviour
{

    public float moveSpeed = 10f;
    public float disableTime = 2f;
    public float fireIncreaseValue = 20f;
    
    private GameObject _player;
    private Transform _playerTransform;
    private PlayerController _playerController;
    private ObjectPooler _objectPooler;
    private GameManager _gameManager;
    
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
        // On top of the player
        //Vector3 spawnPosition = new Vector3(_playerTransform.position.x, _playerTransform.position.y + 3f, _playerTransform.position.z);
        
        // Spawn the absorb animation
        //GameObject spawnedFireAbsorb =  _objectPooler.SpawnFromPool("Fire Absorb Effect", spawnPosition, Quaternion.Euler(270, 0 ,0));
        GameObject spawnedFireAbsorb =  _objectPooler.SpawnFromPool("Fire Absorb Effect", _playerTransform.position, Quaternion.Euler(270, 0 ,0));
        
        // Set the animation to be parented within the player so it moves with the player
        spawnedFireAbsorb.transform.SetParent(_playerTransform);
        
        // Increases player's Fire by the amount specified in the inspector
        _playerController.SetFireValue(fireIncreaseValue);
        
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
        _gameManager = GameManager.Instance;
    }
    
}
