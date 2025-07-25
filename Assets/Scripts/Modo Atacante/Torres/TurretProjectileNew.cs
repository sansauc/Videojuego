using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretProjectileNew : MonoBehaviour
{

    [SerializeField] protected float delayBtwAttacks = 2f;
    [SerializeField] protected float damage = 2f;
    [SerializeField] protected Transform projectileSpawnPosition;

    public float Damage { get; set; }
    public float DelayPerShot { get; set; }


    protected float _nextAttackTime;

    protected ObjectPoolerNew _pooler;
    protected TurretNew _turret;

    protected ProjectileNew _currentProjectileLoaded;


    private void Start()
    {
        _turret = GetComponent<TurretNew>();
        _pooler = GetComponent<ObjectPoolerNew>();
        _pooler.Initialize(); //pool listo

        Damage = damage;
        DelayPerShot = delayBtwAttacks;

        LoadProjectile();
    }

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
                    ObjectPoolerNew.ReturnToPool(_currentProjectileLoaded.gameObject);
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

        _currentProjectileLoaded = newInstance.GetComponent<ProjectileNew>();
        _currentProjectileLoaded.TurretOwner = this;
        _currentProjectileLoaded.ResetProjectile();
        _currentProjectileLoaded.Damage = Damage;

        newInstance.SetActive(true);
    }

    private bool IsTurretEmpty()
    {
        return _currentProjectileLoaded == null;
    }

    public void ResetTurretProjectile()
    {
        _currentProjectileLoaded = null;
    }




}
