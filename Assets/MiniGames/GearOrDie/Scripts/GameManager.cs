using UnityEngine;
using TMPro;
using System.Collections;
using System.Linq;

public class GameManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject gearPrefab;
    public GameObject batteryPrefab;

    [Header("UI")]
    public TMP_Text scoreText;
    public TMP_Text livesText;
    public GameObject gameOverPanel;
    public TMP_Text resultText;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip goodSound;
    public AudioClip badSound;

    [Header("Configuración")]
    public float spawnRate = 0.6f;
    public int maxObjects = 40;

    private int score = 0;
    private int lives = 3;
    private int targetScore = 20;
    private bool gameOver = false;

    void Start()
    {
        gameOverPanel.SetActive(false);
        UpdateUI();
        StartCoroutine(SpawnRoutine());

        if (Camera.main != null)
        {
            Camera.main.transform.position = new Vector3(0, 8, -14);
            Camera.main.transform.rotation = Quaternion.Euler(25, 0, 0);
        }
    }

    // 🔥 NUEVO: Cuenta objetos sin usar tags
    int CountActiveItems()
    {
        // Busca todos los objetos que sean Gear o Battery por su nombre
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        int count = 0;
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("Gear") || obj.name.Contains("Battery") ||
                obj.name.Contains("gear") || obj.name.Contains("battery"))
            {
                count++;
            }
        }
        return count;
    }

    IEnumerator SpawnRoutine()
    {
        while (!gameOver)
        {
            if (CountActiveItems() < maxObjects)
            {
                Vector3 pos = new Vector3(
                    Random.Range(-12f, 12f),
                    Random.Range(0.3f, 2.5f),
                    Random.Range(-12f, 12f)
                );

                float badChance = 0.25f + (score / (float)targetScore) * 0.55f;
                badChance = Mathf.Min(0.8f, badChance);

                GameObject item;
                if (Random.value < badChance)
                    item = batteryPrefab;
                else
                    item = gearPrefab;

                if (item != null)
                {
                    Instantiate(item, pos, Quaternion.identity);
                }
            }

            yield return new WaitForSeconds(spawnRate);
        }
    }

    public void HitGear(Vector3 position)
    {
        if (gameOver) return;

        score += 2;
        UpdateUI();
        PlaySound(true);

        int extraBatteries = Random.Range(1, 4);
        for (int i = 0; i < extraBatteries; i++)
        {
            Vector3 extraPos = new Vector3(
                Random.Range(-10f, 10f),
                Random.Range(0.3f, 2.5f),
                Random.Range(-10f, 10f)
            );
            if (batteryPrefab != null)
                Instantiate(batteryPrefab, extraPos, Quaternion.identity);
        }

        StartCoroutine(FlashScore());

        if (score >= targetScore)
            EndGame(true);
    }

    public void HitBattery(Vector3 position)
    {
        if (gameOver) return;

        lives--;
        UpdateUI();
        PlaySound(false);
        StartCoroutine(CameraShake(0.15f, 0.15f));

        if (lives <= 0)
            EndGame(false);
    }

    void PlaySound(bool isGood)
    {
        if (audioSource != null)
        {
            if (isGood && goodSound != null)
                audioSource.PlayOneShot(goodSound);
            else if (!isGood && badSound != null)
                audioSource.PlayOneShot(badSound);
        }
    }

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "PUNTOS: " + score + " / " + targetScore;
        if (livesText != null)
            livesText.text = "VIDAS: " + lives;
    }

    IEnumerator FlashScore()
    {
        if (scoreText != null)
        {
            scoreText.color = Color.yellow;
            yield return new WaitForSeconds(0.2f);
            scoreText.color = Color.white;
        }
    }

    IEnumerator CameraShake(float duration, float magnitude)
    {
        Vector3 originalPos = Camera.main.transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            Camera.main.transform.position = originalPos + new Vector3(x, y, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }
        Camera.main.transform.position = originalPos;
    }

    void EndGame(bool win)
    {
        gameOver = true;
        if (resultText != null)
            resultText.text = win ? "¡GANASTE!" : "GAME OVER";
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }
}