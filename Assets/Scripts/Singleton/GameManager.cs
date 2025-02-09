using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class GameManager : MonoSingleton<GameManager>
{
    public GameConfig gameConfig;
    public PlayerInput playerInput;
    public ObjectPool<PileCtrl> pilePool = null;

    [SerializeField] private PileCtrl _pilePrefab;
    [SerializeField] private SpriteRenderer _temperatureVisualizer;

    [SerializeField] private float lowTemp = -30f;
    [SerializeField] private float highTemp = 30f;

    private float _sinceLastTempUpdate = 0f;

    /// <summary>
    /// Current ambient temperature
    /// </summary>
    public float ambientTemperature;

    public bool inputReady = false;

    public Texture2D temperatureTexture;

    public override void Init()
    {
        ambientTemperature = gameConfig.initialAmbientTemperature;

        InitPlayerInput();
        StartCoroutine(WaitForGridManager());
        InitPilePool();
        InitTemperatureTexture();
    }

    private void FixedUpdate()
    {
        //UpdateTemperatureTexture();
    }

    private void InitTemperatureTexture()
    {
        temperatureTexture = new Texture2D(100, 101, TextureFormat.RFloat, false);
        _temperatureVisualizer.material.SetTexture("_TemperatureTex", temperatureTexture);
    }

    private void UpdateTemperatureTexture()
    {
        if (_sinceLastTempUpdate > 1f)
        {
            _sinceLastTempUpdate = 0f;
            Vector2Int playerPos = PlayerManager.Instance.PlayerGridPosition;
            Vector2Int offset = new Vector2Int(playerPos.x - 50, playerPos.y - 50);
            Debug.Log($"picking offset {offset}");
            temperatureTexture.SetPixel(0, 100, new Color(0, 0, offset.x)); // offset x in texture
            temperatureTexture.SetPixel(1, 100, new Color(0, 0, offset.y));
            for (int x = 0; x < 100; ++x)
            {
                for (int y = 0; y < 100; ++y)
                {
                    Debug.Log("temp: " + GridManager.Instance.GetGridSquareAt(new Vector2Int(x + offset.x, y + offset.y)).Temperature);
                    float tempRatio = Mathf.Clamp(GridManager.Instance.GetGridSquareAt(new Vector2Int(x + offset.x, y + offset.y)).Temperature, lowTemp, highTemp);
                    tempRatio = (tempRatio - lowTemp) / (highTemp - lowTemp);
                    if (tempRatio != 0.5) Debug.Log("Temp ratio: " + tempRatio);
                    temperatureTexture.SetPixel(x, y, new Color(0, 0, tempRatio));
                }
            }
            temperatureTexture.Apply();
        }
        
        _sinceLastTempUpdate += Time.deltaTime;
    }

    private void InitPilePool()
    {
        pilePool = new ObjectPool<PileCtrl>(() =>
        {
            return Instantiate(_pilePrefab);
        },
            pile =>
            {
                pile.gameObject.SetActive(true);
            },
            pile =>
            {
                pile.gameObject.SetActive(false);
            },
            null, true, 10);
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
        inputReady = true;
    }
}
