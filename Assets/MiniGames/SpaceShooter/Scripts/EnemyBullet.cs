using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 8f;
    private Vector3 direction = Vector3.back;

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
    }

    void Start()
    {
        Destroy(gameObject, 4f);
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerShip>()?.TakeDamage(1);
            Destroy(gameObject);
        }
    }
}