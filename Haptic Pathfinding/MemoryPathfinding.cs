using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class MemoryPathfinding : MonoBehaviour
{
    MemoryGrid GridReference;//For referencing the grid class
    public Transform StartPosition;//Starting position to pathfind from
    public Transform TargetPosition;//Starting position to pathfind to
    public Transform DisappearingWall;
    LocationTreatment Experiment; //Referencing the experimental class
    public GameObject DisappearingPlane;


    public Transform[,] HapticCubes; //GameObject containing all of the cubes used for haptic feedback

    public GameObject wallObj;
    public double percentChanceSwap;
    //bool Deactivated;
    bool planeHidden;
    //bool onceFlag;

    private void Awake()//When the program starts
    {
        Experiment = (LocationTreatment)GetComponentInChildren(typeof(LocationTreatment));
        GridReference = GetComponent<MemoryGrid>();//Get a reference to the game manager
        HapticCubes = new Transform[transform.childCount * transform.GetChild(0).childCount, transform.GetChild(0).GetChild(0).childCount]; //Initialize array for haptic cubes
        //onceFlag = false;
        int rowCount = 0;

        foreach(Transform row in transform.GetChild(0))
        {
            int cubeCount = 0;
            foreach (Transform cube in row)
            {
                HapticCubes[rowCount, cubeCount] = cube;
                ++cubeCount;
            }
            ++rowCount;
        }
        //Deactivated = false;
    }

    private void Update()//Every frame
    {
        PlaneHider(Experiment.getCurrentTreatment(), Experiment.getActive());
        FindPath(StartPosition.position, TargetPosition.position);//Find a path to the goal
        /*
        double rand = Random.Range(0f, 1f);
        if (rand <= percentChanceSwap)
        {
            if (Deactivated) ReAppear(DisappearingWall.position);
            else DisappearTheObj(DisappearingWall.position);
        }
        */
        if (Input.GetKey(KeyCode.UpArrow))
        {
            ChangeHapticFeedback(1.0f);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            ChangeHapticFeedback(0.0f);
        }
        if (Input.GetKey(KeyCode.K))
        {
            PlaneHider(1, true);
        }
        if (Input.GetKey(KeyCode.L))
        {
            PlaneHider(1, false);
        }
        if (Experiment.getActive())
        {
            UpdateDistances();
            UpdateHaptics(Experiment.getCurrentTreatment());
            /*
            onceFlag = true;
            if (!onceFlag)
            {
                
            }
            */
            
        }
        /*
        if (!Experiment.getActive())
        {
            onceFlag = false;
        }
        */
        
    }

    public void ChangeHapticFeedback(float magnitude)
    {
        foreach(Transform cube in HapticCubes)
        {
            //using StreamWriter file = new("TestingChangeHaptics.txt", append: true);
            

            HapticEffect e = (HapticEffect)cube.gameObject.GetComponent(typeof(HapticEffect));
            if (e != null)
            {
                e.effectType = HapticEffect.EFFECT_TYPE.VIBRATE;
                e.Gain = 0.6;
                //magnitude;
                e.Magnitude = magnitude;
            }



        }
    }



    private void UpdateDistances()
    {
        //using StreamWriter file = new("TestingUpdateHaptics.txt", append: true);
        for (int finalPathNodes = 0; finalPathNodes < GridReference.FinalPath.Count; ++finalPathNodes)
        {
            //file.Write(j.ToString());
            List<Node> ClosedNodes = new List<Node>();

            foreach (Node NeighborNode in GridReference.GetNeighboringNodes(GridReference.FinalPath[finalPathNodes]))
            {
                //If the node is in the final path, and hasnt already been changed, change it
                if (GridReference.FinalPath.Contains(NeighborNode) && NeighborNode.distance > 0)
                {
                    NeighborNode.distance = 0;
                }
                //If the node isnt in the final path and hasnt been changed back, fix it
                else if (!GridReference.FinalPath.Contains(NeighborNode) && NeighborNode.distance == 0)
                {
                    NeighborNode.distance = 9999;
                }
                else
                {
                    //Find the length from the final path node to the neighbor
                    int testdistance = FindPathLength(GridReference.FinalPath[finalPathNodes].Position, NeighborNode.Position);
                    if (testdistance < NeighborNode.distance) NeighborNode.distance = testdistance;
                }
                ClosedNodes.Add(NeighborNode);
                foreach (Node CousinNode in GridReference.GetNeighboringNodes(NeighborNode))
                {
                    if (ClosedNodes.Contains(CousinNode)) continue;
                    //If the node is in the final path, and hasnt already been changed, change it
                    if (GridReference.FinalPath.Contains(CousinNode) && CousinNode.distance > 0)
                    {
                        NeighborNode.distance = 0;
                    }
                    //If the node isnt in the final path and hasnt been changed back, fix it
                    else if (!GridReference.FinalPath.Contains(CousinNode) && CousinNode.distance == 0)
                    {
                        NeighborNode.distance = 9999;
                    }
                    else
                    {
                        //Find the length from a final path node to the cousin
                        int testdistance = FindPathLength(GridReference.FinalPath[finalPathNodes].Position, CousinNode.Position);
                        if (testdistance < CousinNode.distance) CousinNode.distance = testdistance;
                    }
                    ClosedNodes.Add(CousinNode);
                }
            }
        }
    }

    private void UpdateHaptics(int treatment)
    {
        //Expect distances to be: 0, 1, 2, or 9999

        if (treatment == 1 || treatment == 2)
        {
            for (int row = 0; row < 20; ++row)
            {
                for (int cube = 0; cube < 20; ++cube)
                {
                    int distance = GridReference.NodeFromWorldPoint(HapticCubes[row, cube].position).distance;
                    if (distance > 2)
                    {
                        HapticEffect e = (HapticEffect)HapticCubes[row, cube].gameObject.GetComponent(typeof(HapticEffect));
                        if (e)
                        {
                            e.Gain = 0.6;
                            e.Magnitude = 0.8;
                        }
                    }
                    else
                    {
                        HapticEffect e = (HapticEffect)HapticCubes[row, cube].gameObject.GetComponent(typeof(HapticEffect));
                        if (e)
                        {
                            if(distance == 0)
                            {
                                e.Gain = 0.6;
                                e.Magnitude = 0.0;
                            }
                            else if(distance == 1)
                            {
                                e.Gain = 0.6;
                                e.Magnitude = 0.1;
                            }
                            else
                            {
                                e.Gain = 0.6;
                                e.Magnitude = 0.4;
                            }
                            
                        }

                    }

                }
            }
        }
        //If only visual or control
        else
        {
            for (int row = 0; row < 20; ++row)
            {
                for (int cube = 0; cube < 20; ++cube)
                {

                    HapticEffect e = (HapticEffect)HapticCubes[row, cube].gameObject.GetComponent(typeof(HapticEffect));
                    if (e)
                    {
                        e.Gain = 0.0;
                        e.Magnitude = 0.0;
                    }
                }
            }
        }
    }






private void PlaneHider(int treatment, bool active)
    {
        if((treatment == 1 || treatment == 3) && active)
        {
            if (!DisappearingPlane.activeSelf) DisappearingPlane.SetActive(true);
        }
        else
        {
            DisappearingPlane.SetActive(false);
        }
    }

    void DisappearTheObj(Vector3 wall)
    {
        Node DisWallNode = GridReference.NodeFromWorldPoint(wall);
        DisWallNode.isRandomWall(false);
        //Deactivated = true;
        wallObj.SetActive(false);
    }

    void ReAppear(Vector3 wall)
    {
        wallObj.SetActive(true);
        Node DisWallNode = GridReference.NodeFromWorldPoint(wall);
        DisWallNode.isRandomWall(true);
        //Deactivated = false;
    }


    void FindPath(Vector3 a_StartPos, Vector3 a_TargetPos)
    {
        Node StartNode = GridReference.NodeFromWorldPoint(a_StartPos);//Gets the node closest to the starting position
        Node TargetNode = GridReference.NodeFromWorldPoint(a_TargetPos);//Gets the node closest to the target position

        List<Node> OpenList = new List<Node>();//List of nodes for the open list
        HashSet<Node> ClosedList = new HashSet<Node>();//Hashset of nodes for the closed list

        OpenList.Add(StartNode);//Add the starting node to the open list to begin the program

        while (OpenList.Count > 0)//Whilst there is something in the open list
        {
            Node CurrentNode = OpenList[0];//Create a node and set it to the first item in the open list
            for (int i = 1; i < OpenList.Count; i++)//Loop through the open list starting from the second object
            {
                if (OpenList[i].FCost < CurrentNode.FCost || OpenList[i].FCost == CurrentNode.FCost && OpenList[i].hCost < CurrentNode.hCost)//If the f cost of that object is less than or equal to the f cost of the current node
                {
                    CurrentNode = OpenList[i];//Set the current node to that object
                }
            }
            OpenList.Remove(CurrentNode);//Remove that from the open list
            ClosedList.Add(CurrentNode);//And add it to the closed list

            if (CurrentNode == TargetNode)//If the current node is the same as the target node
            {
                GetFinalPath(StartNode, TargetNode);//Calculate the final path
            }

            foreach (Node NeighborNode in GridReference.GetNeighboringNodes(CurrentNode))//Loop through each neighbor of the current node
            {
                if (!NeighborNode.isWall || ClosedList.Contains(NeighborNode) || NeighborNode.disappearingWall)//If the neighbor is a wall or has already been checked
                {
                    continue;//Skip it
                }
                int MoveCost = CurrentNode.gCost + GetManhattenDistance(CurrentNode, NeighborNode);//Get the F cost of that neighbor

                if (MoveCost < NeighborNode.gCost || !OpenList.Contains(NeighborNode))//If the f cost is greater than the g cost or it is not in the open list
                {
                    NeighborNode.gCost = MoveCost;//Set the g cost to the f cost
                    NeighborNode.hCost = GetManhattenDistance(NeighborNode, TargetNode);//Set the h cost
                    NeighborNode.Parent = CurrentNode;//Set the parent of the node for retracing steps

                    if (!OpenList.Contains(NeighborNode))//If the neighbor is not in the openlist
                    {
                        OpenList.Add(NeighborNode);//Add it to the list
                    }
                }
            }

        }
    }



    void GetFinalPath(Node a_StartingNode, Node a_EndNode)
    {
        if(GridReference.FinalPath != null)
        {
            GridReference.LastPath = GridReference.FinalPath;
        }

        List<Node> FinalPath = new List<Node>();//List to hold the path sequentially 
        Node CurrentNode = a_EndNode;//Node to store the current node being checked

        while (CurrentNode != a_StartingNode)//While loop to work through each node going through the parents to the beginning of the path
        {
            FinalPath.Add(CurrentNode);//Add that node to the final path
            CurrentNode = CurrentNode.Parent;//Move onto its parent node
        }

        FinalPath.Reverse();//Reverse the path to get the correct order


        /*
         *         bool flag = false;
        foreach(PotentialPath path in GridReference.PotentialPaths)
            {
            if (path.Contain(FinalPath))
                {
                path.IsActive++;
                flag = true;
                }
            }
        if (!flag) {
            PotentialPath newPath = new PotentialPath(FinalPath);
            GridReference.PotentialPaths.Add(newPath);
                }
        */

        GridReference.FinalPath = FinalPath;//Set the final path

    }

    int GetManhattenDistance(Node a_nodeA, Node a_nodeB)
    {
        int ix = Mathf.Abs(a_nodeA.gridX - a_nodeB.gridX);//x1-x2
        int iy = Mathf.Abs(a_nodeA.gridY - a_nodeB.gridY);//y1-y2

        return ix + iy;//Return the sum
    }

    int FindPathLength(Vector3 a_StartPos, Vector3 a_TargetPos)
    {
        Node StartNode = GridReference.NodeFromWorldPoint(a_StartPos);//Gets the node closest to the starting position
        Node TargetNode = GridReference.NodeFromWorldPoint(a_TargetPos);//Gets the node closest to the target position

        List<Node> OpenList = new List<Node>();//List of nodes for the open list
        HashSet<Node> ClosedList = new HashSet<Node>();//Hashset of nodes for the closed list

        OpenList.Add(StartNode);//Add the starting node to the open list to begin the program

        while (OpenList.Count > 0)//Whilst there is something in the open list
        {
            Node CurrentNode = OpenList[0];//Create a node and set it to the first item in the open list
            for (int i = 1; i < OpenList.Count; i++)//Loop through the open list starting from the second object
            {
                if (OpenList[i].FCost < CurrentNode.FCost || OpenList[i].FCost == CurrentNode.FCost && OpenList[i].hCost < CurrentNode.hCost)//If the f cost of that object is less than or equal to the f cost of the current node
                {
                    CurrentNode = OpenList[i];//Set the current node to that object
                }
            }
            OpenList.Remove(CurrentNode);//Remove that from the open list
            ClosedList.Add(CurrentNode);//And add it to the closed list

            if (CurrentNode == TargetNode)//If the current node is the same as the target node
            {
                List<Node> FinalPath = new List<Node>();//List to hold the path sequentially 
                Node CheckNode = TargetNode;//Node to store the current node being checked

                while (CheckNode != StartNode)//While loop to work through each node going through the parents to the beginning of the path
                {
                    FinalPath.Add(CheckNode);//Add that node to the final path
                    CheckNode = CheckNode.Parent;//Move onto its parent node
                }

                return FinalPath.Count;
            }

            foreach (Node NeighborNode in GridReference.GetNeighboringNodes(CurrentNode))//Loop through each neighbor of the current node
            {
                if (!NeighborNode.isWall || ClosedList.Contains(NeighborNode) || NeighborNode.disappearingWall)//If the neighbor is a wall or has already been checked
                {
                    continue;//Skip it
                }
                int MoveCost = CurrentNode.gCost + GetManhattenDistance(CurrentNode, NeighborNode);//Get the F cost of that neighbor

                if (MoveCost < NeighborNode.gCost || !OpenList.Contains(NeighborNode))//If the f cost is greater than the g cost or it is not in the open list
                {
                    NeighborNode.gCost = MoveCost;//Set the g cost to the f cost
                    NeighborNode.hCost = GetManhattenDistance(NeighborNode, TargetNode);//Set the h cost
                    NeighborNode.Parent = CurrentNode;//Set the parent of the node for retracing steps

                    if (!OpenList.Contains(NeighborNode))//If the neighbor is not in the openlist
                    {
                        OpenList.Add(NeighborNode);//Add it to the list
                    }
                }
            }

        }
        return 9999;
    }

}
