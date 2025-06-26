using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private int lives = 10;

    //private int enemiesKilled; Esta variable la uso si quiero llevar un contador de kills

    public int TotalLives
    {
        get => lives;
        private set => lives = value;
    }

    private void Start()
    {
        TotalLives = lives;
    }


    private void ReduceLives()
    {
        TotalLives--; // Esto ahora realmente modifica el campo 'lives'
        Debug.Log($"Vidas restantes: {TotalLives}");

        if (TotalLives <= 0)
        {
            TotalLives = 0;
            Debug.Log("¡Game Over!");
        }
    }

    private void OnEnable()
    {
        Enemy.OnEndReached += ReduceLives;
        //Esto es nuevo:
        Enemy.OnEnemyKilled += ReduceLives; // También reducimos una vida si es eliminado
    
        /** Si en vez de reducir vidas quiero un contador de kill, lo invocamos asi:
        Enemy.OnEnemyKilled += CountKill; en OnEnable y 
        Enemy.OnEnemyKilled -= CountKill; en OnDisable
        **/

    }

    private void OnDisable()
    {
        Enemy.OnEndReached -= ReduceLives;
        Enemy.OnEnemyKilled += ReduceLives;
    }

    /**Metodo para contar kills
    private void CountKill()
    {
    enemiesKilled++;
    Debug.Log($"Enemigos eliminados: {enemiesKilled}");
    }
    **/

}
