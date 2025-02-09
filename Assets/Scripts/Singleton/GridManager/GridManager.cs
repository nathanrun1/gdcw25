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

    public static List<Vector2Int> GetCircleCoordinatesOptimized(Vector2Int center, int radius)
    {
        List<Vector2Int> coordinates = new List<Vector2Int>();
        int h = center.x;
        int k = center.y;
        int rSquared = radius * radius;

        for (int x = h - radius; x <= h + radius; x++)
        {
            int dx = x - h;
            int dxSquared = dx * dx;

            if (dxSquared > rSquared) continue;

            int dyMax = Mathf.FloorToInt(Mathf.Sqrt(rSquared - dxSquared));
            for (int y = k - dyMax; y <= k + dyMax; y++)
            {
                coordinates.Add(new Vector2Int(x, y));
            }
        }

        return coordinates;
    }

    // Returns the perimeter coordinates of a circle on a grid
    public static List<Vector2Int> GetPerimeterCoordinates(Vector2Int center, int radius)
    {
        HashSet<Vector2Int> perimeterPoints = new HashSet<Vector2Int>();
        int x = 0;
        int y = radius;
        int d = 1 - radius; // Decision parameter

        // Midpoint Circle Algorithm
        while (x <= y)
        {
            // Add points for all 8 octants
            AddOctantPoints(center.x, center.y, x, y, perimeterPoints);

            if (d < 0)
            {
                d += 2 * x + 3; // Move east
            }
            else
            {
                d += 2 * (x - y) + 5; // Move southeast
                y--;
            }
            x++;
        }

        return new List<Vector2Int>(perimeterPoints);
    }

    // Add points for all 8 octants (symmetry)
    private static void AddOctantPoints(int h, int k, int x, int y, HashSet<Vector2Int> points)
    {
        AddPoint(h + x, k + y, points);
        AddPoint(h - x, k + y, points);
        AddPoint(h + x, k - y, points);
        AddPoint(h - x, k - y, points);
        AddPoint(h + y, k + x, points);
        AddPoint(h - y, k + x, points);
        AddPoint(h + y, k - x, points);
        AddPoint(h - y, k - x, points);
    }

    private static void AddPoint(int x, int y, HashSet<Vector2Int> points)
    {
        points.Add(new Vector2Int(x, y));
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
