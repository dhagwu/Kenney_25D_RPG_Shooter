using UnityEngine;

public class CurrencyPickup : MonoBehaviour
{
    [SerializeField] private int amount = 5;
    [SerializeField] private AudioClip pickupClip;
    [SerializeField] private Transform visualRoot;
    [SerializeField] private float rotateSpeed = 180f;

    public void SetAmount(int value)
    {
        amount = Mathf.Max(1, value);
    }

    private void Update()
    {
        if (visualRoot != null)
        {
            visualRoot.Rotate(0f, rotateSpeed * Time.deltaTime, 0f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerCurrency playerCurrency = other.GetComponentInParent<PlayerCurrency>();
        if (playerCurrency == null)
            return;

        playerCurrency.AddGold(amount);

        if (AudioManager.Instance != null && pickupClip != null)
        {
            AudioManager.Instance.PlaySFX(pickupClip, 1f);
        }

        Destroy(gameObject);
    }
}