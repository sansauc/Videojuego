using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
<<<<<<< HEAD

=======
>>>>>>> 4d87c4c4b9925b4c72bea6e19678438d7b7b0a48

public class EnemyPanelManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyShopPanel;
    [SerializeField] private Transform container;
    [SerializeField] private GameObject enemyCardPrefab;
    [SerializeField] private List<EnemyData> enemyDatabase;
    [SerializeField] private NewSpawner newSpawner;

    [SerializeField] private Button startWaveButton; // ← AÑADIR BOTÓN



    [SerializeField] private GameObject enemyShopPanel; //para luego poder cerrar el panel

    [SerializeField] private Button startWaveButton; // Para poder iniciar la oleada

    [SerializeField] private NewSpawner spawner;


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
<<<<<<< HEAD


=======
>>>>>>> 4d87c4c4b9925b4c72bea6e19678438d7b7b0a48
    public void OnStartWaveButtonClicked()
    {
        var selected = GetSelectedEnemies();

        if (selected.Count == 0)
        {
            Debug.LogWarning("⚠️ No se han seleccionado enemigos para iniciar la oleada.");
            return;
        }

<<<<<<< HEAD
        newSpawner.SetupSelectedEnemies(selected);
        newSpawner.StartWaves();

        startWaveButton.interactable = false;
        
        if (enemyShopPanel != null)
            enemyShopPanel.SetActive(false);
    }

=======
        spawner.StartWave(selected); // CORREGIDO: spawner, no newSpawner
        //startWaveButton.interactable = false;

        if (enemyShopPanel != null)
            enemyShopPanel.SetActive(false);
    }
>>>>>>> 4d87c4c4b9925b4c72bea6e19678438d7b7b0a48
}

public class EnemyInstance
{
    public EnemyData Data;
    public int Quantity;
}
