using System;
using System.Collections;
using System.Collections.Generic;
using MEC;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.UI;

public enum SceneIndexes
{
    Manager = 0,
    GameScreen = 1
}


public class SceneDirector : MonoBehaviour
{

    public static SceneDirector Instance;

    public GameObject loadingScreen;
    public ProgressBarController progressBar;
    public TextMeshProUGUI textField;
    
    private List<AsyncOperation> _scenesLoading = new List<AsyncOperation>();
    private float _totalSceneProgress;
    private float _totalSpawnProgress;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadGame();
    }

    public void LoadGame()
    {
        // Show the loading screen
        loadingScreen.gameObject.SetActive(true);
        
        // Add more scenes to load here:
        _scenesLoading.Add(SceneManager.LoadSceneAsync((int) SceneIndexes.GameScreen, LoadSceneMode.Additive));

       
        Timing.RunCoroutine(GetSceneLoadProgress().CancelWith(gameObject));
        
        Timing.RunCoroutine(GetTotalProgress().CancelWith(gameObject));
    }

    public IEnumerator<float> GetSceneLoadProgress()
    {    
        // Go thru all scenes and check if they are done loading
        for (int i = 0; i < _scenesLoading.Count; i++)
        {
            while (!_scenesLoading[i].isDone)
            {
                _totalSceneProgress = 0;

                foreach (AsyncOperation operation in _scenesLoading)
                {
                    _totalSceneProgress += operation.progress;
                }

                _totalSceneProgress = (_totalSceneProgress / _scenesLoading.Count) * 100f;

                textField.text = string.Format("Loading Scene: {0}%", _totalSceneProgress);

                yield return 0;
            }
        }
    }

    public IEnumerator<float> GetTotalProgress()
    {
        float totalProgress = 0;
        
        while (ObjectPooler.Instance == null || !ObjectPooler.Instance.isDone)
        {
            if (ObjectPooler.Instance == null)
            {
                _totalSpawnProgress = 0;
                //Debug.LogWarning("Object Pooler is null. Check the singleton.");
            }
            else
            {
                _totalSpawnProgress = Mathf.Round(ObjectPooler.Instance.progress * 100f);
                
                textField.text = string.Format("Populating: {0}%", _totalSpawnProgress);
            }

            totalProgress = Mathf.Round((_totalSceneProgress + _totalSpawnProgress) / 2f);
            
            progressBar.current = Mathf.RoundToInt(totalProgress);
            
            yield return 0;
        }
        
        // Hide the loading screen when all scenes have been loaded
        loadingScreen.gameObject.SetActive(false);
    }
}
