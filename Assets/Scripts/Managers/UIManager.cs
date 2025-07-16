using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [Header("Panels")]
    [SerializeField] private GameObject turretShopPanel;
    [SerializeField] private GameObject nodeUIPanel;
    [SerializeField] private GameObject achievementPanel;
    [SerializeField] private GameObject trapShopPanel;
    [SerializeField] private GameObject nodeTrapUIPanel;



    [Header("Text")]
    [SerializeField] private TextMeshProUGUI upgradeText;
    [SerializeField] private TextMeshProUGUI sellText;
    [SerializeField] private TextMeshProUGUI turretLevelText;
    [SerializeField] private TextMeshProUGUI totalCoinsText;
    [SerializeField] private TextMeshProUGUI lifesText;

    //Desde aca lo de Trampas
    [SerializeField] private TextMeshProUGUI trapUpgradeText;
    [SerializeField] private TextMeshProUGUI trapSellText;
    [SerializeField] private TextMeshProUGUI trapLevelText;



    private Node _currentNodeSelected;
    private NodeTrap _currentTrapNodeSelected; //Para las trampas


    private void Update()
    {
        totalCoinsText.text = CurrencySystem.Instance.TotalCoins.ToString();
        lifesText.text = LevelManager.Instance.TotalLives.ToString();
    }

    public void OpenAchievementPanel(bool status)
    {
        achievementPanel.SetActive(status);
    }


    public void CloseTurretShopPanel()
    {
        turretShopPanel.SetActive(false);
    }

    public void CloseNodeUIPanel()
    {
        _currentNodeSelected.CloseAttackRangeSprite();
        nodeUIPanel.SetActive(false);
    }

    // NUEVO MÉTODO: Cerrar nodeTrapUIPanel
    public void CloseNodeTrapUIPanel()
    {
        _currentTrapNodeSelected?.CloseTrapRangeSprite();
        nodeTrapUIPanel.SetActive(false);
    }

    //Esto es de las trampas
    public void CloseTrapShopPanel()
    {
        _currentTrapNodeSelected?.CloseTrapRangeSprite();
        trapShopPanel.SetActive(false);
    }

    // ==================== TURRET ====================

    public void UpgradeTurret()
    {
        _currentNodeSelected.Turret.TurretUpgrade.UpgradeTurret();
        UpdateUpgradeText();
        UpdateTurretLevel();
        UpdateSellValue();
    }


    public void SellTurret()
    {
        _currentNodeSelected.SellTurret();
        _currentNodeSelected = null;
        nodeUIPanel.SetActive(false);
    }

    private void ShowNodeUI()
    {
        nodeUIPanel.SetActive(true);
        UpdateUpgradeText();
        UpdateTurretLevel();
        UpdateSellValue();
    }

    private void UpdateUpgradeText()
    {
        upgradeText.text = _currentNodeSelected.Turret.TurretUpgrade.UpgradeCost.ToString();
    }

    private void UpdateTurretLevel()
    {
        turretLevelText.text = $"Level {_currentNodeSelected.Turret.TurretUpgrade.Level}";
    }

    private void UpdateSellValue()
    {
        int sellAmount = _currentNodeSelected.Turret.TurretUpgrade.GetSellValue();
        sellText.text = sellAmount.ToString();
    }

    // ==================== TRAP ====================

    public void UpgradeTrap()
    {
        var upgrade = _currentTrapNodeSelected.Trap.GetComponent<TrapUpgrade>();
        if (upgrade != null)
        {
            upgrade.UpgradeTrap();
            UpdateTrapUpgradeText(upgrade);
            UpdateTrapLevel(upgrade);
            UpdateTrapSellValue(upgrade);
        }
    }

    public void SellTrap()
    {
        _currentTrapNodeSelected.SellTrap();
        _currentTrapNodeSelected = null;
        //trapShopPanel.SetActive(false);
        nodeTrapUIPanel.SetActive(false); // Cerramos el panel de mejora
    }

    private void ShowTrapUI()
    {
        nodeTrapUIPanel.SetActive(true); // <-- usamos el panel de mejora, no el de compra
        trapShopPanel.SetActive(false);  // por si estaba abierto

        var upgrade = _currentTrapNodeSelected.Trap.GetComponent<TrapUpgrade>();
        if (upgrade != null)
        {
            UpdateTrapUpgradeText(upgrade);
            UpdateTrapLevel(upgrade);
            UpdateTrapSellValue(upgrade);
        }
    }

    private void UpdateTrapUpgradeText(TrapUpgrade upgrade)
    {
        trapUpgradeText.text = upgrade.GetUpgradeCost().ToString();
    }

    private void UpdateTrapLevel(TrapUpgrade upgrade)
    {
        trapLevelText.text = $"Level {upgrade.GetCurrentLevel()}";
    }

    private void UpdateTrapSellValue(TrapUpgrade upgrade)
    {
        trapSellText.text = upgrade.GetSellValue().ToString();
    }


    // ==================== NODE SELECT EVENTS ====================




    private void NodeSelected(Node nodeSelected)
    {
        _currentNodeSelected = nodeSelected;
        if (_currentNodeSelected.IsEmpty())
        {
            turretShopPanel.SetActive(true);
        }
        else
        {
            ShowNodeUI();
        }
    }


    private void NodeTrapSelected(NodeTrap trapNode)
    {
        _currentTrapNodeSelected = trapNode;

        if (_currentTrapNodeSelected.IsEmpty())
        {
            trapShopPanel.SetActive(true); // AÑADIR ESTA LÍNEA
            nodeTrapUIPanel.SetActive(false); // aseguramos que el panel de mejora esté oculto

        }
        else
        {
            trapShopPanel.SetActive(false); // aseguramos que el panel de compra esté oculto
            ShowTrapUI();
        }
    }

    private void OnEnable()
    {
        Node.OnNodeSelected += NodeSelected;
        NodeTrap.OnNodeTrapSelected += NodeTrapSelected;

    }

    private void OnDisable()
    {
        Node.OnNodeSelected -= NodeSelected;
        NodeTrap.OnNodeTrapSelected -= NodeTrapSelected;

    }
}
