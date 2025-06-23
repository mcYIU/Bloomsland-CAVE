using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// PoemData is a ScriptableObject that stores all information about a poem, including its content, conditions, and state.
/// </summary>
[CreateAssetMenu(fileName = "New Poem", menuName = "Poem/Poem Data")]
public class PoemData : ScriptableObject
{
    [Header("Content")]
    public string title; // Title of the poem
    public string sentences; // Main poem text
    public string description; // Description or explanation of the poem
    public AudioClip audio; // Audio clip for the poem

    [Header("Conditions")]
    public Season season; // Season required for this poem
    public Weather weather; // Weather required for this poem
    public TimeofDay timeOfDay; // Time of day required for this poem
    public Emotion emotion; // Emotion associated with this poem

    [Header("State")]
    public bool isShown; // Whether this poem has been shown already

    public enum Emotion
    {
        None,
        Confused,
        Feared,
        Relaxed,
        Sad
    }

    public void ResetState()
    {
        isShown = false;
    }
}
