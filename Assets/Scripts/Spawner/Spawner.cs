using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum SpawnModes
{
    Fixed,
    Random
}

public class Spawner : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private SpawnModes spawnMode = SpawnModes.Fixed;
    [SerializeField] private int enemyCount = 10;
    [SerializeField] private float delayBtwWaves = 1f; //Se implementa con el sistema de oleadas



    [Header("Fixed Delay")]
    [SerializeField] private float delayBtwSpawns;

    [Header("Random Delay")]
    [SerializeField] private float minRandomDelay;
    [SerializeField] private float maxRandomDelay;

    private float _spawnTimer;
    private int _enemiesSpawned;
    private int _enemiesRamaining; //Se utiliza para el sistema de oleadas
    private ObjectPooler _pooler;

    private Waypoint _waypoint;

    private bool _isWaveActive = true; //para controlar si se activa la oleada

    private void Start()
    {
        _pooler = GetComponent<ObjectPooler>();
        _waypoint = GetComponent<Waypoint>();

        _enemiesRamaining = enemyCount;

    }

    /**private void Update()
    {
        _spawnTimer -= Time.deltaTime;
        if (_spawnTimer < 0)
        {
            _spawnTimer = GetSpawnDelay();
            if (_enemiesSpawned < enemyCount)
            {
                _enemiesSpawned++;
                SpawnEnemy();
            }
        }
    }**/ //Antiguo metodo sin oleadas

    private void Update()
    {
        if (!_isWaveActive) return;

        _spawnTimer -= Time.deltaTime;
        if (_spawnTimer < 0)
        {
            _spawnTimer = GetSpawnDelay();
            if (_enemiesSpawned < enemyCount)
            {
                _enemiesSpawned++;
                SpawnEnemy();
            }
            else
            {
                // Oleada completa, deja de spawnear
                _isWaveActive = false;
                Debug.Log("La oleada se desactivo, bandera: " + _isWaveActive);
            }
        }
    }

    private void SpawnEnemy()
    {
        GameObject newInstance = _pooler.GetInstanceFromPool();

        Enemy enemy = newInstance.GetComponent<Enemy>();
        enemy.Waypoint = _waypoint;

        enemy.ResetEnemy();

        enemy.transform.localPosition = _waypoint.GetWaypointPosition(0);

        newInstance.SetActive(true);
    }

    private float GetSpawnDelay()
    {
        float delay = 0f;
        if (spawnMode == SpawnModes.Fixed)
        {
            delay = delayBtwSpawns;
        }
        else
        {
            delay = GetRandomDelay();
        }

        return delay;
    }

    private float GetRandomDelay()
    {
        float randomTimer = Random.Range(minRandomDelay, maxRandomDelay);
        return randomTimer;
    }

    private IEnumerator NextWave()
    {
        yield return new WaitForSeconds(delayBtwWaves);
        _enemiesRamaining = enemyCount;
        _spawnTimer = 0f;
        _enemiesSpawned = 0;
        _isWaveActive = true; //reiniciamos la bandera
    }

    private void RecordEnemyEndReached()
    {
        ReduceRemainingEnemies();
    }

    private void RecordEnemyKilled(Enemy enemy)
    {
        ReduceRemainingEnemies();
    }

    private void ReduceRemainingEnemies()
    {
        _enemiesRamaining--;
        Debug.Log("Enemy eliminado. Restantes: " + _enemiesRamaining);
        if (_enemiesRamaining <= 0)
        {
            StartCoroutine(NextWave());
        }
    }

    private void OnEnable()
    {
        Enemy.OnEndReached += RecordEnemyEndReached;
        EnemyHealth.OnEnemyKilled += RecordEnemyKilled;
    }

    private void OnDisable()
    {
        Enemy.OnEndReached -= RecordEnemyEndReached;
        EnemyHealth.OnEnemyKilled -= RecordEnemyKilled;
    }
}

