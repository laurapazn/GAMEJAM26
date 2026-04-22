using UnityEngine;
using TMPro;

public class Palabra : MonoBehaviour
{
    public TMP_Text texto;

    private bool esCorrecta;
    private bool tocada = false;

    public float velocidad = 5f; 

    public void Configurar(string palabraTexto, bool correcta)
    {
        esCorrecta = correcta;

        if (texto != null)
            texto.text = palabraTexto;
    }

    void Start()
    {
        
        BoxCollider col = GetComponent<BoxCollider>();
        if (col != null)
        {
            col.size = new Vector3(3f, 1.5f, 1f);
        }
    }

    void Update()
    {
        if (tocada) return;

        transform.Translate(Vector3.down * velocidad * Time.deltaTime);

        if (transform.position.y < -6f)
        {
            if (esCorrecta && ControlJuego.Instance != null)
                ControlJuego.Instance.PalabraPerdida();

            Destroy(gameObject);
        }
    }

    void OnMouseDown()
    {
        if (tocada) return;

        tocada = true;

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        if (ControlJuego.Instance != null)
        {
            if (esCorrecta)
                ControlJuego.Instance.PalabraAcertada();
            else
                ControlJuego.Instance.PalabraErrada();
        }

        Destroy(gameObject);
    }
}