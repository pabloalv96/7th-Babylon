using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//NPCs always stare at the player
    //(except pride)

//raycast to to player
//Comment when they see the player do stuff
public class WatchPlayer : MonoBehaviour
{
    public Transform player;

    public DialogueInitiator dialogueInitiator;

    public List<ResponseToPlayerActions> responseToPlayerActionsList;

    public float viewDistance = 10, viewRadius = 25;

    public PlayerInteractionRaycast playerInteractionRaycast;

    private NPCBrain brain;
    public Transform eyes;

    private int playerLayer;

    void Start()
    {
        brain = GetComponent<NPCBrain>();
    }

    void Update()
    {
        LookAtPlayer();


    }

    public void LookAtPlayer()
    {
        Vector3 targetPostition = new Vector3(player.position.x,
                                       transform.position.y,
                                       player.position.z);
        transform.LookAt(targetPostition);

        RaycastHit hit;
        playerLayer = 1 << 7;

        if (Physics.Raycast(eyes.position, transform.forward, out hit, Mathf.Infinity, playerLayer))
        {

            if (hit.transform.CompareTag("Player"))
            {
                Debug.DrawRay(eyes.position, transform.TransformDirection(Vector3.forward) * 1000, Color.magenta);
                eyes.GetComponent<MeshRenderer>().material.color = Color.red;

                JudgePlayerActions();
            }
           
        }
        else
        {
            Debug.DrawRay(eyes.position, transform.TransformDirection(Vector3.forward) * 1000, Color.blue);
            eyes.GetComponent<MeshRenderer>().material.color = Color.cyan;
        }
    }

    public void JudgePlayerActions()
    {
        if (playerInteractionRaycast.isItemInteracted)
        {
            foreach(ResponseToPlayerActions responseToPlayerActions in responseToPlayerActionsList)
            {
                if (responseToPlayerActions.playerAction == PlayerAction.steal)
                {
                    dialogueInitiator.BeginSubtitleSequence(brain.npcInfo, responseToPlayerActions.npcDialogueResponse);
                }
            }
        }

        if (playerInteractionRaycast.isLookSinInteracted)
        {
            foreach (ResponseToPlayerActions responseToPlayerActions in responseToPlayerActionsList)
            {
                if (responseToPlayerActions.playerAction == PlayerAction.lookSin)
                {
                    dialogueInitiator.BeginSubtitleSequence(brain.npcInfo, responseToPlayerActions.npcDialogueResponse);
                }
            }
        }

        if (playerInteractionRaycast.isConsumableInteracted)
        {
            foreach (ResponseToPlayerActions responseToPlayerActions in responseToPlayerActionsList)
            {
                if (responseToPlayerActions.playerAction == PlayerAction.consume)
                {
                    dialogueInitiator.BeginSubtitleSequence(brain.npcInfo, responseToPlayerActions.npcDialogueResponse);
                }
            }
        }

        if (playerInteractionRaycast.isBreakableInteracted)
        {
            foreach (ResponseToPlayerActions responseToPlayerActions in responseToPlayerActionsList)
            {
                if (responseToPlayerActions.playerAction == PlayerAction.destroy)
                {
                    dialogueInitiator.BeginSubtitleSequence(brain.npcInfo, responseToPlayerActions.npcDialogueResponse);
                }
            }
        }
    }

    [System.Serializable]
    public enum PlayerAction { steal, lookSin, consume, destroy }

    [System.Serializable]
    public class ResponseToPlayerActions
    {
        public PlayerAction playerAction;

        public NPCDialogueOption npcDialogueResponse;
    }
}
