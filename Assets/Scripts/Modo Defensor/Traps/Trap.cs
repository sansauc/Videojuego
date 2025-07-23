using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Trap : MonoBehaviour
{
    public float activationCooldown = 1f; // Tiempo entre activaciones
    protected bool isOnCooldown = false;

    [Header("Configuración de trampa")]
    public TrapSetting trapSetting;

    private bool _hasActivated = false; // <-- para evitar múltiples llamadas a la destrucción


    /**protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (isOnCooldown) return;

        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                ActivateTrap(enemy);
                StartCoroutine(StartCooldown());
            }
        }
    } Metodo considerando que las trampas no son de un solo uso **/

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (isOnCooldown) return;

        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                // Ignorar ciertas combinaciones de trampa y enemigo
                if (ShouldIgnoreEnemy(enemy)) return;

                ActivateTrap(enemy);

                if (!_hasActivated && trapSetting != null && trapSetting.lifetimeAfterFirstActivation > 0f)
                {
                    _hasActivated = true;
                    StartCoroutine(DestroyAfterTime(trapSetting.lifetimeAfterFirstActivation));
                }

                StartCoroutine(StartCooldown());
            }
        }
    }


    protected abstract void ActivateTrap(Enemy enemy);

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

    protected virtual bool ShouldIgnoreEnemy(Enemy enemy)
    {
        // Ejemplo: Trampa ID 1 no afecta a Enemigo ID 1
        if (trapSetting.idTrup == 1 && enemy.IdEnemy == 3)
        {
            return true;
        }

        return false;
    }

}
