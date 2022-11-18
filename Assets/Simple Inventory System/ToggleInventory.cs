using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Activate Inventory UI by pressing 'Q'
public class ToggleInventory : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel;
    void Start()
    {
        inventoryPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ToggleInventoryUI();
        }
    }

    private void ToggleInventoryUI()
    {
        if (inventoryPanel.activeSelf)
        {
            inventoryPanel.SetActive(false);

            if (FindObjectOfType<Inventory>().inventoryPanel.activeSelf && !FindObjectOfType<DialogueListSystem>().inDialogue)
            {
                FindObjectOfType<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = true;
            }
        }
        else
        {
            inventoryPanel.SetActive(true);

            if (FindObjectOfType<Inventory>().inventoryPanel.activeSelf && FindObjectOfType<Inventory>().selectedItem != null)
            {
                FindObjectOfType<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = false;
            }
        }
    }
}
