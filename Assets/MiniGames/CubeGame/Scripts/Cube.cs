using UnityEngine;

public class Cube : MonoBehaviour
{
    public bool isBad = false;

    private Vector3 direction;
    private float speed;

    void Start()
    {
        // 🎯 HITBOX MUY PEQUEÑA
        GetComponent<Collider>().transform.localScale *= 0.5f;

        direction = Random.insideUnitSphere;

        if (isBad)
            speed = Random.Range(7f, 12f);
        else
            speed = Random.Range(2f, 3f);
    }

    void Update()
    {
        if (isBad)
        {
            // 💀 IA: predice movimiento del mouse
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            if (plane.Raycast(ray, out float dist))
            {
                Vector3 target = ray.GetPoint(dist);

                // 🔥 anticipación
                Vector3 future = target + (target - transform.position) * 0.3f;

                Vector3 dir = (future - transform.position).normalized;
                transform.Translate(dir * speed * Time.deltaTime);
            }
        }
        else
        {
            transform.Translate(direction * speed * Time.deltaTime);
        }

        transform.Rotate(200 * Time.deltaTime, 200 * Time.deltaTime, 0);
    }

    void OnMouseDown()
    {
        var gm = FindFirstObjectByType<CubeGameManager>();
        if (gm == null) return;

        if (isBad)
        {
            if (gm.badSound != null)
                AudioSource.PlayClipAtPoint(gm.badSound, transform.position);

            gm.AddScore(-1);
        }
        else
        {
            if (gm.goodSound != null)
                AudioSource.PlayClipAtPoint(gm.goodSound, transform.position);

            gm.AddScore(1);
        }

        gm.PlayDestroyEffect(transform.position);
        Destroy(gameObject);
    }
}