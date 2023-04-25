using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookSinTimer : MonoBehaviour
{
    public List<StatContainer.Stat> relatedSin;
    public float lookTimer;
    float statIncreaseMultiplier = 0.01f;

    public bool isLooking;

    private PlayerInfoController playerInfoController;
    private PlayerInteractionRaycast playerInteractionRaycast;

    private void Start()
    {
        playerInfoController = FindObjectOfType<PlayerInfoController>();
        playerInteractionRaycast = FindObjectOfType<PlayerInteractionRaycast>();
    }

    public void Update()
    {
        if (isLooking)
        {
            lookTimer += Time.deltaTime * statIncreaseMultiplier;
            relatedSin[0].statValue = lookTimer;

            //playerInfoController.AffectStatValues(relatedSin);

            if (playerInteractionRaycast.selectedObject != this)
            {
                isLooking = false;
            }
        }
        else
        {
            if (lookTimer > 0)
            {
                lookTimer = 0;
            }
        }
    }
}
