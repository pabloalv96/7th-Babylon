using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakObjectsQuest : MonoBehaviour
{
    public OJQuest breakObjectsQuest;
    public OJQuestManager questManager;

    public PlayerInteractionRaycast interactionRaycast;


    public float itemsBrokenCount, requiredBrokenCount;
    private void Update()
    {
        //if (interactionRaycast.isBreakableInteracted && Input.GetKeyDown(interactionRaycast.breakInput))
        //{

        //     itemsBrokenCount ++;
            

        //}

        if (breakObjectsQuest.questStarted)
        {
            if (itemsBrokenCount >= requiredBrokenCount)
            {
                questManager.EndQuest(breakObjectsQuest);
            }
        }
    }
}
