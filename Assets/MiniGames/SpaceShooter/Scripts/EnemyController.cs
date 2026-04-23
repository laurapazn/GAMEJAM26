using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public enum MovementPattern { StraightDown, SineWave, ZigZag }

    public MovementPattern pattern;
    public float moveSpeed = 3f;
    public float amplitude = 2f;
    public float frequency = 2f;

    public int health = 2;
    public int scoreValue = 100;

    public GameObject enemyBulletPrefab;
    public Transform firePoint;
    public float fireRate = 2f;

    public GameObject explosionPrefab;

    private float startX;
    private float spawnTime;
    private float nextFireTime;

    void Start()
    {
        startX = transform.position.x;
        spawnTime = Time.time;
        nextFireTime = Time.time + fireRate;
    }

    void Update()
    {
        Move();
        Shoot();

        if (transform.position.z < -15f)
            Destroy(gameObject);
    }

    void Move()
    {
        float t = Time.time - spawnTime;
        Vector3 pos = transform.position;

        pos.z -= moveSpeed * Time.deltaTime;

        if (pattern == MovementPattern.SineWave)
            pos.x = startX + Mathf.Sin(t * frequency) * amplitude;

        if (pattern == MovementPattern.ZigZag)
            pos.x = startX + Mathf.Sign(Mathf.Sin(t * frequency)) * amplitude;

        transform.position = pos;
    }

    void Shoot()
    {
        if (Time.time >= nextFireTime && enemyBulletPrefab != null)
        {
            nextFireTime = Time.time + fireRate;

            GameObject b = Instantiate(enemyBulletPrefab, firePoint.position, Quaternion.identity);
            b.tag = "EnemyBullet";

            EnemyBullet eb = b.GetComponent<EnemyBullet>();
            if (eb != null)
                eb.SetDirection(Vector3.back);
        }
    }

    public void TakeDamage(int dmg)
    {
        health -= dmg;

        if (health <= 0)
            Die();
    }

    void Die()
    {
        if (explosionPrefab != null)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        SpaceGameManager.Instance.AddScore(scoreValue);
        Destroy(gameObject);
    }
}