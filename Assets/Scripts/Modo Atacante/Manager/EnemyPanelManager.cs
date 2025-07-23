using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPanelManager : MonoBehaviour
{
    [SerializeField] private Transform container;
    [SerializeField] private GameObject enemyCardPrefab;
    [SerializeField] private List<EnemyData> enemyDatabase;

    private List<EnemyCard> activeCards = new List<EnemyCard>();

    private void Start()
    {
        LoadEnemyCards();
    }

    private void LoadEnemyCards()
    {
        foreach (var enemyData in enemyDatabase)
        {
            GameObject cardObj = Instantiate(enemyCardPrefab, container);
            EnemyCard card = cardObj.GetComponent<EnemyCard>();
            card.SetupCard(enemyData);
            activeCards.Add(card);
        }
    }

    public List<EnemyInstance> GetSelectedEnemies()
    {
        List<EnemyInstance> selectedEnemies = new List<EnemyInstance>();

        foreach (var card in activeCards)
        {
            if (card.QuantitySelected > 0)
            {
                selectedEnemies.Add(new EnemyInstance
                {
                    Data = card.EnemyLoaded,
                    Quantity = card.QuantitySelected
                });
            }
        }

        return selectedEnemies;
    }
}

public class EnemyInstance
{
    public EnemyData Data;
    public int Quantity;
}
