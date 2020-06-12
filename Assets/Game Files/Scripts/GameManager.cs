using System;
using System.Collections;
using System.Collections.Generic;
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


    // Start is called before the first frame update
    void Start()
    {
        SetCursor(CursorType.Normal);
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

    
}
