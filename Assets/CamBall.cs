using UnityEngine;

public class CamBall : MonoBehaviour 
{
    // Reference to the ball or spaceship that the camera will follow
    public Transform ballTarget;

    // Offset from the target to position the camera
    public Vector3 offset = new Vector3(0, 2, -5);

    // Smoothing factor for camera movement
    public float smoothSpeed = 5f;

    private Vector3 currentVelocity = Vector3.zero; // For SmoothDamp

    void Start()
    {
        // If no target is assigned in the inspector, try to find one
        if (ballTarget == null)
        {
            ballTarget = GameObject.FindGameObjectWithTag("Ball")?.transform;
        }

        // Verify that a target exists
        if (ballTarget == null)
        {
            Debug.LogError("No target assigned or found! Attach a target object or tag it with 'Ball'.");
        }
    }

    void LateUpdate()
    {
        // Only proceed if we have a target
        if (ballTarget == null) return;

        // Calculate the desired position with the offset
        Vector3 desiredPosition = ballTarget.position + ballTarget.rotation * offset;

        // Smoothly interpolate between current position and desired position
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, smoothSpeed * Time.deltaTime);

        // Set the camera's position
        transform.position = smoothedPosition;

    }
}
