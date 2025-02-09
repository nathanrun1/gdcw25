using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ObstacleManager : MonoSingleton<ObstacleManager>
{
    public ObstacleData obstacleData;

    /// <summary>
    /// Returns whether or not grid position is occupied by an obstacle
    /// Sets 'obstacle' to obstacle occupying grid square if any
    /// </summary>
    public bool IsOccupied(Vector2Int pos, out Obstacle obstacle)
    {
        obstacle = GridManager.Instance.GetGridSquareAt(pos).obstacle;
        return obstacle != null;
    }

    /// <summary>
    /// Returns whether or not grid position is occupied by an obstacle
    /// </summary>
    public bool IsOccupied(Vector2Int pos)
    {
        return GridManager.Instance.GetGridSquareAt(pos).obstacle != null;
    }

    /// <summary>
    /// Removes the obstacle (if any) at given position
    /// </summary>
    /// <param name="pos"></param>
    public void RemoveObstacleAt(Vector2Int pos)
    {
        if (IsOccupied(pos, out Obstacle obstacle))
        {
            GridManager.Instance.GetGridSquareAt(pos).obstacle = null;
            obstacle.DestroyObstacle();
        }
    }

    /// <summary>
    /// Places obstacle at position (if any) with new obstacle of given id, if not already occupied
    /// </summary>
    public void PlaceObstacleAt(Vector2Int pos, int obstacleId)
    {
        if (obstacleId < 0 || obstacleId >= obstacleData.obstacleDict.Count())
        {
            throw new System.Exception("Attempt to place obstacle with invalid obstacle id");
        }

        if (IsOccupied(pos)) return; // Already occupied

        Obstacle newObstacle = (Obstacle)PrefabUtility.InstantiatePrefab(obstacleData.obstacleDict[obstacleId]);
        //Obstacle newObstacle = Instantiate(obstacleData.obstacleDict[obstacleId]);
        newObstacle.id = obstacleId;
        newObstacle.pos = pos;

        GridManager.Instance.GetGridSquareAt(pos).obstacle = newObstacle;
        newObstacle.transform.position = GridManager.Instance.GridToCenterOfGridWorldPos(pos);
    }

    /// <summary>
    /// Removes any obstacle at position, and places obstacle of given id at that position
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="obstacleId"></param>
    public void ReplaceObstacleAt(Vector2Int pos, int obstacleId)
    {
        if (obstacleId < 0 || obstacleId >= obstacleData.obstacleDict.Count())
        {
            throw new System.Exception("Attempt to replace with obstacle of invalid obstacle id");
        }
        RemoveObstacleAt(pos);
        PlaceObstacleAt(pos, obstacleId);
    }
}
