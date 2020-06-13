using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class LavaMover : MonoBehaviour
{
    [Header("Scrolling values")]
    public float xScrollSpeed;
    public float yScrollSpeed;

    [Header("Intensity glow effect")]
    [Tooltip("Minimum value. Should be 1 unless you want it brighter or darker than it is originally.")]
    public float intensityMin = 1f;
    [Tooltip("Maximum value for the glow.")]
    public float intensityMax = 1.5f;
    [Tooltip("Time between colors, in seconds")]
    public float glowTime = 1f;

    private Material _lavaMaterial;
    private Color _originalColor;
    private Color _emissionColor1;
    private Color _emissionColor2;
    private bool _isGlowing;
    private Sequence _glow;
    
    // Start is called before the first frame update
    void Start()
    {
        _lavaMaterial = GetComponent<Renderer>().material;
        _originalColor = _lavaMaterial.GetColor("_EmissionColor");
        _emissionColor1 = _originalColor * intensityMin;
        _emissionColor2 = _originalColor * intensityMax;
        DOTween.Init();
        DOTween.defaultAutoKill = false;
    }

    // Update is called once per frame
    void Update()
    {
        ScrollLava();
        LavaGlow();
    }

    private void ScrollLava()
    {
        float offsetX = Time.time * xScrollSpeed;
        float offsetY = Time.time * yScrollSpeed;
        _lavaMaterial.mainTextureOffset = new Vector2(offsetX, offsetY);
    }

    private void LavaGlow()
    {
        if (!_isGlowing)
        {
            _isGlowing = true;
            _glow = DOTween.Sequence();

            _glow.Append(_lavaMaterial.DOColor(_emissionColor1, glowTime));
            _glow.Append(_lavaMaterial.DOColor(_emissionColor2, glowTime));

            _glow.Play();
        }
        else
        {
            if (_glow.IsComplete())
            {
                _isGlowing = false;
            }
        }
        
    }
}
