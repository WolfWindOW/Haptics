using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryGrid : MonoBehaviour
{
    public Transform StartPosition; //This is where the program will start the pathfinding from.
    public LayerMask WallMask; //This is the mask that the program will look for when trying to find obstructions to the path.
    public Vector2 gridWorldSize; //A vector2 to store the width and height of the graph in world units.
    public float nodeRadius; //This stores how big each square on the graph will be
    public float Distance; //The distance that the squares will spawn from eachother.


    Node[,] grid; 
    public List<Node> FinalPath;
    //public List<PotentialPath> PotentialPaths;
    public float trustRatio;

    public List<Node> LastPath;


    float nodeDiameter;
    int gridSizeX, gridSizeY; //Size of the grid in array u

    private void Start()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }



    private void CreateGrid()
    {
        //Declare the array of nodes.
        grid = new Node[gridSizeX, gridSizeY];
        //Get the real world position of the bottom left of the grid.
        Vector3 bottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
        for (int x = 0; x < gridSizeX; ++x)
        {
            for (int y = 0; y < gridSizeY; ++y)
            {
                Vector3 worldPoint = bottomLeft + Vector3.right * (x * nodeDiameter + nodeDiameter) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool Wall = true;
                //If the node is not being obstructed
                //Quick collision check against the current node and anything in the world at its position. If it is colliding with an object with a WallMask,
                //The if statement will return false.
                if (Physics.CheckSphere(worldPoint, nodeRadius, WallMask)) Wall = false;
                grid[x, y] = new Node(Wall, worldPoint, x, y);
            }
        }
    }

    public List<Node> GetNeighboringNodes(Node neighborNode)
    {
        //Make a new list of all available neighbors.
        List<Node> NeighborList = new List<Node>();
        //Variables to check if the Position is within range of the node array to avoid out of range errors.
        int iCheckX;
        int iCheckY;
        
        //Check the right side of the current node.
        iCheckX = neighborNode.gridX + 1;
        iCheckY = neighborNode.gridY;

        //If the XPosition is in range of the array
        if (iCheckX >= 0 && iCheckX < gridSizeX)
        {
            //If the YPosition is in range of the array
            //Add the grid to the available neighbors list
            if (iCheckY >= 0 && iCheckY < gridSizeY) NeighborList.Add(grid[iCheckX, iCheckY]);
        }

        //Check the Left side of the current node.
        iCheckX = neighborNode.gridX - 1;
        iCheckY = neighborNode.gridY;
        //If the XPosition is in range of the array
        if (iCheckX >= 0 && iCheckX < gridSizeX)
        {
            //If the YPosition is in range of the array
            //Add the grid to the available neighbors list
            if (iCheckY >= 0 && iCheckY < gridSizeY) NeighborList.Add(grid[iCheckX, iCheckY]);
        }

        //Check the Top side of the current node.
        iCheckX = neighborNode.gridX;
        iCheckY = neighborNode.gridY + 1;
        //If the XPosition is in range of the array
        if (iCheckX >= 0 && iCheckX < gridSizeX)
        {
            //If the YPosition is in range of the array
            //Add the grid to the available neighbors list
            if (iCheckY >= 0 && iCheckY < gridSizeY) NeighborList.Add(grid[iCheckX, iCheckY]);
        }
        //Check the Bottom side of the current node.
        iCheckX = neighborNode.gridX;
        iCheckY = neighborNode.gridY - 1;
        //If the XPosition is in range of the array
        if (iCheckX >= 0 && iCheckX < gridSizeX)
        {
            //If the YPosition is in range of the array
            //Add the grid to the available neighbors lis
            if (iCheckY >= 0 && iCheckY < gridSizeY) NeighborList.Add(grid[iCheckX, iCheckY]);
        }

        return NeighborList;//Return the neighbors list
    }


    //Gets the closest node to the given world position.
    public Node NodeFromWorldPoint(Vector3 vWorldPos)
    {
        float ixPos = ((vWorldPos.x + gridWorldSize.x / 2) / gridWorldSize.x);
        float iyPos = ((vWorldPos.z + gridWorldSize.y / 2) / gridWorldSize.y);

        ixPos = Mathf.Clamp01(ixPos);
        iyPos = Mathf.Clamp01(iyPos);

        int ix = Mathf.RoundToInt((gridSizeX - 1) * ixPos);
        int iy = Mathf.RoundToInt((gridSizeY - 1) * iyPos);

        return grid[ix, iy];
    }

    //Draws wireframes
    private void OnDrawGizmos()
    {
        //Draw wire cube with given dimensions 
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
        //If grid isnt empty
        if(grid != null)
        {
            foreach(Node node in grid)
            {
                if (node.isWall) Gizmos.color = Color.white;
                else Gizmos.color = Color.yellow;

                //If final path has been found
                if (FinalPath != null)
                {
                    //If the current node is in the final path
                    if(FinalPath.Contains(node)) Gizmos.color = Color.red;
                }
                //Draw the node
                Gizmos.DrawCube(node.Position, Vector3.one * (nodeDiameter - Distance));
            }
        }
    }
}
