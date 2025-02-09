using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;

public partial class PlayerManager : MonoSingleton<PlayerManager>
{
    private Dictionary<ResourceType, int> _inventory = new Dictionary<ResourceType, int>();

    [Header("Inventory")]
    /// <summary>
    /// Current carry capacity of player (max total items)
    /// </summary>
    public int carryCapacity = 20;
    [SerializeField] private TextMeshProUGUI _woodCount;
    [SerializeField] private ResourceGain _rscGainUI;
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
    }

    private void Inventory_InitUI()
    {
        _rscTypeToUICount.Add(ResourceType.Wood, _woodCount);
        //_rscTypeToUICount.Add(ResourceType.Metal, _metalCount); (ADD)
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

    /// <summary>
    /// Changes resource count in inventory by amount
    /// </summary>
    public void Inventory_DeltaResource(ResourceType rsc, int amnt)
    {
        _inventory[rsc] += amnt;
        _rscGainUI.GainResourceAnim(rsc, amnt);
        Inventory_UpdateUICount(rsc);
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
}
