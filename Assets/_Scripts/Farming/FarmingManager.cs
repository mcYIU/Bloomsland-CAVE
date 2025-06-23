using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// FarmingManager tracks all farm plots, manages rice jar visuals, and plays dig/grow sounds.
/// </summary>
public class FarmingManager : MonoBehaviour
{
    private static FarmingManager _instance;
    public static FarmingManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<FarmingManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("FarmingManager");
                    _instance = go.AddComponent<FarmingManager>();
                }
            }
            return _instance;
        }
    }

    [Header("Farming Settings")]
    [SerializeField] private float _coolingTime = 1f; // Cooldown between digs
    [SerializeField] private AudioClip[] _digAudioClips; // Digging sounds
    [SerializeField] private AudioClip[] _growAudioClips; // Growing sounds

    [Header("Rice Jar")]
    [SerializeField] private GameObject[] _riceJars; // Rice jar visuals
    [SerializeField] private GameObject _visualEffect; // Effect for filling rice

    private Dictionary<Farming, bool> _farms = new Dictionary<Farming, bool>(); // Track farm completion
    private int _completedFarms = 0; // Number of completed farms
    private int _fillRiceIndex; // Farms per rice jar
    private int _multipleCounter = 1; // Tracks rice jar filling

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        if (_riceJars.Length > 0)
        {
            _fillRiceIndex = _farms.Count / _riceJars.Length;
        }
    }

    // Register a new farm plot
    public void RegisterFarm(Farming farm)
    {
        if (!_farms.ContainsKey(farm))
        {
            _farms.Add(farm, false);
        }
    }

    // Called when a farm is completed
    public static void OnFarmCompleted(Farming farm)
    {
        if (Instance._farms.ContainsKey(farm) && !Instance._farms[farm])
        {
            Instance._farms[farm] = true;
            Instance._completedFarms++;

            // Fill a rice jar at each milestone
            if (Instance._completedFarms == Instance._fillRiceIndex * Instance._multipleCounter && Instance._riceJars.Length > 0)
            {
                Instance.FillRiceJar();
            }
        }
    }

    // Play a random dig sound
    public void PlayDigSound()
    {
        if (_digAudioClips.Length > 0)
        {
            int randomIndex = Random.Range(0, _digAudioClips.Length);
            AudioManager.PlaySFX(_digAudioClips[randomIndex]);
        }
    }

    // Play a random grow sound
    public void PlayGrowSound()
    {
        if (_growAudioClips.Length > 0)
        {
            int randomIndex = Random.Range(0, _growAudioClips.Length);
            AudioManager.PlaySFX(_growAudioClips[randomIndex]);
        }
    }

    // Fill the next rice jar and play effect
    private void FillRiceJar()
    {
        foreach (var riceJar in _riceJars)
        {
            if (!riceJar.activeSelf)
            {
                Instantiate(_visualEffect, riceJar.transform);
                riceJar.SetActive(true);
                _multipleCounter++;
                break;
            }
        }
    }

    public float GetCoolingTime() => _coolingTime;
}