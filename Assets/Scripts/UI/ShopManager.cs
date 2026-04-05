using System;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject hubPanel;
    [SerializeField] private GameObject shopPanel;

    [Header("Shop Header UI")]
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text feedbackText;

    [Header("Health Upgrade UI")]
    [SerializeField] private TMP_Text healthPriceText;
    [SerializeField] private TMP_Text healthInfoText;
    [SerializeField] private Button healthBuyButton;

    [Header("Gold Multiplier UI")]
    [SerializeField] private TMP_Text goldMultiplierPriceText;
    [SerializeField] private TMP_Text goldMultiplierInfoText;
    [SerializeField] private Button goldMultiplierBuyButton;

    [Header("Move Speed Upgrade UI")]
    [SerializeField] private TMP_Text moveSpeedPriceText;
    [SerializeField] private TMP_Text moveSpeedInfoText;
    [SerializeField] private Button moveSpeedBuyButton;

    [Header("Damage Upgrade UI")]
    [SerializeField] private TMP_Text damagePriceText;
    [SerializeField] private TMP_Text damageInfoText;
    [SerializeField] private Button damageBuyButton;

    [Header("Battle Heal UI")]
    [SerializeField] private TMP_Text healPriceText;
    [SerializeField] private TMP_Text healInfoText;
    [SerializeField] private Button healBuyButton;

    [Header("Supply Pack UI")]
    [SerializeField] private TMP_Text supplyPriceText;
    [SerializeField] private TMP_Text supplyInfoText;
    [SerializeField] private Button supplyBuyButton;

    [Header("Base Costs")]
    [SerializeField] private int healthBaseCost = 50;
    [SerializeField] private int goldGainBaseCost = 75;
    [SerializeField] private int moveSpeedBaseCost = 80;
    [SerializeField] private int damageBaseCost = 90;
    [SerializeField] private int battleHealCost = 40;
    [SerializeField] private int supplyPackCost = 100;

    [Header("Cost Growth")]
    [SerializeField] private float healthCostGrowth = 1.35f;
    [SerializeField] private float goldGainCostGrowth = 1.40f;
    [SerializeField] private float moveSpeedCostGrowth = 1.35f;
    [SerializeField] private float damageCostGrowth = 1.45f;

    [Header("Upgrade Values")]
    [SerializeField] private int healthUpgradeAmount = 20;
    [SerializeField] private float goldMultiplierAmount = 0.10f;
    [SerializeField] private float moveSpeedBonusPercent = 0.05f;
    [SerializeField] private float damageBonusPercent = 0.10f;
    [SerializeField] private int supplyPackAmount = 1;

    private MethodInfo cachedSaveMethod;

    private void OnEnable()
    {
        GameSession.OnGoldChanged += HandleGoldChanged;
        GameSession.OnProgressionChanged += HandleProgressionChanged;
        RefreshUI();
    }

    private void OnDisable()
    {
        GameSession.OnGoldChanged -= HandleGoldChanged;
        GameSession.OnProgressionChanged -= HandleProgressionChanged;
    }

    private void Start()
    {
        ShowHubPanel();
        RefreshUI();
    }

    private void HandleGoldChanged(int newGold)
    {
        RefreshUI();
    }

    private void HandleProgressionChanged()
    {
        RefreshUI();
    }

    public void OpenShop()
    {
        ShowShopPanel();
        ClearFeedback();
        RefreshUI();
    }

    public void CloseShop()
    {
        ShowHubPanel();
        ClearFeedback();
    }

    public void BuyHealthUpgrade()
    {
        if (!CanUseSession()) return;

        int cost = GetHealthUpgradeCost();
        if (!GameSession.Instance.SpendGold(cost))
        {
            SetFeedback("Not enough gold for Max HP upgrade.");
            return;
        }

        GameSession.Instance.AddBonusMaxHealth(healthUpgradeAmount);
        GameSession.Instance.RegisterHealthUpgradePurchase();
        NotifyQuestShopPurchase();
        TryAutoSave();

        SetFeedback($"Purchased: Max HP +{healthUpgradeAmount}");
        RefreshUI();
    }

    public void BuyGoldMultiplierUpgrade()
    {
        if (!CanUseSession()) return;

        int cost = GetGoldGainUpgradeCost();
        if (!GameSession.Instance.SpendGold(cost))
        {
            SetFeedback("Not enough gold for Gold Gain upgrade.");
            return;
        }

        GameSession.Instance.AddGoldGainMultiplier(goldMultiplierAmount);
        GameSession.Instance.RegisterGoldGainUpgradePurchase();
        NotifyQuestShopPurchase();
        TryAutoSave();

        SetFeedback($"Purchased: Gold Gain +{goldMultiplierAmount * 100f:0}%");
        RefreshUI();
    }

    public void BuyMoveSpeedUpgrade()
    {
        if (!CanUseSession()) return;

        int cost = GetMoveSpeedUpgradeCost();
        if (!GameSession.Instance.SpendGold(cost))
        {
            SetFeedback("Not enough gold for Move Speed upgrade.");
            return;
        }

        GameSession.Instance.AddBonusMoveSpeedPercent(moveSpeedBonusPercent);
        GameSession.Instance.RegisterMoveSpeedUpgradePurchase();
        NotifyQuestShopPurchase();
        TryAutoSave();

        SetFeedback($"Purchased: Move Speed +{moveSpeedBonusPercent * 100f:0}%");
        RefreshUI();
    }

    public void BuyDamageUpgrade()
    {
        if (!CanUseSession()) return;

        int cost = GetDamageUpgradeCost();
        if (!GameSession.Instance.SpendGold(cost))
        {
            SetFeedback("Not enough gold for Damage upgrade.");
            return;
        }

        GameSession.Instance.AddBonusDamagePercent(damageBonusPercent);
        GameSession.Instance.RegisterDamageUpgradePurchase();
        NotifyQuestShopPurchase();
        TryAutoSave();

        SetFeedback($"Purchased: Damage +{damageBonusPercent * 100f:0}%");
        RefreshUI();
    }

    public void BuyBattleHeal()
    {
        if (!CanUseSession()) return;

        if (GameSession.Instance.AutoRestoreHealthOnBattleStart)
        {
            SetFeedback("Battle Heal is already ready. Carry limit is 1.");
            return;
        }

        if (!GameSession.Instance.SpendGold(battleHealCost))
        {
            SetFeedback("Not enough gold for Battle Heal.");
            return;
        }

        GameSession.Instance.SetAutoRestoreHealthOnBattleStart(true);
        NotifyQuestShopPurchase();
        TryAutoSave();

        SetFeedback("Purchased: Battle Heal ready.");
        RefreshUI();
    }

    public void BuySupplyPack()
    {
        if (!CanUseSession()) return;

        if (!GameSession.Instance.SpendGold(supplyPackCost))
        {
            SetFeedback("Not enough gold for Supply Pack.");
            return;
        }

        GameSession.Instance.AddBattleSupply(supplyPackAmount);
        NotifyQuestShopPurchase();
        TryAutoSave();

        SetFeedback($"Purchased: Supply Pack x{supplyPackAmount}");
        RefreshUI();
    }

    private void NotifyQuestShopPurchase()
    {
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.AddShopPurchaseProgress(1);
        }
    }

    private bool CanUseSession()
    {
        if (GameSession.Instance != null) return true;

        SetFeedback("GameSession not found.");
        return false;
    }

    private int GetScaledPrice(int baseCost, float growthFactor, int purchaseCount)
    {
        return Mathf.RoundToInt(baseCost * Mathf.Pow(growthFactor, purchaseCount));
    }

    private int GetHealthUpgradeCost()
    {
        return GetScaledPrice(
            healthBaseCost,
            healthCostGrowth,
            GameSession.Instance != null ? GameSession.Instance.HealthUpgradePurchaseCount : 0);
    }

    private int GetGoldGainUpgradeCost()
    {
        return GetScaledPrice(
            goldGainBaseCost,
            goldGainCostGrowth,
            GameSession.Instance != null ? GameSession.Instance.GoldGainUpgradePurchaseCount : 0);
    }

    private int GetMoveSpeedUpgradeCost()
    {
        return GetScaledPrice(
            moveSpeedBaseCost,
            moveSpeedCostGrowth,
            GameSession.Instance != null ? GameSession.Instance.MoveSpeedUpgradePurchaseCount : 0);
    }

    private int GetDamageUpgradeCost()
    {
        return GetScaledPrice(
            damageBaseCost,
            damageCostGrowth,
            GameSession.Instance != null ? GameSession.Instance.DamageUpgradePurchaseCount : 0);
    }

    private void RefreshUI()
    {
        if (GameSession.Instance == null)
        {
            RefreshFallbackUI();
            return;
        }

        if (goldText != null)
            goldText.text = $"Gold: {GameSession.Instance.CurrentGold}";

        int healthCost = GetHealthUpgradeCost();
        int goldGainCost = GetGoldGainUpgradeCost();
        int moveSpeedCost = GetMoveSpeedUpgradeCost();
        int damageCost = GetDamageUpgradeCost();

        if (healthPriceText != null)
            healthPriceText.text = $"Price: {healthCost} Gold";

        if (healthInfoText != null)
            healthInfoText.text =
                $"Effect: Max HP +{healthUpgradeAmount}\n" +
                $"Purchased: {GameSession.Instance.HealthUpgradePurchaseCount}\n" +
                $"Current Bonus: +{GameSession.Instance.BonusMaxHealth}";

        if (goldMultiplierPriceText != null)
            goldMultiplierPriceText.text = $"Price: {goldGainCost} Gold";

        if (goldMultiplierInfoText != null)
            goldMultiplierInfoText.text =
                $"Effect: Gold Gain +{goldMultiplierAmount * 100f:0}%\n" +
                $"Purchased: {GameSession.Instance.GoldGainUpgradePurchaseCount}\n" +
                $"Current: x{GameSession.Instance.GoldGainMultiplier:0.00}";

        if (moveSpeedPriceText != null)
            moveSpeedPriceText.text = $"Price: {moveSpeedCost} Gold";

        if (moveSpeedInfoText != null)
            moveSpeedInfoText.text =
                $"Effect: Move Speed +{moveSpeedBonusPercent * 100f:0}%\n" +
                $"Purchased: {GameSession.Instance.MoveSpeedUpgradePurchaseCount}\n" +
                $"Current Bonus: +{GameSession.Instance.BonusMoveSpeedPercent * 100f:0}%";

        if (damagePriceText != null)
            damagePriceText.text = $"Price: {damageCost} Gold";

        if (damageInfoText != null)
            damageInfoText.text =
                $"Effect: Damage +{damageBonusPercent * 100f:0}%\n" +
                $"Purchased: {GameSession.Instance.DamageUpgradePurchaseCount}\n" +
                $"Current Bonus: +{GameSession.Instance.BonusDamagePercent * 100f:0}%";

        if (healPriceText != null)
            healPriceText.text = $"Price: {battleHealCost} Gold";

        if (healInfoText != null)
            healInfoText.text =
                "Effect: Restore to full at battle start\n" +
                $"Status: {(GameSession.Instance.AutoRestoreHealthOnBattleStart ? "Ready" : "Not Ready")}\n" +
                "Carry Limit: 1";

        if (supplyPriceText != null)
            supplyPriceText.text = $"Price: {supplyPackCost} Gold";

        if (supplyInfoText != null)
            supplyInfoText.text =
                $"Effect: Supply Pack x{supplyPackAmount}\n" +
                $"Current Supply: {GameSession.Instance.BattleSupplyCount}\n" +
                "Type: Stackable";

        RefreshButtonStates(healthCost, goldGainCost, moveSpeedCost, damageCost);
    }

    private void RefreshButtonStates(int healthCost, int goldGainCost, int moveSpeedCost, int damageCost)
    {
        if (GameSession.Instance == null)
        {
            SetButtonState(healthBuyButton, false);
            SetButtonState(goldMultiplierBuyButton, false);
            SetButtonState(moveSpeedBuyButton, false);
            SetButtonState(damageBuyButton, false);
            SetButtonState(healBuyButton, false);
            SetButtonState(supplyBuyButton, false);
            return;
        }

        int currentGold = GameSession.Instance.CurrentGold;

        SetButtonState(healthBuyButton, currentGold >= healthCost);
        SetButtonState(goldMultiplierBuyButton, currentGold >= goldGainCost);
        SetButtonState(moveSpeedBuyButton, currentGold >= moveSpeedCost);
        SetButtonState(damageBuyButton, currentGold >= damageCost);
        SetButtonState(
            healBuyButton,
            !GameSession.Instance.AutoRestoreHealthOnBattleStart &&
            currentGold >= battleHealCost);
        SetButtonState(supplyBuyButton, currentGold >= supplyPackCost);
    }

    private void SetButtonState(Button button, bool state)
    {
        if (button != null)
        {
            button.interactable = state;
        }
    }

    private void RefreshFallbackUI()
    {
        if (goldText != null) goldText.text = "Gold: 0";

        if (healthPriceText != null) healthPriceText.text = $"Price: {healthBaseCost} Gold";
        if (healthInfoText != null) healthInfoText.text = $"Effect: Max HP +{healthUpgradeAmount}";

        if (goldMultiplierPriceText != null) goldMultiplierPriceText.text = $"Price: {goldGainBaseCost} Gold";
        if (goldMultiplierInfoText != null) goldMultiplierInfoText.text = $"Effect: Gold Gain +{goldMultiplierAmount * 100f:0}%";

        if (moveSpeedPriceText != null) moveSpeedPriceText.text = $"Price: {moveSpeedBaseCost} Gold";
        if (moveSpeedInfoText != null) moveSpeedInfoText.text = $"Effect: Move Speed +{moveSpeedBonusPercent * 100f:0}%";

        if (damagePriceText != null) damagePriceText.text = $"Price: {damageBaseCost} Gold";
        if (damageInfoText != null) damageInfoText.text = $"Effect: Damage +{damageBonusPercent * 100f:0}%";

        if (healPriceText != null) healPriceText.text = $"Price: {battleHealCost} Gold";
        if (healInfoText != null) healInfoText.text = "Effect: Restore to full at battle start";

        if (supplyPriceText != null) supplyPriceText.text = $"Price: {supplyPackCost} Gold";
        if (supplyInfoText != null) supplyInfoText.text = $"Effect: Supply Pack x{supplyPackAmount}";

        SetButtonState(healthBuyButton, false);
        SetButtonState(goldMultiplierBuyButton, false);
        SetButtonState(moveSpeedBuyButton, false);
        SetButtonState(damageBuyButton, false);
        SetButtonState(healBuyButton, false);
        SetButtonState(supplyBuyButton, false);
    }

    private void ShowHubPanel()
    {
        if (hubPanel != null) hubPanel.SetActive(true);
        if (shopPanel != null) shopPanel.SetActive(false);
    }

    private void ShowShopPanel()
    {
        if (hubPanel != null) hubPanel.SetActive(false);
        if (shopPanel != null) shopPanel.SetActive(true);
    }

    private void SetFeedback(string message)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
        }

        Debug.Log($"[ShopManager] {message}");
    }

    private void ClearFeedback()
    {
        if (feedbackText != null)
        {
            feedbackText.text = string.Empty;
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
            Debug.LogWarning($"[ShopManager] Auto save skipped: {ex.Message}");
        }
    }
}