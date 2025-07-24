using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretNew : MonoBehaviour
{
    [SerializeField] private float attackRange = 6f;

    public EnemyNew CurrentEnemyTarget { get; set; }
    public float AttackRange => attackRange;

    private bool _gameStarted;
    private List<EnemyNew> _enemies;

    private void Start()
    {
        _gameStarted = true;
        _enemies = new List<EnemyNew>();
        GetComponent<CircleCollider2D>().radius = attackRange;
    }

    private void Update()
    {
        GetCurrentEnemyTarget();
        RotateTowardsTarget();
    }

    private void GetCurrentEnemyTarget()
    {
        if (_enemies.Count <= 0)
        {
            CurrentEnemyTarget = null;
            return;
        }

        CurrentEnemyTarget = _enemies[0];
    }

    private void RotateTowardsTarget()
    {
        if (CurrentEnemyTarget == null)
        {
            return;
        }

        Vector3 targetPos = CurrentEnemyTarget.transform.position - transform.position;
        float angle = Vector3.SignedAngle(transform.up, targetPos, transform.forward);
        transform.Rotate(0f, 0f, angle);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyNew newEnemy = other.GetComponent<EnemyNew>();
            if (newEnemy != null)
            {
                _enemies.Add(newEnemy);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyNew enemy = other.GetComponent<EnemyNew>();
            if (enemy != null && _enemies.Contains(enemy))
            {
                _enemies.Remove(enemy);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!_gameStarted)
        {
            GetComponent<CircleCollider2D>().radius = attackRange;
        }

        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
