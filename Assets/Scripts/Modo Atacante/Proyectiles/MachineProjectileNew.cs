using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineProjectileNew : ProjectileNew
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyNew enemy = other.GetComponent<EnemyNew>();
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

            ObjectPoolerNew.ReturnToPool(gameObject);
        }
    }

    private void OnEnable()
    {
        StartCoroutine(ObjectPoolerNew.ReturnToPoolWithDelay(gameObject, 5f));
    }

}
