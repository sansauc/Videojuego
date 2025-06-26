using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretUpgrade : MonoBehaviour
{
    [SerializeField] private int upgradeInitialCost;
    [SerializeField] private int upgradeCostIncremental;
    [SerializeField] private float damageIncremental;
    [SerializeField] private float delayReduce;

    public int UpgradeCost { get; set; }

    private TurretProjectile _turretProjectile;

    private void Start()
    {
        _turretProjectile = GetComponent<TurretProjectile>();
        UpgradeCost = upgradeInitialCost;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            UpgradeTurret();
        }
    }

    private void UpgradeTurret()
    {

       if (CurrencySystem.Instance.TotalCoins >= UpgradeCost)
        {
            _turretProjectile.Damage += damageIncremental;
            _turretProjectile.DelayPerShot -= delayReduce;
            UpdateUpgrade();
        }
    }

    
    private void UpdateUpgrade()
    {
        CurrencySystem.Instance.RemoveCoins(UpgradeCost);
        UpgradeCost += upgradeCostIncremental;
    }
}
