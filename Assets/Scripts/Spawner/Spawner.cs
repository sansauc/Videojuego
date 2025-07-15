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

[System.Serializable]
public class WaveData
{
    public GameObject enemyPrefab;
    public int enemyCount;
}

public class Spawner : MonoBehaviour
{
    public static Action OnWaveCompleted;

    [SerializeField] private List<WaveData> waves;
    private WaveData currentWave => waves[currentWaveIndex];

    private int currentWaveIndex = 0; // ✅ Declaración añadida

    [Header("Settings")]
    [SerializeField] private SpawnModes spawnMode = SpawnModes.Fixed;
    [SerializeField] private int enemyCount = 10;
    [SerializeField] private float delayBtwWaves = 1f;

    [Header("Fixed Delay")]
    [SerializeField] private float delayBtwSpawns;

    [Header("Random Delay")]
    [SerializeField] private float minRandomDelay;
    [SerializeField] private float maxRandomDelay;

    [Header("Poolers")]
    [SerializeField] private ObjectPooler enemyWave1Pooler;
    [SerializeField] private ObjectPooler enemyWave2Pooler;
    [SerializeField] private ObjectPooler enemyWave3Pooler;

    private float _spawnTimer;
    private int _enemiesSpawned;
    private int _enemiesRamaining;

    private ObjectPooler _pooler; // ✅ Declaración añadida

    private Waypoint _waypoint;

    private bool _isWaveActive = true;

    public int CurrentWave { get; set; }

    public int TotalWaves => waves.Count;


    private void Start()
    {
        _waypoint = GetComponent<Waypoint>();
        _enemiesRamaining = currentWave.enemyCount;
        _pooler = GetPooler(); // Inicializar pooler al comienzo
    }

    private void Update()
    {
        if (!_isWaveActive) return;

        _spawnTimer -= Time.deltaTime;
        if (_spawnTimer < 0)
        {
            _spawnTimer = GetSpawnDelay();
            if (_enemiesSpawned < _enemiesRamaining)
            {
                _enemiesSpawned++;
                SpawnEnemy();
            }
            else
            {
                _isWaveActive = false;
                Debug.Log("La oleada se desactivó, bandera: " + _isWaveActive);
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
        return spawnMode == SpawnModes.Fixed ? delayBtwSpawns : GetRandomDelay();
    }

    private float GetRandomDelay()
    {
        return Random.Range(minRandomDelay, maxRandomDelay);
    }

    private ObjectPooler GetPooler()
    {
        int currentWave = LevelManager.Instance.CurrentWave;

        if (currentWave <= 1)
            return enemyWave1Pooler;
        else if (currentWave == 2)
            return enemyWave2Pooler;
        else if (currentWave >= 3)
            return enemyWave3Pooler;

        return null;
    }

    private IEnumerator NextWave()
    {
        yield return new WaitForSeconds(delayBtwWaves);

        if (currentWaveIndex + 1 >= waves.Count)
        {
            Debug.Log("¡Todas las oleadas han terminado!");
            yield break;
        }

        currentWaveIndex++;

        _pooler = GetPooler(); // ✅ Actualizamos el pooler con la nueva oleada
        if (_pooler != null)
        {
            _pooler.SetPrefab(currentWave.enemyPrefab); // Cambiamos el prefab del pool si es necesario
        }

        _enemiesRamaining = currentWave.enemyCount;
        _enemiesSpawned = 0;
        _spawnTimer = 0f;
        _isWaveActive = true;
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
            OnWaveCompleted?.Invoke();
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