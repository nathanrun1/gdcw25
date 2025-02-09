using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Singleton attached to player object, manages player data
/// </summary>
public partial class PlayerManager : MonoSingleton<PlayerManager>
{
    public PlayerConfig playerConfig;

    public Vector2 PlayerWorldPosition = new Vector2();
    public Vector2Int PlayerGridPosition = new Vector2Int();
    public Vector2 PlayerMouseDirection = new Vector2();

    public float PlayerGridSquareTemp = 0;

    [SerializeField] private TextMeshProUGUI _playerTemp;
    [SerializeField] private TextMeshProUGUI _gridTemp;


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

    private Color TempLerp(float val, float min, float max)
    {
        val = Mathf.Clamp(val, min, max);
        float l = (val - min) / (max - min);
        return l < 0.5 ? Color.Lerp(new Color(0, 0, 1), new Color(1, 1, 1), l * 2) : Color.Lerp(new Color(1, 1, 1), new Color(1, 0, 0), l * 2 - 0.5f);
    }

    private void FixedUpdate()
    {
        _playerTemp.text = (Mathf.Round(PlayerTemperature * 10)/10f).ToString() + "°";
        _playerTemp.color = TempLerp(PlayerTemperature, 30f, 45f);
        _gridTemp.text = $"Feels like {PlayerGridSquareTemp}°";
        _gridTemp.color = TempLerp(PlayerGridSquareTemp, -10, 30f);
    }

    private PlayerManager() { }
}
