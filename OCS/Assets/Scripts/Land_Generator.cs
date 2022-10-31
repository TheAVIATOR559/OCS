using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Land_Generator : MonoBehaviour
{
    [SerializeField] private int Seed;
    [SerializeField] private int HorizontalSize = 100, VerticalSize = 100;

    [SerializeField] private float DiagonalFlipChance = 0.5f;

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
        RebalanceGrid();
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
    }

    private void RemoveNeighbors()
    {
        //TOOD fill in
    }

    private void RebalanceGrid()
    {
        //TODO fill in
    }

    private void OnDrawGizmosSelected()
    {
        foreach(Grid_Node node in Grid)
        {
            if(node.IsMovable)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(node.PositionToVector3(), 0.1f);
            }
            else
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(node.PositionToVector3(), 0.1f);
            }

            foreach(KeyValuePair<Grid_Node, float> neighbor in node.Neighbors)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(node.PositionToVector3(), neighbor.Key.PositionToVector3());
            }
        }
    }
}
