using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum Type { TripleShot, Shield, SpeedBoost }
    public Type powerUpType;

    public float fallSpeed = 3f;

    void Update()
    {
        transform.Translate(Vector3.back * fallSpeed * Time.deltaTime);
        transform.Rotate(Vector3.up * 120f * Time.deltaTime);

        if (transform.position.z < -15f)
            Destroy(gameObject);
    }

    public void Apply(PlayerShip player)
    {
        Debug.Log("PowerUp: " + powerUpType);
    }
}