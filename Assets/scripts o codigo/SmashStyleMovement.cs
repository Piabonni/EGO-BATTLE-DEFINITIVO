using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SmashStyleMovement : MonoBehaviour
{
    [Header("Teclas de Control")]
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Movimiento")]
    public float speed = 5f;

    [Header("Salto y Gravedad (Rigidbody)")]
    public float jumpForce = 7f;
    public float gravityMultiplier = 2f;

    [Header("Push")]
    [Tooltip("Fuerza de empuje al chocar contra un rival quieto")]
    public float pushForce = 3f;
    [Tooltip("Velocidad máxima del rival para considerarlo \"quieto\"")]
    public float maxTargetSpeed = 0.1f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.1f;
    public LayerMask groundMask;

    Rigidbody rb;
    bool isGrounded;
    float inputX;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void FixedUpdate()
    {
        // Aplica gravedad aumentada
        rb.AddForce(Physics.gravity * gravityMultiplier, ForceMode.Acceleration);
    }

    void Update()
    {
        // — Ground Check —
        isGrounded = Physics.Raycast(
            groundCheck.position,
            Vector3.down,
            groundDistance,
            groundMask);

        // — Captura input horizontal —
        inputX = 0f;
        if (Input.GetKey(leftKey)) inputX = -1f;
        if (Input.GetKey(rightKey)) inputX = 1f;

        // — Mueve solo en X —
        Vector3 vel = rb.velocity;
        vel.x = inputX * speed;
        rb.velocity = vel;

        // — Salto —
        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
        }
    }

    void OnCollisionStay(Collision col)
    {
        // Solo interesa empujar a otros chibis
        var other = col.gameObject.GetComponent<SmashStyleMovement>();
        if (other == null) return;

        // Ambos deben estar en suelo y este jugador moviéndose
        if (!isGrounded || Mathf.Approximately(inputX, 0f))
            return;

        // El otro debe estar en tierra y casi quieto
        var otherVel = other.rb.velocity;
        if (!other.isGrounded || Mathf.Abs(otherVel.x) > maxTargetSpeed)
            return;

        // Empuja al otro en X
        Vector3 push = new Vector3(inputX * pushForce, 0f, 0f);
        other.rb.AddForce(push, ForceMode.VelocityChange);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(
            groundCheck.position,
            groundCheck.position + Vector3.down * groundDistance);
    }
}
