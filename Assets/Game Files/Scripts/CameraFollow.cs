using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraFollow : MonoBehaviour
{

    public Transform targetToFollow;
    [Range(0f,1f)]
    public float smoothSpeed = 0.125f; // Smaller values will reach the target faster

    public Vector3 offset;
    private Vector3 _velocity = Vector3.zero;
    private Vector3 _offsetPosition;
    
    // Start is called before the first frame update
    void Start()
    {
        transform.position = targetToFollow.position + offset;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void FixedUpdate()
    {
        FollowTarget();
    }

    private void FollowTarget()
    {
        Vector3 desiredPosition = targetToFollow.position + offset;
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref _velocity, smoothSpeed);
        transform.position = smoothedPosition;
    }

    
}
