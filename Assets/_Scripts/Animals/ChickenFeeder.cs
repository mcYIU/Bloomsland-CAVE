using System.Collections;
using TMPro;
using UnityEngine;

public class ChickenFeeder : MonoBehaviour
{
    [Header("Corn")]
    [SerializeField] private MeshRenderer corn;

    [Header("Chicken")]
    [SerializeField] private Animator[] chickens;

    [Header("Interactable")]
    [SerializeField] private vGearInteractablesTemp interactable;
    [SerializeField] private float coolingTime;

    [Header("Tag")]
    [SerializeField] private string objectTag; // Tag to display
    [SerializeField] private TextMeshProUGUI objectTagText; // UI for tag

    private bool isFeeding = false;

    private void Awake()
    {
        if (objectTagText != null) objectTagText.text = "";
    }

    public void OnWandSelect()
    {
        if (!isFeeding && objectTagText != null)
        {
            objectTagText.text = objectTag;
        }
    }

    public void OnWandDeselect()
    {
        if (!isFeeding && objectTagText != null)
        {
            objectTagText.text = "";
        }
    }

    // Start feeding process if not already feeding
    public void FeedChicken()
    {
        if (!isFeeding)
        {
            isFeeding = true;
            StartCoroutine(Feed());
            DisableTrigger();
        }
    }

    // Enable/disable chicken feeding animation
    private void ActiveChickenAnimation(bool _isActive)
    {
        foreach (Animator c in chickens) c.SetBool("IsFeeding", _isActive);
    }

    // Show/hide corn visual
    private void ActiveCorn(bool _isActive)
    {
        if (corn) corn.enabled = !_isActive;
    }

    // Coroutine for feeding process and cooldown
    private IEnumerator Feed()
    {
        ActiveChickenAnimation(isFeeding);
        ActiveCorn(isFeeding);

        yield return new WaitForSeconds(coolingTime);

        isFeeding = false;

        ActiveChickenAnimation(isFeeding);
        ActiveCorn(isFeeding);
        EnableTrigger();
    }

    private void EnableTrigger()
    {
        if (interactable != null) interactable.enabled = true;
        if (objectTagText != null) objectTagText.text = "";
    }

    private void DisableTrigger()
    {
        if (interactable != null) interactable.enabled = false;
        if (objectTagText != null) objectTagText.text = "";
    }
}