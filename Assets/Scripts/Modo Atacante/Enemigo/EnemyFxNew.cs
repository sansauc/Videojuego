using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyFxNew : MonoBehaviour
{
    [SerializeField] private Transform textDamageSpawnPosition;

    private EnemyNew _enemy;

    private void Start()
    {
        _enemy = GetComponent<EnemyNew>();
    }


    private void EnemyHit(EnemyNew enemy, float damage)
    {
        if (_enemy == enemy)
        {
            GameObject newInstance = DamageTextManager.Instance.Pooler.GetInstanceFromPool();
            TextMeshProUGUI damageText = newInstance.GetComponent<DamageText>().DmgText;
            damageText.text = damage.ToString();

            newInstance.transform.SetParent(textDamageSpawnPosition);
            newInstance.transform.position = textDamageSpawnPosition.position;
            newInstance.SetActive(true);
        }
    }



    private void OnEnable()
    {
        //Projectile.OnEnemyHit += EnemyHit; //Ver projectile.OnEnemyHit --> Tiene que ser EnemyNew
    }

    private void OnDisable()
    {
        //Projectile.OnEnemyHit -= EnemyHit;
    }
}
