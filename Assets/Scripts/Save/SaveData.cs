using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public GameSessionSaveData gameSession = new GameSessionSaveData();
    public List<QuestSaveData> quests = new List<QuestSaveData>();
}

[Serializable]
public class GameSessionSaveData
{
    public int currentGold;
    public int bonusMaxHealth;
    public float goldGainMultiplier = 1f;
    public bool autoRestoreHealthOnBattleStart;
    public int battleSupplyCount;

    public float bonusMoveSpeedPercent;
    public float bonusDamagePercent;

    public int healthUpgradePurchaseCount;
    public int goldGainUpgradePurchaseCount;
    public int moveSpeedUpgradePurchaseCount;
    public int damageUpgradePurchaseCount;
}

[Serializable]
public class QuestSaveData
{
    public string questName;
    public QuestType questType;
    public int currentAmount;
    public bool isCompleted;
    public bool rewardClaimed;
}