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

    public int CurrentWave { get; set; }

    private int maxWaves; // ✅ Para controlar el número total de oleadas

    private Coroutine countdownCoroutine; //Para el contador de inicio de las oleadas

    //Metodos para Iniciar la Oleada

    public void StartWaveCountdown(float countdownSeconds)
    {
        if (countdownCoroutine != null)
            StopCoroutine(countdownCoroutine);

        countdownCoroutine = StartCoroutine(WaveCountdownRoutine(countdownSeconds));
    }

    private IEnumerator WaveCountdownRoutine(float countdown)
    {
        float timer = countdown;

        while (timer > 0)
        {
            UIManager.Instance.UpdateWaveCountdown(Mathf.CeilToInt(timer)); // Actualiza UI
            yield return new WaitForSeconds(1f);
            timer -= 1f;
        }

        UIManager.Instance.UpdateWaveCountdown(0); // Muestra "0" segundos

        // 🔥 Inicia la oleada
        FindObjectOfType<Spawner>()?.StartFirstWave(); // Solo si Spawner tiene este método
    }

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

        StartWaveCountdown(40f);
    }

    private void ReduceLives()
    {
        TotalLives--;
        Debug.Log($"Vidas restantes: {TotalLives}");

        if (TotalLives == 0)
        {
            TotalLives = 0;
            this.GameOver();
        }
    }

    private void GameOver()
    {
        UIManager.Instance.ShowGameOverPanel();
    }


    private void WaveCompleted()
    {
        if (CurrentWave < maxWaves)
        {
            CurrentWave++;
            Debug.Log($"Oleada completada. Próxima oleada: {CurrentWave}");
        }
    }

    private void Victory()
    {
        UIManager.Instance.ShowNextLevelPanel();
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