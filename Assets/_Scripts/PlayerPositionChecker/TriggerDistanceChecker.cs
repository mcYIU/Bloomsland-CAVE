using TMPro;
using UnityEngine;
using Votanic.vXR.vCast;

/// <summary>
/// TriggerDistanceChecker enables or disables UI and effects based on the player's proximity to this object.
/// </summary>
public class TriggerDistanceChecker : MonoBehaviour
{
    public TextMeshProUGUI objectTag; // interactable tag to show/hide
    public ParticleSystem triggerParticle; // Particle effect to play/stop
    private Transform playerPos; // Reference to player position
    private float promixityDistance = 10.0f; // Distance threshold

    void Start()
    {
        playerPos = vCast.FindObjectOfType<vCast_User>().transform; // Find player
    }

    void Update()
    {
        // If player is close, hide tag and stop effect
        if (Vector3.Distance(transform.position, playerPos.position) < promixityDistance)
        {
            objectTag.enabled = false;
            triggerParticle.Clear();
        }
        else
        {
            objectTag.enabled = true;
            if (!triggerParticle.isPlaying) triggerParticle.Play();
        }
    }
}
