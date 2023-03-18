using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Timer : MonoBehaviour
{
    LocationTreatment Experiment;
    bool timerOn;
    float timer;

    private void Awake()//When the program starts
    {
        Experiment = (LocationTreatment)GetComponent(typeof(LocationTreatment));
        timerOn = false;
        timer = 0f;
        using StreamWriter file = new("data.txt", append: true);
        file.WriteLine("Treatment,Location,Time");
    }

        // Update is called once per frame
        void Update()
    {
        //If experiment is on, and the timer is off, turn the timer on
        //if the experiment is off, and the timer is off, if the timer was previously on then output the result
        //Adding time out of 2 minutes to line 29
        if (timerOn)
        {
            if (Experiment.getActive() && timer < 60f)
            {
                timer += Time.deltaTime;
            }
            else
            {
                if(timer > 60f)
                {
                    Experiment.signaledEnd();
                }
                timer += Time.deltaTime;
                using StreamWriter file = new("data.txt", append: true);
                file.WriteLine(Experiment.latestTreatment() + "," + Experiment.latestLocation() + "," + timer.ToString());
                timerOn = false;
                timer = 0f;
            }
        }
        else
        {
            if (Experiment.getActive())
            {
                timerOn = true;
            }
            
        }
    }

}
