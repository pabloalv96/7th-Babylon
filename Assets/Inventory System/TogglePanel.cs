using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TogglePanel : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private KeyCode toggleKey;

    [SerializeField] private Inventory inventory;
    void Start()
    {
        panel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleInventoryUI();
        }
    }

    private void ToggleInventoryUI()
    {
        if (panel.activeSelf)
        {
            panel.SetActive(false);

            if (inventory.inventoryPanel.activeSelf && !FindObjectOfType<DialogueListSystem>().inDialogue)
            {
                FindObjectOfType<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = true;
            }
        }
        else
        {
            panel.SetActive(true);

            if (inventory.inventoryPanel.activeSelf)
            {
                FindObjectOfType<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = false;
            }
        }
    }
}
