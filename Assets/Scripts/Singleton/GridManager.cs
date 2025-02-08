using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Data for one grid square
/// </summary>
public class GridSquare
{
    /// <summary>
    /// Difference from ambient temperature
    /// </summary>
    public float temperatureDelta = 0;
    /// <summary>
    /// Temperature of this grid square
    /// </summary>
    public float Temperature { get => GameManager.Instance.ambientTemperature + temperatureDelta; }
}

/// <summary>
/// Singleton that manages grid system
/// </summary>
public class GridManager : MonoSingleton<GridManager>
{
    public GridConfig gridConfig;
    public Dictionary<Vector2Int, GridSquare> gridData = new Dictionary<Vector2Int, GridSquare>();

    [SerializeField] private SpriteRenderer _gridVisualizerRenderer;

    public override void Init()
    {
        Debug.Log(WorldToGridPos(15.4f, 27.1f));
        _gridVisualizerRenderer.material.SetFloat("_GridSize", gridConfig.gridSquareSize);
        _gridVisualizerRenderer.gameObject.SetActive(true);
    }

    /// <summary>
    /// Generates grid square data in a large square region around given center position, with half of square length
    /// equal to given distance
    /// </summary>
    public void GenerateGrid(Vector2Int center, int distance)
    {
        for (int x = center.x - distance; x < center.x + distance; x++)
        {
            for (int y = center.y - distance; y < center.y + distance; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                GridSquare data = new GridSquare();
                // temporary
                data.temperatureDelta = UnityEngine.Random.Range(-50f, 50f);
                // --
                if (!gridData.ContainsKey(pos)) 
                    gridData.Add(pos, data);
            }
        }
    }

    /// <summary>
    /// Returns grid square data at specified grid position
    /// </summary>
    public GridSquare GetGridSquareAt(Vector2Int pos)
    {
        return gridData[pos];
    }

    /// <summary>
    /// Returns grid square data at specified grid position
    /// </summary>
    public GridSquare GetGridSquareAt(int x, int y)
    {
        return GetGridSquareAt(new Vector2Int(x, y));
    }

    /// <summary>
    /// Returns which grid square a world position resides in
    /// </summary>
    public Vector2Int WorldToGridPos(float x, float y)
    {
        return WorldToGridPos(new Vector2(x, y));
    }

    /// <summary>
    /// Returns which grid square a world position resides in
    /// </summary>
    public Vector2Int WorldToGridPos(Vector2 worldPos)
    {
        return new Vector2Int(
            (int)Mathf.Floor(worldPos.x / gridConfig.gridSquareSize), 
            (int)Mathf.Floor(worldPos.y / gridConfig.gridSquareSize)
            );
    }
}
