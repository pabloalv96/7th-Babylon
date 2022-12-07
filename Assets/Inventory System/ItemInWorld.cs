using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Add this script to all item prefabs, item = the respective InventoryItem scriptable object
public class ItemInWorld : MonoBehaviour
{
    public InventoryItem item;

    public List<ItemInteraction> itemInteractions;

    //public bool isQuestItem;

    //public AudioClip itemCollectedAudio;

    //public bool hasDialogue;
    //public UnlockNewDialogue unlockNewDialogue;

   
    [System.Serializable]
    public class ItemInteraction
    {
        public EnvironmentalItemInteraction interactableObject; // change to environment interaction

        public PlayerDialogueOption interactionDialogue;

    }

}
