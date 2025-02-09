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

    /// <summary>
    /// Current ambient temperature
    /// </summary>
    public float ambientTemperature;

    public bool inputReady = false;

    public override void Init()
    {
        ambientTemperature = gameConfig.initialAmbientTemperature;

        InitPlayerInput();
        StartCoroutine(WaitForGridManager());
        InitPilePool();
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
