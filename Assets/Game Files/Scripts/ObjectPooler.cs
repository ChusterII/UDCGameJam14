﻿using System;
using System.Collections;
using System.Collections.Generic;
using MEC;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class ObjectPooler : MonoBehaviour
{
    #region Singleton

    public static ObjectPooler Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    [HideInInspector]
    public float progress;
    [HideInInspector]
    public bool isDone;
    private int _count;
    private int _totalSpawns;
    
    // Start is called before the first frame update
    private void Start()
    {
        GetTotalSpawns();
        Timing.RunCoroutine(InitializeDictionary());
    }

    private void GetTotalSpawns()
    {
        _totalSpawns = 0;
        foreach (Pool pool in pools)
        {
            _totalSpawns += pool.size;
        }
    }

    private IEnumerator<float> InitializeDictionary()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        _count = 0;

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
                progress = (float) _count / _totalSpawns;
                _count++;
            }

            poolDictionary.Add(pool.tag, objectPool);
        }

        yield return Timing.WaitForSeconds(0.5f);

        isDone = true;
    }

    public GameObject SpawnFromPool(string objectTag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(objectTag))
        {
            // Pool doesn't exist
            return null;
        }

        GameObject objectToSpawn;

        switch (objectTag)
        {
            case "Enemy":
                if (poolDictionary["Enemy"].Count > 0)
                {
                    objectToSpawn = poolDictionary[objectTag].Dequeue();
        
                    objectToSpawn.SetActive(true);
                    objectToSpawn.transform.position = position;
                    objectToSpawn.transform.rotation = rotation;
        
                    //poolDictionary[objectTag].Enqueue(objectToSpawn);

                    return objectToSpawn;
                }
                else
                {
                    // pool empty
                    return null;
                }
            default:
                objectToSpawn = poolDictionary[objectTag].Dequeue();
        
                objectToSpawn.SetActive(true);
                objectToSpawn.transform.position = position;
                objectToSpawn.transform.rotation = rotation;
        
                poolDictionary[objectTag].Enqueue(objectToSpawn);

                return objectToSpawn;
        }

    }
    
}
