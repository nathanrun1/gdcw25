using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public partial class PlayerManager : MonoSingleton<PlayerManager>
{
    public PlayerInv Inventory = new PlayerInv();

    public class PlayerInv
    {
        private Dictionary<ResourceType, uint> _data = new Dictionary<ResourceType, uint>();

        public Dictionary<ResourceType, uint> RawData { get => _data; }

        public void DeltaResource(ResourceType rsc, uint amnt)
        {
            _data[rsc] += amnt;
            // can add effects here
        }
    }
}
