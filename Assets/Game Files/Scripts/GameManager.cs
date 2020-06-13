using System;
using System.Collections;
using System.Collections.Generic;
using MEC;
using UnityEngine;

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
        Instance = this;
    }

    #endregion

    [Header("Cursors")]
    public Texture2D normalCursor;
    public Texture2D fireCursor;
    public Texture2D chargedCursor;

    public bool clickedOnFire;
    
    private float _playerFire;
    private PlayerController _player;


    // Start is called before the first frame update
    void Start()
    {
        SetCursor(CursorType.Normal);
        _player = FindObjectOfType<PlayerController>();
        
        // Play the starting sequence!
        Timing.RunCoroutine(_player.StartingSequence());
    }

    // Update is called once per frame
    void Update()
    {
        
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
    }

    
}
