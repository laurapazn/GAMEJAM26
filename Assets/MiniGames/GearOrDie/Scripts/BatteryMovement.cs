using UnityEngine;

public class BatteryMovement : MonoBehaviour
{
    public float speed = 1.5f;
    private Vector3 target;

    void Start()
    {
        PickNewTarget();
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, target) < 0.5f)
            PickNewTarget();
    }

    void PickNewTarget()
    {
        target = new Vector3(
            Random.Range(-7f, 7f),
            Random.Range(0.5f, 3f),
            Random.Range(-7f, 7f)
        );
    }
}