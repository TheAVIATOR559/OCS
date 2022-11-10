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

    public void AddNeighbor(Grid_Node neighbor, float weight, bool mutual = false)
    {
        if(Neighbors.ContainsKey(neighbor))
        {
            return;
        }

        Neighbors.Add(neighbor, weight);

        if (mutual)
        {
            neighbor.AddNeighbor(this, weight);
        }
    }

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

    public void Merge(Grid_Node other)
    {
        if(other == this)
        {
            return;
        }

        foreach(KeyValuePair<Grid_Node, float> kvp in other.Neighbors)
        {
            AddNeighbor(kvp.Key, kvp.Value);
            other.RemoveNeighbor(kvp.Key);
        }

        other.Position = new Vector2(-1, -1);
    }
}
