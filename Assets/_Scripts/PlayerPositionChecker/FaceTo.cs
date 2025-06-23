using UnityEngine;
using Votanic.vXR.vCast;

/// <summary>
/// FaceTo rotates this GameObject to always face the player.
/// </summary>
public class FaceTo : MonoBehaviour
{
    Transform playerPos; // Reference to player position

    private void Start()
    {
        playerPos = vCast.FindObjectOfType<vCast_User>().transform; // Find player
    }

    private void Update()
    {
        // Calculate direction and rotate to face player (with 180-degree flip)
        Vector3 directionToPlayer = playerPos.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        Quaternion reversedRotation = targetRotation * Quaternion.Euler(0f, 180f, 0f);
        transform.rotation = reversedRotation;
    }
}
