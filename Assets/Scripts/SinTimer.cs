using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinTimer : MonoBehaviour
{
    public float gameTimer;

    public List<TimeCheckpoint> timeCheckpointsList;

    public void FixedUpdate()
    {
        gameTimer += Time.deltaTime;
    }

    public void CheckTime(GameObject checkpoint)
    {
        foreach(TimeCheckpoint point in timeCheckpointsList)
        {
            if (point.timeCheckpoint == checkpoint)
            {
                if (gameTimer > point.minTime && gameTimer < point.maxTime)
                {
                    return;
                }
                else if (gameTimer < point.minTime)
                {
                    //add pride points
                }
                else if (gameTimer > point.maxTime)
                {
                    //add sloth points
                }
            }
        }
    }

    public class TimeCheckpoint
    {
        public float minTime, maxTime;
        public GameObject timeCheckpoint;
    }
}
