using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.Rendering;

//change textures, props, lighting, and audio based on sins
public class EnvironmentalChangeController : MonoBehaviour
{
    public PlayerInfoController playerInfo;

    public List<EnvironmentalChangesPerStat> environmentalChanges;

    public List<GameObject> propSpawnPoints;

    //public Material wallpapertexture;
    public Material blankPaintingMaterial;

    public AudioSource backgroundAudioSource;

    //public Color lightsColour;
    public List<Light> activeLightsList;

    //public Volume processVolume;

    public bool checkHighestStat;

    [SerializeField] private string highestStatName;
    [SerializeField] private float highestStatValue;

    private DialogueInitiator dialogueInitiator;
    private DialogueListSystem dialogueSystem;

    public NPCInfo narrator;

    public NPCDialogueOption narratorSinDialogue;

    //Check highest stat's value
    //determine what assets to set

    private void Start()
    {
        foreach (EnvironmentalChangesPerStat environmentalChange in environmentalChanges)
        {
            foreach (EnvironmentalChanges change in environmentalChange.environmentalChangesList)
            {
                foreach (GameObject painting in change.paintingsToChange)
                {
                    if (painting.GetComponent<MeshRenderer>().material != blankPaintingMaterial)
                    {
                        painting.GetComponent<MeshRenderer>().material = blankPaintingMaterial;
                    }
                }
            }
        }

        dialogueInitiator = FindObjectOfType<DialogueInitiator>();
        dialogueSystem = FindObjectOfType<DialogueListSystem>();
    }

    private void Update()
    {
        if (checkHighestStat)
        {
            CheckStatValues();
        }
    }

    static int SortStatByValues(StatContainer.Stat s1, StatContainer.Stat s2)
    {
        return s2.statValue.CompareTo(s1.statValue);
    }


    public void CheckStatValues()
    {

        playerInfo.playerStats.listOfStats.Sort(SortStatByValues);

        playerInfo.playerStats.highestStat = playerInfo.playerStats.listOfStats[0];

        if (highestStatName != playerInfo.playerStats.highestStat.statName || highestStatName == playerInfo.playerStats.highestStat.statName && highestStatValue <= playerInfo.playerStats.highestStat.statValue - 10)
        {
            SetEnvironmentalChanges();

            highestStatName = playerInfo.playerStats.highestStat.statName;
            highestStatValue = playerInfo.playerStats.highestStat.statValue;
        }

        //highestStatName = playerInfo.playerStats.highestStat.statName;
        //highestStatValue = playerInfo.playerStats.highestStat.statValue;
    }

    public void SetEnvironmentalChanges()
    {
        foreach (EnvironmentalChangesPerStat environmentalChange in environmentalChanges)
        {

            if (playerInfo.playerStats.highestStat.statName == environmentalChange.statName)
            {
                foreach (EnvironmentalChanges change in environmentalChange.environmentalChangesList)
                {
                    if (change.requiredValue <= playerInfo.playerStats.highestStat.statValue)
                    {
                        //If there is a painting to change to

                        if (change.paintingTextures != null)
                        {
                            foreach (GameObject painting in change.paintingsToChange)
                            {
                                int rand = Random.Range(0, change.paintingTextures.Count);

                                if (!change.paintingTextures.Contains(painting.GetComponent<MeshRenderer>().material))
                                {
                                    painting.GetComponent<MeshRenderer>().material = change.paintingTextures[rand];
                                }

                            }
                        }
                        

                        //if there are props to spawn
                        if (change.props.Count > 0)
                        {
                            // desirable: a way to change number of spawn positions that are utilised based on value of sin stat
                            foreach(GameObject propPosition in propSpawnPoints)
                            {
                                if (propPosition.transform.childCount > 0)
                                {
                                    foreach(Transform child in propPosition.transform)
                                    {
                                        Destroy(child.gameObject); 
                                    }  
                                }

                                int r = Random.Range(0, change.props.Count);

                                GameObject newProp = Instantiate(change.props[r], propPosition.transform.position, Quaternion.identity);
                                newProp.transform.parent = propPosition.transform;
                            }
                        }

                        if (change.backgroundAudio != null)
                        {
                            if (!backgroundAudioSource.isPlaying || backgroundAudioSource.clip != change.backgroundAudio)
                            {
                                backgroundAudioSource.clip = change.backgroundAudio;
                                backgroundAudioSource.Play();
                                backgroundAudioSource.loop = true;
                            }
                        }

                        if (change.changeLights)
                        {
                            foreach (Light light in FindObjectsOfType<Light>())
                            {
                                light.color = change.lightsColour;
                                light.intensity = change.lightIntensity;
                            }

                            //activeLightsList.Clear();

                            //foreach(Light light in change.lightsList)
                            //{
                            //    activeLightsList.Add(light);
                            //    light.enabled = true;
                            //}
                        }

                        narratorSinDialogue.dialogue = "A sense of " + playerInfo.playerStats.highestStat.statName + " washes over you";

                        if (!dialogueSystem.enabled)
                        {
                            dialogueInitiator.BeginSubtitleSequence(narrator, narratorSinDialogue);
                        }

                        //if (change.changeColourFilter)
                        //{

                        //}
                        Debug.Log("environment changed");
                    }
                }
            }
        }
    }
}


[System.Serializable]
public struct EnvironmentalChangesPerStat
{
    public string statName;

    public List<EnvironmentalChanges> environmentalChangesList;
}

[System.Serializable]
public struct EnvironmentalChanges
{
    public float requiredValue;

    public List<GameObject> paintingsToChange;

    public List<Material> paintingTextures;
    //public Sprite wallpaperTexture;
    public List<GameObject> props;

    public AudioClip backgroundAudio;

    public bool changeLights;
    public Color lightsColour;
    public float lightIntensity;

    //public bool changeColourFilter;
    //public Color postProcessingColourFilter;
}