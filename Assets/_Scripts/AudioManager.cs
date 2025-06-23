using UnityEngine;
using System.Collections;

/// <summary>
/// AudioManager handles all audio playback in the game, including ambient, SFX, and poem audio.
/// </summary>
public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;
    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<AudioManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("AudioManager");
                    _instance = go.AddComponent<AudioManager>();
                }
            }
            return _instance;
        }
    }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource readingSource; // For poem reading audio
    [SerializeField] private AudioSource poemEmotionSource; // For emotion SFX
    [SerializeField] private AudioSource seasonSource; // For season ambient audio
    [SerializeField] private AudioSource weatherSource; // For weather ambient audio
    [SerializeField] private AudioSource soundEffectSource; // For general SFX

    [Header("Audio Clips")]
    [SerializeField] private AudioClip[] seasonClips; // Clips for each season
    [SerializeField] private AudioClip[] weatherClips; // Clips for each weather
    [SerializeField] private EmotionSFX[] emotionSFX; // SFX for poem emotions
    [SerializeField] private AudioClip[] digClips; // SFX for digging
    [SerializeField] private AudioClip growClip; // SFX for growing

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Play the audio associated with a poem
    public static void PlayPoemAudio(PoemData poemData)
    {
        if (poemData?.audio != null)
        {
            Instance.readingSource.Stop();
            Instance.readingSource.PlayOneShot(poemData.audio);
        }
    }

    // Play a sound effect based on the poem's emotion
    public static void PlayEmotionSFX(PoemData.Emotion emotion)
    {
        foreach (var sfx in Instance.emotionSFX)
        {
            if (sfx.emotion == emotion)
            {
                Instance.poemEmotionSource.Stop();
                Instance.poemEmotionSource.PlayOneShot(sfx.audioClip[Random.Range(0, sfx.audioClip.Length)]);
                break;
            }
        }
    }

    // Play ambient audio for the current season
    public static void PlaySeasonAudio(int seasonIndex)
    {
        if (seasonIndex > 0 && seasonIndex < Instance.seasonClips.Length)
        {
            Instance.StartCoroutine(CrossFadeAudio(Instance.seasonSource, Instance.seasonClips[seasonIndex]));
        }
    }

    // Play ambient audio for the current weather
    public static void PlayWeatherAudio(int weatherIndex)
    {
        if (weatherIndex > 0 && weatherIndex < Instance.weatherClips.Length)
        {
            Instance.StartCoroutine(CrossFadeAudio(Instance.weatherSource, Instance.weatherClips[weatherIndex]));
        }
    }

    // Play a one-shot sound effect
    public static void PlaySFX(AudioClip sfx)
    {
        Instance.soundEffectSource.PlayOneShot(sfx);
    }

    // Play a random dig sound
    public static void PlayDigSound()
    {
        if (Instance.digClips.Length == 0) return;
        Instance.soundEffectSource.PlayOneShot(Instance.digClips[Random.Range(0, Instance.digClips.Length)]);
    }

    // Play the grow sound
    public static void PlayGrowSound()
    {
        if (Instance.growClip != null)
        {
            Instance.soundEffectSource.PlayOneShot(Instance.growClip);
        }
    }

    // Coroutine to cross-fade between audio clips
    private static IEnumerator CrossFadeAudio(AudioSource source, AudioClip newClip)
    {
        float fadeDuration = 4.0f;
        float timer = 0f;

        // Fade out current audio
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            source.volume = 1f - (timer / fadeDuration);
            yield return null;
        }

        // Change to new clip and play
        source.clip = newClip;
        source.Play();

        // Fade in new audio
        timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            source.volume = timer / fadeDuration;
            yield return null;
        }
    }
}