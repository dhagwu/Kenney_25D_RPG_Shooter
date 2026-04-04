using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    public static event Action OnQuestDataChanged;

    [Header("Quest List")]
    [SerializeField]
    private List<QuestData> quests = new List<QuestData>()
    {
        new QuestData
        {
            questName = "First Hunt",
            description = "Kill 5 enemies.",
            questType = QuestType.KillEnemies,
            targetAmount = 5,
            rewardGold = 50,
            rewardBonusMaxHealth = 0
        },
        new QuestData
        {
            questName = "Gold Collector",
            description = "Collect 100 gold.",
            questType = QuestType.CollectGold,
            targetAmount = 100,
            rewardGold = 75,
            rewardBonusMaxHealth = 10
        }
    };

    public IReadOnlyList<QuestData> Quests => quests;

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

    private void OnEnable()
    {
        EnemyHealth.OnEnemyDied += HandleEnemyDied;
    }

    private void OnDisable()
    {
        EnemyHealth.OnEnemyDied -= HandleEnemyDied;
    }

    private void Start()
    {
        NotifyChanged();
    }

    private void HandleEnemyDied()
    {
        AddProgress(QuestType.KillEnemies, 1);
    }

    public void AddGoldProgress(int amount)
    {
        AddProgress(QuestType.CollectGold, amount);
    }

    public void AddProgress(QuestType questType, int amount)
    {
        if (amount <= 0) return;

        bool changed = false;

        for (int i = 0; i < quests.Count; i++)
        {
            QuestData quest = quests[i];

            if (quest.questType != questType)
                continue;

            if (quest.isCompleted)
                continue;

            int before = quest.currentAmount;
            quest.AddProgress(amount);

            if (quest.currentAmount != before)
            {
                changed = true;
                Debug.Log($"[QuestManager] {quest.questName} progress: {quest.currentAmount}/{quest.targetAmount}");
            }

            if (quest.isCompleted)
            {
                Debug.Log($"[QuestManager] Quest Completed: {quest.questName}");
            }
        }

        if (changed)
        {
            NotifyChanged();
        }
    }

    public void ClaimReward(int questIndex)
    {
        if (questIndex < 0 || questIndex >= quests.Count)
            return;

        QuestData quest = quests[questIndex];

        if (!quest.isCompleted)
        {
            Debug.Log("[QuestManager] Reward claim failed: quest not completed.");
            return;
        }

        if (quest.rewardClaimed)
        {
            Debug.Log("[QuestManager] Reward claim failed: reward already claimed.");
            return;
        }

        if (GameSession.Instance != null)
        {
            if (quest.rewardGold > 0)
            {
                GameSession.Instance.AddGold(quest.rewardGold);
            }

            if (quest.rewardBonusMaxHealth > 0)
            {
                GameSession.Instance.AddBonusMaxHealth(quest.rewardBonusMaxHealth);
            }
        }

        quest.rewardClaimed = true;

        Debug.Log(
            $"[QuestManager] Reward claimed: {quest.questName}, " +
            $"Gold +{quest.rewardGold}, BonusMaxHealth +{quest.rewardBonusMaxHealth}"
        );

        NotifyChanged();
    }

    public void ResetAllQuests()
    {
        for (int i = 0; i < quests.Count; i++)
        {
            quests[i].ResetProgress();
        }

        NotifyChanged();
    }

    private void NotifyChanged()
    {
        OnQuestDataChanged?.Invoke();
    }
}