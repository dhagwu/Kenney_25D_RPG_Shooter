using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }
    public static event Action OnQuestDataChanged;

    [Header("Quest List")]
    [SerializeField] private List<QuestData> quests = new List<QuestData>();

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

        EnsureDefaultQuests();
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

    private void EnsureDefaultQuests()
    {
        if (quests == null)
        {
            quests = new List<QuestData>();
        }

        while (quests.Count < 3)
        {
            if (quests.Count == 0)
            {
                quests.Add(CreateKillQuest());
            }
            else if (quests.Count == 1)
            {
                quests.Add(CreateGoldQuest());
            }
            else if (quests.Count == 2)
            {
                quests.Add(CreateShopQuest());
            }
        }
    }

    private QuestData CreateKillQuest()
    {
        return new QuestData
        {
            questName = "First Hunt",
            description = "Kill 5 enemies.",
            questType = QuestType.KillEnemies,
            targetAmount = 5,
            rewardGold = 50,
            rewardBonusMaxHealth = 0,
            rewardSupplyCount = 0
        };
    }

    private QuestData CreateGoldQuest()
    {
        return new QuestData
        {
            questName = "Gold Collector",
            description = "Collect 100 gold.",
            questType = QuestType.CollectGold,
            targetAmount = 100,
            rewardGold = 75,
            rewardBonusMaxHealth = 10,
            rewardSupplyCount = 0
        };
    }

    private QuestData CreateShopQuest()
    {
        return new QuestData
        {
            questName = "Big Spender",
            description = "Buy 3 items in the Hub shop.",
            questType = QuestType.BuyShopItems,
            targetAmount = 3,
            rewardGold = 40,
            rewardBonusMaxHealth = 0,
            rewardSupplyCount = 1
        };
    }

    private void HandleEnemyDied(EnemyHealth enemy)
    {
        AddProgress(QuestType.KillEnemies, 1);
    }

    public void AddGoldProgress(int amount)
    {
        AddProgress(QuestType.CollectGold, amount);
    }

    public void AddShopPurchaseProgress(int amount = 1)
    {
        AddProgress(QuestType.BuyShopItems, amount);
    }

    public void AddProgress(QuestType questType, int amount)
    {
        if (amount <= 0) return;

        bool changed = false;

        for (int i = 0; i < quests.Count; i++)
        {
            QuestData quest = quests[i];

            if (quest.questType != questType) continue;
            if (quest.isCompleted) continue;

            int before = quest.currentAmount;
            bool wasCompleted = quest.isCompleted;

            quest.AddProgress(amount);

            if (quest.currentAmount != before)
            {
                changed = true;
                Debug.Log($"[QuestManager] {quest.questName} progress: {quest.currentAmount}/{quest.targetAmount}");
            }

            if (!wasCompleted && quest.isCompleted)
            {
                Debug.Log($"[QuestManager] Quest Completed: {quest.questName}");
            }
        }

        if (changed)
        {
            NotifyChanged();
        }
    }

    public bool ClaimReward(int questIndex, out string resultMessage)
    {
        resultMessage = string.Empty;

        if (questIndex < 0 || questIndex >= quests.Count)
        {
            resultMessage = "Invalid quest index.";
            return false;
        }

        QuestData quest = quests[questIndex];

        if (!quest.isCompleted)
        {
            resultMessage = "Quest not completed yet.";
            return false;
        }

        if (quest.rewardClaimed)
        {
            resultMessage = "Reward already claimed.";
            return false;
        }

        if (GameSession.Instance == null)
        {
            resultMessage = "GameSession not found.";
            return false;
        }

        List<string> rewardParts = new List<string>();

        if (quest.rewardGold > 0)
        {
            GameSession.Instance.AddGold(quest.rewardGold);
            rewardParts.Add($"{quest.rewardGold} Gold");
        }

        if (quest.rewardBonusMaxHealth > 0)
        {
            GameSession.Instance.AddBonusMaxHealth(quest.rewardBonusMaxHealth);
            rewardParts.Add($"+{quest.rewardBonusMaxHealth} Max HP");
        }

        if (quest.rewardSupplyCount > 0)
        {
            GameSession.Instance.AddBattleSupply(quest.rewardSupplyCount);
            rewardParts.Add($"Supply x{quest.rewardSupplyCount}");
        }

        quest.rewardClaimed = true;

        resultMessage = rewardParts.Count > 0
            ? $"Reward claimed: {quest.questName} -> {string.Join(", ", rewardParts)}"
            : $"Reward claimed: {quest.questName}";

        Debug.Log($"[QuestManager] {resultMessage}");

        NotifyChanged();
        return true;
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