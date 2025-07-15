using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] private int lives = 10;

    public int TotalLives
    {
        get => lives;
        private set => lives = value;
    }

    public int CurrentWave { get; private set; }

    private int maxWaves; // ✅ Para controlar el número total de oleadas

    private void Start()
    {
        TotalLives = lives;
        CurrentWave = 1;

        // ✅ Obtener cantidad de oleadas desde el Spawner
        var spawner = FindObjectOfType<Spawner>();
        if (spawner != null)
        {
            maxWaves = spawner.TotalWaves; // accede a propiedad pública que expondremos en Spawner
        }
        else
        {
            Debug.LogError("Spawner no encontrado en la escena.");
        }
    }

    private void ReduceLives()
    {
        TotalLives--;
        Debug.Log($"Vidas restantes: {TotalLives}");

        if (TotalLives <= 0)
        {
            TotalLives = 0;
            Debug.Log("¡Game Over!");
        }
    }

    private void WaveCompleted()
    {
        if (CurrentWave < maxWaves)
        {
            CurrentWave++;
            Debug.Log($"Oleada completada. Próxima oleada: {CurrentWave}");
        }
        else
        {
            Debug.Log("✅ Todas las oleadas han sido completadas. ¡Nivel superado!");
            // Aquí podrías activar pantalla de victoria, pasar de nivel, etc.
        }
    }

    private void OnEnable()
    {
        Enemy.OnEndReached += ReduceLives;
        Enemy.OnEnemyKilled += ReduceLives;

        Spawner.OnWaveCompleted += WaveCompleted;
    }

    private void OnDisable()
    {
        Enemy.OnEndReached -= ReduceLives;
        Enemy.OnEnemyKilled -= ReduceLives;

        Spawner.OnWaveCompleted -= WaveCompleted;
    }
}