using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "Trap Shop Setting", menuName = "TowerDefense/TrapSetting")]
public class TrapSetting : ScriptableObject
{

    [Header("Trampa")]
    [SerializeField] public GameObject trapPrefab;
    [SerializeField] public int trapCost;
    [SerializeField] public Sprite trapShopSprite;
    [SerializeField] public int idTrup;

    [Header("Duración de vida")]
    [Tooltip("Tiempo en segundos desde la primera activación hasta que la trampa se destruye automáticamente.")]
    public float lifetimeAfterFirstActivation = -1f; // -1 significa duración infinita


}
