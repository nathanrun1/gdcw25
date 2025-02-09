using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Config/BuildingList", menuName = "ScriptableObjects/BuildingList", order = 1)]
public class BuildingList : ScriptableObject
{
    public BuildingData[] builds;
}
