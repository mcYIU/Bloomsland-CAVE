using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Votanic.vXR.vGear;

/// <summary>
/// PoemTrigger is attached to world objects and manages player interaction for triggering poems.
/// </summary>
public class PoemTrigger : MonoBehaviour
{
    [Header("Visual Elements")]
    [SerializeField] private ParticleSystem triggerParticle; // Visual effect for trigger
    [SerializeField] private string objectTag; // Tag to display on selection
    [SerializeField] private TextMeshProUGUI objectTagText; // UI text for tag

    [Header("Text Display")]
    [SerializeField] private Animator inkImage; // Animator for ink effect
    [SerializeField] private TextMeshProUGUI text; // UI text for poem
    [SerializeField] private TextMeshProUGUI title; // UI text for poem title
    [SerializeField] private PoemData[] poems; // List of possible poems

    private List<PoemData> _availablePoems = new List<PoemData>(); // Poems that can be shown
    private List<PoemData> _unavailablePoems = new List<PoemData>(); // Poems that don't match conditions
    private vGear_Interactables _interactable; // Reference to interactable component

    private void Start()
    {
        _interactable = GetComponent<vGear_Interactables>();
        objectTagText.text = "";

        ResetPoemState(); // Reset all poems to not shown
    }

    // Reset all poems' shown state
    private void ResetPoemState()
    {
        foreach (var poem in poems)
            poem.ResetState();
    }

    // When player enters trigger, disable it to prevent re-triggering
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<vGear_User>() != null)
        {
            DisableTrigger();
        }
    }

    // When player exits trigger, re-enable it
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<vGear_User>() != null)
        {
            EnableTrigger();
        }
    }

    // Show tag when player points at object
    public void OnWandSelect()
    {
        if (!PoemManager.IsTexting)
        {
            objectTagText.text = objectTag;
        }
    }

    // Hide tag when player stops pointing
    public void OnWandDeselect()
    {
        if (!PoemManager.IsTexting)
        {
            objectTagText.text = "";
        }
    }

    // Trigger the poem reading process if allowed
    public void TriggerText()
    {
        if (!PoemManager.IsTexting)
        {
            PoemManager.StartText(inkImage, text, title, SelectPoem());
            DisableTrigger();
        }
    }

    // Enable the trigger for interaction
    private void EnableTrigger()
    {
        _interactable.enabled = true;
        objectTagText.text = "";
        triggerParticle.Play(); // Show visual effect
    }

    // Disable the trigger and set cooldown based on audio length
    private void DisableTrigger()
    {
        _interactable.enabled = false;
        objectTagText.text = "";
        triggerParticle.Stop(); // Hide visual effect

        if (PoemManager.IsTexting)
        {
            float waitingTime = 5f; // Default cooldown
            if (poems.Length > 0 && poems[0].audio != null)
            {
                waitingTime = poems[0].audio.length;
            }
            Invoke(nameof(EnableTrigger), waitingTime); // Re-enable after cooldown
        }
    }

    // Select a poem to show based on availability and conditions
    private PoemData SelectPoem()
    {
        _availablePoems.Clear();
        _unavailablePoems.Clear();

        foreach (var poem in poems)
        {
            if (IsPoemAvailable(poem))
            {
                if (!poem.isShown)
                {
                    _availablePoems.Add(poem);
                }
                else
                {
                    _unavailablePoems.Add(poem);
                }
            }
        }

        // Prefer showing new poems, otherwise repeat old ones
        if (_availablePoems.Count > 0)
        {
            int randomIndex = Random.Range(0, _availablePoems.Count);
            _availablePoems[randomIndex].isShown = true;
            return _availablePoems[randomIndex];
        }
        else if (_unavailablePoems.Count > 0)
        {
            int randomIndex = Random.Range(0, _unavailablePoems.Count);
            return _unavailablePoems[randomIndex];
        }
        else return null;
    }

    // Check if a poem matches the current season and weather
    private bool IsPoemAvailable(PoemData poem)
    {
        return (poem.season == Season.None || poem.season == (Season)GameStateManager.CurrentSeason) &&
               (poem.weather == Weather.None || poem.weather == (Weather)GameStateManager.CurrentWeather);
    }
}