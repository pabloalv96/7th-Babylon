//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class AffectStats : MonoBehaviour
//{
//    public void AffectStatValues(List<StatContainer.Stat> statsToEffectList)
//    {
//        foreach (StatContainer.Stat statToEffect in statsToEffectList)
//        {
//            foreach (StatContainer.Stat stat in FindObjectOfType<PlayerInfoController>().playerStats.listOfStats)
//            {
//                if (stat.statName == statToEffect.statName)
//                {
//                    stat.statValue += statToEffect.statValue;
//                }
//            }
//        }

//        CheckStatValues();
//    }

//    static int SortStatByValues(StatContainer.Stat s1, StatContainer.Stat s2)
//    {
//        return s2.statValue.CompareTo(s1.statValue);
//    }


//    public void CheckStatValues()
//    {

//        FindObjectOfType<PlayerInfoController>().playerStats.listOfStats.Sort(SortStatByValues);

//        FindObjectOfType<PlayerInfoController>().playerStats.highestStat = FindObjectOfType<PlayerInfoController>().playerStats.listOfStats[0];

//    }
//}
