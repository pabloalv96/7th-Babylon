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
        if (questManager.activeQuestList.Contains(breakObjectsQuest))
        {
            if (interactionRaycast.isBreakable && Input.GetKeyDown(interactionRaycast.consumeInput))
            {
                if (itemsBrokenCount < requiredBrokenCount)
                {
                    itemsBrokenCount += 1;
                }
                
            }
        }

        if (itemsBrokenCount >= requiredBrokenCount)
        {
            questManager.EndQuest(breakObjectsQuest);
        }
    }
}
