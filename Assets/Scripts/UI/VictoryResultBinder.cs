using TMPro;
using UnityEngine;

public class VictoryResultBinder : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BattleResultTracker battleResultTracker;

    [Header("Texts")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text killsText;
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text wavesText;
    [SerializeField] private TMP_Text battleHealText;
    [SerializeField] private TMP_Text supplyPackText;
    [SerializeField] private TMP_Text killQuestText;
    [SerializeField] private TMP_Text goldQuestText;
    [SerializeField] private TMP_Text hintText;

    private void Awake()
    {
        if (battleResultTracker == null)
        {
            battleResultTracker = FindFirstObjectByType<BattleResultTracker>();
        }
    }

    public void RefreshResult()
    {
        if (battleResultTracker == null)
        {
            battleResultTracker = FindFirstObjectByType<BattleResultTracker>();
        }

        if (titleText != null)
        {
            titleText.text = "Battle Results";
        }

        if (battleResultTracker == null)
        {
            ApplyFallback();
            return;
        }

        battleResultTracker.PrepareSummary();

        if (killsText != null)
        {
            killsText.text = $"Kills: {battleResultTracker.KillsThisBattle}";
        }

        if (goldText != null)
        {
            goldText.text = $"Gold Gained: +{battleResultTracker.GoldGained}";
        }

        if (wavesText != null)
        {
            if (battleResultTracker.MaxWaveCount > 0)
            {
                wavesText.text = $"Waves Cleared: {battleResultTracker.WavesCleared}/{battleResultTracker.MaxWaveCount}";
            }
            else
            {
                wavesText.text = $"Waves Cleared: {battleResultTracker.WavesCleared}";
            }
        }

        if (battleHealText != null)
        {
            battleHealText.text = $"Battle Heal Used: {(battleResultTracker.UsedBattleHeal ? "Yes" : "No")}";
        }

        if (supplyPackText != null)
        {
            supplyPackText.text = $"Supply Pack Used: {(battleResultTracker.UsedSupplyPack ? "Yes" : "No")}";
        }

        if (killQuestText != null)
        {
            killQuestText.text =
                $"Kill Quest: {battleResultTracker.BuildQuestProgressSummary(QuestType.KillEnemies)}";
        }

        if (goldQuestText != null)
        {
            goldQuestText.text =
                $"Gold Quest: {battleResultTracker.BuildQuestProgressSummary(QuestType.CollectGold)}";
        }

        if (hintText != null)
        {
            hintText.text = "Return to Hub to claim rewards and continue progression.";
        }
    }

    private void ApplyFallback()
    {
        if (killsText != null) killsText.text = "Kills: 0";
        if (goldText != null) goldText.text = "Gold Gained: +0";
        if (wavesText != null) wavesText.text = "Waves Cleared: 0";
        if (battleHealText != null) battleHealText.text = "Battle Heal Used: No";
        if (supplyPackText != null) supplyPackText.text = "Supply Pack Used: No";
        if (killQuestText != null) killQuestText.text = "Kill Quest: Not tracked";
        if (goldQuestText != null) goldQuestText.text = "Gold Quest: Not tracked";

        if (hintText != null)
        {
            hintText.text = "Battle result data not found.";
        }
    }
}