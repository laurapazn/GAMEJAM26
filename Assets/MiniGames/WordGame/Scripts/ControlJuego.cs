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

    public List<Categoria> categorias;
    public GameObject prefabPalabra;

    public float rangoX = 6f;
    public float spawnY = 6f;
    public int palabrasPorSpawn = 20;
    public float intervalo = 0.25f;

    public int vidasIniciales = 5;
    public int puntajeObjetivo = 100;

    public TMP_Text textoPuntaje;
    public TMP_Text textoVidas;
    public TMP_Text textoCategoria;

    public GameObject panelGameOver;
    public TMP_Text textoMensaje;

    private int puntaje = 0;
    private int vidas;
    private bool juegoTerminado = false;

    private Categoria categoriaActual;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        vidas = vidasIniciales;

        if (panelGameOver != null)
            panelGameOver.SetActive(false);

        ElegirCategoria();
        InvokeRepeating(nameof(GenerarPalabras), 0.2f, intervalo);
        ActualizarUI();
    }

    void Update()
    {
        DetectarClick();
    }

    void DetectarClick()
    {
        if (juegoTerminado) return;

        if (Input.GetMouseButtonDown(0))
        {
            int layerMask = LayerMask.GetMask("Palabra");

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f, layerMask))
            {
                Palabra p = hit.collider.GetComponentInParent<Palabra>();

                if (p != null)
                    p.Tocar();
            }
        }
    }

    void ElegirCategoria()
    {
        categoriaActual = categorias[Random.Range(0, categorias.Count)];

        if (textoCategoria != null)
            textoCategoria.text = "Categoria: " + categoriaActual.nombre;
    }

    void GenerarPalabras()
    {
        if (vidas <= 0 || juegoTerminado) return;

        for (int i = 0; i < palabrasPorSpawn; i++)
        {
            float r = Random.value;

            bool esCorrecta = false;
            bool esTrampa = false;

            if (r > 0.65f)
                esCorrecta = true;
            else if (r < 0.2f)
                esTrampa = true;

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

            float posX = Random.Range(-rangoX, rangoX);

            Vector3 posicion = new Vector3(
                posX,
                spawnY + Random.Range(0f, 3f),
                0
            );

            GameObject obj = Instantiate(prefabPalabra, posicion, Quaternion.identity);

            if (obj == null) continue;

            Palabra p = obj.GetComponent<Palabra>();
            if (p != null)
            {
                p.Configurar(palabra, esCorrecta, esTrampa);
            }
        }
    }

    public void PalabraAcertada()
    {
        if (juegoTerminado) return;

        puntaje += 10;

        if (puntaje >= puntajeObjetivo)
        {
            TerminarJuego("GANASTE");
            return;
        }

        ActualizarUI();
    }

    public void PalabraErrada(int daño = 1)
    {
        if (juegoTerminado) return;

        vidas = Mathf.Max(0, vidas - daño);

        if (vidas == 0)
        {
            TerminarJuego("GAME OVER");
            return;
        }

        ActualizarUI();
    }

    public void PalabraPerdida()
    {
        if (juegoTerminado) return;

        vidas = Mathf.Max(0, vidas - 1);

        if (vidas == 0)
        {
            TerminarJuego("GAME OVER");
            return;
        }

        ActualizarUI();
    }

    void ActualizarUI()
    {
        if (textoPuntaje) textoPuntaje.text = "Score: " + puntaje;
        if (textoVidas) textoVidas.text = "Lives: " + vidas;
    }

    void TerminarJuego(string mensaje)
    {
        juegoTerminado = true;

        CancelInvoke();

        if (panelGameOver != null)
            panelGameOver.SetActive(true);

        if (textoMensaje != null)
            textoMensaje.text = mensaje;
    }

    public void Reiniciar()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}