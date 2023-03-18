using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class LocationTreatment : MonoBehaviour
{
    //locations that the start location will take, as well as the treatment
    public List<(int, int)> experimentalOrder;
    private List<(int, int)> tempOrder;
    public List<Vector3> locations;
    private bool pressKeyFlag;
    public bool activeFlag;
    public bool timerEnd;
    private int lastLoc;
    private int lastTreatment;
    public GameObject manager;

    // Start is called before the first frame update
    void Start()
    {

        activeFlag = false;
        pressKeyFlag = false;
        timerEnd = false;
        lastTreatment = -1 ;
        lastLoc = -1;
        locations = new List<Vector3>();
        Vector3 a = new Vector3(0.05f, 0.0f, 0.33f);
        locations.Add(a);
        a = new Vector3(0.43f, 0.0f, -0.41f);
        locations.Add(a);
        a = new Vector3(-0.10f, 0.0f, -0.41f);
        locations.Add(a);
        a = new Vector3(-0.22f, 0.0f, 0.36f);
        locations.Add(a);
        a = new Vector3(-0.41f, 0.0f, -0.12f);
        locations.Add(a);

        tempOrder = new List<(int loc, int treatment)>();
        tempOrder.Add((0, 0));
        tempOrder.Add((0, 1));
        tempOrder.Add((0, 2));
        tempOrder.Add((0, 3));
        tempOrder.Add((1, 0));
        tempOrder.Add((1, 1));
        tempOrder.Add((1, 2));
        tempOrder.Add((1, 3));
        tempOrder.Add((2, 0));
        tempOrder.Add((2, 1));
        tempOrder.Add((2, 2));
        tempOrder.Add((2, 3));
        tempOrder.Add((3, 0));
        tempOrder.Add((3, 1));
        tempOrder.Add((3, 2));
        tempOrder.Add((3, 3));
        tempOrder.Add((4, 0));
        tempOrder.Add((4, 1));
        tempOrder.Add((4, 2));
        tempOrder.Add((4, 3));

        experimentalOrder = new List<(int loc, int treatment)>();

        int elCount = 20;
        //using StreamWriter file = new("WriteLines.txt", append: true);

        while (experimentalOrder.Count != 20)
        {
            int LocNum = Random.Range(0, elCount);
            experimentalOrder.Add(tempOrder[LocNum]);
            tempOrder.RemoveAt(LocNum);
            elCount--;
            //file.Write(elCount.ToString());
            
        }

        tempOrder.Clear();

    }

    // Update is called once per frame
    void Update()
    {
        //If spacebar is pressed, and the key hasnt been reset, and the location hasnt been collided with
        if (Input.GetKey(KeyCode.Space) && (!pressKeyFlag && !activeFlag))
        {
            if (experimentalOrder.Count > 0)
            {
                this.transform.position = locations[experimentalOrder[0].Item1];
                //Visual
                if (experimentalOrder[0].Item2 == 0)
                {
                    gameObject.GetComponent<Renderer>().material.color = new Color(255, 0, 0);
                }
                //Haptic
                else if (experimentalOrder[0].Item2 == 1)
                {
                    gameObject.GetComponent<Renderer>().material.color = new Color(0, 255, 0);
                }
                //Visuo-Haptic
                else if (experimentalOrder[0].Item2 == 2)
                {
                    gameObject.GetComponent<Renderer>().material.color = new Color(0, 0, 255);
                }
                //Control
                else
                {
                    gameObject.GetComponent<Renderer>().material.color = new Color(1, 1, 1);
                }
                pressKeyFlag = true;
                activeFlag = true;
                lastLoc = experimentalOrder[0].Item1;
                lastTreatment = experimentalOrder[0].Item2;
                //experimentalOrder.RemoveAt(0);

            }   
        }
        if (Input.GetKeyUp(KeyCode.Space)) pressKeyFlag = false;
        if(timerEnd)
        {
            wasTouched();
            timerEnd = false;
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            print();
        }
    }

    public int getCurrentTreatment()
    {
        if (experimentalOrder.Count > 0)
        {
            return experimentalOrder[0].Item2;
        }
        else return -1;
    }

    public bool getActive()
    {
        return activeFlag;
    }

    public void wasTouched()
    {
        activeFlag = false;
        removeCurrentExp();
        /*
        MemoryPathfinding e = (MemoryPathfinding)manager.gameObject.GetComponent(typeof(MemoryPathfinding));
        if (e)
        {
            e.ChangeHapticFeedback(0.0f);
        }
        */
    }

    public string latestLocation()
    {
        return lastLoc.ToString();
    }

    public void signaledEnd()
    {
        timerEnd = true;
    }

    public string latestTreatment()
    {
        return lastTreatment.ToString();
    }

    public void removeCurrentExp()
    {
        if(experimentalOrder.Count > 0)
        {
            experimentalOrder.RemoveAt(0);
        }
    }

    public void print()
    {
        using StreamWriter file = new("log.txt", append: true);
        file.WriteLine("Remaining Locations");
        for(int i = 0; i<experimentalOrder.Count; ++i)
        {
            file.WriteLine(experimentalOrder[i].Item1.ToString());
            file.WriteLine(experimentalOrder[i].Item2.ToString());
        }
        file.WriteLine("Flags:");
        file.WriteLine("Active:" + activeFlag.ToString());
        file.WriteLine("Pressed Key:" + pressKeyFlag.ToString());
        file.WriteLine("Last Loc:" + lastLoc.ToString());
        file.WriteLine("Last Treatment" + lastTreatment.ToString());
    }

}


