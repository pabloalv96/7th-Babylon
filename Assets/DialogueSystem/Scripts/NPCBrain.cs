using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBrain : MonoBehaviour
{
    public NPCInfo npcInfo;

    [SerializeField] private Transform head;

    //public NPCEmotions npcEmotions;

    //public NPCDialogue npcDialogue;

    public NPCDialogueOption startingDialogue;

    public List<DialogueMemory> dialogueMemories;

    public bool isSpeakingToPlayer;

    public void SpeakingToPlayer()
    {
        if (FindObjectOfType<DialogueListSystem>().inDialogue && FindObjectOfType<DialogueListSystem>().npc == npcInfo)
        {
            transform.LookAt(Camera.main.transform);
        }
    }

    [System.Serializable]
    public class DialogueMemory
    {
        public NPCDialogueOption npcUsedDialogue;
        public PlayerDialogueOption playerResponse;
    }

}
