using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    private ParticleSystem[] _finalExplosionParticles;
    
    // Start is called before the first frame update
    void Start()
    {
        // Get all the particle systems in the object
        _finalExplosionParticles = GetComponentsInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        IsFinalExplosionPlaying();
    }
    
    private void IsFinalExplosionPlaying()
    {
        bool[] isPlaying = new bool[_finalExplosionParticles.Length];
        
        // Loop through them, and find if any is still playing
        for (int i = 0; i < _finalExplosionParticles.Length; i++)
        {
            isPlaying[i] = _finalExplosionParticles[i].isPlaying;
        }

        bool trueExists = Array.Find(isPlaying, playing => playing);
        if (!trueExists)
        {
            gameObject.SetActive(false);
        }
        
    }
}
