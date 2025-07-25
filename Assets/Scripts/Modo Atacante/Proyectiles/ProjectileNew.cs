using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileNew : MonoBehaviour
{
    public static Action<EnemyNew, float> OnEnemyHit;

    [SerializeField] protected int idProjectile;
    [SerializeField] protected float moveSpeed = 10f;
    [SerializeField] private float minDistanceToDealDamage = 0.5f;

    public int IdProjectile => idProjectile; // Getter expuesto

    public TurretProjectileNew TurretOwner { get; set; }

    public float Damage { get; set; }

    protected EnemyNew _enemyTarget;

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

    protected virtual void MoveProjectile()
    {
        if (_enemyTarget == null || !_enemyTarget.gameObject.activeInHierarchy)
        {
            Debug.Log("❌ Target inválido o desactivado");
            ObjectPoolerNew.ReturnToPool(gameObject);
            TurretOwner.ResetTurretProjectile();
            return;
        }

        transform.position = Vector2.MoveTowards(transform.position,
            _enemyTarget.transform.position, moveSpeed * Time.deltaTime);

        float distanceToTarget = (_enemyTarget.transform.position - transform.position).magnitude;

        Debug.Log($"🟡 Distancia al enemigo: {distanceToTarget}");

        if (distanceToTarget < minDistanceToDealDamage)
        {
            Debug.Log("💥 Dentro del rango de impacto");

            if (!ignorePairs.Contains((idProjectile, _enemyTarget.IdEnemy)))
            {
                Debug.Log("✅ Daño infligido");
                OnEnemyHit?.Invoke(_enemyTarget, Damage);
                _enemyTarget.EnemyHealth.DealDamage(Damage);
            }
            else
            {
                Debug.Log($"⚠️ Projectile {idProjectile} ignoró al Enemy {_enemyTarget.IdEnemy}");
            }

            TurretOwner.ResetTurretProjectile();
            ObjectPoolerNew.ReturnToPool(gameObject);
        }
    }
    
    private void RotateProjectile()
    {
        Vector3 enemyPos = _enemyTarget.transform.position - transform.position;
        float angle = Vector3.SignedAngle(transform.up, enemyPos, transform.forward);
        transform.Rotate(0f, 0f, angle);
    }

    public bool SetEnemy(EnemyNew enemy)
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
