using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueInitiator : MonoBehaviour
{

    private DialogueListSystem dialogueSystem;
    //private DialogueInitiator dialogueInitiator;
    private OJQuestManager questManager;
    //private Inventory inventorySystem;
    private PlayerDialogue playerDialogue;
    //private PlayerInfoController playerInfoController;
    private PlayerInteractionRaycast playerInteractionRaycast;

    private Transform player;


    private void Awake()
    {
        dialogueSystem = FindObjectOfType<DialogueListSystem>();
        //dialogueInitiator = FindObjectOfType<DialogueInitiator>();
        questManager = FindObjectOfType<OJQuestManager>();
        //inventorySystem = FindObjectOfType<Inventory>();
        playerDialogue = FindObjectOfType<PlayerDialogue>();
        //playerInfoController = FindObjectOfType<PlayerInfoController>();
        playerInteractionRaycast = FindObjectOfType<PlayerInteractionRaycast>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    public void EnterDialogue( NPCInfo npcInfo)
    {
        //Set NPC
        //Set greeting text
        //Set player options
        if (dialogueSystem.npcDialogue != null && dialogueSystem.npcDialogue.toOtherNPC)
        {
            dialogueSystem.pausedSubtitleDialogue = dialogueSystem.npcDialogue;
            dialogueSystem.pausedSubtitleNPC = dialogueSystem.npc;

            //dialogueSystem.LeaveDialogue();
        }

        dialogueSystem.inDialogue = true;

        dialogueSystem.enabled = true;

        dialogueSystem.npc = npcInfo;
        //dialogueSystem.npc.npcEmotions.SetMood();
        dialogueSystem.npcNameText.text = npcInfo.npcName;


        if (playerInteractionRaycast.selectedObject.GetComponent<NPCBrain>() && playerInteractionRaycast.selectedObject.GetComponent<NPCBrain>().npcInfo == npcInfo)
        {
           

            NPCBrain currentNPC = playerInteractionRaycast.selectedObject.GetComponent<NPCBrain>();

            //currentNPC.gameObject.transform.LookAt(player.transform);

            Vector3 targetPostition = new Vector3(player.position.x,
                                       currentNPC.transform.position.y,
                                       player.position.z);
            currentNPC.transform.LookAt(targetPostition);

            List<NPCDialogueOption> usedDialogue = new List<NPCDialogueOption>();
            List<PlayerDialogueOption> playerDialogueChoices = new List<PlayerDialogueOption>();

            foreach (NPCBrain.DialogueMemory dialogueMemory in currentNPC.dialogueMemories)
            {
                usedDialogue.Add(dialogueMemory.npcUsedDialogue);

                playerDialogueChoices.Add(dialogueMemory.playerResponse);
            }

            if (currentNPC.startingDialogue != null && (!usedDialogue.Contains(currentNPC.startingDialogue) || currentNPC.startingDialogue.isRepeatable))
            {
                NPCInitiatedDialogue(npcInfo, currentNPC.startingDialogue);
                return;
            }
        }

        NPCDialogueOption greetingDialogue = npcInfo.npcDialogue.greetingDialogue[Random.Range(0, npcInfo.npcDialogue.greetingDialogue.Count)];

        //foreach(OJQuest quest in questManager.activeQuestList)
        //{
        //    if (quest.objective.objectiveType == OJQuestObjectiveType.dialogueBased && quest.objective.isItemDialogue)
        //    {

        //    }
        //}

        if (npcInfo.npcDialogue.dialogueConnections.Count > 0 /*&& !greetingDialogue.isNPCInitatied*/) //if the player has stuff to say to the npc
        {
            playerDialogue.AddDialogueOptions();
        }

        greetingDialogue.playerResponses = playerDialogue.SetPlayerDialogueBasedOnCurrentNPCAndDialogue(npcInfo, greetingDialogue).playerResponses;


        //Deactivate Player Controller
        FindObjectOfType<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = false;



        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        //Debug.Log("Cursor Mode Confined");

        dialogueSystem.playerIsLeading = true;

        dialogueSystem.SetNewDialogueText(greetingDialogue);
        dialogueSystem.BeginDialogue();


    }

    public void NPCInitiatedDialogue(NPCInfo npc, NPCDialogueOption startingDialogue)
    {
        //if (dialogueSystem.npcDialogue != null && dialogueSystem.npcDialogue.toOtherNPC)
        //{
        //    dialogueSystem.pausedSubtitleDialogue = dialogueSystem.npcDialogue;
        //    dialogueSystem.pausedSubtitleNPC = dialogueSystem.npc;

        //    //dialogueSystem.LeaveDialogue();
        //}

        if (startingDialogue.toOtherNPC)
        {
            BeginSubtitleSequence(npc, startingDialogue);
        }
        else
        {
            //Set NPC
            //Set greeting text
            //Set player options
            
            dialogueSystem.inDialogue = true;

            dialogueSystem.enabled = true;


            dialogueSystem.npc = npc;

            //dialogueSystem.npc.npcEmotions.SetMood();
            dialogueSystem.npcNameText.text = npc.npcName;


            dialogueSystem.playerMovement.enabled = false;

            //Lock Camera to NPC target

            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;

            //playerDialogue.AddResponseOptions();



            dialogueSystem.playerIsLeading = false;

            dialogueSystem.SetNewDialogueText(startingDialogue);
            dialogueSystem.BeginDialogue();
        }
        
    }

    public void BeginSubtitleSequence(NPCInfo npc, NPCDialogueOption startingDialogue)
    {
       
        if (dialogueSystem.npcDialogue != null && dialogueSystem.npcDialogue.toOtherNPC)
        {
            dialogueSystem.pausedSubtitleDialogue = dialogueSystem.npcDialogue;
            dialogueSystem.pausedSubtitleNPC = dialogueSystem.npc;

            //dialogueSystem.LeaveDialogue();
        }
        

        dialogueSystem.enabled = true;

        dialogueSystem.SetNewDialogueText(startingDialogue);

        dialogueSystem.inDialogue = false;

        dialogueSystem.npc = npc;

        dialogueSystem.npcNameText.text = npc.npcName;

        dialogueSystem.playerMovement.enabled = true;


        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;

        dialogueSystem.BeginDialogue();

    }

    public void EnterDialogueWithRandomNPC()
    {
        EnterDialogue(GetRandomNPC());
    }

    public NPCInfo GetRandomNPC()
    {
        int rand = Random.Range(0, playerDialogue.playerQuestions.Count);

        NPCInfo npc = playerDialogue.playerQuestions[rand].npc;

        return npc;
    }

    public void TriggerNPCDialogue(NPCBrain npc)
    {
        if (npc.startingDialogue.toOtherNPC)
        {
            BeginSubtitleSequence(npc.npcInfo, npc.startingDialogue);
            npc.isSpeakingToPlayer = true;
        }
        else
        {
            NPCInitiatedDialogue(npc.npcInfo, npc.startingDialogue);
            npc.isSpeakingToPlayer = false;
        }
    }

    public void ResetStartingDialogue(NPCBrain npc)
    {
        npc.startingDialogue = null;
    }

    public void DisableObjectAfterDialogue(GameObject prefab)
    {
        GameObject.Find(prefab.name).SetActive(false);
    }


}
