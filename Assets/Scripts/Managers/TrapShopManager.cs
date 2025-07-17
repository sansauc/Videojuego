using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapShopManager : MonoBehaviour
{
    [SerializeField] private GameObject trapCardPrefab;
    [SerializeField] private Transform trapPanelContainer;

    [Header("Trap Settings")]
    [SerializeField] private TrapSetting[] traps;

    private NodeTrap _currentNodeTrapSelected;

    private void Start()
    {
        for (int i = 0; i < traps.Length; i++)
        {
            CreateTrapCard(traps[i]);
        }
    }

    private void CreateTrapCard(TrapSetting trapSettings)
    {
        GameObject newInstance = Instantiate(trapCardPrefab, trapPanelContainer.position, Quaternion.identity);
        newInstance.transform.SetParent(trapPanelContainer);
        newInstance.transform.localScale = Vector3.one;

        TrapCard cardButton = newInstance.GetComponent<TrapCard>();
        cardButton.SetupTrapButton(trapSettings);
    }

    private void NodeTrapSelected(NodeTrap nodeSelected)
    {
        _currentNodeTrapSelected = nodeSelected;
    }

    private void PlaceTrap(TrapSetting trapLoaded)
    {
        if (_currentNodeTrapSelected != null)
        {
            GameObject trapInstance = Instantiate(trapLoaded.trapPrefab);
            trapInstance.transform.position = _currentNodeTrapSelected.transform.position;
            trapInstance.transform.SetParent(_currentNodeTrapSelected.transform);

            Trap trapPlaced = trapInstance.GetComponent<Trap>();
            _currentNodeTrapSelected.SetTrap(trapPlaced, trapLoaded.trapCost); // <-- aquí está el fix
        }
    }

    private void TrapRemoved()
    {
        _currentNodeTrapSelected = null;
    }

    private void OnEnable()
    {
        NodeTrap.OnNodeTrapSelected += NodeTrapSelected;
        NodeTrap.OnTrapRemoved += TrapRemoved;
        TrapCard.OnPlaceTrap += PlaceTrap;
    }

    private void OnDisable()
    {
        NodeTrap.OnNodeTrapSelected -= NodeTrapSelected;
        NodeTrap.OnTrapRemoved -= TrapRemoved;
        TrapCard.OnPlaceTrap -= PlaceTrap;
    }
}
