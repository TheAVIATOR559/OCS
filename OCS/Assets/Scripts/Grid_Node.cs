using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid_Node
{
    public Vector2 Position;
    public Dictionary<Grid_Node, float> Neighbors;
    //public bool IsMovable
    //{
    //    get;
    //    private set;
    //}

    public Grid_Node(int x, int y)
    {
        Position = new Vector2(x, y);
        Neighbors = new Dictionary<Grid_Node, float>();
        //IsMovable = movable;
    }

    public void AddNeighbor(Grid_Node neighbor, float weight)
    {
        if(Neighbors.ContainsKey(neighbor))
        {
            return;
        }

        Neighbors.Add(neighbor, weight);
    }

    public void AddNeighborMutual(Grid_Node neighbor, float toWeight, float fromWeight)
    {
        if (Neighbors.ContainsKey(neighbor))
        {
            return;
        }

        Neighbors.Add(neighbor, toWeight);
        neighbor.AddNeighbor(this, fromWeight);
    }

    //public Vector3 PositionToVector3()
    //{
    //    return new Vector3(Position.x, 0, Position.y);
    //}

    public void RemoveNeighbor(Grid_Node neighbor, bool mutual = false)
    {
        if(Neighbors.ContainsKey(neighbor))
        {
            Neighbors.Remove(neighbor);

            if(mutual)
            {
                neighbor.RemoveNeighbor(this);
            }
        }
    }
}
