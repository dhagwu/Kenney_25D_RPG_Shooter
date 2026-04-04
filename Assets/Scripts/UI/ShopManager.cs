using TMPro;
using UnityEngine;

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

    [Header("Gold Multiplier UI")]
    [SerializeField] private TMP_Text goldMultiplierPriceText;
    [SerializeField] private TMP_Text goldMultiplierInfoText;

    [Header("Battle Heal UI")]
    [SerializeField] private TMP_Text healPriceText;
    [SerializeField] private TMP_Text healInfoText;

    [Header("Supply Pack UI")]
    [SerializeField] private TMP_Text supplyPriceText;
    [SerializeField] private TMP_Text supplyInfoText;

    [Header("Costs")]
    [SerializeField] private int healthUpgradeCost = 50;
    [SerializeField] private int goldMultiplierCost = 75;
    [SerializeField] private int battleHealCost = 40;
    [SerializeField] private int supplyPackCost = 100;

    [Header("Values")]
    [SerializeField] private int healthUpgradeAmount = 20;
    [SerializeField] private float goldMultiplierAmount = 0.1f;
    [SerializeField] private int supplyPackAmount = 1;

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

        if (!GameSession.Instance.SpendGold(healthUpgradeCost))
        {
            SetFeedback("Not enough gold for Max HP upgrade.");
            return;
        }

        GameSession.Instance.AddBonusMaxHealth(healthUpgradeAmount);
        SetFeedback($"Purchased: Max HP +{healthUpgradeAmount}");
        RefreshUI();
    }

    public void BuyGoldMultiplierUpgrade()
    {
        if (!CanUseSession()) return;

        if (!GameSession.Instance.SpendGold(goldMultiplierCost))
        {
            SetFeedback("Not enough gold for Gold Gain upgrade.");
            return;
        }

        GameSession.Instance.AddGoldGainMultiplier(goldMultiplierAmount);
        SetFeedback($"Purchased: Gold Gain +{goldMultiplierAmount * 100f:0}%");
        RefreshUI();
    }

    public void BuyBattleHeal()
    {
        if (!CanUseSession()) return;

        if (!GameSession.Instance.SpendGold(battleHealCost))
        {
            SetFeedback("Not enough gold for Battle Heal.");
            return;
        }

        GameSession.Instance.SetAutoRestoreHealthOnBattleStart(true);
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
        SetFeedback($"Purchased: Supply Pack x{supplyPackAmount}");
        RefreshUI();
    }

    private bool CanUseSession()
    {
        if (GameSession.Instance != null)
            return true;

        SetFeedback("GameSession not found.");
        return false;
    }

    private void RefreshUI()
    {
        if (GameSession.Instance == null)
        {
            RefreshFallbackUI();
            return;
        }

        goldText.text = $"Gold: {GameSession.Instance.CurrentGold}";

        if (healthPriceText != null)
            healthPriceText.text = $"Price: {healthUpgradeCost} Gold";

        if (healthInfoText != null)
            healthInfoText.text =
                $"Effect: Max HP +{healthUpgradeAmount}\n" +
                $"Current Bonus: +{GameSession.Instance.BonusMaxHealth}";

        if (goldMultiplierPriceText != null)
            goldMultiplierPriceText.text = $"Price: {goldMultiplierCost} Gold";

        if (goldMultiplierInfoText != null)
            goldMultiplierInfoText.text =
                $"Effect: Gold Gain +{goldMultiplierAmount * 100f:0}%\n" +
                $"Current Multiplier: x{GameSession.Instance.GoldGainMultiplier:0.0#}";

        if (healPriceText != null)
            healPriceText.text = $"Price: {battleHealCost} Gold";

        if (healInfoText != null)
            healInfoText.text =
                $"Effect: Restore to full at battle start\n" +
                $"Ready: {(GameSession.Instance.AutoRestoreHealthOnBattleStart ? "Yes" : "No")}";

        if (supplyPriceText != null)
            supplyPriceText.text = $"Price: {supplyPackCost} Gold";

        if (supplyInfoText != null)
            supplyInfoText.text =
                $"Effect: +{supplyPackAmount} Supply Pack\n" +
                $"Current Supply: {GameSession.Instance.BattleSupplyCount}";
    }

    private void RefreshFallbackUI()
    {
        if (goldText != null)
            goldText.text = "Gold: 0";

        if (healthPriceText != null)
            healthPriceText.text = $"Price: {healthUpgradeCost} Gold";

        if (healthInfoText != null)
            healthInfoText.text = $"Effect: Max HP +{healthUpgradeAmount}";

        if (goldMultiplierPriceText != null)
            goldMultiplierPriceText.text = $"Price: {goldMultiplierCost} Gold";

        if (goldMultiplierInfoText != null)
            goldMultiplierInfoText.text = $"Effect: Gold Gain +{goldMultiplierAmount * 100f:0}%";

        if (healPriceText != null)
            healPriceText.text = $"Price: {battleHealCost} Gold";

        if (healInfoText != null)
            healInfoText.text = "Effect: Restore to full at battle start";

        if (supplyPriceText != null)
            supplyPriceText.text = $"Price: {supplyPackCost} Gold";

        if (supplyInfoText != null)
            supplyInfoText.text = $"Effect: +{supplyPackAmount} Supply Pack";
    }

    private void ShowHubPanel()
    {
        if (hubPanel != null)
            hubPanel.SetActive(true);

        if (shopPanel != null)
            shopPanel.SetActive(false);
    }

    private void ShowShopPanel()
    {
        if (hubPanel != null)
            hubPanel.SetActive(false);

        if (shopPanel != null)
            shopPanel.SetActive(true);
    }

    private void SetFeedback(string message)
    {
        if (feedbackText != null)
            feedbackText.text = message;

        Debug.Log($"[ShopManager] {message}");
    }

    private void ClearFeedback()
    {
        if (feedbackText != null)
            feedbackText.text = string.Empty;
    }
}