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
    public GameObject paintingObject;
    public Material defaultPaintingMaterial;

    public AudioSource backgroundAudioSource;

    //public Color lightsColour;
    public List<Light> activeLightsList;

    //public Volume processVolume;

    public bool checkHighestStat;

    //Check highest stat's value
    //determine what assets to set

    private void Start()
    {
        paintingObject.GetComponent<MeshRenderer>().material = defaultPaintingMaterial;
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

        SetEnvironmentalChanges();

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
                        if (change.paintingTexture != null)
                        {
                            paintingObject.GetComponent<MeshRenderer>().material = change.paintingTexture;
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

                        //if (change.changeColourFilter)
                        //{

                        //}
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

    public Material paintingTexture;
    //public Sprite wallpaperTexture;
    public List<GameObject> props;

    public AudioClip backgroundAudio;

    public bool changeLights;
    public Color lightsColour;
    public float lightIntensity;

    //public bool changeColourFilter;
    //public Color postProcessingColourFilter;
}