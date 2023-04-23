using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SinsBreakdown : MonoBehaviour
{
    private PlayerInfoController playerInfoController;

    public Slider lustSlider, gluttSlider, greedSlider, wrathSlider, slothSlider, envySlider, prideSlider;
    

    void Start()
    {
        playerInfoController = FindObjectOfType<PlayerInfoController>();

        string currentStat;
        for (int i = 0; i < playerInfoController.playerStats.listOfStats.Count; i++)
        {
            currentStat = playerInfoController.playerStats.listOfStats[i].statName;
            switch (currentStat)
            {
                case "Lust":
                    lustSlider.value = playerInfoController.playerStats.listOfStats[i].statValue;
                    break;
                case "Gluttony":
                    gluttSlider.value = playerInfoController.playerStats.listOfStats[i].statValue;
                    break;
                case "Greed":
                    greedSlider.value = playerInfoController.playerStats.listOfStats[i].statValue;
                    break;
                case "Wrath":
                    wrathSlider.value = playerInfoController.playerStats.listOfStats[i].statValue;
                    break;
                case "Sloth":
                    slothSlider.value = playerInfoController.playerStats.listOfStats[i].statValue;
                    break;
                case "Envy":
                    envySlider.value = playerInfoController.playerStats.listOfStats[i].statValue;
                    break;
                case "Pride":
                    prideSlider.value = playerInfoController.playerStats.listOfStats[i].statValue;
                    break;
            }
        }
        
    }

    void Update()
    {
        
    }
}
