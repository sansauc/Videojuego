using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrapCard : MonoBehaviour
{
    public static Action<TrapSetting> OnPlaceTrap;

    [SerializeField] private Image trapImage;
    [SerializeField] private TextMeshProUGUI trapCost;

    public TrapSetting TrapLoaded { get; private set; }

    public void SetupTrapButton(TrapSetting trapSettings)
    {
        TrapLoaded = trapSettings;
        trapImage.sprite = trapSettings.trapShopSprite;
        trapCost.text = trapSettings.trapCost.ToString();
    }

    public void PlaceTrap()
    {
        if (CurrencySystem.Instance.TotalCoins >= TrapLoaded.trapCost)
        {
            CurrencySystem.Instance.RemoveCoins(TrapLoaded.trapCost);
            UIManager.Instance.CloseTrapShopPanel(); // Asegúrate de tener este método en tu UIManager
            OnPlaceTrap?.Invoke(TrapLoaded);
        }
    }
}
