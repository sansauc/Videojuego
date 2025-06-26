using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Turret Shop Setting")]
public class TurretSetting : ScriptableObject
{
    [SerializeField] private GameObject TurretPrefab;
    [SerializeField] private int TurretShopCost;
    [SerializeField] private Sprite TurretShopSprite;

}
