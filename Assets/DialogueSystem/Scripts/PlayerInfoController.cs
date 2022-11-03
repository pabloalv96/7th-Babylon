using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerInfoController : MonoBehaviour
{
    public StatContainer playerStats;

    public PlayerDialogue playerDialogue;

    public float AffectStatalues(StatContainer.Stat stat)
    {
        for (int i = 0; i < playerStats.listOfStats.Count; i++)
        {
            if (playerStats.listOfStats[i].statName == stat.statName)
                playerStats.listOfStats[i].statValue += stat.statValue;

        }

        return stat.statValue;
    }

    //static int SortStatByValues(StatContainer.Stat s1, StatContainer.Stat s2)
    //{
    //    return s2.statValue.CompareTo(s1.statValue);
    //}


    //public void CheckStatValues()
    //{

    //    playerStats.listOfStats.Sort(SortStatByValues);

    //    playerStats.highestStat = playerStats.listOfStats[0];

    //}
}

//public class SortStatByValues : IComparer<StatContainer.Stat>
//{
//    public int Compare(StatContainer.Stat s1, StatContainer.Stat s2)
//    {
//        return s2.statValue.CompareTo(s1.statValue);
//    }
//}
