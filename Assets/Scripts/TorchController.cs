using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchController : MonoBehaviour
{

    public GameObject highlightSphere;
    public ParticleSystem fire;

    private Material _highlightMaterial;
    [HideInInspector] 
    public bool _isMousedOver;


    // Start is called before the first frame update
    void Start()
    {
        _highlightMaterial = highlightSphere.GetComponent<Renderer>().material;
        _isMousedOver = false;
        // Turn off the highlight
        SetHighlightFade(1);
    }

    // Update is called once per frame
    void Update()
    {
        if (_isMousedOver)
        {
            print("mouse over!");
        }
    }
    
    /// <summary>
    /// Sets the highlight material's fade property
    /// </summary>
    /// <param name="value">0 for On, 1 for Off</param>
    private void SetHighlightFade(float value)
    {
        _highlightMaterial.SetFloat("Fade", value);
    }
    
    
}
