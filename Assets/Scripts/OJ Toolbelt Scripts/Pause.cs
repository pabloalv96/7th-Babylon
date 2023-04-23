using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    [SerializeField] private GameObject pauseUI;

    private void Awake()
    {
        pauseUI.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
    }

    private void PauseGame()
    {
        if (!pauseUI.activeSelf)
        {
            //Time.timeScale = 0f;
            pauseUI.SetActive(true);
        }
        else
        {
            //Time.timeScale = 1f;
            pauseUI.SetActive(false);
        }
    }
}
