using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthNew : MonoBehaviour
{
    [SerializeField] private GameObject healthBarPrefab;
    [SerializeField] private Transform barPosition;
    // Start is called before the first frame update
    [SerializeField] private float initialHealth = 10f;
    [SerializeField] private float maxHealth = 10f;

    public float CurrentHealth { get; set; }

    public static event Action<EnemyNew> OnEnemyKilled;

    private Image _healthBar;

    private EnemyNew _enemy;


    private void Start()
    {
        CreateHealthBar();
        CurrentHealth = initialHealth;

        _enemy = GetComponent<EnemyNew>(); //Descomentamos para lo del daño, permite obtener una referencia

    }

    private void Update()
    {
        _healthBar.fillAmount = Mathf.Lerp(_healthBar.fillAmount,
            CurrentHealth / maxHealth, Time.deltaTime * 10f);
    }

    private void CreateHealthBar()
    {
        GameObject newBar = Instantiate(healthBarPrefab, barPosition.position, Quaternion.identity);
        newBar.transform.SetParent(transform);

        EnemyHealthContainerNew container = newBar.GetComponent<EnemyHealthContainerNew>();

        _healthBar = container.FillAmountImage;
    }


    public void DealDamage(float damageReceived)
    {
        CurrentHealth -= damageReceived;

        if (CurrentHealth <= 0)
        {
            Die(); // Die() desactiva el objeto
        }
        else
        {
            // Solo aturdir si el enemigo sigue activo
            if (gameObject.activeInHierarchy)
            {
                _enemy.Stun();
            }
        }
    }

    private void Die()
    {
        CurrentHealth = initialHealth;
        _healthBar.fillAmount = 1f;


        OnEnemyKilled?.Invoke(_enemy); // IMPORTANTE: este evento es el que decrementa enemigos en Spawner

        ObjectPoolerNew.ReturnToPool(gameObject);
    }
}
