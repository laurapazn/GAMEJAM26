using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class ControlJuego : MonoBehaviour
{
    public static ControlJuego Instance;

    [Header("Lista de palabras buenas y malas")]
    public List<string> palabrasCorrectas = new List<string> { "PERRO", "GATO", "RATON", "ELEFANTE" };
    public List<string> palabrasIncorrectas = new List<string> { "MESA", "ROJO", "COCHE", "AVION" };

    [Header("Spawner")]
    public GameObject prefabPalabra;
    public float rangoX = 5f;
    public float spawnY = 5f;
    public float intervalo = 2f;

    [Header("UI")]
    public TMP_Text textoPuntaje;
    public TMP_Text textoVidas;
    public TMP_Text textoCategoria;
    public GameObject panelGameOver;
    public TMP_Text textoMensaje;

    private int puntaje = 0;
    private int vidas = 3;
    private float velocidadActual = 2f;
    private float intervaloActual;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        intervaloActual = intervalo;
        ActualizarUI();
        if (panelGameOver) panelGameOver.SetActive(false);
        if (textoCategoria) textoCategoria.text = "Toca: " + palabrasCorrectas[0];
        InvokeRepeating("GenerarPalabra", 0f, intervaloActual);
    }

    void GenerarPalabra()
    {
        if (vidas <= 0) return;
        bool esCorrecta = Random.value > 0.5f;
        string palabra;
        if (esCorrecta)
            palabra = palabrasCorrectas[Random.Range(0, palabrasCorrectas.Count)];
        else
            palabra = palabrasIncorrectas[Random.Range(0, palabrasIncorrectas.Count)];
        Vector3 pos = new Vector3(Random.Range(-rangoX, rangoX), spawnY, 0);
        GameObject obj = Instantiate(prefabPalabra, pos, Quaternion.identity);
        Palabra p = obj.GetComponent<Palabra>();
        p.Configurar(palabra, esCorrecta);
        p.velocidad = velocidadActual;
    }

    public void PalabraAcertada()
    {
        if (vidas <= 0) return;
        puntaje += 10;
        velocidadActual += 0.2f;
        intervaloActual = Mathf.Max(0.6f, intervaloActual - 0.1f);
        CancelInvoke("GenerarPalabra");
        InvokeRepeating("GenerarPalabra", intervaloActual, intervaloActual);
        ActualizarUI();
        if (puntaje >= 20) TerminarJuego(true);
    }

    public void PalabraErrada()
    {
        if (vidas <= 0) return;
        vidas--;
        ActualizarUI();
        if (vidas <= 0) TerminarJuego(false);
    }

    public void PalabraPerdida()
    {
        if (vidas <= 0) return;
        vidas--;
        ActualizarUI();
        if (vidas <= 0) TerminarJuego(false);
    }

    void ActualizarUI()
    {
        if (textoPuntaje) textoPuntaje.text = "Score: " + puntaje;
        if (textoVidas) textoVidas.text = "Lives: " + vidas;
    }

    void TerminarJuego(bool victoria)
    {
        CancelInvoke("GenerarPalabra");
        if (panelGameOver)
        {
            panelGameOver.SetActive(true);
            if (textoMensaje) textoMensaje.text = victoria ? "¡GANASTE!" : "GAME OVER";
        }
    }

    public void Reiniciar()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Menu()
    {
        SceneManager.LoadScene("GamePanel");
    }
}