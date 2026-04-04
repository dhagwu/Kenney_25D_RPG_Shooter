using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public static event Action<EnemyHealth> OnEnemyDied;

    public static int TotalKills { get; private set; }

    [SerializeField] private float maxHp = 30f;
    [SerializeField] private bool destroyOnDeath = true;
    [SerializeField] private AudioClip hurtClip;
    [SerializeField] private AudioClip deathClip;

    private float currentHp;
    private bool isDead;

    public bool IsDead => isDead;
    public float CurrentHp => currentHp;
    public float MaxHp => maxHp;

    private void Awake()
    {
        currentHp = maxHp;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;
        if (damage <= 0f) return;

        currentHp -= damage;

        if (!isDead && AudioManager.Instance != null && hurtClip != null)
        {
            AudioManager.Instance.PlaySFX(hurtClip, 1f);
        }

        if (currentHp <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        TotalKills++;

        OnEnemyDied?.Invoke(this);

        if (AudioManager.Instance != null && deathClip != null)
        {
            AudioManager.Instance.PlaySFX(deathClip, 1f);
        }

        if (destroyOnDeath)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}