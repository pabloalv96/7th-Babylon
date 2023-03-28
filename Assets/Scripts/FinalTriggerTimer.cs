using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalTriggerTimer : MonoBehaviour
{
    [SerializeField] float delayBeforeLoading = 24f;
    [SerializeField] public bool endScreen = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CoorutineStarter()
    {
        StartCoroutine(ChangeSceneAfterSeconds());
    }


    public IEnumerator ChangeSceneAfterSeconds()
    {
        endScreen = true;
        yield return new WaitForSeconds(delayBeforeLoading);
        SceneManager.LoadScene("Ending Screen");
    }
}
