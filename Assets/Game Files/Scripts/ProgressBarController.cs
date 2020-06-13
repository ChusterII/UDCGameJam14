using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ProgressBarController : MonoBehaviour
{

    public int maximum;

    public int current;

    public Image fill;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetCurrentFill();
    }

    private void GetCurrentFill()
    {
        if (current > 100)
        {
            current = 100;
        }

        if (current < 0)
        {
            current = 0;
        }
        float fillAmount = (float)current / maximum;
        fill.fillAmount = fillAmount;
    }
}
