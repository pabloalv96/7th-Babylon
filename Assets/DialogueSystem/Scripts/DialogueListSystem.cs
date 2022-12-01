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
    [SerializeField] private PlayerDialogue playerDialogue;

    [SerializeField] private GameObject dialogueUI;

    private void Start()
    {
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

            if (npcDialogue.relatedQuest != null)
            {
                if (!npcDialogue.relatedQuest.questStarted)
                {
                    FindObjectOfType<OJQuestManager>().StartQuest(npcDialogue.relatedQuest);
                }
                else if (!npcDialogue.relatedQuest.questEnded)
                {
                    FindObjectOfType<OJQuestManager>().EndQuest(npcDialogue.relatedQuest);
                }
            }

            if (npcDialogue.newStartingDialogue != null)
            {
                if (FindObjectOfType<PlayerInteractionRaycast>().selectedObject.GetComponent<NPCBrain>() && FindObjectOfType<PlayerInteractionRaycast>().selectedObject.GetComponent<NPCBrain>().npcInfo == npc)
                {
                    FindObjectOfType<PlayerInteractionRaycast>().selectedObject.GetComponent<NPCBrain>().startingDialogue = npcDialogue.newStartingDialogue;
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
                        newDialogue.GetComponent<DialogueListButton>().dialogueOption = dialogueOption;
                        newDialogue.transform.SetParent(listDialoguePanel.transform);
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
            RememberDialogueChoices();

            if (selectedDialogueOption.statsToEffectList.Count > 0)
            {
                //selectedDialogueOption.AffectStatValues();
                FindObjectOfType<PlayerInfoController>().AffectStatValues(selectedDialogueOption.statsToEffectList);
            }
            //npc.npcEmotions.SetMood();

            if (selectedDialogueOption.relatedQuest != null)
            {

                FindObjectOfType<OJQuestManager>().EndQuest(selectedDialogueOption.relatedQuest);
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

        enabled = false;
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

    public void AddItemBasedOnPlayerDialogue()
    {
        foreach(InventoryItem item in selectedDialogueOption.itemsToRecieve)
        {
            FindObjectOfType<Inventory>().AddItemToInventory(item);
        }
    }

    public void RemoveItemBasedOnPlayerDialogue()
    {
        foreach(InventoryItem item in selectedDialogueOption.itemsToGive)
        {
            FindObjectOfType<Inventory>().RemoveItemFromInventory(item);
        }
    } 
    
    public void AddItemBasedOnNPCDialogue()
    {
        foreach(InventoryItem item in npcDialogue.itemsToGive)
        {
            FindObjectOfType<Inventory>().AddItemToInventory(item);
        }
    }

    public void RemoveItemBasedOnNPCDialogue()
    {
        foreach(InventoryItem item in npcDialogue.itemsToTake)
        {
            FindObjectOfType<Inventory>().RemoveItemFromInventory(item);
        }
    }

    public void RememberDialogueChoices()
    {
        if (FindObjectOfType<PlayerInteractionRaycast>().selectedObject.GetComponent<NPCBrain>() && FindObjectOfType<PlayerInteractionRaycast>().selectedObject.GetComponent<NPCBrain>().npcInfo == npc)
        {
            NPCBrain currentNPC = FindObjectOfType<PlayerInteractionRaycast>().selectedObject.GetComponent<NPCBrain>();

            NPCBrain.DialogueMemory dialogueMemory = new NPCBrain.DialogueMemory();

            dialogueMemory.npcUsedDialogue = npcDialogue;

            dialogueMemory.playerResponse = selectedDialogueOption;

            currentNPC.dialogueMemories.Add(dialogueMemory);
        }
    }

    public void ResetDialogueVariables()
    {
        //reset whether dialogue has been seen by player;

    }
}