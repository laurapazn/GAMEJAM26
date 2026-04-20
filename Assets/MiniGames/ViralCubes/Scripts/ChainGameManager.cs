using UnityEngine;
using TMPro;

public class ChainGameManager : MonoBehaviour
{
    public int lives = 3;
    public int score = 0;

    public int winScore = 50; // 🎯 ganas con 50 puntos

    public TMP_Text livesText;
    public TMP_Text scoreText;

    public GameObject gameOverPanel;
    public TMP_Text resultText;

    private bool gameEnded = false;

    void Start()
    {
        UpdateUI();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    void Update()
    {
        if (gameEnded) return;

        // 🎯 condición de victoria por score
        if (score >= winScore)
        {
            EndGame(true);
        }
    }

    public void LoseLife()
    {
        if (gameEnded) return;

        lives--;
        UpdateUI();

        if (lives <= 0)
        {
            EndGame(false);
        }
    }

    public void AddScore(int amount)
    {
        if (gameEnded) return;

        score += amount;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (livesText != null)
            livesText.text = " Vidas: " + lives;

        if (scoreText != null)
            scoreText.text = "Score: " + score + " / " + winScore;
    }

    void EndGame(bool win)
    {
        gameEnded = true;

        Debug.Log(win ? "GANASTE" : "PERDISTE"); // 👈 para debug

        Time.timeScale = 0f;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (resultText != null)
        {
            resultText.text = win ? " GANASTE " : "PERDISTE ";
        }
    }
}