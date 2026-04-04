using System;

[Serializable]
public enum QuestType
{
    KillEnemies,
    CollectGold
}

[Serializable]
public class QuestData
{
    public string questName;
    public string description;
    public QuestType questType;
    public int targetAmount;
    public int currentAmount;
    public int rewardGold;
    public int rewardBonusMaxHealth;
    public bool isCompleted;
    public bool rewardClaimed;

    public bool CanComplete => currentAmount >= targetAmount;

    public void ResetProgress()
    {
        currentAmount = 0;
        isCompleted = false;
        rewardClaimed = false;
    }

    public void AddProgress(int amount)
    {
        if (isCompleted) return;
        if (amount <= 0) return;

        currentAmount += amount;

        if (currentAmount >= targetAmount)
        {
            currentAmount = targetAmount;
            isCompleted = true;
        }
    }
}