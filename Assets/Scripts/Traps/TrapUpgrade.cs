using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DamageTrap))]
public class TrapUpgrade : MonoBehaviour
{
    [Header("Daño por nivel")]
    [SerializeField] private float[] damagePerLevel = new float[] { 10f, 20f, 35f };

    [Header("Costos de mejora")]
    [SerializeField] private int upgradeInitialCost = 50;
    [SerializeField] private int upgradeCostIncremental = 25;

    [Header("Reventa")]
    [Range(0f, 1f)]
    [SerializeField] private float sellPercent = 0.5f;

    private int currentLevel = 0; // Nivel base = 0 (nivel 1)
    private int currentUpgradeCost;
    private int totalInvested = 0;

    private DamageTrap damageTrap;

    void Start()
    {
        damageTrap = GetComponent<DamageTrap>();
        currentUpgradeCost = upgradeInitialCost;
        ApplyUpgrade(); // Aplica daño de nivel inicial
    }

    public void UpgradeTrap()
    {
        if (currentLevel >= damagePerLevel.Length - 1)
        {
            Debug.Log("La trampa ya está en el nivel máximo.");
            return;
        }

        if (CurrencySystem.Instance.TotalCoins >= currentUpgradeCost)
        {
            CurrencySystem.Instance.RemoveCoins(currentUpgradeCost);
            totalInvested += currentUpgradeCost;

            currentLevel++;
            ApplyUpgrade();

            currentUpgradeCost += upgradeCostIncremental;

            Debug.Log("Trampa mejorada al nivel " + (currentLevel + 1));
        }
        else
        {
            Debug.Log("No tienes suficientes monedas para mejorar.");
        }
    }

    private void ApplyUpgrade()
    {
        if (damageTrap != null && currentLevel < damagePerLevel.Length)
        {
            damageTrap.damageAmount = damagePerLevel[currentLevel];
        }
    }

    public void SellTrap()
    {
        int refundAmount = Mathf.RoundToInt(totalInvested * sellPercent);
        CurrencySystem.Instance.AddCoins(refundAmount);
        Destroy(gameObject);
    }

    public int GetCurrentLevel()
    {
        return currentLevel + 1; // Nivel mostrado al jugador (1-3)
    }

    public int GetUpgradeCost()
    {
        return currentUpgradeCost;
    }

    public int GetSellValue()
    {
        return Mathf.RoundToInt(totalInvested * sellPercent);
    }
}