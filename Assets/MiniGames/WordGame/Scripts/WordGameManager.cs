using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WordGameManager : MonoBehaviour
{
    public static WordGameManager Instance;

    public List<WordCategory> categories;
    public WordSpawner spawner;

    public int score = 0;
    public int lives = 3;

    private WordCategory current;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartGame();
    }

    void StartGame()
    {
        current = categories[Random.Range(0, categories.Count)];
        spawner.Init(current, categories);

        Debug.Log("Toca solo: " + current.categoryName);
    }

    public void OnCorrectWord(Word3D w)
    {
        score += 10;
        Destroy(w.gameObject);
        Debug.Log("Correcto +10");
    }

    public void OnWrongWord(Word3D w)
    {
        lives--;
        Destroy(w.gameObject);
        Debug.Log("Error -1 vida");

        if (lives <= 0)
            GameOver();
    }

    public void OnWordMissed()
    {
        lives--;
        Debug.Log("Se escapó!");

        if (lives <= 0)
            GameOver();
    }

    void GameOver()
    {
        Debug.Log("GAME OVER");

        Invoke("BackToMenu", 2f);
    }

    void BackToMenu()
    {
        SceneManager.LoadScene("GamePanel");
    }
}