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
    /// <summary>
    /// Obstacle placed on this square
    /// </summary>
    public Obstacle obstacle = null;
}

/// <summary>
/// Singleton that manages grid system
/// </summary>
public class GridManager : MonoSingleton<GridManager>
{
    public GridConfig gridConfig;
    public Dictionary<Vector2Int, GridSquare> gridData = new Dictionary<Vector2Int, GridSquare>();

    [SerializeField] private SpriteRenderer _gridVisualizerRenderer;
    [SerializeField] private bool _visualizeGrid = false;

    public static bool IsInitialized { get; private set; } = false;

    public override void Init()
    {
        Debug.Log(WorldToGridPos(new Vector2(15.4f, 27.1f)));
        GridManager.Instance.GenerateGrid(new Vector2Int(0, 0), 100);
        IsInitialized = true;
        if (_visualizeGrid) VisualizeGrid();
    }

    private void VisualizeGrid()
    {
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
                data.temperatureDelta = 0;
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
    /// Returns which grid square a world position resides in
    /// </summary>
    public Vector2Int WorldToGridPos(Vector2 worldPos)
    {
        return new Vector2Int(
            (int)Mathf.Floor(worldPos.x / gridConfig.gridSquareSize), 
            (int)Mathf.Floor(worldPos.y / gridConfig.gridSquareSize)
            );
    }

    /// <summary>
    /// Returns world position of bottom left corner of grid square given by grid position
    /// </summary>
    public Vector2 GridToWorldPos(Vector2Int gridPos)
    {
        return new Vector2(
            gridPos.x * gridConfig.gridSquareSize,
            gridPos.y * gridConfig.gridSquareSize
            );
    }

    /// <summary>
    /// Returns world position of center of grid square given by grid position
    /// </summary>
    public Vector2 GridToCenterOfGridWorldPos(Vector2Int gridPos)
    {
        return GridToWorldPos(gridPos) + Vector2.one * (gridConfig.gridSquareSize / 2);
    }
}
