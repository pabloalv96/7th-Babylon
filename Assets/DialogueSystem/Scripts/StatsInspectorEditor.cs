using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//Create and edit npc emotions in the inspector
# if UNITY_EDITOR
[CustomEditor(typeof(PlayerInfoController))]

public class StatsInspectorEditor : Editor
{
    StatContainer statContainer;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PlayerInfoController playerInfo = (PlayerInfoController)target;
        if (playerInfo == null) return;

        statContainer = playerInfo.playerStats;

        GUILayout.Space(5f);

        GUILayout.Label("Create New Stats for the Player");

        GUILayout.BeginHorizontal();

        statContainer.newStatName = GUILayout.TextField(statContainer.newStatName, 25);

        if (GUILayout.Button("Create New Stat"))
        {
            // Create a new emotion and add it to the list
            StatContainer.Stat newStat = CreateNewStat(statContainer.newStatName);
            AddStatToList(newStat);

            CheckStatValues();

            // reset text field
            statContainer.newStatName = "New Stat";

        }
        GUILayout.EndHorizontal();

        GUILayout.Space(5f);

        GUILayout.Label("Increase & Decrease Stat Values");

        for (int i = 0; i < statContainer.listOfStats.Count; i++)
        {
            //CheckStatValues();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Increase value"))
            {
                statContainer.listOfStats[i].statValue++;

                CheckStatValues();
            }

            GUILayout.Label(statContainer.listOfStats[i].statName);

            if (GUILayout.Button("Decrease value"))
            {
                statContainer.listOfStats[i].statValue--;

                CheckStatValues();
            }
            GUILayout.EndHorizontal();


        }
    }

    public StatContainer.Stat CreateNewStat(string statName)
    {
        float statValue = 0;

        StatContainer.Stat newStat = new StatContainer.Stat();
        newStat.statName = statName;
        newStat.statValue = statValue;

        return newStat;
    }

    public void AddStatToList(StatContainer.Stat newStat)
    {
        statContainer.listOfStats.Add(newStat);

    }

    static int SortStatByValues(StatContainer.Stat s1, StatContainer.Stat s2)
    {
        return s2.statValue.CompareTo(s1.statValue);
    }

    public void CheckStatValues()
    {
       
        statContainer.listOfStats.Sort(SortStatByValues);

        statContainer.highestStat = statContainer.listOfStats[0];

    }
}
#endif
