using System;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HubFlowController : MonoBehaviour
{
    [Header("Main UI")]
    [SerializeField] private TMP_Text goldText;

    [Header("Status Panel")]
    [SerializeField] private TMP_Text bonusHpText;
    [SerializeField] private TMP_Text goldMultiplierText;
    [SerializeField] private TMP_Text moveSpeedText;
    [SerializeField] private TMP_Text damageText;
    [SerializeField] private TMP_Text battleHealText;
    [SerializeField] private TMP_Text supplyCountText;

    [Header("Shop")]
    [SerializeField] private ShopManager shopManager;

    [Header("Scene Names")]
    [SerializeField] private string combatSceneName = "TestCombat";
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private MethodInfo cachedSaveMethod;

    private void OnEnable()
    {
        GameSession.OnGoldChanged += HandleGoldChanged;
        GameSession.OnProgressionChanged += HandleProgressionChanged;
        RefreshAll();
    }

    private void OnDisable()
    {
        GameSession.OnGoldChanged -= HandleGoldChanged;
        GameSession.OnProgressionChanged -= HandleProgressionChanged;
    }

    private void HandleGoldChanged(int newGold)
    {
        RefreshAll();
    }

    private void HandleProgressionChanged()
    {
        RefreshAll();
    }

    public void RefreshAll()
    {
        RefreshGold();
        RefreshStatusPanel();
    }

    public void RefreshGold()
    {
        int currentGold = 0;

        if (GameSession.Instance != null)
        {
            currentGold = GameSession.Instance.CurrentGold;
        }

        if (goldText != null)
        {
            goldText.text = $"Gold: {currentGold}";
        }
    }

    public void RefreshStatusPanel()
    {
        if (GameSession.Instance == null)
        {
            if (bonusHpText != null) bonusHpText.text = "Max HP Bonus: +0";
            if (goldMultiplierText != null) goldMultiplierText.text = "Gold Multiplier: x1.0";
            if (moveSpeedText != null) moveSpeedText.text = "Move Speed Bonus: +0%";
            if (damageText != null) damageText.text = "Damage Bonus: +0%";
            if (battleHealText != null) battleHealText.text = "Battle Heal Ready: No";
            if (supplyCountText != null) supplyCountText.text = "Supply Count: 0";
            return;
        }

        if (bonusHpText != null)
            bonusHpText.text = $"Max HP Bonus: +{GameSession.Instance.BonusMaxHealth}";

        if (goldMultiplierText != null)
            goldMultiplierText.text = $"Gold Multiplier: x{GameSession.Instance.GoldGainMultiplier:0.0#}";

        if (moveSpeedText != null)
            moveSpeedText.text = $"Move Speed Bonus: +{GameSession.Instance.BonusMoveSpeedPercent * 100f:0}%";

        if (damageText != null)
            damageText.text = $"Damage Bonus: +{GameSession.Instance.BonusDamagePercent * 100f:0}%";

        if (battleHealText != null)
            battleHealText.text = $"Battle Heal Ready: {(GameSession.Instance.AutoRestoreHealthOnBattleStart ? "Yes" : "No")}";

        if (supplyCountText != null)
            supplyCountText.text = $"Supply Count: {GameSession.Instance.BattleSupplyCount}";
    }

    public void OnClickStartCombat()
    {
        TryAutoSave();
        SceneManager.LoadScene(combatSceneName);
    }

    public void OnClickBackToMenu()
    {
        TryAutoSave();
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void OnClickOpenShop()
    {
        if (shopManager != null)
        {
            shopManager.OpenShop();
        }
    }

    private void TryAutoSave()
    {
        try
        {
            if (cachedSaveMethod == null)
            {
                Type saveSystemType = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .FirstOrDefault(t => t.Name == "SaveSystem");

                if (saveSystemType != null)
                {
                    cachedSaveMethod = saveSystemType.GetMethod(
                        "SaveGame",
                        BindingFlags.Public | BindingFlags.Static);
                }
            }

            cachedSaveMethod?.Invoke(null, null);
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[HubFlowController] Auto save skipped: {ex.Message}");
        }
    }
}