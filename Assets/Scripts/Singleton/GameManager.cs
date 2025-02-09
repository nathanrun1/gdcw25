using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public GameConfig gameConfig;
    public PlayerInput playerInput;

    /// <summary>
    /// Current ambient temperature
    /// </summary>
    public float ambientTemperature;

    public override void Init()
    {
        ambientTemperature = gameConfig.initialAmbientTemperature;

        InitPlayerInput();
        StartCoroutine(WaitForGridManager());
    }

    private IEnumerator WaitForGridManager()
    {
        while (!GridManager.IsInitialized)
        {
            yield return null;
        }

        for (int i = 0; i < 40; ++i)
        {
            ObstacleManager.Instance.PlaceObstacleAt(new Vector2Int(UnityEngine.Random.Range(-20, 20), UnityEngine.Random.Range(-20, 20)), 0);
        }
    }

    private void InitPlayerInput()
    {
        playerInput = new PlayerInput();
        playerInput.Player.Enable(); // Enabled by default
    }
}
