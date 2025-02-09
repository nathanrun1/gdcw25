using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

/// <summary>
/// Singleton attached to player object, manages player data
/// </summary>
public partial class PlayerManager : MonoSingleton<PlayerManager>
{
    public PlayerConfig playerConfig;

    public Vector2 PlayerWorldPosition = new Vector2();
    public Vector2Int PlayerGridPosition = new Vector2Int();
    public Vector2 PlayerMouseDirection = new Vector2();

    // temporary
    public float PlayerGridSquareTemp = 0;
    // --

    public override void Init()
    {
        InitTemperature();
        Inventory_InitInventory();
    }

    private void Update()
    {
        PlayerWorldPosition = transform.position;
        if (GridManager.IsInitialized)
        {
            PlayerGridPosition = GridManager.Instance.WorldToGridPos(PlayerWorldPosition);
            //Debug.Log($"Player world pos: {PlayerWorldPosition}\nPlayer grid pos: {PlayerGridPosition}");

            PlayerGridSquareTemp = GridManager.Instance.GetGridSquareAt(PlayerGridPosition).Temperature;
            UpdateTemperature(Time.deltaTime);
        }
        //Vector2 diff = Camera.main.ScreenToWorldPoint(mousePos) - new Vector2(transform.position.x, transform.position.y); // diff from player pos
        PlayerMouseDirection = (Mouse.current.position.ReadValue() - new Vector2(Screen.width / 2, Screen.height / 2)).normalized;
    }

    private PlayerManager() { }
}
