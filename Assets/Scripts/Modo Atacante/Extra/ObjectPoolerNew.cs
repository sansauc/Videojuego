using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
<<<<<<< HEAD
public class ObjectPoolerNew : MonoBehaviour, IObjectPooler
=======

public class ObjectPoolerNew : MonoBehaviour
>>>>>>> 4d87c4c4b9925b4c72bea6e19678438d7b7b0a48
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int poolSize = 10;

<<<<<<< HEAD
    private List<GameObject> _pool;
    private GameObject _poolContainer;

    // Ya no hacemos nada en Awake, para evitar usar prefab nulo
    private void Awake()
    {
        // Vacío para evitar usar prefab antes de asignarlo
    }

    // Inicializador que debes llamar para configurar el pooler
    public void InitializePooler(GameObject newPrefab, int newPoolSize = 10)
    {
        prefab = newPrefab;
        poolSize = newPoolSize;

        _pool = new List<GameObject>();
        _poolContainer = new GameObject($"Pool - {prefab.name}");

=======
    private List<GameObject> _pool = new();
    private GameObject _poolContainer;

    public void Initialize()
    {
        if (prefab == null)
        {
            Debug.LogError("❌ No se asignó un prefab al pooler.");
            return;
        }

        _poolContainer = new GameObject($"Pool - {prefab.name}");
>>>>>>> 4d87c4c4b9925b4c72bea6e19678438d7b7b0a48
        CreatePooler();
    }

    private void CreatePooler()
    {
        for (int i = 0; i < poolSize; i++)
        {
            _pool.Add(CreateInstance());
        }
    }

    private GameObject CreateInstance()
    {
        GameObject newInstance = Instantiate(prefab);
        newInstance.transform.SetParent(_poolContainer.transform);
        newInstance.SetActive(false);
        return newInstance;
    }

    public GameObject GetInstanceFromPool()
    {
        for (int i = 0; i < _pool.Count; i++)
        {
            if (!_pool[i].activeInHierarchy)
            {
                return _pool[i];
            }
        }

        return CreateInstance();
    }

<<<<<<< HEAD
    public  void ReturnToPool(GameObject instance)
    {
        instance.SetActive(false);
    }

    public  IEnumerator ReturnToPoolWithDelay(GameObject instance, float delay)
=======
    public static void ReturnToPool(GameObject instance)
    {

        instance.SetActive(false);

    }
    public static IEnumerator ReturnToPoolWithDelay(GameObject instance, float delay)
>>>>>>> 4d87c4c4b9925b4c72bea6e19678438d7b7b0a48
    {
        yield return new WaitForSeconds(delay);
        instance.SetActive(false);
    }

<<<<<<< HEAD
    public void ClearPool()
    {
        foreach (var obj in _pool)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        _pool.Clear();
    }
}
=======

    public void SetPrefab(GameObject newPrefab)
    {
        prefab = newPrefab;
    }
}
>>>>>>> 4d87c4c4b9925b4c72bea6e19678438d7b7b0a48
