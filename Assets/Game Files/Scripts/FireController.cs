using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireController : MonoBehaviour
{

   //public TorchController torchController;
    public GameObject highlightSphere;
    
    private Material _highlightMaterial;
    private GameManager _gameManager;
    private ObjectPooler _objectPooler;
    
    // Start is called before the first frame update
    void Start()
    {
        InitializeComponents();
        SetHighlightFade(1);
    }

    private void InitializeComponents()
    {
        _highlightMaterial = highlightSphere.GetComponent<Renderer>().material;
        _gameManager = GameManager.Instance;
        _objectPooler = ObjectPooler.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseOver()
    {
        SetHighlightFade(0);
        _gameManager.SetCursor(CursorType.Fire);
    }

    private void OnMouseExit()
    {
        SetHighlightFade(1);
        _gameManager.SetCursor(CursorType.Normal);
    }

    private void OnMouseDown()
    {
        _gameManager.clickedOnFire = true;
        _objectPooler.SpawnFromPool("Fire Vessel", transform.position, Quaternion.identity);
    }

    private void OnMouseUp()
    {
        _gameManager.clickedOnFire = false;
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
