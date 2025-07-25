using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManagerNew : Singleton<LevelManagerNew>
{
    [SerializeField] private int startingLives = 10;

    public int CurrentLives { get; private set; }

    private void Start()
    {
        CurrentLives = startingLives;
        //UIManager.Instance?.UpdateLivesUI(CurrentLives);
    }

    private void ReduceLife()
    {
        CurrentLives--;
        Debug.Log($"❤️ Vida perdida. Vidas restantes: {CurrentLives}");

        if (CurrentLives <= 0)
        {
            CurrentLives = 0;
        }
        //UIManager.Instance?.UpdateLivesUI(CurrentLives);
    }

    private void OnEnable()
    {
        EnemyNew.OnEndReached += OnEnemyReachedEnd;
    }

    private void OnDisable()
    {
        EnemyNew.OnEndReached -= OnEnemyReachedEnd;
    }

    private void OnEnemyReachedEnd(EnemyNew enemy)
    {
        ReduceLife();
    }
}
