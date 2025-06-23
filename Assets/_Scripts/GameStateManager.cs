using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// GameStateManager controls the season and weather cycles in the game, updating visuals and audio accordingly.
/// </summary>
public class GameStateManager : MonoBehaviour
{
    private static GameStateManager _instance;
    public static GameStateManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameStateManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("GameStateManager");
                    _instance = go.AddComponent<GameStateManager>();
                }
            }
            return _instance;
        }
    }

    [Header("Season Settings")]
    [SerializeField] private Animator seasonAnimator; // Animator for season visuals
    [SerializeField] private string seasonParameterName; // Parameter name for season in animator
    [SerializeField] private float seasonChangeInterval = 40.0f; // Time between season changes
    [SerializeField] private ParticleSystem rainEffect; // Particle system for rain

    [Header("Weather Settings")]
    [SerializeField] private float minWeatherChangeTime = 30.0f; // Minimum time between weather changes
    [SerializeField] private float maxWeatherChangeTime = 60.0f; // Maximum time between weather changes

    private Season _currentSeason = Season.Spring; // Current season
    private Weather _currentWeather = Weather.Sunny; // Current weather
    private Coroutine _seasonCoroutine;
    private Coroutine _weatherCoroutine;

    // Static properties for other scripts to check current season/weather
    public static Season CurrentSeason => Instance._currentSeason;
    public static Weather CurrentWeather => Instance._currentWeather;

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

    private void Start()
    {
        // Start the season and weather cycles
        StartSeasonCycle();
        StartWeatherCycle();
    }

    private void Update()
    {
        // Reload the scene if R is pressed (for debugging)
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void StartSeasonCycle()
    {
        // Start or restart the season cycle coroutine
        if (_seasonCoroutine != null)
        {
            StopCoroutine(_seasonCoroutine);
        }
        _seasonCoroutine = StartCoroutine(SeasonCycle());
    }

    private void StartWeatherCycle()
    {
        // Start or restart the weather cycle coroutine
        if (_weatherCoroutine != null)
        {
            StopCoroutine(_weatherCoroutine);
        }
        _weatherCoroutine = StartCoroutine(WeatherCycle());
    }

    // Coroutine to cycle through seasons
    private IEnumerator SeasonCycle()
    {
        while (true)
        {
            int seasonIndex = (int)_currentSeason;
            seasonAnimator.SetInteger(seasonParameterName, seasonIndex); // Update visuals
            AudioManager.PlaySeasonAudio(seasonIndex); // Play season audio

            yield return new WaitForSeconds(seasonChangeInterval); // Wait before changing

            // Move to next season, loop back to Spring after Winter
            _currentSeason = seasonIndex >= (int)Season.Winter ? Season.Spring : (Season)(seasonIndex + 1);
            Debug.Log($"Season changed to {_currentSeason}");
        }
    }

    // Coroutine to cycle through weather states
    private IEnumerator WeatherCycle()
    {
        while (true)
        {
            // Only allow rain in Spring and Summer
            if (_currentSeason != Season.Autumn || _currentSeason != Season.Winter)
            {
                // Toggle between Sunny and Rainy
                _currentWeather = _currentWeather == Weather.Sunny ? Weather.Rainy : Weather.Sunny;

                if (_currentWeather == Weather.Rainy)
                {
                    rainEffect.Play(); // Show rain
                }
                else
                {
                    rainEffect.Stop(); // Hide rain
                }

                AudioManager.PlayWeatherAudio((int)_currentWeather); // Play weather audio
            }
            else
            {
                // In Autumn/Winter, always sunny, no rain
                _currentWeather = Weather.Sunny;
                rainEffect.Stop();
                AudioManager.PlayWeatherAudio((int)_currentWeather);
            }

            // Wait a random time before next weather change
            yield return new WaitForSeconds(Random.Range(minWeatherChangeTime, maxWeatherChangeTime));
        }
    }
}

public enum Season
{
    None,
    Spring,
    Summer,
    Autumn,
    Winter
}

public enum Weather
{
    None,
    Sunny,
    Rainy
}

public enum TimeofDay
{
    None,
    Day,
    Night
}