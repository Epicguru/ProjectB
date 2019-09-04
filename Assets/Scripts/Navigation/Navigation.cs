using System.Collections;
using System.Collections.Generic;
using ThreadedPathfinding;
using UnityEngine;

[ExecuteInEditMode]
public class Navigation : MonoBehaviour, ITileProvider
{
    public static Navigation Instance { get; private set; }
    public static int TileSize { get { return Instance._tileSize; } }

    [SerializeField]
    [Range(1, 20)]
    private int _tileSize = 5;
    public LayerMask Mask;
    [HideInInspector]
    public bool[] Graph;
    public RectInt Bounds;
    public bool BuildNow = false;

    public static Vector2Int GetClosestTile(Vector2 worldPos)
    {
        int x = Mathf.RoundToInt(worldPos.x / TileSize);
        int y = Mathf.RoundToInt(worldPos.y / TileSize);

        return new Vector2Int(x, y);
    }

    public static Vector2 GetWorldPos(PNode point)
    {
        return (Vector2)point * TileSize;
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Application.isPlaying)
            return;


        if (BuildNow)
        {
            BuildNow = false;
            Build();
            Debug.Log($"Finished navigation build, {Bounds.width}x{Bounds.height}");
        }
    }

    public void Build()
    {
        if (Bounds.width <= 0 && Bounds.height <= 0)
            return;

        Graph = new bool[Bounds.width * Bounds.height];

        int i = 0, j = 0;

        for (int x = Bounds.xMin; x < Bounds.xMax; x++)
        {
            for (int y = Bounds.yMin; y < Bounds.yMax; y++)
            {
                Graph[i + j * Bounds.width] = !Physics2D.OverlapBox(new Vector2(x, y) * _tileSize, Vector2.one * _tileSize, 0f, Mask);
                j++;
            }
            i++;
            j = 0;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        RectInt b = new RectInt();
        b.min = Bounds.min * _tileSize;
        b.max = Bounds.max * _tileSize;
        Gizmos.DrawWireCube(b.center - Vector2.one * _tileSize * 0.5f, (Vector2)b.size);

        if(Graph != null && Graph.Length == Bounds.width * Bounds.height)
        {
            int i = 0, j = 0;

            for (int x = Bounds.xMin; x < Bounds.xMax; x++)
            {
                for (int y = Bounds.yMin; y < Bounds.yMax; y++)
                {
                    bool canMove = Graph[i + j * Bounds.width];
                    var c = canMove ? Color.green : Color.red;
                    c.a = 0.4f;
                    Gizmos.color = c;
                    Gizmos.DrawCube(new Vector2(x, y) * _tileSize, Vector2.one * _tileSize);
                    j++;
                }
                i++;
                j = 0;
            }
        }

    }

    public bool IsTileWalkable(int x, int y)
    {
        if (x < Bounds.xMin || x >= Bounds.xMax || y < Bounds.yMin || y >= Bounds.yMax)
            return false;
        int index = (x - Bounds.xMin) + (y - Bounds.yMin) * Bounds.width;
        return Graph[index];
    }
}
