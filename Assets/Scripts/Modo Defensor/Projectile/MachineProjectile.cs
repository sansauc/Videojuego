using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MachineProjectile : Projectile
{
    public Vector2 Direction { get; set; }

    private static List<(int projId, int enemyId)> ignorePairs = new List<(int, int)>
    {
        (4, 3),
        (5, 3)
    };


    private new static bool ShouldIgnore(int projId, int enemyId)
    {
        return ignorePairs.Contains((projId, enemyId));
    }

    protected override void Update()
    {

        MoveProjectile();

    }

    protected override void MoveProjectile()
    {
        Vector2 movement = Direction.normalized * moveSpeed * Time.deltaTime;
        transform.Translate(movement);
    }


    /**private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy.EnemyHealth.CurrentHealth > 0f)
            {
                OnEnemyHit?.Invoke(enemy, Damage);
                enemy.EnemyHealth.DealDamage(Damage);
            }

            ObjectPooler.ReturnToPool(gameObject);
        }
    }**///Este metodo no ignora enemigos, a continuacion la modificacion:

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy == null || enemy.EnemyHealth.CurrentHealth <= 0f)
                return;

            // ❌ Verificar si el proyectil debe ignorar al enemigo
            if (ShouldIgnore(idProjectile, enemy.IdEnemy))
            {
                Debug.Log($"❌ MachineProjectile {idProjectile} ignoró al Enemy {enemy.IdEnemy}");
            }
            else
            {
                OnEnemyHit?.Invoke(enemy, Damage);
                enemy.EnemyHealth.DealDamage(Damage);
            }

            ObjectPooler.ReturnToPool(gameObject);
        }
    }

    private void OnEnable()
    {
        StartCoroutine(ObjectPooler.ReturnToPoolWithDelay(gameObject, 5f));
    }

}
