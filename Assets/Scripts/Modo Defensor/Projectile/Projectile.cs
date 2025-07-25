using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    public static Action<Enemy, float> OnEnemyHit;

    [SerializeField] protected int idProjectile;

    public int IdProjectile => idProjectile; // Getter expuesto

    [SerializeField] protected float moveSpeed = 10f;

    [SerializeField] private float minDistanceToDealDamage = 0.1f;

    public TurretProjectile TurretOwner { get; set; }

    public float Damage { get; set; }

    protected Enemy _enemyTarget;

    public IObjectPooler Pooler { get; set; }


    // ✅ Lista estática compartida
    private static List<(int projId, int enemyId)> ignorePairs = new List<(int, int)>
    {
        (1, 2),
        (2, 2),
        (3, 2),
    };

    public static bool ShouldIgnore(int projId, int enemyId)
    {
        return ignorePairs.Contains((projId, enemyId));
    }

    protected virtual void Update()
    {
        if (_enemyTarget != null)
        {
            MoveProjectile();
            RotateProjectile();
        }
    }

    /** protected virtual void MoveProjectile()
     {
         transform.position = Vector2.MoveTowards(transform.position,
             _enemyTarget.transform.position, moveSpeed * Time.deltaTime);
         float distanceToTarget = (_enemyTarget.transform.position - transform.position).magnitude;

         if (distanceToTarget < minDistanceToDealDamage)
         {
             OnEnemyHit?.Invoke(_enemyTarget, Damage);
             _enemyTarget.EnemyHealth.DealDamage(Damage);
             TurretOwner.ResetTurretProjectile();
             ObjectPooler.ReturnToPool(gameObject);
         }
     }**/ //A continuacion nuevo script para que x projectile ignore a x enemigo

    protected virtual void MoveProjectile()
    {
        if (_enemyTarget == null || !_enemyTarget.gameObject.activeInHierarchy)
        {
            //ObjectPooler.ReturnToPool(gameObject); Asi se usaba solamente en el modo defensor, y andaba perfecto
            //TurretOwner.ResetTurretProjectile();  Asi se usaba solamente en el modo defensor, y andaba perfecto
            ReturnProjectileToPool(); // Esto es nuevo se agrega para usar la interfaz pooler con el modo atacante

            return;
        }

        transform.position = Vector2.MoveTowards(transform.position,
            _enemyTarget.transform.position, moveSpeed * Time.deltaTime);
        float distanceToTarget = (_enemyTarget.transform.position - transform.position).magnitude;

        if (distanceToTarget < minDistanceToDealDamage)
        {
            // 🚫 Agregar condición para ignorar el daño entre ciertos IDs
            if (!ignorePairs.Contains((idProjectile, _enemyTarget.IdEnemy)))
            {

                OnEnemyHit?.Invoke(_enemyTarget, Damage);
                _enemyTarget.EnemyHealth.DealDamage(Damage);
            }
            else
            {
                Debug.Log($"⚠️ Projectile {idProjectile} ignoró al Enemy {_enemyTarget.IdEnemy}");
            }

            //TurretOwner.ResetTurretProjectile();
            //ObjectPooler.ReturnToPool(gameObject);
            ReturnProjectileToPool(); //ObjectPooler Utilizado en modo atacante

        }
    }

    private void ReturnProjectileToPool()
    {
        if (Pooler != null)
        {
            Pooler.ReturnToPool(gameObject);
        }
        else
        {
            // Fallback por si acaso
            gameObject.SetActive(false);
        }

        if (TurretOwner != null)
        {
            TurretOwner.ResetTurretProjectile();
        }
    }

    private void RotateProjectile()
    {
        Vector3 enemyPos = _enemyTarget.transform.position - transform.position;
        float angle = Vector3.SignedAngle(transform.up, enemyPos, transform.forward);
        transform.Rotate(0f, 0f, angle);
    }

    /**public void SetEnemy(Enemy enemy)
    {
        _enemyTarget = enemy;
    }**/ //Con este metodo no se ignoran a enemigos que deben ser ignorados, a continuacion, la actualizacion:

    public bool SetEnemy(Enemy enemy)
    {
        if (ignorePairs.Contains((idProjectile, enemy.IdEnemy)))
        {
            Debug.Log($"❌ Projectile {idProjectile} no seguirá al Enemy {enemy.IdEnemy}");
            return false;
        }

        _enemyTarget = enemy;
        return true;
    }

    public void ResetProjectile()
    {
        _enemyTarget = null;
        transform.localRotation = Quaternion.identity;
    }

}
