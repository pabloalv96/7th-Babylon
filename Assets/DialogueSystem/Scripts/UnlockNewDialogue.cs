using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Add method to enable this script in other object
[System.Serializable]
public class UnlockNewDialogue
{
    //private PlayerDialogue playerDialogue = FindObjectOfType<PlayerDialogue>();
    public List<SetDialogueForSpecificNPC> dialogueToUnlock;
    public List<SetDialogueForSpecificNPC> dialogueToLock;

    [System.Serializable]
    public class SetDialogueForSpecificNPC
    {
        public PlayerDialogueOption dialogueOption;
        public NPCInfo npcDialogueTarget;
    }
    public void UnlockDialogueForAll(PlayerDialogue playerDialogue, PlayerDialogueOption dialogueOption)
    {
        playerDialogue.AddQuestionForAllNPCs(dialogueOption);
        playerDialogue.AddDialogueOptions();
    }

    public void UnlockDialogueForSpecificNPC(PlayerDialogue playerDialogue, NPCInfo npc, PlayerDialogueOption dialogueOption)
    {
        playerDialogue.AddQuestionForSpecificNPC(dialogueOption, npc);
        playerDialogue.AddDialogueOptions();
    }
}
