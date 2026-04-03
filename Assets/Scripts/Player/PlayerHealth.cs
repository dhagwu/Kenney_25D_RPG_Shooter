using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHp = 100f;
    [SerializeField] private AudioClip hurtClip;
    [SerializeField] private AudioClip deathClip;

    private float currentHp;
    private bool isDead;

    public float CurrentHp => currentHp;
    public float MaxHp => maxHp;
    public float NormalizedHp => maxHp > 0f ? currentHp / maxHp : 0f;

    private void Awake()
    {
        currentHp = maxHp;
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
            return;

        currentHp -= damage;
        currentHp = Mathf.Max(currentHp, 0f);
        if (AudioManager.Instance != null && hurtClip != null)
        {
            AudioManager.Instance.PlaySFX(hurtClip, 1f);
        }

        Debug.Log($"Player TakeDamage: {damage}, HP = {currentHp}/{maxHp}");

        if (currentHp <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("Player Dead");
        // 当前阶段先不做复活/失败界面，先留在这里
        if (AudioManager.Instance != null && deathClip != null)
        {
            AudioManager.Instance.PlaySFX(deathClip, 1f);
        }
    }
}