using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnableAIDestinationSetter : MonoBehaviour
{
    public AIDestinationSetter aIDestinationSetter;

    public Transform npcIdlePosition;

    public Transform player;

    public bool dialogueBegun, dialogueComplete;

    private DialogueListSystem dialogueSystem;

    private void Start()
    {
        dialogueSystem = FindObjectOfType<DialogueListSystem>();
        aIDestinationSetter.enabled = false;
    }

    public void Update()
    {
        if (dialogueSystem.inDialogue)
        {
            dialogueBegun = true;
        }
        else if (dialogueBegun)
        {
            dialogueComplete = true;
        }

        if (dialogueComplete)
        {
            aIDestinationSetter.target = npcIdlePosition;
            aIDestinationSetter.transform.LookAt(player);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        aIDestinationSetter.enabled = true;
    }
}
