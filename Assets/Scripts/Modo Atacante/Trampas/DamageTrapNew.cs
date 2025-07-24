using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTrapNew : MonoBehaviour
{
    public static void ActivateTrap(EnemyNew enemy, float damageAmount)
    {
        if (enemy != null && enemy.EnemyHealth != null)
        {
            enemy.EnemyHealth.DealDamage(damageAmount);
        }
    }
}
