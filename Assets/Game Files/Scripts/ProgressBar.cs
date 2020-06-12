using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{

    public float fillSpeed = 0.5f;
    
    private Slider _slider;

    
    private float _targetProgress = 0;
    
    private void Awake()
    {
        _slider = GetComponent<Slider>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // If the bar is not already full
        if (_targetProgress <= 1)
        {
            // If the current value is less than the target progress
            if (_slider.value < _targetProgress)
            {
                _slider.value += fillSpeed * Time.deltaTime;
            }
            if (_slider.value > _targetProgress)
            {
                _slider.value -= fillSpeed * Time.deltaTime;
            }
        }
    }
    
    
    /// <summary>
    /// Increments the slider
    /// </summary>
    /// <param name="newProgress">Number between 0 and 1</param>
    public void SetIncrementProgress(float newProgress)
    {
        _targetProgress = _slider.value + newProgress;
    }


}
