using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float playerMaxHealth = 100f;
    public float playerMaxFire = 100f;
    
    [Tooltip("Negative number")]
    public float fireballCost = -10f;
    [HideInInspector]
    public float playerCurrentFire;

    private UIManager _uiManager;
    
    
    // Start is called before the first frame update
    void Start()
    {
        _uiManager = UIManager.Instance;
        
        // Set initial fire value
        playerCurrentFire = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetFireValue(float value)
    {
        // Increments the player's fire value within the player
        playerCurrentFire += value;
        
        // Sends the value so it's updated in the progress bar
        _uiManager.UpdateFireProgressBar(value);
    }
}
