using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SinTimer : MonoBehaviour
{
    public float gameTimer;

    public List<TimeCheckpoint> timeCheckpointsList;

    private PlayerInfoController playerInfoController;

    private void Start()
    {
        playerInfoController = FindObjectOfType<PlayerInfoController>();
    }
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
                    List<StatContainer.Stat> statsToAffectList = new List<StatContainer.Stat>();
                    StatContainer.Stat prideStat = new StatContainer.Stat();
                    prideStat.statName = "Pride";
                    prideStat.statValue = point.pridePoints;

                    statsToAffectList.Add(prideStat);

                    //add pride points
                    playerInfoController.AffectStatValues(statsToAffectList);
                    return;
                }
                else if (gameTimer > point.maxTime)
                {
                    //add sloth points

                    List<StatContainer.Stat> statsToAffectList = new List<StatContainer.Stat>();
                    StatContainer.Stat slothStat = new StatContainer.Stat();
                    slothStat.statName = "Sloth";
                    slothStat.statValue = point.slothPoints;

                    statsToAffectList.Add(slothStat);

                    //add pride points
                    playerInfoController.AffectStatValues(statsToAffectList);
                    return;
                }
            }
        }
    }

    [System.Serializable]
    public class TimeCheckpoint
    {
        public float minTime, maxTime;
        public float pridePoints, slothPoints;
        public GameObject timeCheckpoint;
    }
}
