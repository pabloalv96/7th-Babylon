
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

public class DoorActivator : MonoBehaviour
{
    public UnityEvent unlockDoorEvent;
    public UnityEvent openDoorEvent;
    //public UnityEvent affectStatsEvent;

    [HideInInspector] public Animator animator;
    public AudioClip openSound;
    public AudioClip closeSound;
    [HideInInspector] public AudioSource source;

    public bool isOpen = false;
    public bool isLocked = false;

    public List<DoorKey> keysList;
    public NPCDialogueOption lockedDoorDialogue;
    public NPCDialogueOption unlockDoorDialogue;

    void Awake()
    {
        animator = GetComponent<Animator>();
        source = GetComponent<AudioSource>();

        if (unlockDoorEvent == null)
        {
            unlockDoorEvent = new UnityEvent();
        }
        unlockDoorEvent.AddListener(UnlockDoor);

        if (openDoorEvent == null)
        {
            openDoorEvent = new UnityEvent();
        }
        openDoorEvent.AddListener(OpenDoor);

        //if (affectStatsEvent == null)
        //{
        //    affectStatsEvent = new UnityEvent();
        //}
        //affectStatsEvent.AddListener(FindObjectOfType<PlayerInfoController>().AffectStatValues);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //animator.SetBool("Open", true);
            //source.PlayOneShot(openSound, 1);

            FindObjectOfType<PlayerInteractionRaycast>().isDoor = true;
            FindObjectOfType<PlayerInteractionRaycast>().interactPromptIndicator.SetActive(true);

        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            animator.SetBool("Open", false);
            source.PlayOneShot(closeSound, 1);

            FindObjectOfType<PlayerInteractionRaycast>().isDoor = false;
            FindObjectOfType<PlayerInteractionRaycast>().interactPromptIndicator.SetActive(false);
        }
    }

    private void Update()
    {
        if (animator.GetBool("Open"))
        {
            isOpen = true;
        }
        else
        {
            isOpen = false;
        }
    }
    public void OpenDoor()
    {
        animator.SetBool("Open", true);
        isOpen = true;
        source.PlayOneShot(openSound, 1);
    }

    public void CloseDoor()
    {
        animator.SetBool("Open", false);
        isOpen = false;
        source.PlayOneShot(closeSound, 1);
    }

    public void LockDoor()
    {
        isLocked = true;
    }

    public bool CheckKeysInInventory()
    {
        // create dialogue option for each applicable key in inventory
        foreach (InventoryItem item in FindObjectOfType<Inventory>().inventory)
        {
            for (int i = 0; i < keysList.Count; i++)
            {
                if (keysList[i].keyItem == item)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void UnlockDoor()
    {
        isLocked = false;

        //FindObjectOfType<PlayerInfoController>().AffectStatValues(chosenKey.statsToEffectList);
    }
}

[System.Serializable]
public class DoorKey
{
    public InventoryItem keyItem;
    public List<StatContainer.Stat> statsToEffectList;
}

//public class UnlockDoorStatEffectEvent : UnityEvent
//{
//    public UnlockDoorStatEffectEvent onDoorUnlocked = new UnlockDoorStatEffectEvent();

//    void AffectStats()
//    {
//        FindObjectOfType<PlayerInfoController>().AffectStatValues(key.statsToEffectList)
//    }
//}