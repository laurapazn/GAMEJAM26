using UnityEngine;

public class PlayerShip : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 8f;
    public float tiltAmount = 20f;
    public float tiltSpeed = 5f;

    [Header("Límites")]
    public float xLimit = 8f;
    public float yLimit = 5f;

    [Header("Disparo")]
    public GameObject bulletPrefab;
    public Transform[] firePoints;
    public float fireRate = 0.15f;

    [Header("Vida")]
    public int maxHealth = 3;
    public GameObject explosionEffect;

    private int currentHealth;
    private float nextFireTime;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        currentHealth = maxHealth;

        // 🔥 Actualiza UI al inicio
        SpaceGameManager.Instance.UpdateHealthUI(currentHealth);
    }

    void Update()
    {
        HandleMovement();
        HandleShooting();
    }

    void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 velocity = new Vector3(h, v, 0) * moveSpeed;
        rb.linearVelocity = velocity;

        float clampX = Mathf.Clamp(transform.position.x, -xLimit, xLimit);
        float clampY = Mathf.Clamp(transform.position.y, -yLimit, yLimit);
        transform.position = new Vector3(clampX, clampY, transform.position.z);

        float targetTilt = -h * tiltAmount;
        Quaternion targetRot = Quaternion.Euler(0f, 0f, targetTilt);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, tiltSpeed * Time.deltaTime);
    }

    void HandleShooting()
    {
        if ((Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0)) &&
            Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        foreach (Transform fp in firePoints)
        {
            Instantiate(bulletPrefab, fp.position, fp.rotation);
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        SpaceGameManager.Instance.UpdateHealthUI(currentHealth);

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        if (explosionEffect != null)
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

        SpaceGameManager.Instance.GameOver();
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyBullet"))
        {
            TakeDamage(1);
            Destroy(other.gameObject);
        }

        if (other.CompareTag("Enemy"))
        {
            TakeDamage(1);
        }

        if (other.CompareTag("PowerUp"))
        {
            PowerUp pu = other.GetComponent<PowerUp>();
            if (pu != null) pu.Apply(this);
            Destroy(other.gameObject);
        }
    }
}