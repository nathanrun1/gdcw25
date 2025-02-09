using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class CampfireCtrl : Building
{
    [SerializeField] private int _startFuel;
    [SerializeField] private int _maxFuel;
    [SerializeField] private TextMeshProUGUI _woodCount;
    [SerializeField] private GameObject _woodCountObj;
    [SerializeField] private Animator _campfireAnimator;

    [SerializeField] private float _maxLightIntensity = 1f;
    [SerializeField] private Light2D _campfireLight;


    [SerializeField] private float _timePerWood = 10f;
    [SerializeField] private float _campfireLevel1Temp = 5f;
    [SerializeField] private float _campfireLevel2Temp = 5f;
    [SerializeField] private float _campfireLevel3Temp = 5f;

    private int _curFuel;
    private bool _woodCountShowing = false;

    private float _timeSinceLastWoodExhausted = 0;

    private List<Vector2Int> level1Points = new List<Vector2Int>();
    private List<Vector2Int> level2Points = new List<Vector2Int>();
    private List<Vector2Int> level3Points = new List<Vector2Int>();

    private float level1TempDelta = 0f;
    private float level2TempDelta = 0f;
    private float level3TempDelta = 0f;

    public override float StartingTemperature => 20f;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        Debug.Log("campfire start");
        InitPoints();
        SetFuel(_startFuel);
    }

    private void InitPoints()
    {
        level1Points.AddRange(GridManager.GetCircleCoordinatesOptimized(pos, 3));
        level2Points.AddRange(GridManager.GetCircleCoordinatesOptimized(pos, 4));
        level3Points.AddRange(GridManager.GetCircleCoordinatesOptimized(pos, 5));
    }

    private void AdjustDeltas()
    {
        float curlevel1 = level1TempDelta;
        float curlevel2 = level2TempDelta;
        float curlevel3 = level3TempDelta;

        float newlevel1 = _campfireLevel1Temp * ((float)_curFuel / (float)_maxFuel);
        float newlevel2 = _campfireLevel2Temp * ((float)_curFuel / (float)_maxFuel);
        float newlevel3 = _campfireLevel3Temp * ((float)_curFuel / (float)_maxFuel);

        foreach (Vector2Int point in level1Points)
        {
            GridManager.Instance.GetGridSquareAt(point).temperatureDelta += (newlevel1 - curlevel1);
            Debug.Log(newlevel1);
            Debug.Log(curlevel1);
            Debug.Log("updated square at " + point + " to temp delta " + GridManager.Instance.GetGridSquareAt(point).temperatureDelta);
        }
        foreach (Vector2Int point in level2Points)
        {
            GridManager.Instance.GetGridSquareAt(point).temperatureDelta += (newlevel2 - curlevel2);
        }
        foreach (Vector2Int point in level3Points)
        {
            GridManager.Instance.GetGridSquareAt(point).temperatureDelta += (newlevel3 - curlevel3);
        }

        level1TempDelta = newlevel1;
        level2TempDelta = newlevel2;
        level3TempDelta = newlevel3;

    }

    private void OnMouseEnter()
    {
        _woodCountShowing = true;
        _woodCountObj.SetActive(true);
    }

    private void OnMouseExit()
    {
        _woodCountShowing = false;
        _woodCountObj.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Pickup")
        {
            PileCtrl pile = other.gameObject.GetComponent<PileCtrl>();
            if (pile == null) return;
            if (pile.rsc != ResourceType.Wood) return;
            if (_curFuel >= _maxFuel)
            {
                PlayerManager.Instance.Inventory_DeltaResource(ResourceType.Wood, 1, GridManager.Instance.GridToCenterOfGridWorldPos(pos));
            } else
            {
                AddFuel(1);
            }
            GameManager.Instance.pilePool.Release(pile);
        }
    }

    private void AddFuel(int amnt)
    {
        SetFuel(_curFuel + amnt);
    }

    private void SetFuel(int amnt)
    {
        _curFuel = amnt;
        _campfireAnimator.SetInteger("fuel", amnt);
        _campfireLight.intensity = Mathf.Min(1, ((float)_curFuel / (float)_maxFuel)) * _maxLightIntensity;
        AdjustDeltas();
       // _campfireLight.intensity = Mathf.Lerp(0, _maxLightIntensity, _curFuel / _maxFuel);
    }

    private void UpdateFuel(float deltaTime)
    {
        if (_curFuel > 0)
        {
            _timeSinceLastWoodExhausted += deltaTime;
            if (_timeSinceLastWoodExhausted >= _timePerWood)
            {
                _timeSinceLastWoodExhausted = 0;
                AddFuel(-1);
            }
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        UpdateFuel(Time.deltaTime);
        if (_woodCountShowing)
        {
            _woodCount.text = $"{_curFuel} / {_maxFuel}";
        }
    }

    public override void DestroyObstacle()
    {
        SetFuel(0);
        base.DestroyObstacle();
    }
}
