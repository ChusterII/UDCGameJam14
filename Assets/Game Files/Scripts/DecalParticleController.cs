﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecalParticleController : MonoBehaviour
{
    private ParticleSystem _particleSystem;
    private List<ParticleCollisionEvent> _collisionEvents;
    private ObjectPooler _objectPooler;
    
    // Start is called before the first frame update
    void Start()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        _collisionEvents = new List<ParticleCollisionEvent>();
        _objectPooler = ObjectPooler.Instance;
    }

    private void OnParticleCollision(GameObject other)
    {
        // Get all collisions
        _particleSystem.GetCollisionEvents(other, _collisionEvents);
        
        // Spawn a blood quad using the first collision
        SpawnBlood(_collisionEvents[0]);
        

        //_objectPooler.SpawnFromPool("Blood", )
    }

    private void SpawnBlood(ParticleCollisionEvent particleCollision)
    {
        // Clear the list so it's ready for the next collision
        _collisionEvents.Clear();
        
        // Get the position of where the collision happened
        Vector3 intersectionPosition = new Vector3(particleCollision.intersection.x, 0.0001f, particleCollision.intersection.z);
        
        // We spawn a blood quad from the object pool 
        _objectPooler.SpawnFromPool("Blood", intersectionPosition, Quaternion.Euler(90, 0,0 ));
    }
}
