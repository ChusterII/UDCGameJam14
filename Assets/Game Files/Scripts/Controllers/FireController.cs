using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class FireController : MonoBehaviour
{

    public int maxFireAvailable = 5;
    public GameObject highlightSphere;
    public AudioClip flameSound;
    public AudioClip fireAbsorbSound;
    
    private Material _highlightMaterial;
    private GameManager _gameManager;
    private ObjectPooler _objectPooler;
    private ParticleSystem _fireParticles;
    private int _currentFireAvailable;
    private bool _hasFire;
    private AudioSource _audioSource;
    private bool _disablingAudio;
    
    // Start is called before the first frame update
    void Start()
    {
        InitializeComponents();
        SetHighlightFade(1);
        _audioSource.clip = flameSound;
        _audioSource.Play();
    }

    private void Update()
    {
        if (_gameManager.playedIsDead && !_disablingAudio)
        {
            _audioSource.DOFade(0, 0.5f);
            _disablingAudio = true;
        }
    }

    private void InitializeComponents()
    {
        _highlightMaterial = highlightSphere.GetComponent<Renderer>().material;
        _fireParticles = GetComponent<ParticleSystem>();
        _gameManager = GameManager.Instance;
        _objectPooler = ObjectPooler.Instance;
        _currentFireAvailable = maxFireAvailable;
        _hasFire = true;
        _audioSource = GetComponent<AudioSource>();
    }
    
    private void OnMouseOver()
    {
        if (_hasFire)
        {
            SetHighlightFade(0);
            _gameManager.SetCursor(CursorType.Fire);
        }
    }

    private void OnMouseExit()
    {
            SetHighlightFade(1);
            _gameManager.SetCursor(CursorType.Normal);
    }

    private void OnMouseDown()
    {
        if (_hasFire)
        {
            _gameManager.clickedOnFire = true;
            _objectPooler.SpawnFromPool("Fire Vessel", transform.position, Quaternion.identity);
            _audioSource.PlayOneShot(fireAbsorbSound, 0.7f);
            _currentFireAvailable--;
            if (_currentFireAvailable <= 0)
            {
                _currentFireAvailable = 0;
                _hasFire = false;
                _fireParticles.Stop();
                highlightSphere.SetActive(false);
            }
        }
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
