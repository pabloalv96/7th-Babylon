using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueListButton : MonoBehaviour
{
    public DialogueListSystem dialogueSystem;
    public PlayerDialogueOption dialogueOption;

    private void Awake()
    {
        dialogueSystem = FindObjectOfType<DialogueListSystem>();

        //DialogueEvents.current.onUpdateDialogue += OnClickSelectDialogue;
    }

    public void OnClickSelectDialogue()
    {
        dialogueSystem.selectedDialogueOption = dialogueOption;

        if (dialogueOption.isGoodbyeOption)
        {
            dialogueSystem.LeaveDialogue();
        }
        
        if (dialogueOption.isChangeTopicOption)
        {
            dialogueSystem.ChangeTopic();
        }

        dialogueSystem.LockInResponse();
        Debug.Log("Button Clicked");
    }
}
