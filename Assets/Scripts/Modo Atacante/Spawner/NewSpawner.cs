using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewSpawner : MonoBehaviour
{
<<<<<<< HEAD
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
=======
    [SerializeField] private float delayBetweenSpawns = 0.5f;
    [SerializeField] private float delayBetweenWaves = 1f;

    private Waypoint waypoint;
    private Dictionary<string, ObjectPoolerNew> _enemyPools = new();
    private Coroutine _spawnRoutine;
    private Coroutine _nextWaveRoutine;

    private List<EnemyInstance> _selectedEnemies;

    private int _enemiesAlive;

    // Eventos de victoria/derrota demoníaca
    public static event Action OnDemonVictory;
    public static event Action OnDemonDefeat;

    private void Awake()
    {
        // Obtiene el componente Waypoint del mismo GameObject
        waypoint = GetComponent<Waypoint>();

        if (waypoint == null)
        {
            Debug.Log("❌ Waypoint no encontrado en el GameObject del Spawner.");
        }
    }

    public void StartWave(List<EnemyInstance> selectedEnemies)
    {
        _selectedEnemies = selectedEnemies;
        PreparePools();

        if (_spawnRoutine != null)
            StopCoroutine(_spawnRoutine);

        _spawnRoutine = StartCoroutine(SpawnEnemiesRoutine());
    }

    private void PreparePools()
    {
        _enemyPools.Clear();

        foreach (var instance in _selectedEnemies)
        {
            string enemyID = instance.Data.ID;

            if (!_enemyPools.ContainsKey(enemyID))
            {
                GameObject poolerObj = new GameObject($"Pooler_{enemyID}");
                var pooler = poolerObj.AddComponent<ObjectPoolerNew>();
                pooler.SetPrefab(instance.Data.Prefab);
                pooler.Initialize(); // <- Ahora sí creamos el pool con el prefab correcto
                _enemyPools.Add(enemyID, pooler);
>>>>>>> 4d87c4c4b9925b4c72bea6e19678438d7b7b0a48
            }
        }
    }

<<<<<<< HEAD
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
=======
    private IEnumerator SpawnEnemiesRoutine()
    {
        Dictionary<EnemyPosition, List<EnemyInstance>> wavesByPosition = new();

        // Inicializa las listas para cada posición
        foreach (EnemyPosition pos in Enum.GetValues(typeof(EnemyPosition)))
        {
            wavesByPosition[pos] = new List<EnemyInstance>();
        }

        // Agrupa enemigos seleccionados por su posición
        foreach (var enemy in _selectedEnemies)
        {
            wavesByPosition[enemy.Data.Position].Add(enemy);
        }

        // Ordenamos las oleadas por orden lógico de aparición: Retaguardia → Medio → Vanguardia
        var waveOrder = new List<EnemyPosition> {
        EnemyPosition.Vanguardia,
        EnemyPosition.Medio,
        EnemyPosition.Retaguardia
        };

        foreach (var pos in waveOrder)
        {
            var waveList = wavesByPosition[pos];

            if (waveList.Count == 0) continue; // Si no hay enemigos en esta oleada, la saltamos

            Debug.Log($"🚩 Spawneando oleada de tipo {pos}");

            foreach (var enemy in waveList)
            {
                for (int i = 0; i < enemy.Quantity; i++)
                {
                    SpawnEnemy(enemy.Data);
                    _enemiesAlive++; // ✅ Contamos enemigos vivos
                    yield return new WaitForSeconds(delayBetweenSpawns);
                }
            }

            while (_enemiesAlive > 0)
                yield return null;

            Debug.Log($"⏭️ Suboleada {pos} finalizada. Esperando {delayBetweenWaves}s para la siguiente...");

            yield return new WaitForSeconds(delayBetweenWaves);
        }

        Debug.Log("✅ Todas las oleadas se han spawneado correctamente.");
        EvaluateBattleResult();
    }

    private void SpawnEnemy(EnemyData data)
    {
        if (!_enemyPools.ContainsKey(data.ID))
        {
            Debug.Log($"No se encontró pool para el enemigo {data.ID}");
            return;
        }

        GameObject instance = _enemyPools[data.ID].GetInstanceFromPool();
        instance.transform.position = waypoint.GetWaypointPosition(0);
        instance.SetActive(true);

        EnemyNew enemy = instance.GetComponent<EnemyNew>();
        enemy.Waypoint = waypoint;
        enemy.ResetEnemy();
    }


    private void OnEnable()
    {
        EnemyHealthNew.OnEnemyKilled += HandleEnemyKilled;
        EnemyNew.OnEndReached += HandleEnemyExit;
>>>>>>> 4d87c4c4b9925b4c72bea6e19678438d7b7b0a48
    }

    private void OnDisable()
    {
<<<<<<< HEAD
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
=======
        EnemyHealthNew.OnEnemyKilled -= HandleEnemyKilled;
        EnemyNew.OnEndReached -= HandleEnemyExit;
    }

    private void HandleEnemyKilled(EnemyNew enemy)
    {
        _enemiesAlive--;
        CheckWaveEnded();
    }

    private void HandleEnemyExit(EnemyNew enemy)
    {
        _enemiesAlive--;
        CheckWaveEnded();
    }


    private void CheckWaveEnded()
    {
        if (_enemiesAlive <= 0)
        {
            Debug.Log("✅ Todos los enemigos de la suboleada han sido eliminados o llegaron al final.");

        }
    }
    private void EvaluateBattleResult()
    {
        if (LevelManagerNew.Instance.CurrentLives <= 0)
        {
            Debug.Log("🎉 ¡Victoria demoníaca! El jugador humano se quedó sin vidas.");
            OnDemonVictory?.Invoke();
        }
        else
        {
            Debug.Log("☠️ Derrota demoníaca... No lograste eliminar todas las vidas humanas.");
            OnDemonDefeat?.Invoke();
        }
    }

>>>>>>> 4d87c4c4b9925b4c72bea6e19678438d7b7b0a48
}
