using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Singleton attached to player object, manages player data
/// </summary>
public partial class PlayerManager : MonoSingleton<PlayerManager>
{
    public PlayerConfig playerConfig;

    public Vector2 PlayerWorldPosition = new Vector2();
    public Vector2Int PlayerGridPosition = new Vector2Int();

    // temporary
    public float PlayerGridSquareTemp = 0;
    // --

    public override void Init()
    {
        InitTemperature();
    }

    private void Update()
    {
        PlayerWorldPosition = transform.position;
        PlayerGridPosition = GridManager.Instance.WorldToGridPos(PlayerWorldPosition);
        //Debug.Log($"Player world pos: {PlayerWorldPosition}\nPlayer grid pos: {PlayerGridPosition}");

        PlayerGridSquareTemp = GridManager.Instance.GetGridSquareAt(PlayerGridPosition).Temperature;
        UpdateTemperature(Time.deltaTime);
    }

    private PlayerManager() { }
}
