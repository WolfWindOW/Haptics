using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    //Coordinates in grid system
    //Add Z value for 3d environment
    public int gridX;
    public int gridY;

    public bool disappearingWall;

    public int distance; //Distance from the finalpath

    public bool isWall; //Is the node being obstructed
    public Vector3 Position; //World position of the node

    public Node Parent; //Node it previously came from

    //Cost of the path from the start to node n
    public int gCost;
    //Cost from node n to the goal
    public int hCost;

    //Total cost
    public int FCost { get { return gCost + hCost; } }

    public Node(bool wall, Vector3 pos, int xgrid, int ygrid)
    {
        distance = 9999;
        isWall = wall;
        Position = pos;
        gridX = xgrid;
        gridY = ygrid;
        disappearingWall = false;
    }

    public void setWall(bool wallness)
    {
        isWall = !wallness;
    }

    public void isRandomWall(bool someTimesWall)
    {
        disappearingWall = someTimesWall;
    }
}
