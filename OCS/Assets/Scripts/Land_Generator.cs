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

    private Grid_Node[,] Grid;

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        GenGrid();
    }

    private void GenGrid()
    {
        Grid = new Grid_Node[HorizontalSize, VerticalSize];

        for (int x = 0; x < HorizontalSize; x++)
        {
            for(int y = 0; y < VerticalSize; y++)
            {
                Grid[x, y] = new Grid_Node(x, y);
            }
        }

        for (int x = 0; x < HorizontalSize; x++)
        {
            for (int y = 0; y < VerticalSize; y++)
            {
                if(x + 1 < HorizontalSize)
                {
                    Grid[x, y].AddNeighbor(Grid[x + 1, y]);
                }

                if(x - 1 >= 0)
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
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        foreach(Grid_Node node in Grid)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(node.PositionToVector3(), 0.1f);

            foreach(Grid_Node neighbor in node.Neighbors)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(node.PositionToVector3(), neighbor.PositionToVector3());
            }
        }
    }
}
