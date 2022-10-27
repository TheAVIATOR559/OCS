using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid_Node
{
    public Vector2 Position;
    public List<Grid_Node> Neighbors;

    public Grid_Node(int x, int y)
    {
        Position = new Vector2(x, y);
        Neighbors = new List<Grid_Node>();
    }

    public void AddNeighbor(Grid_Node neighbor, bool oneWay = false)
    {
        if(Neighbors.Contains(neighbor))
        {
            return;
        }

        Neighbors.Add(neighbor);

        if(!oneWay)
        {
            neighbor.AddNeighbor(this, false);
        }
    }

    public Vector3 PositionToVector3()
    {
        return new Vector3(Position.x, 0, Position.y);
    }
}
