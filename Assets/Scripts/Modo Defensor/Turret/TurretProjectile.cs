using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretProjectile : MonoBehaviour
{

    [SerializeField] protected Transform projectileSpawnPosition;
    [SerializeField] protected float delayBtwAttacks = 2f;
    [SerializeField] protected float damage = 2f;

    public float Damage { get; set; }
    public float DelayPerShot { get; set; }


    protected float _nextAttackTime;

    protected ObjectPooler _pooler;
    protected Turret _turret;

    protected Projectile _currentProjectileLoaded;



    private void Start()
    {
        _turret = GetComponent<Turret>();
        _pooler = GetComponent<ObjectPooler>();

        Damage = damage;
        DelayPerShot = delayBtwAttacks;

        LoadProjectile();
    }

    /** Metodo viejo protected virtual void Update()
    {

        if (IsTurretEmpty())
        {
            LoadProjectile();
        }

        if (Time.time > _nextAttackTime)
        {
            if (_turret.CurrentEnemyTarget != null && _currentProjectileLoaded != null &&
                _turret.CurrentEnemyTarget.EnemyHealth.CurrentHealth > 0f)
            {
                _currentProjectileLoaded.transform.parent = null;
                _currentProjectileLoaded.SetEnemy(_turret.CurrentEnemyTarget);
            }

            _nextAttackTime = Time.time + DelayPerShot;
        }
    }**///Metodo nuevo 

    protected virtual void Update()
    {
        if (IsTurretEmpty())
        {
            LoadProjectile();
        }

        if (Time.time > _nextAttackTime)
        {
            if (_turret.CurrentEnemyTarget != null &&
                _currentProjectileLoaded != null &&
                _turret.CurrentEnemyTarget.EnemyHealth.CurrentHealth > 0f)
            {
                // 🚫 Intenta asignar enemigo. Si falla, descarta el proyectil.
                _currentProjectileLoaded.transform.parent = null;
                bool accepted = _currentProjectileLoaded.SetEnemy(_turret.CurrentEnemyTarget);

                if (!accepted)
                {
                    ObjectPooler.ReturnToPool(_currentProjectileLoaded.gameObject);
                    ResetTurretProjectile(); // Evita mantenerlo cargado
                }
                if (IsTurretEmpty())
                {
                    LoadProjectile();
                }
            }

            _nextAttackTime = Time.time + DelayPerShot;
        }
    }

    protected virtual void LoadProjectile()
    {
        GameObject newInstance = _pooler.GetInstanceFromPool();
        newInstance.transform.localPosition = projectileSpawnPosition.position;
        newInstance.transform.SetParent(projectileSpawnPosition);

        _currentProjectileLoaded = newInstance.GetComponent<Projectile>();
        _currentProjectileLoaded.TurretOwner = this;
        _currentProjectileLoaded.ResetProjectile();
        _currentProjectileLoaded.Damage = Damage;

        newInstance.SetActive(true);
    }

    /**protected virtual void LoadProjectile()
    {
        // 🚫 No cargar si no hay enemigo o si el enemigo será ignorado
        if (_turret.CurrentEnemyTarget == null)
            return;

        int enemyId = _turret.CurrentEnemyTarget.IdEnemy;

        // Verificamos el proyectil sin hardcodear el ID, obtenemos uno temporal para ver si lo aceptaría
        GameObject temp = _pooler.GetInstanceFromPool();
        Projectile tempProjectile = temp.GetComponent<Projectile>();

        if (Projectile.ShouldIgnore(tempProjectile.IdProjectile, enemyId))
        {
            ObjectPooler.ReturnToPool(temp); // devolvemos el proyectil sin usar
            return;
        }

        // ✅ Proceder normalmente
        temp.transform.localPosition = projectileSpawnPosition.position;
        temp.transform.SetParent(projectileSpawnPosition);

        _currentProjectileLoaded = tempProjectile;
        _currentProjectileLoaded.TurretOwner = this;
        _currentProjectileLoaded.ResetProjectile();
        _currentProjectileLoaded.Damage = Damage;

        temp.SetActive(true);
    }**/

    private bool IsTurretEmpty()
    {
        return _currentProjectileLoaded == null;
    }

    public void ResetTurretProjectile()
    {
        _currentProjectileLoaded = null;
    }

}
