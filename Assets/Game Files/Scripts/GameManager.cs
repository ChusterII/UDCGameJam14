using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using MEC;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum CursorType
{
    Normal,
    Fire,
    Charged
}

public class GameManager : MonoBehaviour
{
    
    #region Singleton

    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    #endregion

    [Header("Cursors")]
    public Texture2D normalCursor;
    public Texture2D fireCursor;
    public Texture2D chargedCursor;

    public bool clickedOnFire;
    
    private float _playerFire;
    private PlayerController _player;
    private UIManager _uiManager;
    [HideInInspector]
    public bool gameStart;
    [HideInInspector]
    public bool quitGame;
    [HideInInspector]
    public bool playedIsDead;
    [HideInInspector]
    public bool playerWon;

    private float _gameTimer;


    // Start is called before the first frame update
    void Start()
    {
        SetCursor(CursorType.Normal);
        _player = FindObjectOfType<PlayerController>();
        _uiManager = UIManager.Instance;
        _gameTimer = 0;

        Timing.RunCoroutine(StartGame().CancelWith(gameObject));
        Timing.RunCoroutine(PlayerWins().CancelWith(gameObject));

    }

    // Update is called once per frame
    void Update()
    {
        TimerTick();
    }

    private void TimerTick()
    {
        _gameTimer += Time.deltaTime;
    }

    private IEnumerator<float> StartGame()
    {
        // Turn off the other canvas
        DisableStartingCanvas();
        
        
        // Start with a black fade out
        _uiManager.ToggleTransition(true);
        _uiManager.SetTransitionAlpha(1);
        _uiManager.FadeOutTransition();
        yield return Timing.WaitForSeconds(1);

        // Show the main Menu;
        _uiManager.ToggleTransition(false);
        _uiManager.ToggleMainMenu(true);
        _uiManager.SetMainMenuAlpha(0);
    
        yield return Timing.WaitForSeconds(1f);
    
        // Fade the main menu in
        _uiManager.FadeInMenu();

        yield return Timing.WaitForSeconds(0.9f);
        
        // Toggle the buttons
        _uiManager.ToggleStartButtons(true);

        // Wait while player clicks a button
        while (!gameStart)
        {
            yield return Timing.WaitForOneFrame;
        }

        if (!quitGame)
        {
            // Play the starting sequence!
            Timing.RunCoroutine(_player.StartingSequence().CancelWith(gameObject));
        }
        else
        {
            Application.Quit();
        }
    }

    private void DisableStartingCanvas()
    {
        _uiManager.ToggleUI(false);
        _uiManager.ToggleGameOver(false);
        _uiManager.ToggleTransition(false);
        _uiManager.TogglePlayerWon(false);
    }

    public void SetCursor(CursorType cursorType)
    {
        switch (cursorType)
        {
            case CursorType.Normal:
                Cursor.SetCursor(normalCursor, Vector2.zero, CursorMode.ForceSoftware);
                break;
            case CursorType.Fire:
                Cursor.SetCursor(fireCursor, Vector2.zero, CursorMode.ForceSoftware);
                break;
            case CursorType.Charged:
                Cursor.SetCursor(chargedCursor, Vector2.zero, CursorMode.ForceSoftware);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(cursorType), cursorType, null);
        }
    }

    public void PlayerDied()
    {
        // Calls the death animation for the player
        _player.PlayerDeath();
        playedIsDead = true;
        AudioManager.Instance.DisableEnvironmentalAudio();
        _uiManager.GameOverScreen();
    }

    private IEnumerator<float> PlayerWins()
    {
        while (!playerWon)
        {
            yield return Timing.WaitForOneFrame;
        }
        
        _uiManager.TogglePlayerWon(true);
        _uiManager.FadeInPlayerWon();
        
        var ts = TimeSpan.FromSeconds(_gameTimer);
        _uiManager.timeText.text = $"Your time is: {ts.TotalMinutes:00}:{ts.Seconds:00}";

        yield return Timing.WaitForSeconds(1f);

        bool nextStep = false;

        while (!nextStep)
        {
            if (Input.anyKey)
            {
                nextStep = true;
            }
            yield return Timing.WaitForOneFrame;
        }

        _uiManager.ToggleTransition(true);
        _uiManager.FadeInTransition();

        yield return Timing.WaitForSeconds(1.1f);
        
        SceneManager.LoadScene((int)SceneIndexes.GameScreen);
    }
     
    

    
}
