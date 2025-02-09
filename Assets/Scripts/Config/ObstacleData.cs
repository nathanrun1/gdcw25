using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Config/ObstacleData", menuName = "ScriptableObjects/ObstacleData", order = 1)]
public class ObstacleData : ScriptableObject
{
    /// <summary>
    /// All obstacles that can exist in the game, indexed by id
    /// </summary>
    public Obstacle[] obstacleDict;
}
