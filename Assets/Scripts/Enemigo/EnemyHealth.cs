using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{


    [SerializeField] private GameObject healthBarPrefab;
    [SerializeField] private Transform barPosition;
    // Start is called before the first frame update
    [SerializeField] private float initialHealth = 10f;
    [SerializeField] private float maxHealth = 10f;

    public float CurrentHealth { get; set; }

    public static event System.Action<Enemy> OnEnemyKilled;
    
    private Image _healthBar;

    //Modifica tu EnemyHealth.cs para guardar una referencia al Enemy, y llamarlo en DealDamage:

    private Enemy _enemy;


    private void Start()
    {
        CreateHealthBar();
        CurrentHealth = initialHealth;

        _enemy = GetComponent<Enemy>(); //Descomentamos para lo del daño, permite obtener una referencia

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            DealDamage(5f);
        }

        _healthBar.fillAmount = Mathf.Lerp(_healthBar.fillAmount,
            CurrentHealth / maxHealth, Time.deltaTime * 10f);
    }

    private void CreateHealthBar()
    {
        GameObject newBar = Instantiate(healthBarPrefab, barPosition.position, Quaternion.identity);
        newBar.transform.SetParent(transform);

        EnemyHealthContainer container = newBar.GetComponent<EnemyHealthContainer>();
        _healthBar = container.FillAmountImage;
    }

    /**public void DealDamage(float damageReceived)
    {
        CurrentHealth -= damageReceived;

        _enemy.Stun(); // Le decimos que se paralice un momento al recibir daño


        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            Die();
        }
        //else
        //{
        //   OnEnemyHit?.Invoke(_enemy);
        //}
    }**/

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

        AchievementManager.Instance.AddProgress("Kill20Ogro", 1);
        AchievementManager.Instance.AddProgress("Kill50Ogro", 1);
        AchievementManager.Instance.AddProgress("Kill100Ogro", 1);
        //Esto es nuevo, antes estaba comentado esto: OnEnemyKilled?.Invoke(_enemy);
        //Enemy.OnEnemyKilled?.Invoke();
        OnEnemyKilled?.Invoke(_enemy);

        ObjectPooler.ReturnToPool(gameObject);
    }
}
