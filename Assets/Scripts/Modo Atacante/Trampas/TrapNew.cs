using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapNew : MonoBehaviour
{
    public float activationCooldown = 1f; // Tiempo entre activaciones
    protected bool isOnCooldown = false;

    [Header("Trap Configuration")]
    [SerializeField] public GameObject trapPrefab; // Movido desde TrapSetting
    [SerializeField] public int idTrup; // Movido desde TrapSetting
    [SerializeField] public float damageAmount = 5f;

    [Header("Lifetime Settings")]
    [Tooltip("Tiempo en segundos desde la primera activación hasta que la trampa se destruye automáticamente.")]
    public float lifetimeAfterFirstActivation = -1f; // -1 significa duración infinita, movido desde TrapSetting

    private bool _hasActivated = false; // para evitar múltiples llamadas a la destrucción

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (isOnCooldown) return;

        if (other.CompareTag("Enemy"))
        {
            EnemyNew enemy = other.GetComponent<EnemyNew>();
            if (enemy != null)
            {
                // Ignorar ciertas combinaciones de trampa y enemigo
                if (ShouldIgnoreEnemy(enemy)) return;

                DamageTrapNew.ActivateTrap(enemy, damageAmount);

                if (!_hasActivated && lifetimeAfterFirstActivation > 0f)
                {
                    _hasActivated = true;
                    StartCoroutine(DestroyAfterTime(lifetimeAfterFirstActivation));
                }

                StartCoroutine(StartCooldown());
            }
        }
    }

    private IEnumerator StartCooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(activationCooldown);
        isOnCooldown = false;
    }

    private IEnumerator DestroyAfterTime(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }

    protected virtual bool ShouldIgnoreEnemy(EnemyNew enemy)
    {
        // Ejemplo: Trampa ID 1 no afecta a Enemigo ID 1
        if (idTrup == 1 && enemy.IdEnemy == 3)
        {
            return true;
        }

        return false;
    }
}
