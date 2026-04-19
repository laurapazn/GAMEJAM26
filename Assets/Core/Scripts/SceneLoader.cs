using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadWordGame()
    {
        SceneManager.LoadScene("WordGame");
    }

    public void LoadCubeGame()
    {
        SceneManager.LoadScene("CubeGame");
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}