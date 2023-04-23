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

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if(isSpeakingToPlayer)
        {
            animator.SetBool("isSpeaking", true);
        } else
        {
            animator.SetBool("isSpeaking", false);
        }
    }

    public void SpeakingToPlayer()
    {
        if (FindObjectOfType<DialogueListSystem>().inDialogue && FindObjectOfType<DialogueListSystem>().npc == npcInfo)
        {
            transform.LookAt(Camera.main.transform);
        }
    }

    public void SetStartingDialogue(NPCDialogueOption newStartingDialogue)
    {
        startingDialogue = newStartingDialogue;
    }

    [System.Serializable]
    public class DialogueMemory
    {
        public NPCDialogueOption npcUsedDialogue;
        public PlayerDialogueOption playerResponse;
    }

}
