using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ObjectPoolerNew : MonoBehaviour, IObjectPooler
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int poolSize = 10;

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

    public  void ReturnToPool(GameObject instance)
    {
        instance.SetActive(false);
    }

    public  IEnumerator ReturnToPoolWithDelay(GameObject instance, float delay)
    {
        yield return new WaitForSeconds(delay);
        instance.SetActive(false);
    }

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