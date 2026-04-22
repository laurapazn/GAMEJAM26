using UnityEngine;
using TMPro;

public class Palabra : MonoBehaviour
{
    public TMP_Text texto;

    private bool esCorrecta;
    private bool esTrampa;
    private bool tocada = false;

    public float velocidad = 4f;

    public void Configurar(string palabraTexto, bool correcta, bool trampa)
    {
        esCorrecta = correcta;
        esTrampa = trampa;

        if (texto != null)
        {
            texto.text = palabraTexto;

            if (esTrampa)
                texto.color = Color.red;
            else if (esCorrecta)
                texto.color = Color.green;
            else
                texto.color = Color.white;
        }
    }

    void Start()
    {
        BoxCollider col = GetComponent<BoxCollider>();
        if (col != null)
            col.size = new Vector3(4f, 2f, 1f);
    }

    void Update()
    {
        if (tocada) return;

        transform.Translate(Vector3.down * velocidad * Time.deltaTime);

        if (transform.position.y < -7f)
        {
            if (esCorrecta && ControlJuego.Instance != null)
                ControlJuego.Instance.PalabraPerdida();

            Destroy(gameObject);
        }
    }

    public void Tocar()
    {
        if (tocada) return;

        tocada = true;

        if (ControlJuego.Instance != null)
        {
            if (esCorrecta)
                ControlJuego.Instance.PalabraAcertada();
            else if (esTrampa)
                ControlJuego.Instance.PalabraErrada(2);
            else
                ControlJuego.Instance.PalabraErrada(1);
        }

        Destroy(gameObject);
    }
}