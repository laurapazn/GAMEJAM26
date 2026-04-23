using UnityEngine;
using TMPro;

public class GameeManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    private int score;

    void Update()
    {
        score += 1;
        scoreText.text = "Score: " + score;
    }
}