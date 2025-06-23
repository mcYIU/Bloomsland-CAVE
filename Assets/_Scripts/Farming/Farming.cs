using UnityEngine;

/// <summary>
/// Farming manages the growth stages of a farm plot, triggers crop visuals, and interacts with the poem system when fully grown.
/// </summary>
public class Farming : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Crop[] crops; // Array of crop visuals
    [SerializeField] private ParticleSystem farmingPoint; // Visual effect for farming

    [Header("Farm Poem")]
    private FarmPoemTrigger _farmPoem; // Reference to poem trigger

    [Header("State")]
    private bool isGrown; // Is the farm fully grown?
    private int _cropIndex = 0; // Current crop stage

    public bool IsGrown => isGrown;

    public enum FarmingStage
    {
        stage1,
        stage2,
        stage3,
        stage4,
        stage5
    }
    FarmingStage stage = FarmingStage.stage1;

    private void Start()
    {
        _farmPoem = FindObjectOfType<FarmPoemTrigger>();
    }

    // Called when the player interacts with the farm using the tagged tool ("Rake")
    public void SwitchFarmingStage(GameObject farmingTrigger)
    {
        if (!farmingTrigger.CompareTag("Rake") || isGrown) return;

        AudioManager.PlayGrowSound();
        _cropIndex++;

        // Update all crop visuals
        foreach (var crop in crops)
        {
            crop.Grow(_cropIndex);
        }

        stage++;
        // When fully grown, trigger poem and stop farming effect
        if (stage == FarmingStage.stage5 && _farmPoem)
        {
            _farmPoem.ShowText();
            farmingPoint.Stop();
            isGrown = true;
        }
    }
}