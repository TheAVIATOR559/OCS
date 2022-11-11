using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid_Node
{
    public Vector2 Position;
    public List<Grid_Node> Neighbors;
    //public bool IsMovable
    //{
    //    get;
    //    private set;
    //}

    public Grid_Node(int x, int y)
    {
        Position = new Vector2(x, y);
        Neighbors = new List<Grid_Node>();
        //IsMovable = movable;
    }

    public void AddNeighbor(Grid_Node neighbor, bool mutual = false)
    {
        if(Neighbors.Contains(neighbor))
        {
            return;
        }

        Neighbors.Add(neighbor);

        if (mutual)
        {
            neighbor.AddNeighbor(this);
        }
    }

    public void RemoveNeighbor(Grid_Node neighbor, bool mutual = false)
    {
        if(Neighbors.Contains(neighbor))
        {
            Neighbors.Remove(neighbor);

            if(mutual)
            {
                neighbor.RemoveNeighbor(this);
            }
        }
    }

    public void Merge(Grid_Node other)
    {
        if(other == this)
        {
            return;
        }

        foreach(Grid_Node node in other.Neighbors)
        {
            AddNeighbor(node);
            other.RemoveNeighbor(node);
        }

        other.Position = new Vector2(-1, -1);
    }
}
