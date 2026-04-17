using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject mainMenuPanel;
    public GameObject gameUIPanel;
    public GameObject pauseMenuPanel;
    public GameObject gameOverPanel;
    public GameObject instructionsPanel;

    [Header("Textos")]
    public Text titleText;
    public Text bestScoreText;
    public Text survivalTimeText;
    public Text difficultyText;
    public Text modulesAliveText;
    public Text finalTimeText;
    public Text newBestText;

    [Header("Botones")]
    public Button startButton;
    public Button resumeButton;
    public Button restartButton;
    public Button mainMenuButton;
    public Button instructionsButton;
    public Button closeInstructionsButton;
    public Button quitButton;

    private GridController gridController;
    private float currentSurvivalTime = 0f;
    private float bestTime = 0f;
    private bool isGameRunning = false;

    void Start()
    {
        gridController = FindFirstObjectByType<GridController>();

        if (gridController != null)
            gridController.OnGameOver += OnGameOver;

        LoadBestScore();
        SetupButtons();
        ShowMainMenu();
        UpdateBestScoreText();
    }

    void Update()
    {
        if (isGameRunning && gridController != null && gridController.isGameActive)
        {
            currentSurvivalTime += Time.deltaTime;

            if (survivalTimeText != null)
                survivalTimeText.text = "Tiempo: " + FormatTime(currentSurvivalTime);

            if (difficultyText != null && gridController != null)
                difficultyText.text = "Fallos: cada " + gridController.GetCurrentFailInterval().ToString("F1") + "s";

            if (modulesAliveText != null && gridController != null)
                modulesAliveText.text = "Modulos sanos: " + gridController.GetAliveModulesCount();
        }

        try
        {
            if (isGameRunning && UnityEngine.Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }
        }
        catch (System.Exception)
        {
            // Ignorar errores de Input
        }
    }

    void SetupButtons()
    {
        if (startButton != null) startButton.onClick.AddListener(StartGame);
        if (resumeButton != null) resumeButton.onClick.AddListener(ResumeGame);
        if (restartButton != null) restartButton.onClick.AddListener(RestartGame);
        if (mainMenuButton != null) mainMenuButton.onClick.AddListener(ReturnToMainMenu);
        if (instructionsButton != null) instructionsButton.onClick.AddListener(() => instructionsPanel.SetActive(true));
        if (closeInstructionsButton != null) closeInstructionsButton.onClick.AddListener(() => instructionsPanel.SetActive(false));
        if (quitButton != null) quitButton.onClick.AddListener(QuitGame);
    }

    void ShowMainMenu()
    {
        SetAllPanels("main");
        isGameRunning = false;
        Time.timeScale = 1f;
    }

    void StartGame()
    {
        SetAllPanels("game");
        isGameRunning = true;
        currentSurvivalTime = 0f;
        Time.timeScale = 1f;

        if (gridController != null)
            gridController.StartNewGame();
    }

    void OnGameOver()
    {
        SetAllPanels("gameover");
        isGameRunning = false;
        Time.timeScale = 0f;

        if (finalTimeText != null)
            finalTimeText.text = "Tiempo final: " + FormatTime(currentSurvivalTime);

        if (currentSurvivalTime > bestTime)
        {
            bestTime = currentSurvivalTime;
            SaveBestScore();
            if (newBestText != null)
            {
                newBestText.text = "NUEVO RECORD";
                newBestText.gameObject.SetActive(true);
            }
        }
        else
        {
            if (newBestText != null)
                newBestText.gameObject.SetActive(false);
        }

        UpdateBestScoreText();
    }

    void TogglePause()
    {
        if (!isGameRunning) return;

        if (Time.timeScale == 1f)
        {
            Time.timeScale = 0f;
            if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
            if (gameUIPanel != null) gameUIPanel.SetActive(false);
        }
        else
        {
            ResumeGame();
        }
    }

    void ResumeGame()
    {
        Time.timeScale = 1f;
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (gameUIPanel != null) gameUIPanel.SetActive(true);
    }

    void RestartGame()
    {
        Time.timeScale = 1f;
        currentSurvivalTime = 0f;

        if (gridController != null)
            gridController.RestartGame();

        SetAllPanels("game");
        isGameRunning = true;
    }

    void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        if (gridController != null)
            gridController.StopGame();

        ShowMainMenu();
    }

    void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    void SetAllPanels(string active)
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(active == "main");
        if (gameUIPanel != null) gameUIPanel.SetActive(active == "game");
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(active == "pause");
        if (gameOverPanel != null) gameOverPanel.SetActive(active == "gameover");
        if (instructionsPanel != null) instructionsPanel.SetActive(active == "instructions");
    }

    string FormatTime(float seconds)
    {
        int mins = Mathf.FloorToInt(seconds / 60);
        int secs = Mathf.FloorToInt(seconds % 60);
        int millis = Mathf.FloorToInt((seconds * 100) % 100);

        if (mins > 0)
            return string.Format("{0:00}:{1:00}:{2:00}", mins, secs, millis);
        else
            return string.Format("{0:00}.{1:00}s", secs, millis);
    }

    void LoadBestScore()
    {
        bestTime = PlayerPrefs.GetFloat("BestTime", 0f);
    }

    void SaveBestScore()
    {
        PlayerPrefs.SetFloat("BestTime", bestTime);
        PlayerPrefs.Save();
    }

    void UpdateBestScoreText()
    {
        if (bestScoreText != null)
            bestScoreText.text = "Record: " + FormatTime(bestTime);
    }
}