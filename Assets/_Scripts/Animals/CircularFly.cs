using UnityEngine;

/// <summary>
/// CircularFly moves a GameObject (bird) along a set of patrol points in a loop, facing each point as it moves.
/// </summary>
public class CircularFly : MonoBehaviour
{
    public Transform[] patrolPoints; // Array of patrol points
    public float speed = 2f; // Speed of movement

    private int currentPointIndex = 0; // Index of the current patrol point

    private void Update()
    {
        // Get the current target patrol point
        Transform currentPoint = patrolPoints[currentPointIndex];

        // Move towards the current target patrol point
        transform.position = Vector3.MoveTowards(transform.position, currentPoint.position, speed * Time.deltaTime);

        // Rotate to face the current target patrol point
        Vector3 direction = currentPoint.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, speed * Time.deltaTime);

        // Check if the GameObject has reached the current target patrol point
        if (Vector3.Distance(transform.position, currentPoint.position) < 0.1f)
        {
            // Move to the next patrol point
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
        }
    }
}