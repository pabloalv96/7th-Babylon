using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//NPCs always stare at the player
    //(except pride)

//raycast to to player
//Comment when they see the player do stuff
public class WatchPlayer : MonoBehaviour
{
    private Transform player;

    private DialogueInitiator dialogueInitiator;
    private DialogueListSystem dialogueListSystem;

    //public bool watchPlayer;

    public List<ResponseToPlayerActions> responseToPlayerActionsList;

    public float viewDistance = 10, viewRadius = 25;

    private PlayerInteractionRaycast playerInteractionRaycast;

    private NPCBrain brain;
    public Transform eyes;

    private int playerLayer;

    void Start()
    {
        brain = GetComponent<NPCBrain>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        dialogueInitiator = FindObjectOfType<DialogueInitiator>();
        playerInteractionRaycast = FindObjectOfType<PlayerInteractionRaycast>();
        dialogueListSystem = FindObjectOfType<DialogueListSystem>();
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

        int notPlayerLayer = ~playerLayer;

        if (Physics.Raycast(eyes.position, transform.forward, out hit, Mathf.Infinity/*, playerLayer*/))
        {
            if (hit.collider.CompareTag("Wall") || hit.collider.CompareTag("Untagged"))
            {
                Debug.DrawRay(eyes.position, transform.TransformDirection(Vector3.forward) * Vector3.Distance(transform.position, hit.point), Color.blue);
                //eyes.GetComponent<MeshRenderer>().material.color = Color.cyan;
            }
            else if (hit.collider.CompareTag("Player"))
            {
                Debug.DrawRay(eyes.position, transform.TransformDirection(Vector3.forward) * Vector3.Distance(transform.position, hit.point), Color.red);
                //eyes.GetComponent<MeshRenderer>().material.color = Color.magenta;

                JudgePlayerActions();
            }
        }

    }


    public void JudgePlayerActions()
    {
        if (playerInteractionRaycast.isItemInteracted)
        {

            foreach (ResponseToPlayerActions responseToPlayerActions in responseToPlayerActionsList)
            {
                if (responseToPlayerActions.playerAction == PlayerAction.steal)
                {
                    if (!dialogueListSystem.enabled)
                    {
                        dialogueInitiator.BeginSubtitleSequence(brain.npcInfo, responseToPlayerActions.npcDialogueResponses[Random.Range(0, responseToPlayerActions.npcDialogueResponses.Count)]);
                    }
                    Debug.Log("Steal reaction dialogue");
                    playerInteractionRaycast.isItemInteracted = false;
                }
            }
        }

        //if (playerInteractionRaycast.isLookSinInteracted)
        //{
        //    foreach (ResponseToPlayerActions responseToPlayerActions in responseToPlayerActionsList)
        //    {
        //        if (responseToPlayerActions.playerAction == PlayerAction.lookSin)
        //        {
        //            dialogueInitiator.BeginSubtitleSequence(brain.npcInfo, responseToPlayerActions.npcDialogueResponses[Random.Range(0, responseToPlayerActions.npcDialogueResponses.Count)]);
        //        }
        //    }
        //}

        if (playerInteractionRaycast.isConsumableInteracted)
        {
            foreach (ResponseToPlayerActions responseToPlayerActions in responseToPlayerActionsList)
            {
                if (responseToPlayerActions.playerAction == PlayerAction.consume)
                {
                    if (!dialogueListSystem.enabled)
                    {
                        dialogueInitiator.BeginSubtitleSequence(brain.npcInfo, responseToPlayerActions.npcDialogueResponses[Random.Range(0, responseToPlayerActions.npcDialogueResponses.Count)]);
                    }
                    Debug.Log("Consume reaction dialogue");
                    playerInteractionRaycast.isConsumableInteracted = false;

                }
            }
        }

        if (playerInteractionRaycast.isBreakableInteracted)
        {
            foreach (ResponseToPlayerActions responseToPlayerActions in responseToPlayerActionsList)
            {
                if (responseToPlayerActions.playerAction == PlayerAction.destroy)
                {
                    if (!dialogueListSystem.enabled)
                    {
                        dialogueInitiator.BeginSubtitleSequence(brain.npcInfo, responseToPlayerActions.npcDialogueResponses[Random.Range(0, responseToPlayerActions.npcDialogueResponses.Count)]);
                    }
                    Debug.Log("Break reaction dialogue");
                    playerInteractionRaycast.isBreakableInteracted = false;

                }
            }
        }
    }

    [System.Serializable]
    public enum PlayerAction { steal, /*lookSin,*/ consume, destroy }

    [System.Serializable]
    public class ResponseToPlayerActions
    {
        public PlayerAction playerAction;

        public List<NPCDialogueOption> npcDialogueResponses;
    }
}
