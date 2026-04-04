using UnityEngine;

public class BattleSessionApplier : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerCurrency playerCurrency;

    [Header("Supply Pack Settings")]
    [SerializeField] private int supplyPackBonusGold = 25;

    private void Start()
    {
        ApplyBattleStartEffects();
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
        if (!GameSession.Instance.AutoRestoreHealthOnBattleStart)
            return;

        if (playerHealth != null)
        {
            playerHealth.RestoreToFull();
            Debug.Log("[BattleSessionApplier] Applied Battle Heal -> RestoreToFull");
        }

        GameSession.Instance.ConsumeAutoRestoreHealthOnBattleStart();
    }

    private void ApplySupplyPack()
    {
        bool consumed = GameSession.Instance.ConsumeBattleSupply();

        if (!consumed)
            return;

        if (playerCurrency != null)
        {
            playerCurrency.AddGold(supplyPackBonusGold);
            Debug.Log($"[BattleSessionApplier] Applied Supply Pack -> bonusGold={supplyPackBonusGold}");
        }
    }
}