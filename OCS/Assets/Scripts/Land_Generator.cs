using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using csDelaunay;

public class Land_Generator : MonoBehaviour
{
    [SerializeField] private int Seed;
    [SerializeField] private int HorizontalSize = 100, VerticalSize = 100;

    [SerializeField] private float DiagonalFlipChance = 0.5f;
    [SerializeField] private float MinNodeDistance = 0.35f;

    private Grid_Node[,] Grid;
    private int clickCount = 0;

    [SerializeField] private int polygonCount = 200;

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        //GenGrid();
        //SetUpNeighbors();
        //RemoveNeighbors();
        //RebalanceGrid();

        //SANITYCHECKS();

        GenVoronoi();
    }

    private void Update()
    {
        //if(Input.GetKeyDown(KeyCode.G))
        //{
        //    Debug.Log("CLICK :: " + clickCount);
        //    switch(clickCount)
        //    {
        //        case 0:
        //            GenGrid();
        //            break;
        //        case 1:
        //            SetUpNeighbors();
        //            break;
        //        default:
        //            RebalanceGrid();
        //            break;
        //    }
        //    clickCount++;
        //}
        //else if(Input.GetKeyDown(KeyCode.L))
        //{
        //    clickCount = 0;
        //}
    }

    private Dictionary<Vector2f, Site> sites;
    private List<Edge> edges;
    [SerializeField] private int LloydIterations = 5;
    private void GenVoronoi()
    {
        List<Vector2f> points = CreateRandomPoints();

        Rectf bounds = new Rectf(0, 0, 512, 512);

        Voronoi voronoi = new Voronoi(points, bounds, LloydIterations);

        sites = voronoi.SitesIndexedByLocation;
        edges = voronoi.Edges;
    }

    private List<Vector2f> CreateRandomPoints()
    {
        List<Vector2f> points = new List<Vector2f>();
        for (int i = 0; i < polygonCount; i++)
        {
            points.Add(new Vector2f(Random.Range(0, 512), Random.Range(0, 512)));
        }

        return points;
    }

    private void GenGrid()
    {
        Grid = new Grid_Node[HorizontalSize, VerticalSize];

        for (int x = 0; x < HorizontalSize; x++)
        {
            for(int y = 0; y < VerticalSize; y++)
            {
                if(x == 0 || y == 0 || x == HorizontalSize-1 || y == VerticalSize-1)
                {
                    Grid[x, y] = new Grid_Node(x, y);
                }
                else
                {
                    Grid[x, y] = new Grid_Node(x, y);
                }
            }
        }
    }

    private void SetUpNeighbors()
    {
        List<Vector2Int> diagonalsToFlip = new List<Vector2Int>();

        for (int x = 0; x < HorizontalSize; x++)
        {
            for (int y = 0; y < VerticalSize; y++)
            {
                if(x == 0 || x == HorizontalSize - 1)
                {
                    if(y - 1 >= 0)
                    {
                        Grid[x, y].AddNeighbor(Grid[x, y - 1]);
                    }
                    if (y + 1 < VerticalSize)
                    {
                        Grid[x, y].AddNeighbor(Grid[x, y + 1]);
                    }
                }

                if(y == 0 || y == VerticalSize - 1)
                {
                    if (x + 1 < HorizontalSize)
                    {
                        Grid[x, y].AddNeighbor(Grid[x + 1, y]);
                    }
                    if (x - 1 >= 0)
                    {
                        Grid[x, y].AddNeighbor(Grid[x - 1, y]);
                    }
                }

                if (x + 1 < HorizontalSize)
                {
                    Grid[x, y].AddNeighbor(Grid[x + 1, y]);
                }

                if (x - 1 >= 0)
                {
                    Grid[x, y].AddNeighbor(Grid[x - 1, y]);
                }

                if (y + 1 < VerticalSize)
                {
                    Grid[x, y].AddNeighbor(Grid[x, y + 1]);
                }

                if (y - 1 >= 0)
                {
                    Grid[x, y].AddNeighbor(Grid[x, y - 1]);
                }

                if (Random.Range(0f, 1f) >= DiagonalFlipChance)
                {
                    diagonalsToFlip.Add(new Vector2Int(x, y));
                }
                else
                {
                    if (x + 1 < HorizontalSize && y + 1 < VerticalSize)
                    {
                        Grid[x, y].AddNeighbor(Grid[x + 1, y + 1], true);
                    }
                }
            }
        }

        foreach (Vector2Int point in diagonalsToFlip)
        {
            if (point.x + 1 < HorizontalSize && point.y + 1 < VerticalSize)
            {
                Grid[point.x + 1, point.y].AddNeighbor(Grid[point.x, point.y + 1], true);
            }
        }
    }

    private void RebalanceGrid()
    {
        for(int x = 1; x < HorizontalSize - 1; x++)
        {
            for(int y = 1; y < VerticalSize - 1; y++)
            {
                Vector2 avgPosition = Vector2.zero;

                foreach(Grid_Node node in Grid[x,y].Neighbors)
                {
                    avgPosition += node.Position;
                }

                if(Vector2.Distance(Grid[x,y].Position, avgPosition) >= MinNodeDistance)
                {
                    Grid[x, y].Position = (avgPosition / Grid[x, y].Neighbors.Count);
                }
            }
        }
    }

    private void SANITYCHECKS()
    {
        for (int x = 1; x < HorizontalSize - 1; x++)
        {
            for (int y = 1; y < VerticalSize - 1; y++)
            {
                Debug.Log("~~~~~NODE: " + Grid[x,y].Position + "~~~~~");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if(!Application.isPlaying)
        {
            return;
        }

        foreach(KeyValuePair<Vector2f, Site> kvp in sites)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(new Vector3(kvp.Key.x, kvp.Key.y, 0), 1f);
        }

        foreach(Edge edge in edges)
        {
            Gizmos.color = Color.green;
            if(edge.ClippedEnds != null)
            {
                Gizmos.DrawLine(Vector2fToVector3(edge.ClippedEnds[LR.RIGHT]), Vector2fToVector3(edge.ClippedEnds[LR.LEFT]));
            }
        }

        //foreach(Grid_Node node in Grid)
        //{
        //    foreach (Grid_Node neighbor in node.Neighbors)
        //    {
        //        Gizmos.color = Color.magenta;
        //        Gizmos.DrawLine(node.Position, neighbor.Position);
        //    }

        //    Gizmos.color = Color.red;
        //    Gizmos.DrawSphere(node.Position, 0.1f);
        //}
    }

    private static Vector3 Vector2fToVector3(Vector2f vec)
    {
        return new Vector3(vec.x, vec.y, 0);
    }
}
