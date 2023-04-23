using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[System.Serializable]
public class PlayerDialogue : MonoBehaviour
{
    //public List<PlayerDialogueOption> greetingDialogue;

    public List<PlayerDialogueOption> goodbyeDialogue;

    public List<PlayerDialogueOption> changeTopicDialogue;

    //public PlayerDialogueOption viewMoreDialogue;

    public PlayerDialogueOption continueDialogue;
    public PlayerDialogueOption nothingToSayDialogue;

    public NPCDialogueOption questions;

    private DialogueListSystem dialogueSystem;
    private Inventory inventorySystem;

    [Serializable]
    public class PlayerQuestions
    {
        public NPCInfo npc;
        public List<PlayerDialogueOption> questionsForNPC;
    }


    // npc & respective dialogue options set at runtime
    public List<PlayerQuestions> playerQuestions;

    private void Awake()
    {
        dialogueSystem = FindObjectOfType<DialogueListSystem>();
        inventorySystem = FindObjectOfType<Inventory>();
    }


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

                if (playerQuestions[i].questionsForNPC == null)
                {
                    playerQuestions[i].questionsForNPC = new List<PlayerDialogueOption>();
                }

                playerQuestions[i].questionsForNPC.Add(newDialogueOption);
            }
        }
    }

    public void AddDialogueOptions()
    {
        for (int i = 0; i < playerQuestions.Count; i++)
        {
            // find the player dialogue for the selected npc
            if (playerQuestions[i].npc == dialogueSystem.npc) 
            {
                // reset the npc's dialogue options
                playerQuestions[i].questionsForNPC = new List<PlayerDialogueOption>(); 

                // for all of this npc's dialogue options
                for (int d = 0; d < playerQuestions[i].npc.npcDialogue.dialogueConnections.Count; d++) 
                {
                    // check whether it is a response to player dialogue
                    if (playerQuestions[i].npc.npcDialogue.dialogueConnections[d].playerDialogueInput != null) 
                    {
                        // if it is, check whether the player is responding to the npc
                        if (!playerQuestions[i].npc.npcDialogue.dialogueConnections[d].playerDialogueInput.isResponseToNPCDialogue)
                        {
                            // if they aren't responding, check whether it's dialogue for a quest
                            if (playerQuestions[i].npc.npcDialogue.dialogueConnections[d].playerDialogueInput.isQuestDialogue)
                            {
                                foreach(OJQuest quest in playerQuestions[i].npc.npcDialogue.dialogueConnections[d].playerDialogueInput.relatedQuests)
                                {
                                    if (quest.questStarted && !quest.questEnded)
                                    {
                                        if (quest.objective.objectiveType == OJQuestObjectiveType.itemBased)
                                        {
                                            foreach (OJQuestItemObjective questItem in quest.objective.questItems)
                                            {
                                                if (inventorySystem.CheckInventoryForItem(questItem.item) && inventorySystem.CheckItemCount(questItem.item) >= questItem.requiredAmount)
                                                {
                                                    playerQuestions[i].questionsForNPC.Add(playerQuestions[i].npc.npcDialogue.dialogueConnections[d].playerDialogueInput);

                                                }

                                            }
                                        }
                                        else if (quest.objective.objectiveType == OJQuestObjectiveType.dialogueBased)
                                        {
                                            playerQuestions[i].questionsForNPC.Add(playerQuestions[i].npc.npcDialogue.dialogueConnections[d].playerDialogueInput);

                                        }
                                    }
                                }
                            }
                            else
                            {
                                // add it to the player's current dialogue options
                                playerQuestions[i].questionsForNPC.Add(playerQuestions[i].npc.npcDialogue.dialogueConnections[d].playerDialogueInput);
                            }
                        }
                        else
                        {
                            //if they are, remove it from the player's dialogue options
                            playerQuestions[i].questionsForNPC.Remove(playerQuestions[i].npc.npcDialogue.dialogueConnections[d].playerDialogueInput);
                        }
                    }
                    else // if there is nothing for the player to say to the npc
                    {
                        AddResponseOptions();


                        //// for all of the npc's dialogue options
                        //for (int r = 0; r < playerQuestions[i].npc.npcDialogue.dialogueConnections[d].npcResponses.Count; r++)
                        //{ 
                        //    // if the npc has dialogue which requires the player to respond
                        //    if (playerQuestions[i].npc.npcDialogue.dialogueConnections[d].npcResponses[r].response.requiresResponse)
                        //    {
                        //        //dialogueInitiator.NPCInitiatedDialogue(playerQuestions[i].npc, playerQuestions[i].npc.npcDialogue.dialogueConnections[d].npcResponses[r].response);

                        //        AddResponseOptions();
                        //    }
                        //}
                    }
                }
            }
        } 
    }

    public void AddResponseOptions()
    {
        for (int i = 0; i < playerQuestions.Count; i++)
        {

            if (playerQuestions[i].npc == dialogueSystem.npc) // check that you are only affecting the current npc
            {
                playerQuestions[i].questionsForNPC = new List<PlayerDialogueOption>(); // reset their dialogue options

                for (int d = 0; d < playerQuestions[i].npc.npcDialogue.dialogueConnections.Count; d++) //for each npc dialogue object of each npc
                {
                    if (playerQuestions[i].npc.npcDialogue.dialogueConnections[d].playerDialogueInput == null || playerQuestions[i].npc.npcDialogue.dialogueConnections[d].playerDialogueInput.isResponseToNPCDialogue) //check that the player is responding
                    {
                        if (dialogueSystem.npcDialogue.playerResponses.Count > 0) 
                        {
                            foreach (PlayerDialogueOption response in dialogueSystem.npcDialogue.playerResponses)
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
            if (dialogueOption.relatedQuests == null)
            {
                question.questionsForNPC.Remove(dialogueOption);
                dialogueOption.isLocked = true;
            }
        }
    }

    public void RemoveDialogueForSpecificNPC(PlayerDialogueOption dialogueOption, NPCInfo npc)
    {
        for (int i = 0; i < playerQuestions.Count; i++)
        {
            if (playerQuestions[i].npc == npc && dialogueOption.relatedQuests == null)
            {
                playerQuestions[i].questionsForNPC.Remove(dialogueOption);
                dialogueOption.isLocked = true;
            }
        }
    }


}
