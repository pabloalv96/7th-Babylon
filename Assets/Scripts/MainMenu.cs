using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public bool startAnim = false;

    public void Play()
    {
        startAnim = true;
        SceneManager.LoadScene("Pablo Development");
        
    }

    public void Quit()
    {
        Application.Quit();
    }

}
