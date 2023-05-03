using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SinsBreakdown : MonoBehaviour
{
    public PlayerStatScriptableObject playerStatSO;

    public Slider lustSlider, gluttSlider, greedSlider, wrathSlider, slothSlider, envySlider, prideSlider;
    

    void Start()
    {
        //playerInfoController = FindObjectOfType<PlayerInfoController>();

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        string currentStat;
        for (int i = 0; i < playerStatSO.playerStats.listOfStats.Count; i++)
        {
            currentStat = playerStatSO.playerStats.listOfStats[i].statName;
            switch (currentStat)
            {
                case "Lust":
                    lustSlider.value = playerStatSO.playerStats.listOfStats[i].statValue;
                    break;
                case "Gluttony":
                    gluttSlider.value = playerStatSO.playerStats.listOfStats[i].statValue;
                    break;
                case "Greed":
                    greedSlider.value = playerStatSO.playerStats.listOfStats[i].statValue;
                    break;
                case "Wrath":
                    wrathSlider.value = playerStatSO.playerStats.listOfStats[i].statValue;
                    break;
                case "Sloth":
                    slothSlider.value = playerStatSO.playerStats.listOfStats[i].statValue;
                    break;
                case "Envy":
                    envySlider.value = playerStatSO.playerStats.listOfStats[i].statValue;
                    break;
                case "Pride":
                    prideSlider.value = playerStatSO.playerStats.listOfStats[i].statValue;
                    break;
            }
        }  
    }

}
