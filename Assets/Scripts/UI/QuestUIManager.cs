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

    private void ClaimRewardByIndex(int questIndex)
    {
        if (QuestManager.Instance == null)
        {
            SetFeedback("QuestManager not found.");
            return;
        }

        QuestManager.Instance.ClaimReward(questIndex);
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (QuestManager.Instance == null)
            return;

        var quests = QuestManager.Instance.Quests;

        if (quests.Count > 0)
        {
            RefreshQuestBlock(
                quests[0],
                quest1NameText,
                quest1DescriptionText,
                quest1ProgressText,
                quest1RewardText
            );
        }

        if (quests.Count > 1)
        {
            RefreshQuestBlock(
                quests[1],
                quest2NameText,
                quest2DescriptionText,
                quest2ProgressText,
                quest2RewardText
            );
        }
    }

    private void RefreshQuestBlock(
        QuestData quest,
        TMP_Text nameText,
        TMP_Text descriptionText,
        TMP_Text progressText,
        TMP_Text rewardText)
    {
        if (nameText != null)
            nameText.text = quest.questName;

        if (descriptionText != null)
            descriptionText.text = quest.description;

        if (progressText != null)
        {
            string status = quest.rewardClaimed ? "Reward Claimed" :
                            quest.isCompleted ? "Completed" :
                            "In Progress";

            progressText.text = $"Progress: {quest.currentAmount}/{quest.targetAmount} ({status})";
        }

        if (rewardText != null)
        {
            rewardText.text =
                $"Reward: {quest.rewardGold} Gold, +{quest.rewardBonusMaxHealth} Max HP";
        }
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