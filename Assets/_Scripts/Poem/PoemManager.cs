using UnityEngine;
using TMPro;
using System.Collections;
using Votanic.vXR.vGear;

/// <summary>
/// PoemManager manages the display, animation, and state of poem reading in the game.
/// </summary>
public class PoemManager : MonoBehaviour
{
    private static PoemManager _instance;
    public static PoemManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PoemManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("TextManager");
                    _instance = go.AddComponent<PoemManager>();
                }
            }
            return _instance;
        }
    }

    [Header("Reading Settings")]
    [SerializeField] private float poemReadingTime = 10.0f; // Default time to show a poem
    [SerializeField] private float readingTimeIncrement = 6.0f; // Extra time if audio is present
    [SerializeField] private float descriptionReadingTime = 15.0f; // Time to show description
    [SerializeField] private float delayBetweenCharacters = 0.05f; // Typing animation speed
    [SerializeField] private Color defaultFontColor; // Color for poem text
    [SerializeField] private Color descriptionFontColor; // Color for description text
    [SerializeField] private float fadingThreshold = 1.0f; // Fade out duration
    [SerializeField] private float textLengthLimit = 30.0f; // Threshold for long text
    [SerializeField] private float textPositionThreshold = 100.0f; // Offset for long text

    // References to current UI elements and state
    private TextMeshProUGUI _currentCanvasText;
    private TextMeshProUGUI _currentTitleText;
    private Animator _currentInkAnimator;
    private Vector3 _initialCanvasTextPos;
    private PoemData _currentPoemData;
    private Coroutine _currentTextCoroutine;

    public static bool IsTexting { get; private set; } // Is a poem currently being read?

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

    private void Update()
    {
        // Listen for input to switch between poem and description while reading
        if (IsTexting)
        {
            if (Input.GetKeyDown(KeyCode.T) || vGear.Cmd.Received("Description"))
            {
                ShowDescription();
            }
            if (Input.GetKeyDown(KeyCode.Y) || vGear.Cmd.Received("Poem"))
            {
                ShowPoem();
            }
        }
    }

    // Start displaying a poem with animation and audio
    public static void StartText(Animator inkImage, TextMeshProUGUI text, TextMeshProUGUI title, PoemData poemData)
    {
        if (IsTexting) return; // Prevent overlapping

        Instance._currentInkAnimator = inkImage;
        Instance._currentCanvasText = text;
        Instance._currentTitleText = title;
        Instance._currentPoemData = poemData;

        if (Instance._currentInkAnimator != null)
        {
            Instance._currentInkAnimator.SetBool("OnEnable", true); // Show ink animation
        }

        if (Instance._currentTextCoroutine != null)
        {
            Instance.StopCoroutine(Instance._currentTextCoroutine);
        }
        Instance._currentTextCoroutine = Instance.StartCoroutine(Instance.ShowTextCoroutine());
    }

    // Switch to showing the poem's description
    private void ShowDescription()
    {
        if (!IsTexting || Instance._currentPoemData == null) return;

        if (Instance._currentTextCoroutine != null)
        {
            Instance.StopCoroutine(Instance._currentTextCoroutine);
        }
        Instance._currentTextCoroutine = Instance.StartCoroutine(Instance.ShowDescriptionCoroutine());
    }

    // Switch back to showing the poem text
    private void ShowPoem()
    {
        if (!IsTexting || _currentPoemData == null) return;

        if (_currentTextCoroutine != null)
        {
            StopCoroutine(_currentTextCoroutine);
        }
        Instance._currentTextCoroutine = StartCoroutine(ShowTextCoroutine());
    }

    // Coroutine to animate and display the poem text
    private IEnumerator ShowTextCoroutine()
    {
        IsTexting = true;

        Instance._currentCanvasText.color = Instance.defaultFontColor;

        // Reset or store the initial text position
        if (_initialCanvasTextPos != Vector3.zero)
        {
            _currentCanvasText.transform.localPosition = _initialCanvasTextPos;
        }
        else
        {
            _initialCanvasTextPos = _currentCanvasText.transform.localPosition;
        }

        string originalText = _currentPoemData.sentences;
        _currentCanvasText.text = "";
        _currentCanvasText.alpha = 0;

        // Offset text position if it's long
        if (originalText.Length > textLengthLimit)
        {
            _currentCanvasText.transform.localPosition += Vector3.right * textPositionThreshold;
        }

        yield return new WaitForSeconds(1f);

        // Play emotion SFX and poem audio
        AudioManager.PlayEmotionSFX(_currentPoemData.emotion);
        AudioManager.PlayPoemAudio(_currentPoemData);

        // Type out text character by character
        for (int i = 0; i < originalText.Length; i++)
        {
            char currentChar = originalText[i];
            _currentCanvasText.text += "<rotate=90>" + currentChar;
            _currentCanvasText.alpha += 0.1f;

            // Add line breaks for certain punctuation
            if (currentChar == '，' || currentChar == '。' || currentChar == '；' || currentChar == '？')
            {
                _currentCanvasText.text += "\n";
            }

            yield return new WaitForSeconds(delayBetweenCharacters);
        }

        _currentTitleText.text = _currentPoemData.title;

        // Wait for audio to finish or use default time
        float waitingTime = _currentPoemData.audio ? _currentPoemData.audio.length + readingTimeIncrement : poemReadingTime;
        yield return new WaitForSeconds(waitingTime);

        yield return StartCoroutine(FadeTextCoroutine());
    }

    // Coroutine to animate and display the poem's description
    private IEnumerator ShowDescriptionCoroutine()
    {
        _currentTitleText.text = string.Empty;

        string originalText = _currentPoemData.description;
        _currentCanvasText.text = "";
        _currentCanvasText.alpha = 0;

        Instance._currentCanvasText.color = Instance.descriptionFontColor;

        // Offset text position for description
        if (_currentCanvasText.transform.localPosition == _initialCanvasTextPos)
        {
            _currentCanvasText.transform.localPosition += Vector3.right * textPositionThreshold;
        }

        yield return new WaitForSeconds(1f);

        // Type out description character by character
        for (int i = 0; i < originalText.Length; i++)
        {
            char currentChar = originalText[i];
            _currentCanvasText.text += "<rotate=90>" + currentChar; // Characters are set in columns by right to left
            _currentCanvasText.alpha += 0.1f;

            yield return new WaitForSeconds(delayBetweenCharacters);
        }

        yield return new WaitForSeconds(descriptionReadingTime);
        yield return StartCoroutine(FadeTextCoroutine());
    }

    // Coroutine to fade out the text and reset state
    private IEnumerator FadeTextCoroutine()
    {
        if (_currentInkAnimator != null)
        {
            _currentInkAnimator.SetBool("OnEnable", false); // Hide ink animation
        }

        _currentTitleText.text = "";

        Color originalTextColor = _currentCanvasText.color;
        for (float t = fadingThreshold; t >= 0f; t -= Time.deltaTime)
        {
            Color newTextColor = originalTextColor;
            newTextColor.a = t;
            _currentCanvasText.color = newTextColor;
            yield return null;
        }

        _currentCanvasText.text = "";
        _currentCanvasText.transform.localPosition = _initialCanvasTextPos;
        _initialCanvasTextPos = Vector3.zero;

        IsTexting = false;
    }
}

[System.Serializable]
public class EmotionSFX
{
    public PoemData.Emotion emotion;
    public AudioClip[] audioClip;
}