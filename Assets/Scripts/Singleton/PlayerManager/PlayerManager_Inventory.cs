using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using System.Security.Cryptography;
using static UnityEditor.PlayerSettings;

public partial class PlayerManager : MonoSingleton<PlayerManager>
{
    private Dictionary<ResourceType, int> _inventory = new Dictionary<ResourceType, int>();

    [Header("Inventory")]
    /// <summary>
    /// Current carry capacity of player (max total items)
    /// </summary>
    public int carryCapacity = 20;
    [SerializeField] private TextMeshProUGUI _woodCount;
    [SerializeField] private TextMeshProUGUI _metalCount;
    [SerializeField] private ResourceGain _rscGainUIPrefab;
    [SerializeField] private Canvas _mainCanvas;
    [SerializeField] private int _startingWoodAmnt = 0;
    [SerializeField] private int _startingMetalAmnt = 4;
    //[SerializeField] private TextMeshProUGUI _metalCount; (ADD)

    Dictionary<ResourceType, TextMeshProUGUI> _rscTypeToUICount = new Dictionary<ResourceType, TextMeshProUGUI>();

    private void Inventory_InitInventory()
    {
        Inventory_InitUI();
        foreach (var rsc in Enum.GetValues(typeof(ResourceType)).Cast<ResourceType>())
        {
            _inventory[rsc] = 0;
            Inventory_UpdateUICount(rsc);
        }

        _inventory[ResourceType.Wood] = _startingWoodAmnt;
        Inventory_UpdateUICount(ResourceType.Wood);

        _inventory[ResourceType.Metal] = _startingMetalAmnt;
        Inventory_UpdateUICount(ResourceType.Metal);

        StartCoroutine(WaitForPlayerInput());
    }

    private IEnumerator WaitForPlayerInput()
    {
        while (GameManager.Instance == null || !GameManager.Instance.inputReady) yield return null;
        GameManager.Instance.playerInput.Player.DropMetal.started += ctx => { Inventory_DropSingleResource(ResourceType.Metal); };
        GameManager.Instance.playerInput.Player.DropWood.started += ctx => { Inventory_DropSingleResource(ResourceType.Wood); };
    }

    private void Inventory_InitUI()
    {
        _rscTypeToUICount.Add(ResourceType.Wood, _woodCount);
        _rscTypeToUICount.Add(ResourceType.Metal, _metalCount);
    }

    private void Inventory_UpdateUICount(ResourceType rsc)
    {
        if (!_rscTypeToUICount.ContainsKey(rsc)) return;
        _rscTypeToUICount[rsc].text = _inventory[rsc].ToString();
    }

    public int Inventory_GetResourceCount(ResourceType rsc)
    {
        return _inventory[rsc];
    }

    public bool Inventory_CanAfford(int[] cost)
    {
        for (int rscId = 0; rscId < cost.Length; rscId++)
        {
            if (PlayerManager.Instance.Inventory_GetResourceCount((ResourceType)rscId) < cost[rscId]) return false;
        }
        return true;
    }

    public bool Inventory_CanAffordSingleResource(ResourceType rsc, int amnt)
    {
        return PlayerManager.Instance.Inventory_GetResourceCount(rsc) >= amnt;
    }

    /// <summary>
    /// Spends resources if affordable, returns whether or not it was affordable
    /// </summary>
    public bool Inventory_Spend(int[] cost)
    {
        if (!Inventory_CanAfford(cost)) return false;
        for (int rscId = 0; rscId < cost.Length; rscId++)
        {
            Inventory_DeltaResource((ResourceType)rscId, -cost[rscId]);
            Debug.Log($"Adding {-cost[rscId]}");
        }
        return true;
    }

    /// <summary>
    /// Subtracts cost from inventory even if unaffordable
    /// </summary>
    public void Inventory_ForceSpend(int[] cost)
    {
        for (int rscId = 0; rscId < cost.Length; rscId++)
        {
            Inventory_DeltaResource((ResourceType)rscId, -cost[rscId]);
        }
    }

    /// <summary>
    /// Changes resource count in inventory by amount
    /// </summary>
    public void Inventory_DeltaResource(ResourceType rsc, int amnt)
    {
        _inventory[rsc] += amnt;
        Inventory_UpdateUICount(rsc);
    }

    /// <summary>
    /// Changes resource count in inventory by amount
    /// </summary>
    public void Inventory_DeltaResource(ResourceType rsc, int amnt, Vector2 animPos)
    {
        Inventory_DeltaResourceAnim(animPos, rsc, amnt);
        Inventory_DeltaResource(rsc, amnt);
    }

    /// <summary>
    /// Plays resource delta display at given world position
    /// </summary>
    /// <param name="worldPos"></param>
    public void Inventory_DeltaResourceAnim(Vector2 worldPos, ResourceType rsc, int amnt)
    {
        Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        Vector2 screenPosRatio = new Vector2(screenPos.x / Camera.main.pixelWidth, screenPos.y / Camera.main.pixelHeight);
        Debug.Log("Screen pos: " + screenPos);
        Vector2 localPos = new Vector2(
            screenPosRatio.x * _mainCanvas.GetComponent<RectTransform>().rect.width - (_mainCanvas.GetComponent<RectTransform>().rect.width / 2), 
            screenPosRatio.y * _mainCanvas.GetComponent<RectTransform>().rect.height - (_mainCanvas.GetComponent<RectTransform>().rect.height) / 2);

        ResourceGain rscGainUI = Instantiate(_rscGainUIPrefab, _mainCanvas.transform);
        rscGainUI.transform.localScale = new Vector3(1, 1, 1);
        rscGainUI.transform.localPosition = localPos;
        Debug.Log(rscGainUI.transform.position);
        Debug.Log(rscGainUI.transform.localPosition);
        rscGainUI.GainResourceAnim(rsc, amnt);
    }

    /// <summary>
    /// Coroutine to play multiple delta resource anims 
    /// </summary>
    public void Inventory_DeltaResourceAnimMultiple(Vector2 worldPos, int[] rscs)
    {
        StartCoroutine(Inventory_DeltaResourceAnimMultiple_COR(worldPos, rscs));
    }

    private IEnumerator Inventory_DeltaResourceAnimMultiple_COR(Vector2 worldPos, int[] rscs)
    {
        for (int rscId = 0; rscId < rscs.Length; rscId++)
        {
            if (rscs[rscId] == 0) continue;
            Inventory_DeltaResourceAnim(worldPos, (ResourceType)rscId, rscs[rscId]);
            yield return new WaitForSeconds(0.15f);
        }
    }

    /// <summary>
    /// Coroutine to play multiple delta resource anims visualizing a cost spent
    /// </summary>
    public void Inventory_CostResourceAnim(Vector2 worldPos, int[] cost)
    {
        StartCoroutine(Inventory_CostResourceAnim_COR(worldPos, cost));
    }

    private IEnumerator Inventory_CostResourceAnim_COR(Vector2 worldPos, int[] cost)
    {
        for (int rscId = 0; rscId < cost.Length; rscId++)
        {
            Inventory_DeltaResourceAnim(worldPos, (ResourceType)rscId, -cost[rscId]);
            yield return new WaitForSeconds(0.15f);
        }
    }

    /// <summary>
    /// Returns total amount of currently carried items
    /// </summary>
    public int Inventory_GetTotalCarried()
    {
        int sum = 0;
        foreach (KeyValuePair<ResourceType, int> kv in _inventory)
        {
            sum += kv.Value;
        }
        return sum;
    }

    /// <summary>
    /// Changes resource count in inventory by amount and returns true if new count is valid, else returns false
    /// </summary>
    public bool Inventory_DeltaResourceCheck(ResourceType rsc, int amnt)
    {
        if (_inventory[rsc] + amnt < 0) return false; // Can't "afford" resource loss
        if (Inventory_GetTotalCarried() + amnt > carryCapacity) return false; // Not enough carry capacity
        Inventory_DeltaResource(rsc, amnt);
        return true;
    }

    /// <summary>
    /// Changes resource count in inventory by amount and returns true if new count is valid, else returns false
    /// </summary>
    public bool Inventory_DeltaResourceCheck(ResourceType rsc, int amnt, Vector2 animPos)
    {
        
        if (Inventory_DeltaResourceCheck(rsc, amnt))
        {
            Inventory_DeltaResourceAnim(animPos, rsc, amnt);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Attempts to drop a resource from inv, success if can afford, returns success value
    /// </summary>
    /// <param name="rsc"></param>
    /// <returns></returns>
    //NEXT CONNECT THIS TO Q AND Z
    public bool Inventory_DropSingleResource(ResourceType rsc)
    {
        if (!Inventory_CanAffordSingleResource(rsc, 1)) return false;

        PileCtrl drop = GameManager.Instance.pilePool.Get();
        drop.Setup(rsc, 1);
        Vector2 dropPos = PlayerWorldPosition + PlayerMouseDirection;
        float gridSize = GridManager.Instance.gridConfig.gridSquareSize;
        drop.transform.position = new Vector3(dropPos.x, dropPos.y, 0);
        drop.transform.eulerAngles = new Vector3(0, 0, UnityEngine.Random.Range(-35f, 35f));

        Inventory_DeltaResource(rsc, -1, dropPos);
        return true;
    }
}
