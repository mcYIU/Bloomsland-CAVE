using System.Collections;
using UnityEngine;
using Votanic.vXR.vGear;

/// <summary>
/// FarmingZone manages the player's entry into the farming area, handles smooth transition to the stand point, and equips the hoe tool.
/// </summary>
public class FarmingZone : MonoBehaviour
{
    [Header("Scene Setup")]
    [SerializeField] private Transform standPoint; // Where the player stands to farm
    [SerializeField] private float transitionDuration = 1f; // Time to move player

    [Header("Interactables")]
    [SerializeField] private vGearInteractablesTemp hoe; // The hoe tool
    [SerializeField] private vGear_Controller wand; // The player's wand/controller

    private bool _isTriggered = false; // Has the transition started?
    private bool _isEntered = false; // Is the player in the zone?
    private Coroutine _transitionCoroutine;

    private void Update()
    {
        // Equip the hoe when the player presses the farm command
        if (_isEntered && (vGear.Cmd.Received("Farm") || Input.GetKeyUp(KeyCode.G)))
        {
            wand.Pick(hoe);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Start transition if a player enters
        if (other.TryGetComponent<vGear_User>(out var user))
        {
            StartTransition(user);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Drop the hoe when the player leaves
        if (other.TryGetComponent<vGear_User>(out _))
        {
            if (_isEntered)
            {
                _isEntered = false;
                wand.Drop();
            }
        }
    }

    // Start the coroutine to move the player to the stand point
    private void StartTransition(vGear_User user)
    {
        if (_isTriggered || _transitionCoroutine != null) return;

        _transitionCoroutine = StartCoroutine(TransitionUser(user));
    }

    // Smoothly move and rotate the player to the stand point
    private IEnumerator TransitionUser(vGear_User user)
    {
        Vector3 initialPosition = user.position;
        Quaternion initialRotation = user.rotation;
        float transitionTimer = 0.0f;

        while (transitionTimer < transitionDuration)
        {
            transitionTimer += Time.deltaTime;
            float t = transitionTimer / transitionDuration;

            user.transform.position = Vector3.Lerp(initialPosition, standPoint.position, t);
            user.transform.rotation = Quaternion.Lerp(initialRotation, standPoint.rotation, t);

            yield return null;
        }

        user.transform.position = standPoint.position;
        user.transform.rotation = standPoint.rotation;

        _isTriggered = true;
        _isEntered = true;
    }
}