using UnityEngine;
using System.Collections;
using TMPro;

/// <summary>
/// FarmPoemTrigger manages the display of poems as the player progresses through farm stages, controls rice visuals, and disables interaction when complete.
/// </summary>
public class FarmPoemTrigger : MonoBehaviour
{
    [Header("Farms")]
    [SerializeField] private Farming[] farms; // Array of farm plots
    [SerializeField] private PoemData[] poems; // Poems to display
    [SerializeField] private string title; // Title for all poems

    [Header("Text Display")]
    [SerializeField] private Animator inkImage; // Ink animation for poem
    [SerializeField] private TextMeshProUGUI canvasText; // Poem text UI
    [SerializeField] private TextMeshProUGUI titleText; // Poem title UI

    [Header("Interactables")]
    [SerializeField] private vGearInteractablesTemp[] interactables; // Interactable triggers

    [Header("Visual Effects")]
    [SerializeField] private GameObject[] rice; // Rice jar visuals
    [SerializeField] private GameObject visualEffect; // Effect to play when filling rice

    private int _poemIndex = 0; // Current poem index
    private int _fillRiceIndex; // How many farms per rice jar
    private int _multipleCounter = 1; // Tracks rice jar filling

    private void Start()
    {
        // Set the title for all poems
        foreach (var poem in poems)
        {
            poem.title = title;
        }

        // Calculate how many farms per rice jar
        if (rice.Length > 0)
        {
            _fillRiceIndex = farms.Length / rice.Length;
        }
    }

    // Show the next poem and update rice jars as needed
    public void ShowText()
    {
        if (_poemIndex >= poems.Length) return;

        PoemManager.StartText(inkImage, canvasText, titleText, poems[_poemIndex]);
        _poemIndex++;

        // Fill a rice jar at each milestone
        if (_poemIndex == _fillRiceIndex * _multipleCounter && rice.Length > 0)
        {
            FillRiceJar();
        }

        // When all farms are done, disable interaction and show final text
        if (_poemIndex == farms.Length)
        {
            DisableInteractables();
            StartCoroutine(ShowCompleteText());
        }
    }

    // Coroutine to show the final poem after a delay
    private IEnumerator ShowCompleteText()
    {
        yield return new WaitForSeconds(5f); // Default waiting time
        if (poems.Length > 0 && poems[0].audio != null)
        {
            yield return new WaitForSeconds(poems[0].audio.length);
        }
        ShowText();
    }

    // Fill the next rice jar and play effect
    private void FillRiceJar()
    {
        foreach (var riceObject in rice)
        {
            if (!riceObject.activeSelf)
            {
                Instantiate(visualEffect, riceObject.transform);
                riceObject.SetActive(true);
                _multipleCounter++;
                break;
            }
        }
    }

    // Disable all interactable triggers
    private void DisableInteractables()
    {
        foreach (var interactable in interactables)
        {
            interactable.Drop();
            interactable.enabled = false;
        }
    }
}