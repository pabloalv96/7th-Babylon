using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerInfoController : MonoBehaviour
{
    public StatContainer playerStats;

    public PlayerDialogue playerDialogue;

    public int foodConsumed;

    //public float AffectStatalues(StatContainer.Stat stat)
    //{
    //    for (int i = 0; i < playerStats.listOfStats.Count; i++)
    //    {
    //        if (playerStats.listOfStats[i].statName == stat.statName)
    //            playerStats.listOfStats[i].statValue += stat.statValue;

    //    }

    //    return stat.statValue;
    //}

    public void AffectStatValues(List<StatContainer.Stat> statsToEffectList)
    {
        foreach (StatContainer.Stat statToEffect in statsToEffectList)
        {
            foreach (StatContainer.Stat stat in FindObjectOfType<PlayerInfoController>().playerStats.listOfStats)
            {
                if (stat.statName == statToEffect.statName)
                {
                    stat.statValue += statToEffect.statValue;
                }
            }
        }

        CheckStatValues();
    }

    static int SortStatByValues(StatContainer.Stat s1, StatContainer.Stat s2)
    {
        return s2.statValue.CompareTo(s1.statValue);
    }


    public void CheckStatValues()
    {

        FindObjectOfType<PlayerInfoController>().playerStats.listOfStats.Sort(SortStatByValues);

        FindObjectOfType<PlayerInfoController>().playerStats.highestStat = FindObjectOfType<PlayerInfoController>().playerStats.listOfStats[0];

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
