using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HapticToucher : MonoBehaviour
{
    public GameObject goal;
    public GameObject start;
    public HapticPlugin HapticDevice = null;

    // Start is called before the first frame update
    void Start()
    {
        if (HapticDevice == null)
            HapticDevice = (HapticPlugin)FindObjectOfType(typeof(HapticPlugin));

    }

    // Update is called once per frame
    void Update()
    {
        isTouchingGoal();
        if (Input.GetKey(KeyCode.T))
        {
            teleportToGoal();
        }
    }

    void isTouchingGoal()
    {
        LocationTreatment e = (LocationTreatment)goal.GetComponent(typeof(LocationTreatment));
        if (e)
        {
            if (HapticDevice.touching == goal && e.getActive())
            {
                //this.transform.position = start.transform.position;
                goal.transform.position = start.transform.position;
                


                e.wasTouched();

            }
        }
        
    }

    void teleportToGoal()
    {
        this.transform.position = goal.transform.position;
    }
}
