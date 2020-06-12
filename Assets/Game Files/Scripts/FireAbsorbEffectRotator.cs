using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireAbsorbEffectRotator : MonoBehaviour
{

    private Quaternion _rotation;
    
    // Start is called before the first frame update
    void Awake()
    {
        _rotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = _rotation;
    }
}
