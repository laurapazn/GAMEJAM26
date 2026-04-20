using UnityEngine;
using TMPro;

public class Palabra : MonoBehaviour
{
    public TextMeshPro texto;
    public string palabra;
    public bool esCorrecta;
    private bool tocada = false;
    public float velocidad = 2f;

    public void Configurar(string palabraTexto, bool correcta)
    {
        palabra = palabraTexto;
        esCorrecta = correcta;
        if (texto != null)
            texto.text = palabra.ToUpper();
        else
            Debug.LogError("Asigna el TextMeshPro en el prefab");
    }

    void Update()
    {
        if (tocada) return;
        transform.Translate(Vector3.down * velocidad * Time.deltaTime);
        if (transform.position.y < -6f)
        {
            if (esCorrecta) ControlJuego.Instance.PalabraPerdida();
            Destroy(gameObject);
        }
    }

    void OnMouseDown()
    {
        if (tocada) return;
        tocada = true;
        if (esCorrecta) ControlJuego.Instance.PalabraAcertada();
        else ControlJuego.Instance.PalabraErrada();
        Destroy(gameObject);
    }
}