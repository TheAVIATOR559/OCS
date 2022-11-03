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

    private Grid_Node[,] Grid;
    private int clickCount = 0;

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

        //SANITYCHECKS();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.G))
        {
            switch(clickCount)
            {
                case 0:
                    GenGrid();
                    break;
                case 1:
                    SetUpNeighbors();
                    break;
                case 2:
                    RemoveNeighbors();
                    break;
                default:
                    RebalanceGrid();
                    break;
            }
            clickCount++;
        }
        else if(Input.GetKeyDown(KeyCode.H))
        {
            clickCount = 0;
        }
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
                        Grid[x, y].AddNeighbor(Grid[x, y - 1], -1);
                    }
                    if (y + 1 < VerticalSize)
                    {
                        Grid[x, y].AddNeighbor(Grid[x, y + 1], -1);
                    }
                }

                if(y == 0 || y == VerticalSize - 1)
                {
                    if (x + 1 < HorizontalSize)
                    {
                        Grid[x, y].AddNeighbor(Grid[x + 1, y], -1);
                    }
                    if (x - 1 >= 0)
                    {
                        Grid[x, y].AddNeighbor(Grid[x - 1, y], -1);
                    }
                }

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

                if (node.Neighbors.Count - connectionsToRemove.Count <= MinConnectionCount)
                {
                    continue;
                }

                if(neighbor.Value == -1)
                {
                    continue;
                }

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
        for(int x = 1; x < HorizontalSize - 1; x++)
        {
            for(int y = 1; y < VerticalSize - 1; y++)
            {
                Vector2 avgPosition = Vector2.zero;

                foreach(KeyValuePair<Grid_Node, float> node in Grid[x,y].Neighbors)
                {
                    avgPosition += (node.Key.Position * (1 - node.Value));
                }

                Grid[x, y].Position = (avgPosition / Grid[x,y].Neighbors.Count);
            }
        }
    }

    private void SANITYCHECKS()
    {
        //SANITY CHECK FOR NEIGHBORS HERE
        foreach (Grid_Node node in Grid)
        {
            //Debug.Log("~~~~~NODE: " + node.Position + "~~~~~");
            if (node.Neighbors.Count < MinConnectionCount)
            {
                Debug.LogWarning("NODE :: " + node.Position + " :: Neighbor Count :: " + node.Neighbors.Count);
            }

            //foreach (KeyValuePair<Grid_Node, float> kvp in node.Neighbors)
            //{
            //    Debug.Log("Neighbor: " + kvp.Key.Position + " :: Distance:" + Vector2.Distance(node.Position, kvp.Key.Position));
            //}
        }
    }

    private void OnDrawGizmosSelected()
    {
        if(!Application.isPlaying)
        {
            return;
        }

        foreach(Grid_Node node in Grid)
        {
            foreach (KeyValuePair<Grid_Node, float> neighbor in node.Neighbors)
            {
                if(neighbor.Value == -1)
                {
                    Gizmos.color = Color.magenta;
                }
                else
                {
                    Gizmos.color = new Color(1 - neighbor.Value, neighbor.Value, 1 - neighbor.Value);
                }

                DrawArrow(node.Position, neighbor.Key.Position, 0.9f, 25, 0.2f);
            }

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(node.Position, 0.1f);
        }
    }

    
    private void DrawArrow(Vector3 startPos, Vector3 endPos, float arrowDistance, float arrowheadAngle, float arrowheadLength)
    {
        Vector3 dir = endPos - startPos;

        Vector3 arrowPos = startPos + (dir * arrowDistance);

        Vector3 up = Quaternion.LookRotation(dir) * new Vector3(0f, Mathf.Sin(arrowheadAngle / 72), -1f) * arrowheadLength;
        Vector3 down = Quaternion.LookRotation(dir) * new Vector3(0f, -Mathf.Sin(arrowheadAngle / 72), -1f) * arrowheadLength;
        Vector3 left = Quaternion.LookRotation(dir) * new Vector3(Mathf.Sin(arrowheadAngle / 72), 0f, -1f) * arrowheadLength;
        Vector3 right = Quaternion.LookRotation(dir) * new Vector3(-Mathf.Sin(arrowheadAngle / 72), 0f, -1f) * arrowheadLength;

        Vector3 upPos = arrowPos + up;
        Vector3 downPos = arrowPos + down;
        Vector3 leftPos = arrowPos + left;
        Vector3 rightPos = arrowPos + right;

        Gizmos.DrawLine(startPos, endPos);

        Gizmos.DrawRay(arrowPos, up);
        Gizmos.DrawRay(arrowPos, down);
        Gizmos.DrawRay(arrowPos, left);
        Gizmos.DrawRay(arrowPos, right);

        //Gizmos.DrawLine(upPos, leftPos);
        //Gizmos.DrawLine(leftPos, downPos);
        //Gizmos.DrawLine(downPos, rightPos);
        //Gizmos.DrawLine(rightPos, upPos);
    }
}
