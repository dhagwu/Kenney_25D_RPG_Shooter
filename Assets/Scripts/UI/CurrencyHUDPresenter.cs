using TMPro;
using UnityEngine;

public class CurrencyHUDPresenter : MonoBehaviour
{
    [SerializeField] private PlayerCurrency playerCurrency;
    [SerializeField] private TMP_Text goldText;

    private void OnEnable()
    {
        PlayerCurrency.OnGoldChanged += HandleGoldChanged;
    }

    private void OnDisable()
    {
        PlayerCurrency.OnGoldChanged -= HandleGoldChanged;
    }

    private void Start()
    {
        if (playerCurrency == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerCurrency = player.GetComponent<PlayerCurrency>();
            }
        }

        Refresh();
    }

    private void HandleGoldChanged(int value)
    {
        Refresh();
    }

    private void Refresh()
    {
        if (goldText == null || playerCurrency == null)
            return;

        goldText.text = $"Gold {playerCurrency.CurrentGold}";
    }
}