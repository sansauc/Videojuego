using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Turret Shop Setting")]
public class TurretSetting : ScriptableObject
{
    public GameObject TurretPrefab;
    public int TurretShopCost;
    public Sprite TurretShopSprite;

}
