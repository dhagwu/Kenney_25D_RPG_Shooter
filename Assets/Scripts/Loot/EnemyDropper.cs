using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
public class EnemyDropper : MonoBehaviour
{
    [SerializeField] private GameObject coinPickupPrefab;
    [SerializeField] private Transform dropRoot;
    [SerializeField] private int minGold = 3;
    [SerializeField] private int maxGold = 8;

    private EnemyHealth enemyHealth;
    private bool hasDropped;

    private void Awake()
    {
        enemyHealth = GetComponent<EnemyHealth>();

        if (dropRoot == null)
        {
            dropRoot = transform;
        }
    }

    private void OnEnable()
    {
        EnemyHealth.OnEnemyDied += HandleEnemyDied;
        hasDropped = false;
    }

    private void OnDisable()
    {
        EnemyHealth.OnEnemyDied -= HandleEnemyDied;
    }

    private void HandleEnemyDied(EnemyHealth deadEnemy)
    {
        if (deadEnemy != enemyHealth)
            return;

        if (hasDropped)
            return;

        hasDropped = true;

        if (coinPickupPrefab == null)
            return;

        Vector3 spawnPosition = dropRoot != null ? dropRoot.position : transform.position;
        GameObject pickup = Instantiate(coinPickupPrefab, spawnPosition, Quaternion.identity);

        CurrencyPickup currencyPickup = pickup.GetComponent<CurrencyPickup>();
        if (currencyPickup != null)
        {
            int goldAmount = Random.Range(minGold, maxGold + 1);
            currencyPickup.SetAmount(goldAmount);
        }
    }
}