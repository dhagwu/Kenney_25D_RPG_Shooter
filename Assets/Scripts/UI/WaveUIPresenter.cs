using System.Collections;
using TMPro;
using UnityEngine;

public class WaveUIPresenter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private TMP_Text waveStatusText;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private VictoryResultBinder victoryResultBinder;

    [Header("HUD Panels To Hide On Victory")]
    [SerializeField] private GameObject wavePanel;
    [SerializeField] private GameObject currencyPanel;
    [SerializeField] private GameObject weaponPanel;
    [SerializeField] private GameObject enemyInfoPanel;
    [SerializeField] private GameObject hpPanel;

    [Header("Status Timing")]
    [SerializeField] private float statusDisplayTime = 2f;

    private Coroutine statusCoroutine;
    private bool victoryShown;

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

        if (victoryResultBinder == null && victoryPanel != null)
        {
            victoryResultBinder = victoryPanel.GetComponent<VictoryResultBinder>();
        }

        victoryShown = false;

        // 场景开始时确保 HUD 是开着的
        SetGameplayHudVisible(true);

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }

        if (waveStatusText != null)
        {
            waveStatusText.text = string.Empty;
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

        if (currentWave >= maxWave)
        {
            ShowVictory();
            return;
        }

        ShowStatusTemporarily($"Wave {currentWave} Cleared");
    }

    private void HandleAllWavesFinished()
    {
        ShowVictory();
    }

    private void ShowVictory()
    {
        if (victoryShown) return;
        victoryShown = true;

        if (statusCoroutine != null)
        {
            StopCoroutine(statusCoroutine);
            statusCoroutine = null;
        }

        if (waveStatusText != null)
        {
            waveStatusText.text = "All Waves Cleared";
        }

        // 先隐藏关卡 HUD
        SetGameplayHudVisible(false);

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
            victoryPanel.transform.SetAsLastSibling();
        }

        if (victoryResultBinder != null)
        {
            victoryResultBinder.RefreshResult();
        }

        Debug.Log("[WaveUIPresenter] Victory panel shown.");
    }

    private void SetGameplayHudVisible(bool visible)
    {
        if (hpPanel != null)
        {
            hpPanel.SetActive(visible);
        }

        if (wavePanel != null)
        {
            wavePanel.SetActive(visible);
        }

        if (currencyPanel != null)
        {
            currencyPanel.SetActive(visible);
        }

        if (weaponPanel != null)
        {
            weaponPanel.SetActive(visible);
        }

        if (enemyInfoPanel != null)
        {
            enemyInfoPanel.SetActive(visible);
        }
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

        if (!victoryShown && waveStatusText != null)
        {
            waveStatusText.text = string.Empty;
        }

        statusCoroutine = null;
    }
}