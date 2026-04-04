using System;
using UnityEngine;

public class PlayerCurrency : MonoBehaviour
{
    public static event Action<int> OnGoldChanged;

    [SerializeField] private int startingGold = 0;

    private int currentGold;
    public int CurrentGold => currentGold;

    private void Awake()
    {
        if (GameSession.Instance != null)
        {
            currentGold = GameSession.Instance.CurrentGold;
        }
        else
        {
            currentGold = startingGold;
        }

        OnGoldChanged?.Invoke(currentGold);

        Debug.Log($"[PlayerCurrency] Init -> currentGold={currentGold}");
    }

    public void AddGold(int amount)
    {
        if (amount <= 0) return;

        int finalAmount = amount;

        if (GameSession.Instance != null)
        {
            finalAmount = Mathf.RoundToInt(amount * GameSession.Instance.GoldGainMultiplier);
        }

        finalAmount = Mathf.Max(1, finalAmount);

        currentGold += finalAmount;

        if (GameSession.Instance != null)
        {
            GameSession.Instance.SetGold(currentGold);
        }

        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.AddGoldProgress(finalAmount);
        }

        OnGoldChanged?.Invoke(currentGold);

        Debug.Log(
            $"[PlayerCurrency] AddGold -> " +
            $"base={amount}, " +
            $"multiplier={(GameSession.Instance != null ? GameSession.Instance.GoldGainMultiplier : 1f)}, " +
            $"final={finalAmount}, " +
            $"currentGold={currentGold}"
        );
    }

    public bool SpendGold(int amount)
    {
        if (amount <= 0) return false;
        if (currentGold < amount) return false;

        currentGold -= amount;

        if (GameSession.Instance != null)
        {
            GameSession.Instance.SetGold(currentGold);
        }

        OnGoldChanged?.Invoke(currentGold);

        Debug.Log($"[PlayerCurrency] SpendGold -> amount={amount}, currentGold={currentGold}");
        return true;
    }
}