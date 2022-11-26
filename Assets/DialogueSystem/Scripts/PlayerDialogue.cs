using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[System.Serializable]
public class PlayerDialogue : MonoBehaviour
{
    public List<PlayerDialogueOption> greetingDialogue;

    public List<PlayerDialogueOption> goodbyeDialogue;

    public List<PlayerDialogueOption> changeTopicDialogue;

    //public PlayerDialogueOption viewMoreDialogue;

    public PlayerDialogueOption continueDialogue;
    public PlayerDialogueOption nothingToSayDialogue;

    public NPCDialogueOption questions;

    [Serializable]
    public class PlayerQuestions
    {
        public NPCInfo npc;
        public List<PlayerDialogueOption> questionsForNPC;
    }


    // npc & respective dialogue options set at runtime
    public List<PlayerQuestions> playerQuestions;


    //Set the questions the player can ask the npc for specified NPC Dialogue Option
    public NPCDialogueOption SetPlayerDialogueBasedOnCurrentNPCAndDialogue(NPCInfo npc, NPCDialogueOption npcDialogue)
    {
        Debug.Log("Set Dialogue: " + npcDialogue.name + " for " + npc.npcName);
        //create a new dialogue option to hold all the questions the player can ask the npc
        questions = ScriptableObject.CreateInstance<NPCDialogueOption>();

        questions.dialogue = npcDialogue.dialogue;

        //enable the player to choose a dialogue option
        questions.requiresResponse = true;

        if (playerQuestions.Count > 0)
        {

            for (int i = 0; i < playerQuestions.Count; i++)
            {
                //Set appropriate dialogue options
                if (playerQuestions[i].npc == npc)
                {
                    questions.playerResponses = playerQuestions[i].questionsForNPC;
                }
            }
        }
        //else
        //{
        //    foreach (NPCDialogueOption dialogue in npc.npcDialogue.greetingDialogue)
        //    {
        //        dialogue.requiresResponse = false;
        //        dialogue.continuedDialogue = npc.npcDialogue.nothingToSayDialogue;
        //        dialogue.playerCanChangeTopic = false;
        //    }
        //}

        return questions;
    }

    //Add new dialogue option to playerQuestions for all npc's
    public void AddQuestionForAllNPCs(PlayerDialogueOption newDialogueOption)
    {
        foreach (PlayerQuestions question in playerQuestions)
        {
            question.questionsForNPC.Add(newDialogueOption);
        }
    }

    //Add new dialogue option to playerQuestions for specific npc
    public void AddQuestionForSpecificNPC(PlayerDialogueOption newDialogueOption, NPCInfo npc)
    {
        for (int i = 0; i < playerQuestions.Count; i++)
        {
            if (playerQuestions[i].npc == npc)
            {
                if (newDialogueOption.isLocked)
                {
                    newDialogueOption.isLocked = false;
                }
                playerQuestions[i].questionsForNPC.Add(newDialogueOption);
            }
        }
    }

    public void AddDialogueOptions()
    {

        for (int i = 0; i < playerQuestions.Count; i++)
        {
            if (playerQuestions[i].npc == FindObjectOfType<DialogueListSystem>().npc) // check that you are only getting dialogue for the current npc
            {
                playerQuestions[i].questionsForNPC = new List<PlayerDialogueOption>(); // reset their dialogue options

                for (int d = 0; d < playerQuestions[i].npc.npcDialogue.dialogueConnections.Count; d++) //for each npc dialogue object of each npc
                {
                    if (playerQuestions[i].npc.npcDialogue.dialogueConnections[d].playerDialogueInput != null)
                    {
                        if (!playerQuestions[i].npc.npcDialogue.dialogueConnections[d].playerDialogueInput.isResponseToNPCDialogue) // check the dialogue object for if the player is responding or is for a quest
                        {
                            if (playerQuestions[i].npc.npcDialogue.dialogueConnections[d].playerDialogueInput.isQuestDialogue && FindObjectOfType<OJQuestManager>().activeQuestList.Contains(playerQuestions[i].npc.npcDialogue.dialogueConnections[d].playerDialogueInput.relatedQuest))
                            {
                                playerQuestions[i].questionsForNPC.Add(playerQuestions[i].npc.npcDialogue.dialogueConnections[d].playerDialogueInput); //if not add it to the player's current dialogue selection
                            }
                            else if (!playerQuestions[i].npc.npcDialogue.dialogueConnections[d].playerDialogueInput.isQuestDialogue)
                            {
                                playerQuestions[i].questionsForNPC.Add(playerQuestions[i].npc.npcDialogue.dialogueConnections[d].playerDialogueInput); //if not add it to the player's current dialogue selection

                            }
                            else if (playerQuestions[i].npc.npcDialogue.dialogueConnections[d].playerDialogueInput.isQuestDialogue && !FindObjectOfType<OJQuestManager>().activeQuestList.Contains(playerQuestions[i].npc.npcDialogue.dialogueConnections[d].playerDialogueInput.relatedQuest))
                            {
                                if (playerQuestions[i].npc.npcDialogue.dialogueConnections[0].playerDialogueInput == null && playerQuestions[i].npc.npcDialogue.dialogueConnections[0].npcResponses[0].response.requiresResponse)
                                {
                                    FindObjectOfType<StartDialogue>().NPCInitiatedDialogue(playerQuestions[i].npc, playerQuestions[i].npc.npcDialogue.dialogueConnections[0].npcResponses[0].response);

                                    AddResponseOptions();
                                }
                            }
                        }
                        else
                        {
                            playerQuestions[i].questionsForNPC.Remove(playerQuestions[i].npc.npcDialogue.dialogueConnections[d].playerDialogueInput); //otherwise remove it
                        }
                    }
                    else // if the player has nothing to start with, let the npc say something first
                    {
                        if (playerQuestions[i].npc.npcDialogue.dialogueConnections[0].playerDialogueInput == null && playerQuestions[i].npc.npcDialogue.dialogueConnections[0].npcResponses[0].response.requiresResponse)
                        {
                            FindObjectOfType<StartDialogue>().NPCInitiatedDialogue(playerQuestions[i].npc, playerQuestions[i].npc.npcDialogue.dialogueConnections[0].npcResponses[0].response);

                            AddResponseOptions();
                        }
                    }

                    //Debug.Log("Dialogue Option '" + playerQuestions[i].npc.npcDialogue.dialogueConnections[d].playerDialogueInput.name + "' has been added to questions for '" + playerQuestions[i].npc.name + "'");
                }
            }
        }
        
    }

    public void AddResponseOptions()
    {
        for (int i = 0; i < playerQuestions.Count; i++)
        {

            if (playerQuestions[i].npc == FindObjectOfType<DialogueListSystem>().npc) // check that you are only affecting the current npc
            {
                playerQuestions[i].questionsForNPC = new List<PlayerDialogueOption>(); // reset their dialogue options

                for (int d = 0; d < playerQuestions[i].npc.npcDialogue.dialogueConnections.Count; d++) //for each npc dialogue object of each npc
                {
                    if (playerQuestions[i].npc.npcDialogue.dialogueConnections[d].playerDialogueInput == null || playerQuestions[i].npc.npcDialogue.dialogueConnections[d].playerDialogueInput.isResponseToNPCDialogue) //check that the player is responding
                    {
                        if (FindObjectOfType<DialogueListSystem>().npcDialogue.playerResponses.Count > 0) 
                        {
                            foreach (PlayerDialogueOption response in FindObjectOfType<DialogueListSystem>().npcDialogue.playerResponses)
                            {
                                if (!playerQuestions[i].questionsForNPC.Contains(response))
                                {
                                    playerQuestions[i].questionsForNPC.Add(response);
                                }
                            }
                        }
                    }

                    //Debug.Log("Dialogue Option '" + playerQuestions[i].npc.npcDialogue.dialogueConnections[d].playerDialogueInput.name + "' has been added to questions for '" + playerQuestions[i].npc.name + "'");
                }
            }
        }
    }

    public void RemoveDialogueForAllNPCs(PlayerDialogueOption dialogueOption)
    {
        foreach (PlayerQuestions question in playerQuestions)
        {
            question.questionsForNPC.Remove(dialogueOption);
            dialogueOption.isLocked = true;
        }
    }

    public void RemoveDialogueForSpecificNPC(PlayerDialogueOption dialogueOption, NPCInfo npc)
    {
        for (int i = 0; i < playerQuestions.Count; i++)
        {
            if (playerQuestions[i].npc == npc)
            {
                playerQuestions[i].questionsForNPC.Remove(dialogueOption);
                dialogueOption.isLocked = true;
            }
        }
    }

}
