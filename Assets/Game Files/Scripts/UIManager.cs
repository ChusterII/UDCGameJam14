using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MEC;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    #region Singleton

    public static UIManager Instance;

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
    
    public GameObject[] playerFireImages;

    [SerializeField]
    private GameObject ui;
    [SerializeField]
    private GameObject mainMenu;
    [SerializeField]
    private GameObject gameOver;
    [SerializeField]
    private GameObject playerWon;
    [SerializeField]
    private GameObject transition;
    
    public Button startButton;
    public Button quitButton;

    public Button restartButton;
    public Button quitButtonGameOver;

    public TextMeshProUGUI timeText;

    private GameManager _gameManager;
    private int _lastFireValue = 0;
    private Animator _playerFireAnimator;
    private CanvasGroup _menuCanvas;
    private CanvasGroup _uiCanvas;
    private CanvasGroup _playerWonCanvas;
    private bool _gameOverButtonPressed;
    private bool _gameQuit;
    
    
    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameManager.Instance;
        _menuCanvas = mainMenu.GetComponent<CanvasGroup>();
        _uiCanvas = ui.GetComponent<CanvasGroup>();
        _playerWonCanvas = playerWon.GetComponent<CanvasGroup>();
        _gameOverButtonPressed = false;
        DisableFireContainers();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Method takes the value from the game and converts it to slider values (0-1) before sending it to the slider.
    /// </summary>
    /// <param name="currentFireValue">Value between 0 and 5</param>
    public void UpdateFireContainer(int currentFireValue)
    {
        // We are adding fire
        if (currentFireValue > _lastFireValue)
        {
            AddFireToContainers(currentFireValue);
        }
        else
        {
            if (currentFireValue < _lastFireValue)
            {
                SubstractFireToContainers(currentFireValue);
            }
        }

        _lastFireValue = currentFireValue;
    }

    private void DisableFireContainers()
    {
        for (int i = 0; i < playerFireImages.Length; i++)
        {
            playerFireImages[i].SetActive(false);
        }
    }

    private void AddFireToContainers(int addValue)
    {
        for (int i = 0; i < addValue; i++)
        {
            if (playerFireImages[i].activeSelf == false)
            {
                playerFireImages[i].SetActive(true);
            }
        }
    }

    private void SubstractFireToContainers(int substractValue)
    {
        
        for (int i = playerFireImages.Length - 1; i > substractValue - 1; i--)
        {
            if (playerFireImages[i].activeSelf == true)
            {
                playerFireImages[i].SetActive(false);
            }
        }
    }

    public void ToggleUI(bool value)
    {
        ui.SetActive(value);
    }
    
    public void ToggleMainMenu(bool value)
    {
        mainMenu.SetActive(value);
    }
    public void ToggleGameOver(bool value)
    {
        gameOver.SetActive(value);
    }
    public void ToggleTransition(bool value)
    {
        transition.SetActive(value);
    }
    
    public void TogglePlayerWon(bool value)
    {
        playerWon.SetActive(value);
    }

    public void SetMainMenuAlpha(float value)
    {
        mainMenu.GetComponent<CanvasGroup>().alpha = value;
    }
    
    public void SetTransitionAlpha(float value)
    {
        transition.GetComponent<CanvasGroup>().alpha = value;
    }

    public void FadeInMenu()
    {
        _menuCanvas.DOFade(1, 1f);
    }
    
    public void FadeOutMenu()
    {
        _menuCanvas.DOFade(0, 1f);
    }

    private void FadeInUi()
    {
        _uiCanvas.DOFade(1, 1f);
    }
    
    public void FadeInPlayerWon()
    {
        _playerWonCanvas.DOFade(1, 1f);
    }

    public void FadeOutTransition()
    {
        transition.GetComponent<CanvasGroup>().DOFade(0, 1f);
    }
    
    public void FadeInTransition()
    {
        transition.GetComponent<CanvasGroup>().DOFade(1, 1f);
    }
    
    private void DisableInitialButtons()
    {
        startButton.interactable = false;
        quitButton.interactable = false;
    }
    
    public void ToggleStartButtons(bool value)
    {
        startButton.interactable = value;
        quitButton.interactable = value;
    }
    
    private void ToggleRestartButtons(bool value)
    {
        restartButton.interactable = value;
        quitButtonGameOver.interactable = value;
    }

    public void StartJourneyButton()
    {
        DisableInitialButtons();
        Timing.RunCoroutine(GraphicalIntroSequence().CancelWith(gameObject));
    }

    private IEnumerator<float> GraphicalIntroSequence()
    {
        yield return Timing.WaitForSeconds(0.5f);
        FadeOutMenu();
        yield return Timing.WaitForSeconds(1.5f);
        // Turn off Main Menu
        ToggleMainMenu(false);
        ToggleUI(true);
        FadeInUi();
        yield return Timing.WaitForSeconds(0.5f);
        _gameManager.gameStart = true;
    }

    public void QuitButton()
    {
        _gameManager.quitGame = true;
    }

    public void QuitButtonGameOver()
    {
        _gameOverButtonPressed = true;
        _gameQuit = true;
        Application.Quit();
    }

    public void RestartButton()
    {
        _gameOverButtonPressed = true;
        ToggleRestartButtons(false);
    }

    public void GameOverScreen()
    {
        ToggleRestartButtons(false);
        Timing.RunCoroutine(GraphicalGameOverSequence().CancelWith(gameObject));
    }
    
    private IEnumerator<float> GraphicalGameOverSequence()
    {
        yield return Timing.WaitForSeconds(1f);
        gameOver.SetActive(true);
        CanvasGroup canvasGameOver = gameOver.GetComponent<CanvasGroup>();
        canvasGameOver.DOFade(1, 0.5f);
        yield return Timing.WaitForSeconds(0.5f);
        ToggleRestartButtons(true);
        while (!_gameOverButtonPressed)
        {
            yield return Timing.WaitForOneFrame;
        }

        if (!_gameQuit)
        {
            ToggleTransition(true);
            FadeInTransition();
            yield return Timing.WaitForSeconds(2f);
            canvasGameOver.alpha = 0;
            yield return Timing.WaitForSeconds(1f);
            SceneManager.LoadScene((int)SceneIndexes.GameScreen);
        }
        
    }

    
}
