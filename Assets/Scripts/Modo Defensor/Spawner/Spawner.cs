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

    private bool _isWaveActive = false;

    public int CurrentWave { get; set; }

    public int TotalWaves => waves.Count;

    private int _enemiesToSpawn;  // Cuántos quedan por spawnear
    private int _enemiesAlive;    // Cuántos están activos

    private void Start()
    {
        _waypoint = GetComponent<Waypoint>();
        _enemiesRamaining = currentWave.enemyCount;
        _pooler = GetPooler(); // Inicializar pooler al comienzo
        //PrepareWave();
    }

    /**private void Update()
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
    }**///Nuevo Metodo Update a continuacion, separando enemigos por spawnear y cuantos quedan vivos

    private void Update()
    {
        if (!_isWaveActive) return;

        _spawnTimer -= Time.deltaTime;
        if (_spawnTimer <= 0f)
        {
            _spawnTimer = GetSpawnDelay();

            if (_enemiesToSpawn > 0)
            {
                SpawnEnemy();
                _enemiesToSpawn--;
                _enemiesAlive++;
            }
        }

        // Si ya no hay enemigos por spawnear ni vivos, termina la oleada
        if (_enemiesToSpawn == 0 && _enemiesAlive == 0 && _isWaveActive)
        {
            _isWaveActive = false;
            OnWaveCompleted?.Invoke();
            StartCoroutine(NextWave());
        }
    }


    private void SpawnEnemy()
    {
        GameObject newInstance = _pooler.GetInstanceFromPool();

        Enemy enemy = newInstance.GetComponent<Enemy>();
        enemy.Waypoint = _waypoint;
        enemy.ResetEnemy();

        // Aseguramos de que la referencia esté lista ANTES de activar el GameObject
        enemy.EnemyHealth = newInstance.GetComponent<EnemyHealth>();


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

        if (currentWave <= 0)
            return enemyWave1Pooler;
        else if (currentWave == 1)
            return enemyWave2Pooler;
        else if (currentWave >= 2)
            return enemyWave3Pooler;

        return null;
    }

    /**private IEnumerator NextWave()
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
    }**///Nuevo metodo next wave, separando enemigos por spawnear y enemigos vivos

    private IEnumerator NextWave()
    {
        yield return new WaitForSeconds(delayBtwWaves);

        if (currentWaveIndex + 1 >= waves.Count)
        {
            Debug.Log("✅ Todas las oleadas han terminado. Metodo NextWave en Spwaner");
            yield break;
        }

        currentWaveIndex++;


        _pooler = GetPooler(); //✅ Actualizamos el pooler con la nueva oleada

        if (_pooler != null)
        {
            _pooler.SetPrefab(currentWave.enemyPrefab); // Cambiamos el prefab del pool si es necesario
        }

        PrepareWave();
    }

    /**   private void RecordEnemyEndReached()
       {
           ReduceRemainingEnemies();
       }

       private void RecordEnemyKilled(Enemy enemy)
       {
           ReduceRemainingEnemies();
       }**

       /**private void ReduceRemainingEnemies()
       {
           _enemiesRamaining--;
           Debug.Log("Enemy eliminado. Restantes: " + _enemiesRamaining);

           if (_enemiesRamaining <= 0)
           {
               OnWaveCompleted?.Invoke();
               StartCoroutine(NextWave());
           }
       }**///Nueva versión para evitar que spawneen 2 oleadas al mismo tiempo



    private void PrepareWave()
    {
        _enemiesToSpawn = currentWave.enemyCount;
        _enemiesAlive = 0;
        _spawnTimer = 0f;
        _isWaveActive = true;
    }
    private void OnEnable()
    {
        Enemy.OnEndReached += HandleEnemyExit;
        EnemyHealth.OnEnemyKilled += HandleEnemyKilled;
    }

    private void OnDisable()
    {
        Enemy.OnEndReached -= HandleEnemyExit;
        EnemyHealth.OnEnemyKilled -= HandleEnemyKilled;
    }

    private void HandleEnemyKilled(Enemy enemy)
    {
        _enemiesAlive--;
        Debug.Log($"🚪 Enemy salió. Enemigos vivos: {_enemiesAlive}");

        CheckWaveEnd();
    }

    private void HandleEnemyExit()
    {
        _enemiesAlive--;
        Debug.Log($"☠️ Enemy muerto. Enemigos vivos: {_enemiesAlive}");

        CheckWaveEnd();
    }

    private void CheckWaveEnd()
    {
        if (_enemiesToSpawn == 0 && _enemiesAlive <= 0 && _isWaveActive)
        {
            _isWaveActive = false;

            // Si es la última oleada
            if (currentWaveIndex == waves.Count - 1)
            {
                Debug.Log("🏆 ¡Victoria! Todas las oleadas completadas y enemigos eliminados.");
                StartCoroutine(DelayedVictory()); // 👈 Corrutina con delay visual
            }
            else
            {
                OnWaveCompleted?.Invoke();
                StartCoroutine(NextWave());
            }
        }
    }

    private IEnumerator DelayedVictory()
    {
        yield return new WaitForSeconds(1f); // ⏳ Tiempo para mostrar animación o muerte final
        LevelManager.Instance.SendMessage("Victory"); // o directamente LevelManager.Instance.Victory();
    }


    public void StartFirstWave()
    {
        Debug.Log("🚀 Primera oleada iniciada tras el conteo");
        PrepareWave();
    }

}