using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Turret Shop Setting")]
public class TurretSetting : ScriptableObject
{
    [SerializeField] public GameObject TurretPrefab;
    [SerializeField] public int TurretShopCost;
    [SerializeField] public Sprite TurretShopSprite;

}
