using UnityEngine;

public class ClickableItem : MonoBehaviour
{
    private GameManager manager;

    void Start()
    {
        manager = FindFirstObjectByType<GameManager>();

        // Asegurar collider
        if (GetComponent<Collider>() == null)
        {
            BoxCollider collider = gameObject.AddComponent<BoxCollider>();
            if (GetComponent<Renderer>() != null)
            {
                collider.size = GetComponent<Renderer>().bounds.size;
            }
        }
    }

    void OnMouseDown()
    {
        if (manager == null)
        {
            manager = FindFirstObjectByType<GameManager>();
            if (manager == null) return;
        }

        string nombre = gameObject.name.ToLower();

        if (nombre.Contains("gear") || nombre.Contains("engranaje"))
        {
            manager.HitGear(transform.position);
        }
        else
        {
            manager.HitBattery(transform.position);
        }

        Destroy(gameObject);
    }
}