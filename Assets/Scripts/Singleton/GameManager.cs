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
        Debug.Log(GridManager.Instance);
        GridManager.Instance.GenerateGrid(new Vector2Int(0, 0), 100);
    }

    private void InitPlayerInput()
    {
        playerInput = new PlayerInput();
        playerInput.Player.Enable(); // Enabled by default
    }
}
