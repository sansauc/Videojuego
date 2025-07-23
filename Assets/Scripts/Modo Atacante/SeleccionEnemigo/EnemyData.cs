using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyPosition
{
    Vanguardia,
    Medio,
    Retaguardia
}


[CreateAssetMenu(fileName = "EnemyData", menuName = "Game/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string ID;
    public string Title;
    [TextArea]
    public string Description;
    public int Cost;
    public Sprite Sprite;
    public EnemyPosition Position;
}