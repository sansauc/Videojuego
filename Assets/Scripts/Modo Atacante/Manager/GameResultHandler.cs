using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameResultHandler : MonoBehaviour
{
    private void OnEnable()
    {
        NewSpawner.OnDemonVictory += HandleVictory;
        NewSpawner.OnDemonDefeat += HandleDefeat;
    }

    private void OnDisable()
    {
        NewSpawner.OnDemonVictory -= HandleVictory;
        NewSpawner.OnDemonDefeat -= HandleDefeat;
    }

    private void HandleVictory()
    {
        Debug.Log("🎊 ¡VICTORIA DEMONÍACA!");
        // UIManager.Instance?.ShowVictoryPanel();
        // Time.timeScale = 0;
    }

    private void HandleDefeat()
    {
        Debug.Log("💔 DERROTA DEMONÍACA...");
        // UIManager.Instance?.ShowDefeatPanel();
        // Time.timeScale = 0;
    }
}
