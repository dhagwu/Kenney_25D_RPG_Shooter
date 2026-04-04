using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Base Stats")]
    [SerializeField] private float baseMaxHp = 100f;

    [Header("Audio")]
    [SerializeField] private AudioClip hurtClip;
    [SerializeField] private AudioClip deathClip;

    private float maxHp;
    private float currentHp;
    private bool isDead;

    public float CurrentHp => currentHp;
    public float MaxHp => maxHp;
    public float NormalizedHp => maxHp > 0f ? currentHp / maxHp : 0f;
    public bool IsDead => isDead;

    private void Awake()
    {
        ApplySessionHealthBonus();
        currentHp = maxHp;
        isDead = false;

        Debug.Log(
            $"[PlayerHealth] Init -> " +
            $"baseMaxHp={baseMaxHp}, " +
            $"bonusMaxHp={GetSessionBonusHp()}, " +
            $"finalMaxHp={maxHp}"
        );
    }

    private void ApplySessionHealthBonus()
    {
        float bonusHp = GetSessionBonusHp();
        maxHp = baseMaxHp + bonusHp;
    }

    private float GetSessionBonusHp()
    {
        if (GameSession.Instance != null)
        {
            return GameSession.Instance.BonusMaxHealth;
        }

        return 0f;
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
            return;

        if (damage <= 0f)
            return;

        currentHp -= damage;
        currentHp = Mathf.Max(currentHp, 0f);

        if (AudioManager.Instance != null && hurtClip != null)
        {
            AudioManager.Instance.PlaySFX(hurtClip, 1f);
        }

        Debug.Log($"[PlayerHealth] TakeDamage -> {damage}, HP = {currentHp}/{maxHp}");

        if (currentHp <= 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (isDead)
            return;

        if (amount <= 0f)
            return;

        currentHp += amount;
        currentHp = Mathf.Min(currentHp, maxHp);

        Debug.Log($"[PlayerHealth] Heal -> {amount}, HP = {currentHp}/{maxHp}");
    }

    public void RestoreToFull()
    {
        if (isDead)
            return;

        currentHp = maxHp;
        Debug.Log($"[PlayerHealth] RestoreToFull -> HP = {currentHp}/{maxHp}");
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("[PlayerHealth] Player Dead");

        if (AudioManager.Instance != null && deathClip != null)
        {
            AudioManager.Instance.PlaySFX(deathClip, 1f);
        }
    }
}