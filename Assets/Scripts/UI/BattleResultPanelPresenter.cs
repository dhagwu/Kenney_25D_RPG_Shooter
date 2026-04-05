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

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        resultPanel.SetActive(true);

        // 强制把 VictoryPanel 顶到 HUD 最上层
        resultPanel.transform.SetAsLastSibling();

        // 强制把所有子节点都激活，避免某些文本或按钮被单独关掉
        for (int i = 0; i < resultPanel.transform.childCount; i++)
        {
            resultPanel.transform.GetChild(i).gameObject.SetActive(true);
        }

        RectTransform rect = resultPanel.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.localScale = Vector3.one;
            rect.anchoredPosition3D = Vector3.zero;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        RefreshUI();

        Time.timeScale = 0f;

        Debug.Log(
            $"[BattleResultPanelPresenter] Show -> panel={resultPanel.name}, " +
            $"activeSelf={resultPanel.activeSelf}, activeInHierarchy={resultPanel.activeInHierarchy}, " +
            $"childCount={resultPanel.transform.childCount}"
        );
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
        if (titleText != null)
        {
            titleText.text = "Battle Results";
        }

        if (battleResultTracker == null)
        {
            SetFallbackTexts();
            return;
        }

        if (killsText != null)
        {
            killsText.text = $"Kills: {battleResultTracker.KillsThisBattle}";
        }

        if (goldText != null)
        {
            goldText.text = $"Gold Gained: +{battleResultTracker.GoldGained}";
        }

        if (wavesText != null)
        {
            if (battleResultTracker.MaxWaveCount > 0)
            {
                wavesText.text = $"Waves Cleared: {battleResultTracker.WavesCleared}/{battleResultTracker.MaxWaveCount}";
            }
            else
            {
                wavesText.text = $"Waves Cleared: {battleResultTracker.WavesCleared}";
            }
        }

        if (battleHealText != null)
        {
            battleHealText.text = $"Battle Heal Used: {(battleResultTracker.UsedBattleHeal ? "Yes" : "No")}";
        }

        if (supplyPackText != null)
        {
            supplyPackText.text = $"Supply Pack Used: {(battleResultTracker.UsedSupplyPack ? "Yes" : "No")}";
        }

        if (killQuestText != null)
        {
            killQuestText.text = $"Kill Quest: {battleResultTracker.BuildQuestProgressSummary(QuestType.KillEnemies)}";
        }

        if (goldQuestText != null)
        {
            goldQuestText.text = $"Gold Quest: {battleResultTracker.BuildQuestProgressSummary(QuestType.CollectGold)}";
        }

        if (hintText != null)
        {
            hintText.text = "Return to Hub to claim rewards and continue progression.";
        }
    }

    private void SetFallbackTexts()
    {
        if (killsText != null) killsText.text = "Kills: 0";
        if (goldText != null) goldText.text = "Gold Gained: +0";
        if (wavesText != null) wavesText.text = "Waves Cleared: 0";
        if (battleHealText != null) battleHealText.text = "Battle Heal Used: No";
        if (supplyPackText != null) supplyPackText.text = "Supply Pack Used: No";
        if (killQuestText != null) killQuestText.text = "Kill Quest: Not tracked";
        if (goldQuestText != null) goldQuestText.text = "Gold Quest: Not tracked";

        if (hintText != null)
        {
            hintText.text = "Battle result data not found.";
        }
    }
}