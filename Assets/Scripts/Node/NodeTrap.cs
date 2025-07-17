using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeTrap : MonoBehaviour
{
    public static Action<NodeTrap> OnNodeTrapSelected;
    public static Action OnTrapRemoved;

    [SerializeField] private GameObject trapRangeSprite;

    public Trap Trap { get; private set; }

    public int InitialCost { get; private set; }


    private float _rangeSize;
    private Vector3 _rangeOriginalSize;

    private void Start()
    {
        if (trapRangeSprite != null)
        {
            _rangeSize = trapRangeSprite.GetComponent<SpriteRenderer>().bounds.size.y;
            _rangeOriginalSize = trapRangeSprite.transform.localScale;
            trapRangeSprite.SetActive(false);
        }
    }


    public void SetTrap(Trap trap, int cost)
    {
        Trap = trap;
        InitialCost = cost;
    }

    public bool IsEmpty()
    {
        return Trap == null;
    }

    public void SelectTrap()
    {
        OnNodeTrapSelected?.Invoke(this);
        if (!IsEmpty())
        {
            ShowTrapInfo();
        }
    }

    public void SellTrap()
    {
        if (!IsEmpty())
        {
            // ✅ Se calcula el valor de reventa directamente a partir del costo inicial
            int sellValue = Mathf.RoundToInt(InitialCost * 0.5f);
            CurrencySystem.Instance.AddCoins(sellValue);

            Destroy(Trap.gameObject);
            Trap = null;

            if (trapRangeSprite != null)
                trapRangeSprite.SetActive(false);

            OnTrapRemoved?.Invoke();
        }
    }

    public void CloseTrapRangeSprite()
    {
        if (trapRangeSprite != null)
            trapRangeSprite.SetActive(false);
    }

    private void ShowTrapInfo()
    {
        if (trapRangeSprite != null)
        {
            trapRangeSprite.SetActive(true);

            float activationRange = GetTrapActivationRange();
            trapRangeSprite.transform.localScale = _rangeOriginalSize * activationRange / (_rangeSize / 2);
        }
    }

    private float GetTrapActivationRange()
    {
        Collider2D trapCollider = Trap?.GetComponent<Collider2D>();
        if (trapCollider != null)
        {
            return trapCollider.bounds.size.x; // o .y dependiendo del tipo
        }

        return 1f; // valor por defecto
    }
}
