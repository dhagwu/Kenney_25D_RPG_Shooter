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
        currentGold = startingGold;
        OnGoldChanged?.Invoke(currentGold);
    }

    public void AddGold(int amount)
    {
        if (amount <= 0)
            return;

        currentGold += amount;
        OnGoldChanged?.Invoke(currentGold);

        Debug.Log($"Gold +{amount}, Current Gold = {currentGold}");
    }
}