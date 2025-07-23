using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class EnemyCard : MonoBehaviour
{
    [SerializeField] private Image enemyImage;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private Button increaseButton;
    [SerializeField] private Button decreaseButton;

    public EnemyData EnemyLoaded { get; private set; }
    public int QuantitySelected { get; private set; }

    public System.Action<EnemyCard> OnQuantityChanged;

    public void SetupCard(EnemyData enemyData)
    {
        EnemyLoaded = enemyData;
        enemyImage.sprite = enemyData.Sprite;
        titleText.text = enemyData.Title;
        descriptionText.text = enemyData.Description;
        costText.text = $"{enemyData.Cost}";
        QuantitySelected = 0;
        quantityText.text = QuantitySelected.ToString();

        increaseButton.onClick.AddListener(IncreaseQuantity);
        decreaseButton.onClick.AddListener(DecreaseQuantity);
    }

    private void IncreaseQuantity()
    {
        QuantitySelected++;
        quantityText.text = QuantitySelected.ToString();
        OnQuantityChanged?.Invoke(this);
    }

    private void DecreaseQuantity()
    {
        if (QuantitySelected > 0)
        {
            QuantitySelected--;
            quantityText.text = QuantitySelected.ToString();
            OnQuantityChanged?.Invoke(this);
        }
    }
}
