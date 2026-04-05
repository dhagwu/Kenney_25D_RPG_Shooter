using UnityEngine;

public class BattleResultTracker : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private bool logSummary = true;

    private int startGold;
    private int killsThisBattle;
    private int wavesCleared;
    private int maxWaveCount;

    private bool usedBattleHeal;
    private bool usedSupplyPack;
    private bool summaryPrepared;

    private int killQuestStartProgress;
    private int goldQuestStartProgress;

    public int StartGold => startGold;
    public int EndGold => GetCurrentGold();
    public int GoldGained => Mathf.Max(0, EndGold - startGold);

    public int KillsThisBattle => killsThisBattle;
    public int WavesCleared => wavesCleared;
    public int MaxWaveCount => maxWaveCount;

    public bool UsedBattleHeal => usedBattleHeal;
    public bool UsedSupplyPack => usedSupplyPack;

    private void OnEnable()
    {
        EnemyHealth.OnEnemyDied += HandleEnemyDied;
        EnemySpawner.OnWaveStarted += HandleWaveStarted;
        EnemySpawner.OnWaveCleared += HandleWaveCleared;
        EnemySpawner.OnAllWavesFinished += HandleAllWavesFinished;
    }

    private void OnDisable()
    {
        EnemyHealth.OnEnemyDied -= HandleEnemyDied;
        EnemySpawner.OnWaveStarted -= HandleWaveStarted;
        EnemySpawner.OnWaveCleared -= HandleWaveCleared;
        EnemySpawner.OnAllWavesFinished -= HandleAllWavesFinished;
    }

    private void Start()
    {
        CaptureBattleStartState();
    }

    private void CaptureBattleStartState()
    {
        startGold = GetCurrentGold();

        killsThisBattle = 0;
        wavesCleared = 0;
        maxWaveCount = 0;

        usedBattleHeal = false;
        usedSupplyPack = false;
        summaryPrepared = false;

        killQuestStartProgress = GetQuestCurrentProgress(QuestType.KillEnemies);
        goldQuestStartProgress = GetQuestCurrentProgress(QuestType.CollectGold);

        Debug.Log(
            $"[BattleResultTracker] Start -> " +
            $"startGold={startGold}, " +
            $"killQuestStart={killQuestStartProgress}, " +
            $"goldQuestStart={goldQuestStartProgress}"
        );
    }

    private void HandleEnemyDied(EnemyHealth enemy)
    {
        killsThisBattle++;
    }

    private void HandleWaveStarted(int currentWave, int totalWaveCount)
    {
        maxWaveCount = Mathf.Max(maxWaveCount, totalWaveCount);
    }

    private void HandleWaveCleared(int currentWave, int totalWaveCount)
    {
        wavesCleared = Mathf.Max(wavesCleared, currentWave);
        maxWaveCount = Mathf.Max(maxWaveCount, totalWaveCount);
    }

    private void HandleAllWavesFinished()
    {
        PrepareSummary();
    }

    public void MarkBattleHealUsed()
    {
        usedBattleHeal = true;
    }

    public void MarkSupplyPackUsed()
    {
        usedSupplyPack = true;
    }

    public void PrepareSummary()
    {
        if (summaryPrepared) return;
        summaryPrepared = true;

        if (logSummary)
        {
            Debug.Log(
                $"[BattleResultTracker] Summary -> " +
                $"kills={killsThisBattle}, " +
                $"goldGained={GoldGained}, " +
                $"waves={wavesCleared}/{maxWaveCount}, " +
                $"battleHeal={usedBattleHeal}, " +
                $"supplyPack={usedSupplyPack}, " +
                $"killQuest={BuildQuestProgressSummary(QuestType.KillEnemies)}, " +
                $"goldQuest={BuildQuestProgressSummary(QuestType.CollectGold)}"
            );
        }
    }

    public string BuildQuestProgressSummary(QuestType questType)
    {
        QuestData quest = GetQuestByType(questType);
        if (quest == null)
        {
            return "Not tracked";
        }

        int startProgress = 0;
        switch (questType)
        {
            case QuestType.KillEnemies:
                startProgress = killQuestStartProgress;
                break;
            case QuestType.CollectGold:
                startProgress = goldQuestStartProgress;
                break;
        }

        int delta = Mathf.Max(0, quest.currentAmount - startProgress);

        string status = quest.rewardClaimed
            ? "Reward Claimed"
            : quest.isCompleted
                ? "Completed"
                : "In Progress";

        return $"+{delta} -> {quest.currentAmount}/{quest.targetAmount} ({status})";
    }

    private int GetCurrentGold()
    {
        if (GameSession.Instance != null)
        {
            return GameSession.Instance.CurrentGold;
        }

        return 0;
    }

    private int GetQuestCurrentProgress(QuestType questType)
    {
        QuestData quest = GetQuestByType(questType);
        return quest != null ? quest.currentAmount : 0;
    }

    private QuestData GetQuestByType(QuestType questType)
    {
        if (QuestManager.Instance == null) return null;

        var quests = QuestManager.Instance.Quests;
        if (quests == null) return null;

        for (int i = 0; i < quests.Count; i++)
        {
            if (quests[i].questType == questType)
            {
                return quests[i];
            }
        }

        return null;
    }
}