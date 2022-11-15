using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInteractionRaycast : MonoBehaviour
{

    [SerializeField] private KeyCode selectInput = KeyCode.E;

    [SerializeField] private float reachDistance = 5f;

    public GameObject selectedObject;

    private StartDialogue initiateDialogue;

    [SerializeField] private GameObject interactIndicator;

    private bool isNPC;
    private bool isWorldDialogue;
    private bool isDoor;

    //[SerializeField] private Inventory inventory;
    //[SerializeField] private TextMeshProUGUI checkInventoryIndicator;

    //[SerializeField] private float inventoryIndicatorDisplayTime = 7.5f;
    //private float inventoryIndicatorDisplayTimeReset;

    //[SerializeField] private bool displayInventoryIndicator;

    //[SerializeField] private AudioSource audioSource;

    //[SerializeField] private bool isItem;

    [SerializeField] private DoorActivator doorActivator;



    void Start()
    {
        initiateDialogue = FindObjectOfType<StartDialogue>();

        interactIndicator.SetActive(false);

        //checkInventoryIndicator.text = "New Item! Press '" + selectInput + "' to check your inventory";

        //checkInventoryIndicator.enabled = false;
        //inventoryIndicatorDisplayTimeReset = inventoryIndicatorDisplayTime;
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, reachDistance) && !initiateDialogue.dialogueSystem.enabled) //Camera.main.transform.position, Camera.main.transform.forward
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.magenta);

            //if (hit.transform.GetComponent<ItemInWorld>())
            //{
            //    isItem = true;
            //    selectedObject = hit.transform.gameObject;
            //    Debug.Log("hit = " + selectedObject);
            //    interactIndicator.SetActive(true);
            //}
            //else
            //{
            //    isItem = false;
            //}

            if (hit.transform.GetComponent<DialogueInWorld>())
            {
                selectedObject = hit.transform.gameObject;
                interactIndicator.SetActive(true);
                isWorldDialogue = true;
            }
            else
            {
                isWorldDialogue = false;
            }

            if (hit.transform.GetComponent<NPCBrain>())
            {
                isNPC = true;
                selectedObject = hit.transform.gameObject;
                //Debug.Log("hit = " + selectedObject);
                interactIndicator.SetActive(true);
                
            }
            else
            {
                isNPC = false;
            }

            if (hit.transform.GetComponent<DoorActivator>())
            {
                isDoor = true;
                selectedObject = hit.transform.gameObject;
                interactIndicator.SetActive(true);
            }
            else
            {
                isDoor = false;
            }

            if (selectedObject != null && Input.GetKeyDown(selectInput))
            {
                if (isNPC)
                {
                    SelectNPC();
                    selectedObject.transform.GetComponent<NPCBrain>().isSpeakingToPlayer = true;
                }

                if (isWorldDialogue)
                {
                    ActivateDialogueSystem();
                    Debug.Log("Hit dialogue in world game object");
                }

                //if (isItem)
                //{
                //    PickUpItem();
                //}

                //StartCoroutine(CheckInventoryIndicator());

                if (isDoor)
                {
                    doorActivator = selectedObject.GetComponent<DoorActivator>();
                    if (!doorActivator.isLocked)
                    {
                        if (doorActivator.isOpen)
                        {
                            doorActivator.CloseDoor();
                        }
                        else
                        {
                            doorActivator.OpenDoor();
                        }
                    }
                    else
                    {
                        //communicate locked door to player
                        Debug.Log(selectedObject.name + " is locked");
                    }
                }
            }

           

        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
            //Debug.Log("Did not Hit");
            selectedObject = null;
            interactIndicator.SetActive(false);
        }

        
    }

    public void SelectNPC()
    {
        if (selectedObject != null && selectedObject.GetComponent<NPCBrain>())
        {
            if (selectedObject.GetComponent<NPCBrain>().npcInfo != null)
            {
                selectedObject.GetComponent<NPCBrain>().SpeakingToPlayer();
                Debug.Log("NPC Selected: " + selectedObject.GetComponent<NPCBrain>().npcInfo.npcName);
                initiateDialogue.EnterDialogue(selectedObject.GetComponent<NPCBrain>().npcInfo);
            }
        }
    }

    public void ActivateDialogueSystem()
    {
        if (selectedObject != null && selectedObject.GetComponent<DialogueInWorld>())
        {
            DialogueInWorld dialogueInWorld = selectedObject.GetComponent<DialogueInWorld>();
            Debug.Log("Dialogue Beginning: " + dialogueInWorld.dialogueFromWorld.name);

            initiateDialogue.NPCInitiatedDialogue(dialogueInWorld.narrator, dialogueInWorld.dialogueFromWorld);
        }
    }

    //public void PickUpItem()
    //{
    //    //if (audioSource.isPlaying)
    //    //{
    //    //    audioSource.Stop();

    //    //    audioSource.PlayOneShot(selectedObject.GetComponent<ItemInWorld>().itemCollectedAudio);
    //    //}

    //    if (selectedObject.GetComponent<ItemInWorld>().hasDialogue)
    //    {
    //        selectedObject.GetComponent<ItemInWorld>().unlockNewDialogue.enabled = true;
    //    }

    //    inventory.AddItemToInventory(selectedObject.GetComponent<ItemInWorld>().item);
    //    if (inventory.inventory.Contains(selectedObject.GetComponent<ItemInWorld>().item))
    //    {
    //        Destroy(selectedObject);
    //        selectedObject = null;
    //        interactIndicator.SetActive(false);
    //        //displayInventoryIndicator = true;
    //    }

    //    if (FindObjectOfType<PlayerInfoController>()) // progress collection quest if relevant
    //    {
    //        PlayerInfoController player = FindObjectOfType<PlayerInfoController>();

    //        foreach (Quest quest in player.activeQuestList)
    //        {
    //            if (quest.questType == QuestType.Collection)
    //            {
    //                NumericalQuest numQuest = (NumericalQuest)quest;

    //                numQuest.SetCurrentAmount();
    //            }
    //        }
    //    }
    //}

    //public IEnumerator CheckInventoryIndicator()
    //{
    //    if (displayInventoryIndicator)
    //    {
    //        checkInventoryIndicator.enabled = true;
    //        inventoryIndicatorDisplayTime -= Time.deltaTime;

    //        if (inventoryIndicatorDisplayTime >= inventoryIndicatorDisplayTimeReset - 2f)
    //        {
    //            checkInventoryIndicator.alpha = Mathf.Lerp(0f, 1f, inventoryIndicatorDisplayTimeReset - inventoryIndicatorDisplayTime);

    //        }

    //        if (inventoryIndicatorDisplayTime <= 2f)
    //        {
    //            checkInventoryIndicator.alpha = Mathf.Lerp(0f, 1f, inventoryIndicatorDisplayTime);
    //        }

    //        if (inventoryIndicatorDisplayTime <= 0f)
    //        {

    //            displayInventoryIndicator = false;
    //            inventoryIndicatorDisplayTime = inventoryIndicatorDisplayTimeReset;
    //            displayInventoryIndicator = false;
    //        }
    //    }

    //    yield return null;
    //}
}
