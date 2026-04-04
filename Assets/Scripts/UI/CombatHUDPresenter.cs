using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatHUDPresenter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Image hpFillImage;
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private TMP_Text enemyAliveText;
    [SerializeField] private TMP_Text killCountText;

    [Header("Refresh")]
    [SerializeField] private float enemyCountRefreshInterval = 0.25f;

    private float nextEnemyRefreshTime;

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
        if (playerHealth == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerHealth = player.GetComponent<PlayerHealth>();
            }
        }

        RefreshHp();
        RefreshEnemyInfo();
    }

    private void Update()
    {
        RefreshHp();

        if (Time.time >= nextEnemyRefreshTime)
        {
            RefreshEnemyInfo();
            nextEnemyRefreshTime = Time.time + enemyCountRefreshInterval;
        }
    }

    private void HandleEnemyDied(EnemyHealth enemy)
    {
        RefreshEnemyInfo();
    }

    private void RefreshHp()
    {
        if (playerHealth == null)
            return;

        float normalized = playerHealth.NormalizedHp;

        if (hpFillImage != null)
        {
            hpFillImage.fillAmount = normalized;
        }

        if (hpText != null)
        {
            hpText.text = $"HP {Mathf.CeilToInt(playerHealth.CurrentHp)} / {Mathf.CeilToInt(playerHealth.MaxHp)}";
        }
    }

    private void RefreshEnemyInfo()
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

        if (enemyAliveText != null)
        {
            enemyAliveText.text = $"Enemies {aliveCount}";
        }

        if (killCountText != null)
        {
            killCountText.text = $"Kills {EnemyHealth.TotalKills}";
        }
    }
}