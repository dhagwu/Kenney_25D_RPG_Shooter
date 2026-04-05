using UnityEngine;

public class BattleSessionApplier : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerCurrency playerCurrency;
    [SerializeField] private BattleResultTracker battleResultTracker;

    [Header("Supply Pack Settings")]
    [SerializeField] private int supplyPackBonusGold = 25;

    private void Start()
    {
        AutoFindReferencesIfNeeded();
        ApplyBattleStartEffects();
    }

    private void AutoFindReferencesIfNeeded()
    {
        if (playerHealth == null)
        {
            playerHealth = FindFirstObjectByType<PlayerHealth>();
        }

        if (playerCurrency == null)
        {
            playerCurrency = FindFirstObjectByType<PlayerCurrency>();
        }

        if (battleResultTracker == null)
        {
            battleResultTracker = FindFirstObjectByType<BattleResultTracker>();
        }
    }

    private void ApplyBattleStartEffects()
    {
        if (GameSession.Instance == null)
        {
            Debug.LogWarning("[BattleSessionApplier] GameSession not found.");
            return;
        }

        ApplyBattleHeal();
        ApplySupplyPack();
    }

    private void ApplyBattleHeal()
    {
        if (!GameSession.Instance.AutoRestoreHealthOnBattleStart) return;

        if (playerHealth != null)
        {
            playerHealth.RestoreToFull();
            Debug.Log("[BattleSessionApplier] Applied Battle Heal -> RestoreToFull");
        }

        if (battleResultTracker != null)
        {
            battleResultTracker.MarkBattleHealUsed();
        }

        GameSession.Instance.ConsumeAutoRestoreHealthOnBattleStart();
    }

    private void ApplySupplyPack()
    {
        bool consumed = GameSession.Instance.ConsumeBattleSupply();
        if (!consumed) return;

        if (battleResultTracker != null)
        {
            battleResultTracker.MarkSupplyPackUsed();
        }

        if (playerCurrency != null)
        {
            playerCurrency.AddGold(supplyPackBonusGold);
            Debug.Log($"[BattleSessionApplier] Applied Supply Pack -> bonusGold={supplyPackBonusGold}");
        }
    }
}