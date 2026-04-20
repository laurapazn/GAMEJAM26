using UnityEngine;

public class GearMovement : MonoBehaviour
{
    public float floatSpeed = 1f;
    public float floatHeight = 0.3f;
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float y = Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = startPos + new Vector3(0, y, 0);
        transform.Rotate(0, 90 * Time.deltaTime, 0);
    }
}