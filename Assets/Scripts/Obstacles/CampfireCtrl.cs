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

    private int _curFuel;
    private bool _woodCountShowing = false;

    private float _timeSinceLastWoodExhausted = 0;

    protected override void Awake()
    {
        base.Awake();
        SetFuel(_startFuel);
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
}
