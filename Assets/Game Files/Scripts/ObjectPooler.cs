﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class ObjectPooler : MonoBehaviour
{
    #region Singleton

    public static ObjectPooler Instance;

    private void Awake()
    {
        Instance = this;
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
    
    // Start is called before the first frame update
    private void Start()
    {
        InitializeDictionary();
    }

    private void InitializeDictionary()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
                
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
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

    private void Update()
    {
        //print(poolDictionary["Coin"].Count);
    }
}
