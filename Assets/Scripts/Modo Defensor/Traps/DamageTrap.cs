using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DamageTrap : Trap
{
    public float damageAmount = 5f;

    protected override void ActivateTrap(Enemy enemy)
    {
        enemy.EnemyHealth.DealDamage(damageAmount);
    }
}
