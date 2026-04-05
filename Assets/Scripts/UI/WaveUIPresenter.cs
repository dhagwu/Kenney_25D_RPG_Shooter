using System.Collections;
using TMPro;
using UnityEngine;

public class WaveUIPresenter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private TMP_Text waveStatusText;
    [SerializeField] private BattleResultPanelPresenter battleResultPanelPresenter;

    [Header("Status Timing")]
    [SerializeField] private float statusDisplayTime = 2f;

    private Coroutine statusCoroutine;
    private bool resultPanelShown;

    private void OnEnable()
    {
        EnemySpawner.OnWaveStarted += HandleWaveStarted;
        EnemySpawner.OnWaveCleared += HandleWaveCleared;
        EnemySpawner.OnAllWavesFinished += HandleAllWavesFinished;
    }

    private void OnDisable()
    {
        EnemySpawner.OnWaveStarted -= HandleWaveStarted;
        EnemySpawner.OnWaveCleared -= HandleWaveCleared;
        EnemySpawner.OnAllWavesFinished -= HandleAllWavesFinished;
    }

    private void Start()
    {
        if (enemySpawner == null)
        {
            enemySpawner = FindFirstObjectByType<EnemySpawner>();
        }

        if (battleResultPanelPresenter == null)
        {
            battleResultPanelPresenter = FindFirstObjectByType<BattleResultPanelPresenter>();
        }

        resultPanelShown = false;

        if (battleResultPanelPresenter != null)
        {
            battleResultPanelPresenter.HideImmediately();
        }

        if (waveStatusText != null)
        {
            waveStatusText.text = "";
        }

        RefreshWaveText();
    }

    private void HandleWaveStarted(int currentWave, int maxWave)
    {
        RefreshWaveText(currentWave, maxWave);
        ShowStatusTemporarily($"Wave {currentWave} Start");
    }

    private void HandleWaveCleared(int currentWave, int maxWave)
    {
        RefreshWaveText(currentWave, maxWave);

        // 关键兜底：
        // 只要“当前清掉的是最后一波”，就直接打开结算面板，
        // 不再完全依赖 OnAllWavesFinished 事件。
        if (currentWave >= maxWave)
        {
            OpenBattleResultPanel("All Waves Cleared");
            return;
        }

        ShowStatusTemporarily($"Wave {currentWave} Cleared");
    }

    private void HandleAllWavesFinished()
    {
        OpenBattleResultPanel("All Waves Cleared");
    }

    private void OpenBattleResultPanel(string statusMessage)
    {
        if (resultPanelShown) return;
        resultPanelShown = true;

        if (statusCoroutine != null)
        {
            StopCoroutine(statusCoroutine);
            statusCoroutine = null;
        }

        if (waveStatusText != null)
        {
            waveStatusText.text = statusMessage;
        }

        if (battleResultPanelPresenter != null)
        {
            battleResultPanelPresenter.ShowResultPanel();
        }
        else
        {
            Debug.LogWarning("[WaveUIPresenter] BattleResultPanelPresenter is missing.");
        }

        Debug.Log("[WaveUIPresenter] Battle result panel opened.");
    }

    private void RefreshWaveText()
    {
        if (enemySpawner == null || waveText == null) return;

        int current = Mathf.Max(enemySpawner.CurrentWave, 0);
        int max = Mathf.Max(enemySpawner.MaxWaveCount, 0);
        waveText.text = $"Wave {current} / {max}";
    }

    private void RefreshWaveText(int currentWave, int maxWave)
    {
        if (waveText == null) return;
        waveText.text = $"Wave {currentWave} / {maxWave}";
    }

    private void ShowStatusTemporarily(string message)
    {
        if (waveStatusText == null) return;

        if (statusCoroutine != null)
        {
            StopCoroutine(statusCoroutine);
        }

        statusCoroutine = StartCoroutine(StatusRoutine(message));
    }

    private IEnumerator StatusRoutine(string message)
    {
        waveStatusText.text = message;
        yield return new WaitForSeconds(statusDisplayTime);

        // 如果结果面板已经打开，就不要再清空最终状态提示
        if (!resultPanelShown && waveStatusText != null)
        {
            waveStatusText.text = "";
        }

        statusCoroutine = null;
    }
}