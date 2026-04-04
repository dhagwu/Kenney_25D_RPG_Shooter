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

    public int CurrentGold => currentGold;
    public int BonusMaxHealth => bonusMaxHealth;
    public float GoldGainMultiplier => goldGainMultiplier;
    public bool AutoRestoreHealthOnBattleStart => autoRestoreHealthOnBattleStart;
    public int BattleSupplyCount => battleSupplyCount;

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

    private void NotifyAll()
    {
        OnGoldChanged?.Invoke(currentGold);
        OnProgressionChanged?.Invoke();
    }
}