using UnityEngine;

/// <summary>
/// Fly moves a gameobject (bird) in a random forward direction at a set speed and destroys it if it leaves a defined area.
/// </summary>
public class Fly : MonoBehaviour
{
    public float speed; // Movement speed
    private Vector3 randomForwardDirection; // Chosen forward direction
    private Quaternion randomRotation; // Chosen rotation

    private void Start()
    {
        // Pick a random Y rotation and set the forward direction
        float randomAngle = Random.Range(-30, 30);
        randomRotation = Quaternion.Euler(0f, randomAngle, 0f);
        randomForwardDirection = randomRotation * Vector3.forward;
        transform.localRotation = randomRotation;
    }

    private void Update()
    {
        // Move in the chosen direction
        transform.Translate(randomForwardDirection * speed * Time.deltaTime);

        // Destroy if out of bounds
        if (transform.localPosition.x < -150 
            || transform.localPosition.x > 150
            || transform.localPosition.z < -150
            || transform.localPosition.z > 150)
        {
            Destroy(this.gameObject);
        }
    }
}
