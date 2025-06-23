using UnityEngine;

/// <summary>
/// FarmingTriggerComponent handles player interaction with farm plots, manages dig cooldown, and plays visual/audio effects.
/// </summary>
public class FarmingTriggerComponent : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float coolingTime = 1f; // Cooldown between digs

    private float _timer = 0f;
    private bool _isDigging = false; // Is currently digging?

    private void Update()
    {
        // Handle dig cooldown
        if (_isDigging)
        {
            _timer += Time.deltaTime;
            if (_timer > coolingTime)
            {
                _isDigging = false;
                _timer = 0f;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Only trigger if colliding with a Farming component and not on cooldown
        if (!other.TryGetComponent<Farming>(out var farming) || _isDigging)
        {
            return;
        }

        if (farming.IsGrown) return;

        PlayVisualEffects(other.gameObject);
        AudioManager.PlayDigSound();

        if (!_isDigging)
        {
            farming.SwitchFarmingStage(gameObject);
            _isDigging = true;
        }
    }

    // Play all particle effects on the farm object
    private void PlayVisualEffects(GameObject farmObject)
    {
        var digVisuals = farmObject.GetComponentsInChildren<ParticleSystem>();
        foreach (var visual in digVisuals)
        {
            visual.Play();
        }
    }
}