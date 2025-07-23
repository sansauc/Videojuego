using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : Singleton<AchievementManager>
{
    public static Action<Achievement> OnAchievementUnlocked;
    public static Action<Achievement> OnProgressUpdated;


    [SerializeField] private AchievementCard achievementCardPrefab;
    [SerializeField] private Transform achievementPanelContainer;
    [SerializeField] private Achievement[] achievements;


    private void Start()
    {
        LoadAchievements();
    }

    private void LoadAchievements()
    {
        for (int i = 0; i < achievements.Length; i++)
        {
            AchievementCard card = Instantiate(achievementCardPrefab, achievementPanelContainer);
            card.SetupAchievement(achievements[i]);
        }
    }

    public void AddProgress(string achivementID, int amount)
    {
        Achievement achievementWanted = AchievementExists(achivementID);
        if (achievementWanted != null)
        {
            achievementWanted.AddProgress(amount);
        }
    }


    private Achievement AchievementExists(string achievementID)
    {
        for (int i = 0; i < achievements.Length; i++)
        {
            if (achievements[i].ID == achievementID)
            {
                return achievements[i];
            }
        }

        return null;
    }


    public void AddProgressByEnemyID(int enemyID)
    {
        foreach (var achievement in achievements)
        {
            if (achievement.EnemyID == enemyID && !achievement.IsUnlocked)
            {
                achievement.AddProgress(1);
            }
        }
    } //Función que se llame cuando un enemigo muere y que recorra todos los logros activos. Permite agregar progreso por ID de enemigo muerto

}
