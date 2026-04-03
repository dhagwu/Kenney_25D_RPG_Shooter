using System;
using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static event Action<int, int> OnWaveStarted;
    public static event Action<int, int> OnWaveCleared;
    public static event Action OnAllWavesFinished;

    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject meleeEnemyPrefab;
    [SerializeField] private GameObject rangedEnemyPrefab;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;

    [Header("Wave Settings")]
    [SerializeField] private bool autoStart = true;
    [SerializeField] private float startDelay = 1f;
    [SerializeField] private float timeBetweenSpawns = 0.6f;
    [SerializeField] private float timeBetweenWaves = 3f;

    [Header("Wave Composition")]
    [SerializeField] private int meleeCountPerWave = 2;
    [SerializeField] private int rangedCountPerWave = 1;
    [SerializeField] private int maxWaveCount = 3;

    private int currentWave = 0;
    private bool isSpawning;

    public int CurrentWave => currentWave;
    public int MaxWaveCount => maxWaveCount;

    private void Start()
    {
        if (autoStart)
        {
            StartCoroutine(SpawnLoop());
        }
    }

    private IEnumerator SpawnLoop()
    {
        yield return new WaitForSeconds(startDelay);

        while (currentWave < maxWaveCount)
        {
            currentWave++;
            OnWaveStarted?.Invoke(currentWave, maxWaveCount);

            yield return StartCoroutine(SpawnWave(currentWave));

            yield return new WaitUntil(() => GetAliveEnemyCount() == 0);

            OnWaveCleared?.Invoke(currentWave, maxWaveCount);

            if (currentWave < maxWaveCount)
            {
                yield return new WaitForSeconds(timeBetweenWaves);
            }
        }

        OnAllWavesFinished?.Invoke();
        Debug.Log("All waves finished.");
    }

    private IEnumerator SpawnWave(int waveIndex)
    {
        if (isSpawning)
            yield break;

        isSpawning = true;

        int meleeToSpawn = meleeCountPerWave + (waveIndex - 1);
        int rangedToSpawn = rangedCountPerWave + ((waveIndex - 1) / 2);

        Debug.Log($"Spawn Wave {waveIndex}: melee={meleeToSpawn}, ranged={rangedToSpawn}");

        for (int i = 0; i < meleeToSpawn; i++)
        {
            SpawnEnemy(meleeEnemyPrefab);
            yield return new WaitForSeconds(timeBetweenSpawns);
        }

        for (int i = 0; i < rangedToSpawn; i++)
        {
            SpawnEnemy(rangedEnemyPrefab);
            yield return new WaitForSeconds(timeBetweenSpawns);
        }

        isSpawning = false;
    }

    private void SpawnEnemy(GameObject enemyPrefab)
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning("EnemySpawner: enemyPrefab is null.");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("EnemySpawner: no spawn points assigned.");
            return;
        }

        int index = UnityEngine.Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[index];

        if (spawnPoint == null)
        {
            Debug.LogWarning("EnemySpawner: one spawn point is null.");
            return;
        }

        Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
    }

    private int GetAliveEnemyCount()
    {
        int aliveCount = 0;
        EnemyHealth[] enemies = FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None);

        foreach (EnemyHealth enemy in enemies)
        {
            if (enemy != null && !enemy.IsDead && enemy.gameObject.activeInHierarchy)
            {
                aliveCount++;
            }
        }

        return aliveCount;
    }
}