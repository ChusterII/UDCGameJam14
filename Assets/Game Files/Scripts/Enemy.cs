using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{

    public Material[] bloodSplatsMaterials;
    public float enemyHeight = 0.25f;

    // --------------------------- Private variables start here ---------------------------
    private ObjectPooler _objectPooler;
    private float _bloodPositionY;
    private GameObject _spawnedBlood;
    private Vector3 _forward, _right; // Different from world axis forward and right because of camera
    private Camera _camera;
    private NavMeshAgent _agent;
    private Quaternion _rotation;
    private Animator _animator;
    private Rigidbody _rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        InitializeComponents();
        InitializeForwardVector();
        InitializeRightVector();
        InitializeBloodSplatter();
    }


    // Update is called once per frame
    void Update()
    {
        // Keeps the quad from turning
        transform.rotation = _rotation;
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
        _camera = Camera.main;
        _objectPooler = ObjectPooler.Instance;
        _animator = GetComponent<Animator>();
        _rigidBody = GetComponent<Rigidbody>();
        _rotation = Quaternion.Euler(0,45,0);
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
