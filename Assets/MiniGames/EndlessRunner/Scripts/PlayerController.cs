using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float forwardSpeed = 10f;
    public float laneDistance = 3f;
    public float jumpForce = 7f;
    public LayerMask groundLayer;

    private Rigidbody rb;
    private int desiredLane = 1; // 0 = izquierda, 1 = centro, 2 = derecha
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Movimiento hacia adelante
        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);

        // Cambiar carril
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            desiredLane = Mathf.Max(0, desiredLane - 1);

        if (Input.GetKeyDown(KeyCode.RightArrow))
            desiredLane = Mathf.Min(2, desiredLane + 1);

        // Salto
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        // Detectar suelo
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f, groundLayer);

        // Mover lateralmente
        Vector3 targetPosition = transform.position.z * Vector3.forward;

        if (desiredLane == 0)
            targetPosition += Vector3.left * laneDistance;
        else if (desiredLane == 2)
            targetPosition += Vector3.right * laneDistance;

        Vector3 moveVector = targetPosition - transform.position;
        rb.MovePosition(transform.position + moveVector * 10f * Time.fixedDeltaTime);
    }
}