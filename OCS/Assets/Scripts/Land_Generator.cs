using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using csDelaunay;
using System.Linq;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class Land_Generator : MonoBehaviour
{
    [SerializeField] private int Seed;
    [SerializeField] private int HorizontalSize = 100, VerticalSize = 100;

    [SerializeField] private float DiagonalFlipChance = 0.5f;
    [SerializeField] private float MinNodeDistance = 0.35f;

    private Grid_Node[,] Grid;
    private int clickCount = 0;

    [SerializeField] private int polygonCount = 200;

    [SerializeField] private MeshRenderer meshRend;
    [SerializeField] private MeshFilter meshFilter;

    private void Awake()
    {
        meshRend = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
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
        DetermineLandPolygons();
        DrawLand();
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

    List<Site> closestCenterSites = new List<Site>();
    [SerializeField] private float landPercentage = 0.75f;
    private void DetermineLandPolygons()
    {
        /*
         * find the polygon closest to the center of the generated area
         * create a circular-ish area outwards to fill a percentage of the total generated area
         */

        Vector2f pureCenter = new Vector2f(512 / 2f, 512 / 2f);
        Vector2f closestCenter = Vector2f.zero;
        float prevDistance = Vector2f.DistanceSquare(pureCenter, closestCenter);

        foreach(KeyValuePair<Vector2f, Site> kvp in sites)
        {
            if(Vector2f.DistanceSquare(pureCenter, kvp.Key) < prevDistance)
            {
                prevDistance = Vector2f.DistanceSquare(pureCenter, kvp.Key);
                closestCenter = kvp.Key;
            }
        }

        closestCenterSites.Add(sites[closestCenter]);
        Debug.Log("CLOSEST CENTER: " + closestCenter);
       
        //float landRadius = (512 * landPercentage) / 2f;

        //foreach (KeyValuePair<Vector2f, Site> kvp in sites)
        //{
        //    Debug.Log("LAND RADIUS " + landRadius + " :: " + closestCenter + " :: " + kvp.Key + " :: " + Vector2f.DistanceSquare(closestCenter, kvp.Key));
        //    if (Vector2f.DistanceSquare(closestCenter, kvp.Key) <= landRadius)
        //    {
        //        closestCenterSites.Add(kvp.Value);
        //    }
        //}
    }

    private void DrawLand()
    {
        Mesh mesh = new Mesh();

        //Vector3[] vertices = new Vector3[4]
        //{
        //    new Vector3(0, 0, 0),
        //    new Vector3(512, 0, 0),
        //    new Vector3(0, 512, 0),
        //    new Vector3(512, 512, 0)
        //};

        List<Vector3> vertices = new List<Vector3>();
        vertices.Add(Vector2fToVector3(closestCenterSites[0].Coord));

        /*
         * add all vertices to list
         * cull duplicates
         * sort based on clockwise rotation
         */
        for(int i = 0; i < closestCenterSites[0].Edges.Count; i++)
        {
            vertices.Add(Vector2fToVector3(closestCenterSites[0].Edges[i].ClippedEnds[LR.LEFT]));//this is way busted
            
        }

        mesh.vertices = vertices.ToArray();

        //int[] tris = new int[6]
        //{
        //    // lower left triangle
        //    0, 2, 1,
        //    // upper right triangle
        //    2, 3, 1
        //};

        List<int> tris = new List<int>();

        for (int i = 1; i < vertices.Count; i++)
        {
            tris.Add(0);//center
            tris.Add(i);//left
            //tris.Add(i+2);//right
            if (i + 1 >= vertices.Count)
            {
                tris.Add(1);
                Debug.Log("0," + i + ",1");
            }
            else
            {
                tris.Add(i + 1);//right
                Debug.Log("0," + i + "," + (i + 1));
            }
            Debug.Log(vertices[i]);
        }
        //tris.Add(0);
        //tris.Add(1);
        //tris.Add(2);
        mesh.triangles = tris.ToArray();

        //Vector3[] normals = new Vector3[4]
        //{
        //    -Vector3.forward,
        //    -Vector3.forward,
        //    -Vector3.forward,
        //    -Vector3.forward
        //};
        List<Vector3> normals = new List<Vector3>();
        for(int i = 0; i < vertices.Count; i++)
        {
            normals.Add(Vector3.back);
        }
        mesh.normals = normals.ToArray();


        //Vector2[] uv = new Vector2[4]
        //{
        //    new Vector2(0, 0),
        //    new Vector2(1, 0),
        //    new Vector2(0, 1),
        //    new Vector2(1, 1)
        //};
        List<Vector2> uv = new List<Vector2>();
        foreach(Vector3 vertex in vertices)
        {
            uv.Add(vertex.normalized);
        }

        mesh.uv = uv.ToArray();

        meshFilter.mesh = mesh;

        //Debug.Log("VERTICES :: " + vertices.Count);
        //foreach (Vector3 vertex in vertices)
        //{
        //    Debug.Log(vertex);
        //}
        //Debug.Log("TRIANGLES :: " + tris.Count);
        //foreach (int tri in tris)
        //{
        //    Debug.Log(tri);
        //}
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

        Gizmos.color = Color.yellow;
        foreach(Site site in closestCenterSites)
        {
            Gizmos.DrawSphere(new Vector3(site.x, site.y, 0), 2f);
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
