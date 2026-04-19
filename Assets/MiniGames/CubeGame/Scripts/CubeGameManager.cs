using UnityEngine;
using TMPro;
using System.Collections;

public class CubeGameManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject goodCubePrefab;
    public GameObject badCubePrefab;
    public GameObject destroyEffect;

    [Header("UI")]
    public TMP_Text scoreText;
    public TMP_Text timerText;
    public TMP_Text livesText;
    public TMP_Text comboText;
    public TMP_Text resultText;
    public GameObject gameOverPanel;

    [Header("Audio")]
    public AudioClip goodSound;
    public AudioClip badSound;

    private int score = 0;
    private int lives = 3;
    private int combo = 0;

    private float timeLeft = 20f;

    private float spawnRate = 0.15f;
    private int maxCubes = 80;

    private float badChance = 0.4f;

    private bool gameOver = false;

    private int badStreak = 0; // 💀 seguidos malos

    private Transform cam;
    private Vector3 camOriginalPos;

    void Start()
    {
        cam = Camera.main.transform;
        camOriginalPos = cam.localPosition;

        gameOverPanel.SetActive(false);

        UpdateUI();
        UpdateTimerUI();

        StartCoroutine(SpawnRoutine());
    }

    void Update()
    {
        if (gameOver) return;

        timeLeft -= Time.deltaTime;

        if (timeLeft < 0) timeLeft = 0;

        UpdateTimerUI();

        if (timeLeft <= 0)
            EndGame(false);
    }

    IEnumerator SpawnRoutine()
    {
        while (!gameOver)
        {
            if (GameObject.FindGameObjectsWithTag("Cube").Length < maxCubes)
            {
                Vector3 pos = RandomPos();

                bool spawnBad = Random.value < badChance;

                Instantiate(
                    spawnBad ? badCubePrefab : goodCubePrefab,
                    pos,
                    Quaternion.identity
                );
            }

            yield return new WaitForSeconds(spawnRate);
        }
    }

    Vector3 RandomPos()
    {
        return new Vector3(
            Random.Range(-9f, 9f),
            Random.Range(1f, 6f),
            Random.Range(-9f, 9f)
        );
    }

    public void AddScore(int value)
    {
        if (value > 0)
        {
            combo++;
            score += value * combo;
            badStreak = 0;

            // baja un poco dificultad
            badChance = Mathf.Max(0.3f, badChance - 0.03f);
        }
        else
        {
            combo = 0;
            lives--;
            badStreak++;

            // 💀 MÁS ROJOS
            badChance = Mathf.Min(0.95f, badChance + 0.2f);

            // 💀 tiempo baja más rápido
            timeLeft -= 2f;

            // 📳 cámara fuerte
            StartCoroutine(CameraShake(0.5f, 0.5f));

            // ☠️ MUERTE INSTANTÁNEA
            if (badStreak >= 2)
            {
                lives = 0;
            }
        }

        if (score >= 10)
            EndGame(true);

        if (lives <= 0)
            EndGame(false);

        UpdateUI();
    }

    void UpdateUI()
    {
        scoreText.text = "Puntos: " + score;
        livesText.text = "Vidas: " + lives;
        comboText.text = "Combo: x" + combo;
    }

    void UpdateTimerUI()
    {
        timerText.text = "Tiempo: " + Mathf.CeilToInt(timeLeft);
    }

    public void PlayDestroyEffect(Vector3 pos)
    {
        if (destroyEffect != null)
            Instantiate(destroyEffect, pos, Quaternion.identity);
    }

    IEnumerator CameraShake(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            cam.localPosition = camOriginalPos + Random.insideUnitSphere * magnitude;
            elapsed += Time.deltaTime;
            yield return null;
        }

        cam.localPosition = camOriginalPos;
    }

    void EndGame(bool win)
    {
        gameOver = true;
        resultText.text = win ? "GANASTE" : "PERDISTE";
        gameOverPanel.SetActive(true);
    }
}