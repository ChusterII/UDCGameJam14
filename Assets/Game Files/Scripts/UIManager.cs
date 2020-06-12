using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region Singleton

    public static UIManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    #endregion
    
    public ProgressBar fireProgressBar;

    private GameManager _gameManager;
    
    
    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameManager.Instance;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Method takes the value from the game and converts it to slider values (0-1) before sending it to the slider.
    /// </summary>
    /// <param name="value">Percentage of the bar to be filled</param>
    public void UpdateFireProgressBar(float value)
    {
        float normalizedValue = value / 100f;
        fireProgressBar.SetIncrementProgress(normalizedValue);
    }
}
