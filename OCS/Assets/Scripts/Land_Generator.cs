using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Land_Generator : MonoBehaviour
{
    [SerializeField] private int Seed;
    [SerializeField] private int HorizontalSize = 100, VerticalSize = 100;

    [SerializeField] private float DiagonalFlipChance = 0.5f;

    /* Connection Weights
     * Range: 0f-1f
     * Representation: rigidity of the connection, 0 - fully movable, 1 - immovable
     */
    [SerializeField] private float VerticalConnectionWeightMax = 1.0f;
    [SerializeField] private float VerticalConnectionWeightMin = 0f;
    [SerializeField] private float HorizontalConnectionWeightMax = 1.0f;
    [SerializeField] private float HorizontalConnectionWeightMin = 0f;
    [SerializeField] private float DiagonalConnectionWeightMax = 1.0f;
    [SerializeField] private float DiagonalConnectionWeightMin = 0f;

    [SerializeField] private float NodeMovabilityChance = 0.5f;

    private Grid_Node[,] Grid;

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        GenGrid();
        SetUpNeighbors();
        RemoveNeighbors();
        //RebalanceGrid();
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
                    Grid[x, y] = new Grid_Node(x, y, false);
                }
                else
                {
                    Grid[x, y] = new Grid_Node(x, y, (Random.Range(0f, 1f) >= NodeMovabilityChance));
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
                if (x + 1 < HorizontalSize)
                {
                    Grid[x, y].AddNeighbor(Grid[x + 1, y], Random.Range(HorizontalConnectionWeightMin, HorizontalConnectionWeightMax));
                }

                if (x - 1 >= 0)
                {
                    Grid[x, y].AddNeighbor(Grid[x - 1, y], Random.Range(HorizontalConnectionWeightMin, HorizontalConnectionWeightMax));
                }

                if (y + 1 < VerticalSize)
                {
                    Grid[x, y].AddNeighbor(Grid[x, y + 1], Random.Range(VerticalConnectionWeightMin, VerticalConnectionWeightMax));
                }

                if (y - 1 >= 0)
                {
                    Grid[x, y].AddNeighbor(Grid[x, y - 1], Random.Range(VerticalConnectionWeightMin, VerticalConnectionWeightMax));
                }

                if (Random.Range(0f, 1f) >= DiagonalFlipChance)
                {
                    diagonalsToFlip.Add(new Vector2Int(x, y));
                }
                else
                {
                    if (x + 1 < HorizontalSize && y + 1 < VerticalSize)
                    {
                        Grid[x, y].AddNeighborMutual(Grid[x + 1, y + 1], Random.Range(DiagonalConnectionWeightMin, DiagonalConnectionWeightMax), Random.Range(DiagonalConnectionWeightMin, DiagonalConnectionWeightMax));
                    }
                }
            }
        }

        foreach (Vector2Int point in diagonalsToFlip)
        {
            if (point.x + 1 < HorizontalSize && point.y + 1 < VerticalSize)
            {
                Grid[point.x + 1, point.y].AddNeighborMutual(Grid[point.x, point.y + 1], Random.Range(DiagonalConnectionWeightMin, DiagonalConnectionWeightMax), Random.Range(DiagonalConnectionWeightMin, DiagonalConnectionWeightMax));
            }
        }

        //SANITY CHECK FOR NEIGHBORS HERE
        foreach (Grid_Node node in Grid)
        {
            Debug.Log("~~~~~NODE: " + node.Position + "~~~~~");

            foreach (KeyValuePair<Grid_Node, float> kvp in node.Neighbors)
            {
                Debug.Log("Neighbor: " + kvp.Key.Position + " :: Distance:" + Vector2.Distance(node.Position, kvp.Key.Position));
            }
        }
    }

    [SerializeField] private int MinConnectionCount = 3;
    [SerializeField] private float HorizontalConnectionRemovalChance = 0.5f;
    [SerializeField] private float VerticalConnectionRemovalChance = 0.5f;
    [SerializeField] private float DiagonalConnectionRemovalChance = 0.5f;

    private void RemoveNeighbors()
    {
        foreach(Grid_Node node in Grid)
        {
            //Debug.Log("~~~~~NODE: " + node.Position + "~~~~~");

            if (node.Neighbors.Count <= MinConnectionCount)
            {
                continue;
            }

            List<Grid_Node> connectionsToRemove = new List<Grid_Node>();

            foreach(KeyValuePair<Grid_Node, float> neighbor in node.Neighbors)
            {
                //if distance is > 1, use diagonal removal chance
                //else if neighbor.position.x !=  node.x, use horizontal removal chance
                //else if neighbor.position.y != node.y, use vertical removal chance

                //if (connectionsToRemove.Count > MinConnectionCount)
                //{
                //    continue;
                //}

                if (Vector2.Distance(node.Position, neighbor.Key.Position) > 1f)
                {
                    //Debug.Log("Neighbor: " + neighbor.Key.Position + " :: Diagonal");

                    if (Random.Range(0, 1f) > DiagonalConnectionRemovalChance)
                    {
                        //Debug.Log("Removing Neighbor: " + neighbor.Key.Position);
                        connectionsToRemove.Add(neighbor.Key);
                    }
                }
                else if(node.Position.x != neighbor.Key.Position.x)
                {
                    //Debug.Log("Neighbor: " + neighbor.Key.Position + " :: Horizontal");

                    if (Random.Range(0, 1f) > HorizontalConnectionRemovalChance)
                    {
                        //Debug.Log("Removing Neighbor: " + neighbor.Key.Position);
                        connectionsToRemove.Add(neighbor.Key);
                    }
                }
                else if(node.Position.y != neighbor.Key.Position.y)
                {
                    //Debug.Log("Neighbor: " + neighbor.Key.Position + " :: Vertical");

                    if (Random.Range(0, 1f) > VerticalConnectionRemovalChance)
                    {
                        //Debug.Log("Removing Neighbor: " + neighbor.Key.Position);
                        connectionsToRemove.Add(neighbor.Key);
                    }   
                }
                else
                {
                    Debug.LogWarning("SKIPPING NEIGHBOR: " + neighbor.Key.Position);
                }
            }

            foreach(Grid_Node conn in connectionsToRemove)
            {
                node.RemoveNeighbor(conn);
            }
        }
    }

    private void RebalanceGrid()
    {
        //TODO fill in
    }

    private void OnDrawGizmosSelected()
    {
        foreach(Grid_Node node in Grid)
        {
            foreach (KeyValuePair<Grid_Node, float> neighbor in node.Neighbors)
            {
                Gizmos.color = Color.green;
                DrawArrow(node.Position, neighbor.Key.Position);
            }

            if (node.IsMovable)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(node.Position, 0.01f);
            }
            else
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(node.Position, 0.01f);
            }
        }
    }

    
    private void DrawArrow(Vector3 startPos, Vector3 endPos)
    {
        Vector3 dir = endPos - startPos;

        Vector3 arrowPos = startPos * (dir + arrowDistance);

        Vector3 up = Quaternion.LookRotation(dir) * new Vector3(0f, (Mathf.Sin(arrowHeadAngle / 72)), -1f) * arrowHeadLength;
        Vector3 up = Quaternion.LookRotation(dir) * new Vector3(0f, -(Mathf.Sin(arrowHeadAngle / 72)), -1f) * arrowHeadLength;

        Gizmos.DrawLine(a, b);

        Gizmos.DrawRay(arrowPos, up);
        Gizmos.DrawRay(arrowPos, down);
    }
}
