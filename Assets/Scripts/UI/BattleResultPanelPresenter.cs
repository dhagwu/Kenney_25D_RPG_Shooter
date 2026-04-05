using TMPro;
using UnityEngine;

public class BattleResultPanelPresenter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private BattleResultTracker battleResultTracker;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Texts")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text killsText;
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text wavesText;
    [SerializeField] private TMP_Text battleHealText;
    [SerializeField] private TMP_Text supplyPackText;
    [SerializeField] private TMP_Text killQuestText;
    [SerializeField] private TMP_Text goldQuestText;
    [SerializeField] private TMP_Text hintText;

    private void Awake()
    {
        CacheReferences();
    }

    private void Start()
    {
        HideImmediately();
    }

    private void CacheReferences()
    {
        if (resultPanel == null)
        {
            resultPanel = gameObject;
        }

        if (canvasGroup == null && resultPanel != null)
        {
            canvasGroup = resultPanel.GetComponent<CanvasGroup>();
        }

        if (battleResultTracker == null)
        {
            battleResultTracker = FindFirstObjectByType<BattleResultTracker>();
        }
    }

    public void ShowResultPanel()
    {
        CacheReferences();

        if (resultPanel == null)
        {
            Debug.LogError("[BattleResultPanelPresenter] resultPanel is null.");
            return;
        }

        if (battleResultTracker != null)
        {
            battleResultTracker.PrepareSummary();
        }

        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);

        resultPanel.SetActive(true);
        resultPanel.transform.SetAsLastSibling();

        for (int i = 0; i < resultPanel.transform.childCount; i++)
        {
            resultPanel.transform.GetChild(i).gameObject.SetActive(true);
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        RefreshUI();
        Time.timeScale = 0f;
    }

    public void HideImmediately()
    {
        CacheReferences();

        if (resultPanel == null) return;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        resultPanel.SetActive(false);
    }

    public void RefreshUI()
    {
        if (titleText != null) titleText.text = "Battle Results";
        if (battleResultTracker == null) return;

        if (killsText != null)
            killsText.text = $"Kills: {battleResultTracker.KillsThisBattle}";

        if (goldText != null)
            goldText.text = $"Gold Gained: +{battleResultTracker.GoldGained}";

        if (wavesText != null)
            wavesText.text = $"Waves Cleared: {battleResultTracker.WavesCleared}/{battleResultTracker.MaxWaveCount}";

        if (battleHealText != null)
            battleHealText.text = $"Battle Heal Used: {(battleResultTracker.UsedBattleHeal ? "Yes" : "No")}";

        if (supplyPackText != null)
            supplyPackText.text = $"Supply Pack Used: {(battleResultTracker.UsedSupplyPack ? "Yes" : "No")}";

        if (killQuestText != null)
            killQuestText.text = $"Kill Quest: {battleResultTracker.BuildQuestProgressSummary(QuestType.KillEnemies)}";

        if (goldQuestText != null)
            goldQuestText.text = $"Gold Quest: {battleResultTracker.BuildQuestProgressSummary(QuestType.CollectGold)}";

        if (hintText != null)
            hintText.text = "Return to Hub to claim rewards and continue progression.";
    }
}