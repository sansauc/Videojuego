using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewSpawner : MonoBehaviour
{
    public static Action OnAllWavesCompleted;

    [Header("Pooler Config")]
    [SerializeField] private float delayBetweenSpawns = 1f;
    [SerializeField] private float delayBetweenWaves = 2f;

    private List<EnemyInstance> selectedEnemies = new List<EnemyInstance>();
    private Queue<EnemyInstance> currentWaveQueue = new Queue<EnemyInstance>();

    private Waypoint waypoint;


    private int _enemiesToSpawn = 0;
    private int _enemiesAlive = 0;

    private bool _isWaveRunning = false;
    private int _currentWaveIndex = 0;
    private List<List<EnemyInstance>> sortedWaves = new List<List<EnemyInstance>>();

    private Dictionary<EnemyData, ObjectPoolerNew> poolersByData = new Dictionary<EnemyData, ObjectPoolerNew>();


    private void Awake()
    {
        waypoint = FindObjectOfType<Waypoint>();

        if (waypoint == null)
        {
            Debug.LogError("❌ No se encontró ningún Waypoint en la escena.");
        }
    }


    public void SetupSelectedEnemies(List<EnemyInstance> enemies)
    {
        EnemyHealth.IsDefenderMode = false;
        selectedEnemies = enemies;
        SortEnemiesIntoWaves();
        PreparePoolers();
    }

    private void SortEnemiesIntoWaves()
    {
        sortedWaves.Clear();

        List<EnemyInstance> vanguardia = new List<EnemyInstance>();
        List<EnemyInstance> medio = new List<EnemyInstance>();
        List<EnemyInstance> retaguardia = new List<EnemyInstance>();

        foreach (var instance in selectedEnemies)
        {
            switch (instance.Data.Position)
            {
                case EnemyPosition.Vanguardia:
                    vanguardia.Add(instance);
                    break;
                case EnemyPosition.Medio:
                    medio.Add(instance);
                    break;
                case EnemyPosition.Retaguardia:
                    retaguardia.Add(instance);
                    break;
            }
        }

        if (vanguardia.Count > 0) sortedWaves.Add(vanguardia);
        if (medio.Count > 0) sortedWaves.Add(medio);
        if (retaguardia.Count > 0) sortedWaves.Add(retaguardia);
    }

    private void PreparePoolers()
    {
        poolersByData.Clear();

        foreach (var instance in selectedEnemies)
        {
            if (!poolersByData.ContainsKey(instance.Data))
            {
                GameObject poolerObj = new GameObject($"Pooler - {instance.Data.Title}");
                var pooler = poolerObj.AddComponent<ObjectPoolerNew>();
                pooler.InitializePooler(instance.Data.Prefab, 10); // inicializa aquí con prefab y tamaño
                poolersByData.Add(instance.Data, pooler);
            }
        }
    }

    public void StartWaves()
    {
        _currentWaveIndex = 0;
        StartCoroutine(SpawnWaveRoutine());
    }

    private IEnumerator SpawnWaveRoutine()
    {
        while (_currentWaveIndex < sortedWaves.Count)
        {
            var wave = sortedWaves[_currentWaveIndex];
            _isWaveRunning = true;
            currentWaveQueue.Clear();

            foreach (var instance in wave)
            {
                for (int i = 0; i < instance.Quantity; i++)
                {
                    currentWaveQueue.Enqueue(instance);
                }
            }

            _enemiesToSpawn = currentWaveQueue.Count;
            _enemiesAlive = 0;

            while (_enemiesToSpawn > 0)
            {
                SpawnNextEnemy();
                _enemiesToSpawn--;
                _enemiesAlive++;
                yield return new WaitForSeconds(delayBetweenSpawns);
            }

            while (_enemiesAlive > 0)
            {
                yield return null; // Esperar que mueran todos
            }

            _isWaveRunning = false;
            _currentWaveIndex++;
            yield return new WaitForSeconds(delayBetweenWaves);
        }

        OnAllWavesCompleted?.Invoke();
    }

    private void SpawnNextEnemy()
    {
        var instance = currentWaveQueue.Dequeue();

        if (!poolersByData.TryGetValue(instance.Data, out var pooler))
        {
            Debug.LogError($"❌ No se encontró pooler para {instance.Data.Title}");
            return;
        }

        GameObject newEnemy = pooler.GetInstanceFromPool();
        Enemy enemy = newEnemy.GetComponent<Enemy>();
        enemy.ResetEnemy();
        enemy.Waypoint = waypoint;
        enemy.EnemyHealth = newEnemy.GetComponent<EnemyHealth>();
        newEnemy.transform.position = waypoint.GetWaypointPosition(0);
        newEnemy.SetActive(true);
    }

    private void OnEnable()
    {
        Enemy.OnEndReached += OnEnemyExit;
        EnemyHealth.OnEnemyKilled += OnEnemyKilled;
    }

    private void OnDisable()
    {
        Enemy.OnEndReached -= OnEnemyExit;
        EnemyHealth.OnEnemyKilled -= OnEnemyKilled;
    }

    private void OnEnemyExit()
    {
        _enemiesAlive--;
    }

    private void OnEnemyKilled(Enemy enemy)
    {
        _enemiesAlive--;
    }
}
