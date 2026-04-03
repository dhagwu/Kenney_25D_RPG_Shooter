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

    [Header("Status Timing")]
    [SerializeField] private float statusDisplayTime = 2f;

    private Coroutine statusCoroutine;

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

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
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
        ShowStatusTemporarily($"Wave {currentWave} Cleared");
    }

    private void HandleAllWavesFinished()
    {
        if (waveStatusText != null)
        {
            waveStatusText.text = "All Waves Cleared";
        }

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }
    }

    private void RefreshWaveText()
    {
        if (enemySpawner == null || waveText == null)
            return;

        int current = Mathf.Max(enemySpawner.CurrentWave, 0);
        int max = Mathf.Max(enemySpawner.MaxWaveCount, 0);
        waveText.text = $"Wave {current} / {max}";
    }

    private void RefreshWaveText(int currentWave, int maxWave)
    {
        if (waveText == null)
            return;

        waveText.text = $"Wave {currentWave} / {maxWave}";
    }

    private void ShowStatusTemporarily(string message)
    {
        if (waveStatusText == null)
            return;

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
        waveStatusText.text = "";
        statusCoroutine = null;
    }
}