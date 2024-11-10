using UnityEngine;

public class Bird : MonoBehaviour
{
    public float flightSpeed = 5f;          // Base speed of the bird
    public float glideSpeed = 2f;           // Speed when gliding
    public float rotationSpeed = 50f;       // Speed of directional changes
    public float diveSpeed = 15f;           // Speed when diving
    public float liftForce = 2f;            // Lifting force to simulate gliding
    
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;             // Gravity can be manually applied during diving
    }

    void Update()
    {
        HandleFlight();
        HandleRotation();
    }

    private void HandleFlight()
    {
        // Apply forward flight speed
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? glideSpeed : flightSpeed;
        rb.linearVelocity = transform.forward * currentSpeed;

        // Add lift to maintain gliding
        if (Input.GetKey(KeyCode.LeftShift))
        {
            rb.AddForce(Vector3.up * liftForce, ForceMode.Acceleration);
        }

        // Dive if 'S' key is pressed
        if (Input.GetKey(KeyCode.S))
        {
            rb.linearVelocity += Vector3.down * diveSpeed * Time.deltaTime;
        }
    }

    private void HandleRotation()
    {
        // Rotate left and right with horizontal input (A/D or arrow keys)
        float horizontalInput = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;
        float verticalInput = Input.GetAxis("Vertical") * rotationSpeed * Time.deltaTime;

        // Pitch (up/down) and yaw (left/right)
        transform.Rotate(verticalInput, horizontalInput, 0);
    }
}
