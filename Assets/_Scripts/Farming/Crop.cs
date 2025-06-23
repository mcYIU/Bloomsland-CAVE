using UnityEngine;

/// <summary>
/// Crop manages the visual growth stages of a crop, randomizing height and rotation for each stage.
/// </summary>
public class Crop : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float minHeightOffset = -0.3f;
    [SerializeField] private float maxHeightOffset = 0.3f;
    [SerializeField] private float finalMinHeightOffset = -1f;
    [SerializeField] private float finalMaxHeightOffset = 1f;

    private void Start()
    {
        RandomizeHeightAndRotation(0); // Initialize with random height/rotation
    }

    // Show a specific growth stage and randomize its appearance
    public void Grow(int index)
    {
        if (index >= transform.childCount - 1) return;

        // Hide all children except the 1st child 
        for (int i = 0; i < transform.childCount - 1; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        // Show the current growth stage
        transform.GetChild(index).gameObject.SetActive(true);
        RandomizeHeightAndRotation(index);
    }

    // Randomize the height and rotation of a growth stage
    private void RandomizeHeightAndRotation(int index)
    {
        Transform growthStage = transform.GetChild(index);

        // Random rotation
        float randomRotation = Random.Range(0f, 360f);
        growthStage.rotation = Quaternion.Euler(0f, randomRotation, 0f);

        // Random height offset
        float heightOffset;
        if (index == transform.childCount - 1)
        {
            heightOffset = Random.Range(finalMinHeightOffset, finalMaxHeightOffset);
        }
        else
        {
            heightOffset = Random.Range(minHeightOffset, maxHeightOffset);
        }

        Vector3 currentPosition = growthStage.localPosition;
        growthStage.localPosition = new Vector3(
            currentPosition.x,
            currentPosition.y + heightOffset,
            currentPosition.z
        );
    }
}