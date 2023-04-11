using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using UnityStandardAssets.Characters.FirstPerson;

public class DialogueListSystem : MonoBehaviour
{

    public FirstPersonController playerMovement;
    //public FirstPersonCam playerCam;

    [SerializeField] private GameObject playerDialoguePrefab;
    [SerializeField] private GameObject listDialoguePanel;

    public bool inDialogue;
    public bool playerIsLeading;

    public NPCInfo npc;

    //Dialogue to display in npc text box
    public NPCDialogueOption npcDialogue;
    public NPCDialogueOption pausedSubtitleDialogue;
    public NPCInfo pausedSubtitleNPC;

    //Dialogue to display in the player text box
    public PlayerDialogueOption selectedDialogueOption;

    //Dialogue UI
    public TextMeshProUGUI npcNameText;
    //public TextMeshProUGUI npcMoodText;
    [SerializeField] private TextMeshProUGUI npcDialogueText;
    //public TextMeshProUGUI playerDialogueText;

    //public GameObject npcSubtitlePanel;

    //public bool playerResponseLockedIn;

    public GameObject continueButton, leaveButton, changeTopicButton;


    //Response Timer Variables
    public bool responseTimerActive;
    public float responseTimer = 5f;
    private float responseTimerReset;

    [SerializeField] private Slider responseTimerUI;   // if true the timer will automatically start during a time-limited response and pick a random option if the player doesn't begin viewing the dialogue options
                                                       // if false the timer won't start until the player has begun viewing the dialogue options
                                                       //Default player dialogue
    [SerializeField] private GameObject dialogueUI;

    //private DialogueListSystem dialogueSystem;
    //private DialogueInitiator dialogueInitiator;
    [SerializeField] private OJQuestManager questManager;
    [SerializeField] public Inventory inventorySystem;
    private PlayerDialogue playerDialogue;
    private PlayerInfoController playerInfoController;
    private PlayerInteractionRaycast playerInteractionRaycast;

    public AudioSource audioSource;
    private void Start()
    {
        //dialogueSystem = FindObjectOfType<DialogueListSystem>();
        //dialogueInitiator = FindObjectOfType<DialogueInitiator>();
        //questManager = FindObjectOfType<OJQuestManager>();
        //inventorySystem = FindObjectOfType<Inventory>();
        playerDialogue = FindObjectOfType<PlayerDialogue>();
        playerInfoController = FindObjectOfType<PlayerInfoController>();
        playerInteractionRaycast = FindObjectOfType<PlayerInteractionRaycast>();



        AddNPCsToPlayerDialogue();
    }

    private void Update()
    {
        if (inDialogue)
        {
            if (npcDialogue.requiresResponse)
            {
                if (responseTimerActive)
                {
                    responseTimer -= Time.deltaTime;
                    responseTimerUI.value = responseTimer;

                    if (responseTimer <= 0f)
                    {
                        // select a random option if the player hasn't selected in time
                        selectedDialogueOption = listDialoguePanel.transform.GetChild(Random.Range(0, listDialoguePanel.transform.childCount)).GetComponent<DialogueListButton>().dialogueOption;
                        //playerDialogueText.text = selectedDialogueOption.dialogue;

                        LockInResponse();
                    }
                }
            }
            else if (npcDialogue.endOfConversation)
            {
                LeaveDialogue();
            }

            //if (listDialoguePanel.transform.childCount <= 0)
            //{
            //    CreateLeaveListOption();
            //}

        }
        else if (npcDialogue == null || !responseTimerActive || !npcDialogue.toOtherNPC)
        {
            Debug.Log("No Dialogue. Ending Conversation.");
            LeaveDialogue();
            
        }
        else
        {
            if (responseTimerActive && npcDialogue.toOtherNPC)
            {
                responseTimer -= Time.deltaTime;
                responseTimerUI.value = responseTimer;

                if (responseTimer <= 0f && npcDialogue.continuedDialogue != null)
                {
                    LockInResponse();
                }
            }
            else
            {
                Debug.Log("No Dialogue. Ending Conversation.");

                LeaveDialogue();
            }


        }
    }


    // Update UI Dialogue Text
    public void SetNewDialogueText(NPCDialogueOption npcDialogueOption)
    {
        if ((inDialogue || npcDialogueOption.toOtherNPC) && (npcDialogueOption.isRepeatable || !npcDialogueOption.isRepeatable && !npcDialogueOption.hasBeenSeen))
        {
            
            npcDialogueText.text = npcDialogueOption.dialogue;
            npcDialogue = npcDialogueOption;

            if (npcDialogue.conditionalEvents.Count > 0)
            {
                InvokeNPCConditonalEvents();
            }

            if (npcDialogue.statsToEffectList.Count > 0)
            {
                //selectedDialogueOption.AffectStatValues();
                playerInfoController.AffectStatValues(npcDialogue.statsToEffectList);
            }

            if (npcDialogueOption.audioClip != null)
            {
                if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                }

                audioSource.PlayOneShot(npcDialogueOption.audioClip);
            }

            if (npcDialogue.relatedQuests != null)
            {
                foreach (OJQuest quest in npcDialogue.relatedQuests)
                {
                    if (quest.objective.objectiveType != OJQuestObjectiveType.itemBased)
                    {
                        if (!quest.questStarted)
                        {
                            questManager.StartQuest(quest);
                        }
                        else if (!quest.questEnded)
                        {
                            if (quest.objective.objectiveType == OJQuestObjectiveType.dialogueBased)
                            {
                                foreach (OJQuestDialogue questDialogue in quest.objective.questDialogueOptions)
                                {
                                    if (questDialogue.questDialogueOption.completeRelatedQuest)
                                    {
                                        questManager.EndQuest(quest);
                                        Debug.Log("Ending Quest '" + quest.questID + "' from SetNewDialogueText");

                                    }
                                }
                            }
                            else if (quest.objective.objectiveType == OJQuestObjectiveType.locationBased)
                            {
                                questManager.EndQuest(quest);
                            }
                        }

                       
                    }
                    else
                    {
                        if (!quest.questStarted)
                        {
                            questManager.StartQuest(quest);
                        }
                        else
                        {
                            foreach (OJQuestItemObjective questItem in quest.objective.questItems)
                            {
                                if (!questItem.questCompleted)
                                {
                                    if (inventorySystem.CheckInventoryForItem(questItem.item) && inventorySystem.CheckItemCount(questItem.item) >= questItem.requiredAmount)
                                    {
                                        questItem.requiredAmountCollected = true;
                                        //foreach (OJQuestDialogue dialogue in quest.objective.questDialogueOptions)
                                        //{
                                        //    AddQuestItemDialogue(dialogue, questItem);


                                        //}
                                    }
                                    else
                                    {
                                        questItem.requiredAmountCollected = false;
                                        //foreach (OJQuestDialogue dialogue in quest.objective.questDialogueOptions)
                                        //{
                                        //    RemoveQuestItemDialogue(dialogue, questItem);

                                        //}
                                    }
                                }
                            }
                        }
                        //else if (quest.objective.questItem)
                        //{
                        //    questManager.EndQuest(quest);
                        //}

                        //if (quest.questStarted && !quest.questEnded)
                        //{
                        //    if (quest.objective.objectiveType == OJQuestObjectiveType.dialogueBased && quest.objective.isItemDialogue)
                        //    {
                        //        foreach (OJQuestDialogue questDialogue in quest.objective.questDialogueOptions)
                        //        {
                        //            foreach (OJQuestItemObjective questItemObjective in quest.objective.questItems)
                        //            {
                        //                if (inventorySystem.CheckItemCount(questItemObjective.item) >= questItemObjective.requiredAmount)
                        //                {
                        //                    questManager.AddQuestItemDialogue(quest.objective.questDialogueOptions[0], questItemObjective);
                        //                }
                        //            }
                        //        }
                        //    }
                        //}
                    }
                }
                
            }

            if (npcDialogue.newStartingDialogue != null)
            {
                //if (playerInteractionRaycast.selectedObject.GetComponent<NPCBrain>() && playerInteractionRaycast.selectedObject.GetComponent<NPCBrain>().npcInfo == npc)
                //{
                //    playerInteractionRaycast.selectedObject.GetComponent<NPCBrain>().startingDialogue = npcDialogue.newStartingDialogue;
                //}
                foreach (NPCBrain brain in FindObjectsOfType<NPCBrain>())
                {
                    if (brain.npcInfo == npc)
                    {
                        brain.startingDialogue = npcDialogue.newStartingDialogue;
                        break;
                    }
                }


            }

            if (npcDialogue.itemsToGive.Count > 0)
            {
                AddItemBasedOnNPCDialogue();
            }

            if (npcDialogue.itemsToTake.Count > 0)
            {
                RemoveItemBasedOnNPCDialogue();
            }

            DestroyOldDialogueOptions();

            if (!npcDialogue.requiresResponse || npcDialogue.toOtherNPC)
            {
                playerIsLeading = false;
            }
            else
            {
                if (npcDialogue.playerResponses.Count <= 0)
                {
                    for (int i = 0; i < playerDialogue.playerQuestions.Count; i++)
                    {
                        if (playerDialogue.playerQuestions[i].npc == npc)
                        {
                            if (playerDialogue.playerQuestions[i].questionsForNPC.Count > 0)
                            {
                                npcDialogue.playerResponses = playerDialogue.SetPlayerDialogueBasedOnCurrentNPCAndDialogue(npc, npcDialogue).playerResponses;
                            }
                        }
                    }
                }
            }

            SetResponseTimer();

            if (inDialogue)
            {
                CreateDialogueOptions(npcDialogue);
            }
        }
    }

    public void DestroyOldDialogueOptions()
    {
        for (int i = 0; i < listDialoguePanel.transform.childCount; i++)
        {
            if (i != 0 && i != 1 && i != 2)
            {
                Destroy(listDialoguePanel.transform.GetChild(i).gameObject);
            }
        }
    }

    public void CreateDialogueOptions(NPCDialogueOption npcDialogueOption)
    {
        DestroyOldDialogueOptions();

        if (!npcDialogue.toOtherNPC)
        {
            listDialoguePanel.SetActive(true);

            if (npcDialogue.requiresResponse)
            {
                foreach (PlayerDialogueOption dialogueOption in npcDialogueOption.playerResponses)
                {
                    if (dialogueOption == playerDialogue.continueDialogue)
                    {
                        return;
                    }

                    if (!dialogueOption.isLocked)
                    {
                        GameObject newDialogue = Instantiate(playerDialoguePrefab, listDialoguePanel.transform.position, Quaternion.identity);
                        newDialogue.GetComponentInChildren<TextMeshProUGUI>().text = dialogueOption.dialogue;
                        //newDialogue.GetComponentInChildren<TextMeshProUGUI>().fontSize = leaveButton.GetComponentInChildren<TextMeshProUGUI>().fontSize;
                        newDialogue.GetComponent<DialogueListButton>().dialogueOption = dialogueOption;
                        newDialogue.transform.SetParent(listDialoguePanel.transform);

                        if (dialogueOption.relatedQuests != null)
                        {
                            foreach (OJQuest relatedQuest in dialogueOption.relatedQuests)
                            {
                                if (questManager.activeQuestList.Contains(relatedQuest))
                                {
                                    if (relatedQuest.objective.objectiveType == OJQuestObjectiveType.itemBased)
                                    {
                                        foreach (OJQuestItemObjective questItem in relatedQuest.objective.questItems)
                                        {
                                            if (!questItem.requiredAmountCollected)
                                            {

                                                questItem.requiredAmountCollected = false;
                                                questManager.RemoveQuestItemDialogue(relatedQuest.objective.questDialogueOptions[0], questItem);


                                            }
                                            //else
                                            //{
                                            //    if (dialogueOption.completeRelatedQuest)
                                            //    {
                                            //        questManager.EndQuest(relatedQuest);
                                            //        Debug.Log("Ending Quest '" + relatedQuest.questID + "' from CreateDialogueOptions");

                                            //    }
                                            //}
                                        }
                                    }
                                    //else
                                    //{

                                    //    GameObject newQuestDialogue = Instantiate(playerDialoguePrefab, listDialoguePanel.transform.position, Quaternion.identity);
                                    //    newQuestDialogue.GetComponentInChildren<TextMeshProUGUI>().text = dialogueOption.dialogue;
                                    //    newQuestDialogue.GetComponent<DialogueListButton>().dialogueOption = dialogueOption;
                                    //    newQuestDialogue.transform.SetParent(listDialoguePanel.transform);
                                    //}
                                }

                               
                            }
                        }
                    }
                }

                continueButton.SetActive(false);
            }
            else
            {
                //CreateContinueListOption();

                continueButton.SetActive(true);
                continueButton.GetComponentInChildren<TextMeshProUGUI>().text = continueButton.GetComponent<DialogueListButton>().dialogueOption.dialogue;

            }

            if (npcDialogueOption.playerCanLeaveDialogue)
            {
                //CreateLeaveListOption();
                leaveButton.SetActive(true);
                int rand = Random.Range(0, playerDialogue.goodbyeDialogue.Count);
                leaveButton.GetComponentInChildren<TextMeshProUGUI>().text = playerDialogue.goodbyeDialogue[rand].dialogue;
                leaveButton.GetComponent<DialogueListButton>().dialogueOption = playerDialogue.goodbyeDialogue[rand];
            }
            else
            {
                leaveButton.SetActive(false);
            }

            if (npcDialogueOption.playerCanChangeTopic)
            {
                if (!playerIsLeading)
                {
                    //CreateChangeTopicListOption();
                    changeTopicButton.SetActive(true);
                    int rand2 = Random.Range(0, playerDialogue.changeTopicDialogue.Count);
                    changeTopicButton.GetComponentInChildren<TextMeshProUGUI>().text = playerDialogue.changeTopicDialogue[rand2].dialogue;
                    changeTopicButton.GetComponent<DialogueListButton>().dialogueOption = playerDialogue.changeTopicDialogue[rand2];
                }
                else
                {
                    changeTopicButton.SetActive(false);
                    
                   
                }
            }
            else
            {
                changeTopicButton.SetActive(false);
            }
        }
        else
        {
            listDialoguePanel.SetActive(false);
        }

    }

    public void LockInResponse()
    {
        //playerDialogueText.text = selectedDialogueOption.dialogue;

        responseTimer = responseTimerReset;
        responseTimerUI.value = responseTimer;
        responseTimerActive = false;

        //selectedDialogueOption.AffectEmotionValues();
        if (inDialogue)
        {
            //RememberDialogueChoices();

            if (selectedDialogueOption.statsToEffectList.Count > 0)
            {
                //selectedDialogueOption.AffectStatValues();
                playerInfoController.AffectStatValues(selectedDialogueOption.statsToEffectList);
            }
            //npc.npcEmotions.SetMood();

            if (selectedDialogueOption.relatedQuests != null)
            {
                foreach (OJQuest quest in selectedDialogueOption.relatedQuests)
                {
                    if (!quest.questStarted || (quest.questEnded && quest.isRepeatable))
                    {
                        questManager.StartQuest(quest);
                        quest.questEnded = false;
                    }
                    else if (quest.questStarted && !quest.questEnded)
                    {
                        //foreach(OJQuestDialogue questDialogue in quest.objective.questDialogueOptions)
                        //{
                            if (selectedDialogueOption.completeRelatedQuest)
                            {
                                questManager.EndQuest(quest);
                                Debug.Log("Ending Quest '" + quest.questID + "' from LockInResponse");

                            }
                        //}
                    }
                }
            }

            if (selectedDialogueOption.audioClip != null)
            {
                if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                }

                audioSource.PlayOneShot(selectedDialogueOption.audioClip);
            }

            if (selectedDialogueOption.itemsToRecieve.Count > 0)
            {
                AddItemBasedOnPlayerDialogue();
            }

            if (selectedDialogueOption.itemsToGive.Count > 0)
            {
                RemoveItemBasedOnPlayerDialogue();
            }


            if (selectedDialogueOption.conditionalEvents.Count > 0)
            {
                InvokePlayerConditionalEvents();
            }

            if (playerDialogue.changeTopicDialogue.Contains(selectedDialogueOption) || npcDialogue.changeOfTopic)
            {
                if (!playerIsLeading)
                {
                    ChangeTopic();
                }
            }

            if (selectedDialogueOption.isGoodbyeOption || npcDialogue.endOfConversation)
            {
                LeaveDialogue();
            }
        }


        if (!npcDialogue.requiresResponse)
        {
            npcDialogue = npcDialogue.continuedDialogue;

            if (npcDialogue.toOtherNPC)
            {
                listDialoguePanel.SetActive(false);
            }
            else if (!listDialoguePanel.activeSelf)
            {
                listDialoguePanel.SetActive(true);
            }
        }
        else
        {
            //npcDialogue = playerDialogue.questions.npc;
            npcDialogue = npc.DefaultResponse(selectedDialogueOption);
        }

        //if (!npcDialogue.isRepeatable)
        //{
        //    playerDialogue.RemoveDialogueForSpecificNPC(selectedDialogueOption, npc);
        //    //npcDialogue.hasBeenSeen = true;
        //}

        SetNewDialogueText(npcDialogue);

    }

    public void InvokeNPCConditonalEvents()
    {
        //Invoke player dialogue conditional event
        
        foreach (UnityEvent conditional in npcDialogue.conditionalEvents)
        {
            if (conditional != null)
            {
                //conditional.SetNPC(npc);
                conditional.Invoke();
            }
        }
        
    }

    public void InvokePlayerConditionalEvents()
    {
        //Invoke player dialogue conditional event
        foreach (UnityEvent conditional in selectedDialogueOption.conditionalEvents)
        {
            if (conditional != null)
            {
                //conditional.SetNPC(npc);
                conditional.Invoke();
            }
        }
        
    }
    // set response timer values & activate it
    private void SetResponseTimer()
    {
        if (npcDialogue.limitedTime)
        {
            responseTimer = npcDialogue.timeLimit;

            responseTimerReset = responseTimer;
            responseTimerUI.maxValue = responseTimerReset;
            responseTimerUI.value = responseTimer;

            responseTimerActive = true;
            responseTimerUI.gameObject.SetActive(true);


        }
        else
        {
            responseTimerReset = responseTimer;
            responseTimerUI.maxValue = responseTimerReset;
            responseTimerUI.value = responseTimer;

            responseTimerActive = false;
            responseTimerUI.gameObject.SetActive(false);

        }
    }

    //Initiate Dialogue
    public void BeginDialogue()
    {
        if (npcDialogue != null && !npcDialogue.toOtherNPC)
        {
            //inDialogue = true;
            dialogueUI.SetActive(true);

        }
        else if (npcDialogue != null && npcDialogue.toOtherNPC)
        {
            //inDialogue = false;
            dialogueUI.SetActive(true);
            listDialoguePanel.SetActive(false);

            if (pausedSubtitleDialogue != null && pausedSubtitleDialogue == npcDialogue)
            {
                pausedSubtitleDialogue = null;
                pausedSubtitleNPC = null;
            }


            //pausedSubtitleDialogue = npcDialogue;
            //pausedSubtitleNPC = npc;

            //LeaveDialogue();
        }
    }

    // Close Dialogue
    public void LeaveDialogue()
    {
        
        DestroyOldDialogueOptions();
        dialogueUI.SetActive(false);
        inDialogue = false;

        playerMovement.enabled = true;
        //playerCam.enabled = true;

        foreach (NPCBrain npc in FindObjectsOfType<NPCBrain>())
        {
            npc.isSpeakingToPlayer = false;
        }

        Cursor.lockState = CursorLockMode.Locked;

        if (pausedSubtitleDialogue == null)
        {
            enabled = false;
        }
        else
        {
            FindObjectOfType<DialogueInitiator>().BeginSubtitleSequence(pausedSubtitleNPC, pausedSubtitleDialogue);
        }
    }

    // Return to initial dialogue options
    public void ChangeTopic()
    {
        // get stored inquiries depending on NPC
        npcDialogue = playerDialogue.SetPlayerDialogueBasedOnCurrentNPCAndDialogue(npc, npc.npcDialogue.changeTopicDialogue[Random.Range(0, npc.npcDialogue.changeTopicDialogue.Count)]);


        //npcDialogue = playerDialogue.questions;
        //playerDialogue.questions.dialogue = npcDialogue.dialogue;

        playerIsLeading = true;

        Debug.Log("Changing the topic");
    }
    public void AddNPCsToPlayerDialogue()
    {
        List<NPCInfo> npcInfoList = new List<NPCInfo>();

        foreach(NPCBrain npc in FindObjectsOfType<NPCBrain>())
        {
            npcInfoList.Add(npc.npcInfo);
        }
        // playerDialogue.playerQuestions.Clear();

        for (int i = 0; i < npcInfoList.Count; i++)
        {
            // Add random NPC's & Questions to new list
            PlayerDialogue.PlayerQuestions newPlayerQuestions = new PlayerDialogue.PlayerQuestions();

            newPlayerQuestions.npc = npcInfoList[i];

            //Debug.Log("Generating questions for: " + npcInfoList[i].npcName);


            playerDialogue.playerQuestions.Add(newPlayerQuestions);


        }

        //playerDialogue.AddDialogueOptions();
    }

    public void ResetPlayerQuestions()
    {
        playerDialogue.playerQuestions = new List<PlayerDialogue.PlayerQuestions>();
        AddNPCsToPlayerDialogue();
    }

    public void AddItemBasedOnPlayerDialogue()
    {
        foreach (OJQuestItemObjective item in selectedDialogueOption.itemsToRecieve)
        {
            for (int i = 0; i < item.requiredAmount; i++)
            {
                inventorySystem.AddItemToInventory(item.item);
            }
        }
    }

    public void RemoveItemBasedOnPlayerDialogue() //add pop up text when item removed
    {
        foreach (OJQuestItemObjective item in selectedDialogueOption.itemsToGive)
        {
            for (int i = 0; i < item.requiredAmount; i++)
            {
                inventorySystem.RemoveItemFromInventory(item.item);
            }
        }
    } 
    
    public void AddItemBasedOnNPCDialogue()
    {
        foreach(OJQuestItemObjective item in npcDialogue.itemsToGive)
        {
            for (int i = 0; i < item.requiredAmount; i++)
            {
                inventorySystem.AddItemToInventory(item.item);
            }
        }
    }

    public void RemoveItemBasedOnNPCDialogue() //add pop up text when item removed
    {
        foreach (OJQuestItemObjective item in npcDialogue.itemsToTake)
        {
            for (int i = 0; i < item.requiredAmount; i++)
            {
                inventorySystem.RemoveItemFromInventory(item.item);
            }
        }
        
    }

    //public void RememberDialogueChoices()
    //{
    //    if (playerInteractionRaycast.selectedObject.GetComponent<NPCBrain>() &&playerInteractionRaycast.selectedObject.GetComponent<NPCBrain>().npcInfo == npc)
    //    {
    //        NPCBrain currentNPC = playerInteractionRaycast.selectedObject.GetComponent<NPCBrain>();

    //        NPCBrain.DialogueMemory dialogueMemory = new NPCBrain.DialogueMemory();

    //        dialogueMemory.npcUsedDialogue = npcDialogue;

    //        dialogueMemory.playerResponse = selectedDialogueOption;

    //        currentNPC.dialogueMemories.Add(dialogueMemory);
    //    }
    //}

    public void ResetDialogueVariables()
    {
        //reset whether dialogue has been seen by player;

    }

    public void UnlockPlayerDialogue(PlayerDialogueOption dialogueToUnlock)
    {
        dialogueToUnlock.isLocked = false;
    }

    public void LockPlayerDialogue(PlayerDialogueOption dialogueToLock)
    {
        dialogueToLock.isLocked = true;

    }

    public void ChangeSpeaker(NPCInfo newSpeaker)
    {
        DialogueListSystem dialogueListSystem = FindObjectOfType<DialogueListSystem>();
        dialogueListSystem.npc = newSpeaker;
        dialogueListSystem.npcNameText.text = newSpeaker.npcName;

        Debug.Log("New Speaker = " + newSpeaker.npcName);
        // set new speaker animations
        //stop current speaker animations
    }


    //public void ActivateInvisibleBarrier(string barrierID)
    //{
    //    foreach (OJQuestManager.InvisibleBarrier barrier in questManager.invisibleBarriers)
    //    {
    //        if (barrier.barrierID == barrierID)
    //        {
    //            barrier.barrierObject.SetActive(true);
    //        }
    //    }
    //}
    //public void DeactivateInvisibleBarrier(string barrierID)
    //{
    //    foreach(OJQuestManager.InvisibleBarrier barrier in questManager.invisibleBarriers)
    //    {
    //        if (barrier.barrierID == barrierID)
    //        {
    //            barrier.barrierObject.SetActive(false);
    //            break;
    //        }
    //    }
    //}
}