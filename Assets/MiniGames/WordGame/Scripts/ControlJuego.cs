using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class ControlJuego : MonoBehaviour
{
    public static ControlJuego Instance;

    [System.Serializable]
    public class Categoria
    {
        public string nombre;
        public List<string> palabras;
    }

    [Header("Categorias")]
    public List<Categoria> categorias;

    [Header("Prefab")]
    public GameObject prefabPalabra;

    [Header("Spawn Area")]
    public float rangoX = 4f;  
    public float spawnY = 5f;

    [Header("Cantidad y Frecuencia")]
    public int palabrasPorSpawn = 12;   
    public float intervalo = 0.4f;     

    [Header("Puntos")]
    public int puntosPorAcierto = 10;

    [Header("UI")]
    public TMP_Text textoPuntaje;
    public TMP_Text textoVidas;
    public TMP_Text textoCategoria;
    public GameObject panelGameOver;
    public TMP_Text textoMensaje;

    private int puntaje = 0;
    private int vidas = 3;

    private Categoria categoriaActual;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (panelGameOver) panelGameOver.SetActive(false);

        if (categorias == null || categorias.Count < 2)
        {
            Debug.LogError("Necesitas mínimo 2 categorías");
            return;
        }

        if (prefabPalabra == null)
        {
            Debug.LogError("No asignaste el prefab");
            return;
        }

        ElegirCategoria();
        InvokeRepeating(nameof(GenerarPalabras), 0.5f, intervalo);
        ActualizarUI();
    }

    void ElegirCategoria()
    {
        categoriaActual = categorias[Random.Range(0, categorias.Count)];

        if (textoCategoria != null)
            textoCategoria.text = "Toca: " + categoriaActual.nombre;
    }

    void GenerarPalabras()
    {
        if (vidas <= 0) return;

        float espacio = (rangoX * 2) / palabrasPorSpawn;

        for (int i = 0; i < palabrasPorSpawn; i++)
        {
            bool esCorrecta = Random.value > 0.5f;
            string palabra;

            if (esCorrecta)
            {
                palabra = categoriaActual.palabras[
                    Random.Range(0, categoriaActual.palabras.Count)
                ];
            }
            else
            {
                Categoria otra;
                do
                {
                    otra = categorias[Random.Range(0, categorias.Count)];
                } while (otra == categoriaActual);

                palabra = otra.palabras[
                    Random.Range(0, otra.palabras.Count)
                ];
            }

            float posX = -rangoX + (espacio * i) + espacio / 2;

            Vector3 posicion = new Vector3(
                posX,
                spawnY + Random.Range(0f, 1.5f),
                0
            );

            GameObject obj = Instantiate(prefabPalabra, posicion, Quaternion.identity);

            Palabra p = obj.GetComponent<Palabra>();
            if (p != null)
            {
                p.Configurar(palabra, esCorrecta);
            }
        }
    }

    public void PalabraAcertada()
    {
        puntaje += puntosPorAcierto;
        ActualizarUI();
    }

    public void PalabraErrada()
    {
        vidas = Mathf.Max(0, vidas - 1);
        ActualizarUI();

        if (vidas == 0)
            TerminarJuego();
    }

    public void PalabraPerdida()
    {
        vidas = Mathf.Max(0, vidas - 1);
        ActualizarUI();

        if (vidas == 0)
            TerminarJuego();
    }

    void ActualizarUI()
    {
        if (textoPuntaje) textoPuntaje.text = "Score: " + puntaje;
        if (textoVidas) textoVidas.text = "Lives: " + vidas;
    }

    void TerminarJuego()
    {
        CancelInvoke();

        if (panelGameOver)
        {
            panelGameOver.SetActive(true);
            if (textoMensaje) textoMensaje.text = "GAME OVER";
        }
    }

    public void Reiniciar()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}