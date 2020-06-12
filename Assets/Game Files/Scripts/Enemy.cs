using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public Material[] bloodSplatsMaterials;
    public float enemyHeight = 0.25f;

    // --------------------------- Private variables start here ---------------------------
    private ObjectPooler _objectPooler;
    private float _bloodPositionY;
    private GameObject _spawnedBlood;

    // Start is called before the first frame update
    void Start()
    {
        InitializeComponents();
        InitializeBloodSplatter();
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDeath()
    {
        // Spawn a blood splatter quad on the ground
        Vector3 bloodPosition = new Vector3(transform.position.x, _bloodPositionY, transform.position.z);
        
        // Set the blood to be where the enemy died
        _spawnedBlood.transform.position = bloodPosition;
        
        // Enable blood
        _spawnedBlood.SetActive(true);
        
        // Spawn the death effect
        _objectPooler.SpawnFromPool("Enemy Death Effect", transform.position, Quaternion.Euler(270, 0 , 0));
        
        // Queue the object back for reusage
        _objectPooler.poolDictionary["Enemy"].Enqueue(gameObject);
        
        // Disable the object
        gameObject.SetActive(false);
    }

    private Material PickRandomBloodMaterial()
    {
        // Get a random number between 0 and the size of the array and return it
        int randomIndex = Random.Range(0, bloodSplatsMaterials.Length);
        return bloodSplatsMaterials[randomIndex];
    }
    
    private void InitializeBloodSplatter()
    {
        // Height for the blood from the ground.
        _bloodPositionY = 0.01f;

        // Prespawn the blood and save it
        _spawnedBlood = _objectPooler.SpawnFromPool("Blood Splatter", transform.position, Quaternion.Euler(90, 0, 0));

        // Set the material for the blood at random from the array
        _spawnedBlood.GetComponent<Renderer>().material = PickRandomBloodMaterial();

        // Hide the blood
        _spawnedBlood.SetActive(false);
    }
    
    private void InitializeComponents()
    {
        _objectPooler = ObjectPooler.Instance;
    }
}
