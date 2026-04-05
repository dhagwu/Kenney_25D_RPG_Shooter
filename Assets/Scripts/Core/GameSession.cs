using System;
using UnityEngine;

public class GameSession : MonoBehaviour
{
    public static GameSession Instance { get; private set; }

    public static event Action<int> OnGoldChanged;
    public static event Action OnProgressionChanged;

    [Header("Currency")]
    [SerializeField] private int currentGold = 0;

    [Header("Progression")]
    [SerializeField] private int bonusMaxHealth = 0;
    [SerializeField] private float goldGainMultiplier = 1f;
    [SerializeField] private bool autoRestoreHealthOnBattleStart = false;
    [SerializeField] private int battleSupplyCount = 0;

    [Header("Growth V2")]
    [SerializeField] private float bonusMoveSpeedPercent = 0f;
    [SerializeField] private float bonusDamagePercent = 0f;

    [Header("Shop Purchase Counts V2")]
    [SerializeField] private int healthUpgradePurchaseCount = 0;
    [SerializeField] private int goldGainUpgradePurchaseCount = 0;
    [SerializeField] private int moveSpeedUpgradePurchaseCount = 0;
    [SerializeField] private int damageUpgradePurchaseCount = 0;

    public int CurrentGold => currentGold;
    public int BonusMaxHealth => bonusMaxHealth;
    public float GoldGainMultiplier => goldGainMultiplier;
    public bool AutoRestoreHealthOnBattleStart => autoRestoreHealthOnBattleStart;
    public int BattleSupplyCount => battleSupplyCount;

    public float BonusMoveSpeedPercent => bonusMoveSpeedPercent;
    public float BonusDamagePercent => bonusDamagePercent;

    public int HealthUpgradePurchaseCount => healthUpgradePurchaseCount;
    public int GoldGainUpgradePurchaseCount => goldGainUpgradePurchaseCount;
    public int MoveSpeedUpgradePurchaseCount => moveSpeedUpgradePurchaseCount;
    public int DamageUpgradePurchaseCount => damageUpgradePurchaseCount;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ResetSession()
    {
        currentGold = 0;

        bonusMaxHealth = 0;
        goldGainMultiplier = 1f;
        autoRestoreHealthOnBattleStart = false;
        battleSupplyCount = 0;

        bonusMoveSpeedPercent = 0f;
        bonusDamagePercent = 0f;

        healthUpgradePurchaseCount = 0;
        goldGainUpgradePurchaseCount = 0;
        moveSpeedUpgradePurchaseCount = 0;
        damageUpgradePurchaseCount = 0;

        NotifyAll();
    }

    public void SetGold(int amount)
    {
        currentGold = Mathf.Max(0, amount);
        OnGoldChanged?.Invoke(currentGold);
    }

    public void AddGold(int amount)
    {
        if (amount <= 0) return;

        currentGold += amount;
        OnGoldChanged?.Invoke(currentGold);
    }

    public bool SpendGold(int amount)
    {
        if (amount <= 0) return false;
        if (currentGold < amount) return false;

        currentGold -= amount;
        OnGoldChanged?.Invoke(currentGold);
        return true;
    }

    public void AddBonusMaxHealth(int amount)
    {
        if (amount <= 0) return;

        bonusMaxHealth += amount;
        OnProgressionChanged?.Invoke();
    }

    public void AddGoldGainMultiplier(float amount)
    {
        if (amount <= 0f) return;

        goldGainMultiplier += amount;
        OnProgressionChanged?.Invoke();
    }

    public void SetAutoRestoreHealthOnBattleStart(bool value)
    {
        autoRestoreHealthOnBattleStart = value;
        OnProgressionChanged?.Invoke();
    }

    public void ConsumeAutoRestoreHealthOnBattleStart()
    {
        autoRestoreHealthOnBattleStart = false;
        OnProgressionChanged?.Invoke();
    }

    public void AddBattleSupply(int amount)
    {
        if (amount <= 0) return;

        battleSupplyCount += amount;
        OnProgressionChanged?.Invoke();
    }

    public bool ConsumeBattleSupply()
    {
        if (battleSupplyCount <= 0) return false;

        battleSupplyCount--;
        OnProgressionChanged?.Invoke();
        return true;
    }

    public void AddBonusMoveSpeedPercent(float amount)
    {
        if (amount <= 0f) return;

        bonusMoveSpeedPercent += amount;
        OnProgressionChanged?.Invoke();
    }

    public void AddBonusDamagePercent(float amount)
    {
        if (amount <= 0f) return;

        bonusDamagePercent += amount;
        OnProgressionChanged?.Invoke();
    }

    public void RegisterHealthUpgradePurchase()
    {
        healthUpgradePurchaseCount++;
        OnProgressionChanged?.Invoke();
    }

    public void RegisterGoldGainUpgradePurchase()
    {
        goldGainUpgradePurchaseCount++;
        OnProgressionChanged?.Invoke();
    }

    public void RegisterMoveSpeedUpgradePurchase()
    {
        moveSpeedUpgradePurchaseCount++;
        OnProgressionChanged?.Invoke();
    }

    public void RegisterDamageUpgradePurchase()
    {
        damageUpgradePurchaseCount++;
        OnProgressionChanged?.Invoke();
    }

    public GameSessionSaveData BuildSaveData()
    {
        return new GameSessionSaveData
        {
            currentGold = currentGold,
            bonusMaxHealth = bonusMaxHealth,
            goldGainMultiplier = goldGainMultiplier,
            autoRestoreHealthOnBattleStart = autoRestoreHealthOnBattleStart,
            battleSupplyCount = battleSupplyCount,

            bonusMoveSpeedPercent = bonusMoveSpeedPercent,
            bonusDamagePercent = bonusDamagePercent,

            healthUpgradePurchaseCount = healthUpgradePurchaseCount,
            goldGainUpgradePurchaseCount = goldGainUpgradePurchaseCount,
            moveSpeedUpgradePurchaseCount = moveSpeedUpgradePurchaseCount,
            damageUpgradePurchaseCount = damageUpgradePurchaseCount
        };
    }

    public void ApplySaveData(GameSessionSaveData data)
    {
        if (data == null)
        {
            Debug.LogWarning("[GameSession] ApplySaveData failed: data is null.");
            return;
        }

        currentGold = Mathf.Max(0, data.currentGold);
        bonusMaxHealth = Mathf.Max(0, data.bonusMaxHealth);
        goldGainMultiplier = Mathf.Max(1f, data.goldGainMultiplier);
        autoRestoreHealthOnBattleStart = data.autoRestoreHealthOnBattleStart;
        battleSupplyCount = Mathf.Max(0, data.battleSupplyCount);

        bonusMoveSpeedPercent = Mathf.Max(0f, data.bonusMoveSpeedPercent);
        bonusDamagePercent = Mathf.Max(0f, data.bonusDamagePercent);

        healthUpgradePurchaseCount = Mathf.Max(0, data.healthUpgradePurchaseCount);
        goldGainUpgradePurchaseCount = Mathf.Max(0, data.goldGainUpgradePurchaseCount);
        moveSpeedUpgradePurchaseCount = Mathf.Max(0, data.moveSpeedUpgradePurchaseCount);
        damageUpgradePurchaseCount = Mathf.Max(0, data.damageUpgradePurchaseCount);

        NotifyAll();
    }

    private void NotifyAll()
    {
        OnGoldChanged?.Invoke(currentGold);
        OnProgressionChanged?.Invoke();
    }
}