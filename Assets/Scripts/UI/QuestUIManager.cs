using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestUIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject hubPanel;
    [SerializeField] private GameObject questPanel;

    [Header("Quest 1 UI")]
    [SerializeField] private TMP_Text quest1NameText;
    [SerializeField] private TMP_Text quest1DescriptionText;
    [SerializeField] private TMP_Text quest1ProgressText;
    [SerializeField] private TMP_Text quest1RewardText;

    [Header("Quest 2 UI")]
    [SerializeField] private TMP_Text quest2NameText;
    [SerializeField] private TMP_Text quest2DescriptionText;
    [SerializeField] private TMP_Text quest2ProgressText;
    [SerializeField] private TMP_Text quest2RewardText;

    [Header("Quest 3 UI")]
    [SerializeField] private TMP_Text quest3NameText;
    [SerializeField] private TMP_Text quest3DescriptionText;
    [SerializeField] private TMP_Text quest3ProgressText;
    [SerializeField] private TMP_Text quest3RewardText;

    [Header("Feedback")]
    [SerializeField] private TMP_Text feedbackText;

    private void OnEnable()
    {
        QuestManager.OnQuestDataChanged += RefreshUI;
        GameSession.OnGoldChanged += HandleGoldChanged;
        GameSession.OnProgressionChanged += HandleProgressionChanged;
        RefreshUI();
    }

    private void OnDisable()
    {
        QuestManager.OnQuestDataChanged -= RefreshUI;
        GameSession.OnGoldChanged -= HandleGoldChanged;
        GameSession.OnProgressionChanged -= HandleProgressionChanged;
    }

    private void Start()
    {
        if (questPanel != null)
        {
            questPanel.SetActive(false);
        }

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

    public void OpenQuestPanel()
    {
        if (hubPanel != null)
        {
            hubPanel.SetActive(false);
        }

        if (questPanel != null)
        {
            questPanel.SetActive(true);
        }

        ClearFeedback();
        RefreshUI();
    }

    public void CloseQuestPanel()
    {
        if (questPanel != null)
        {
            questPanel.SetActive(false);
        }

        if (hubPanel != null)
        {
            hubPanel.SetActive(true);
        }

        ClearFeedback();
    }

    public void ClaimQuest1Reward()
    {
        ClaimRewardByIndex(0);
    }

    public void ClaimQuest2Reward()
    {
        ClaimRewardByIndex(1);
    }

    public void ClaimQuest3Reward()
    {
        ClaimRewardByIndex(2);
    }

    private void ClaimRewardByIndex(int questIndex)
    {
        if (QuestManager.Instance == null)
        {
            SetFeedback("QuestManager not found.");
            return;
        }

        bool success = QuestManager.Instance.ClaimReward(questIndex, out string message);
        SetFeedback(message);

        if (!success)
        {
            Debug.Log($"[QuestUIManager] Claim failed: {message}");
        }

        RefreshUI();
    }

    public void RefreshUI()
    {
        if (QuestManager.Instance == null) return;

        IReadOnlyList<QuestData> quests = QuestManager.Instance.Quests;

        RefreshQuestBlockOrClear(
            quests, 0,
            quest1NameText, quest1DescriptionText, quest1ProgressText, quest1RewardText);

        RefreshQuestBlockOrClear(
            quests, 1,
            quest2NameText, quest2DescriptionText, quest2ProgressText, quest2RewardText);

        RefreshQuestBlockOrClear(
            quests, 2,
            quest3NameText, quest3DescriptionText, quest3ProgressText, quest3RewardText);
    }

    private void RefreshQuestBlockOrClear(
        IReadOnlyList<QuestData> quests,
        int index,
        TMP_Text nameText,
        TMP_Text descriptionText,
        TMP_Text progressText,
        TMP_Text rewardText)
    {
        if (index < 0 || index >= quests.Count)
        {
            ClearQuestBlock(nameText, descriptionText, progressText, rewardText);
            return;
        }

        RefreshQuestBlock(quests[index], nameText, descriptionText, progressText, rewardText);
    }

    private void RefreshQuestBlock(
        QuestData quest,
        TMP_Text nameText,
        TMP_Text descriptionText,
        TMP_Text progressText,
        TMP_Text rewardText)
    {
        if (nameText != null) nameText.text = quest.questName;
        if (descriptionText != null) descriptionText.text = quest.description;

        if (progressText != null)
        {
            string status = quest.rewardClaimed
                ? "Reward Claimed"
                : quest.isCompleted
                    ? "Completed"
                    : "In Progress";

            progressText.text = $"Progress: {quest.currentAmount}/{quest.targetAmount} ({status})";
        }

        if (rewardText != null)
        {
            rewardText.text = BuildRewardText(quest);
        }
    }

    private void ClearQuestBlock(
        TMP_Text nameText,
        TMP_Text descriptionText,
        TMP_Text progressText,
        TMP_Text rewardText)
    {
        if (nameText != null) nameText.text = string.Empty;
        if (descriptionText != null) descriptionText.text = string.Empty;
        if (progressText != null) progressText.text = string.Empty;
        if (rewardText != null) rewardText.text = string.Empty;
    }

    private string BuildRewardText(QuestData quest)
    {
        List<string> parts = new List<string>();

        if (quest.rewardGold > 0)
        {
            parts.Add($"{quest.rewardGold} Gold");
        }

        if (quest.rewardBonusMaxHealth > 0)
        {
            parts.Add($"+{quest.rewardBonusMaxHealth} Max HP");
        }

        if (quest.rewardSupplyCount > 0)
        {
            parts.Add($"Supply x{quest.rewardSupplyCount}");
        }

        if (parts.Count == 0)
        {
            return "Reward: None";
        }

        return "Reward: " + string.Join(", ", parts);
    }

    private void SetFeedback(string message)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
        }

        Debug.Log($"[QuestUIManager] {message}");
    }

    private void ClearFeedback()
    {
        if (feedbackText != null)
        {
            feedbackText.text = string.Empty;
        }
    }
}