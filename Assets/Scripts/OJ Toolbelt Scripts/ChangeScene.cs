using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Toolbelt_OJ
{
    public class ChangeScene : MonoBehaviour
    {

        public Animator transition;

        public float transitionTime = 2f;

        //public GameObject menuButtons;

        public GameObject loadingBar;
        public Image loadingBarFill;

        public Camera loadingCam;

        public GameObject menuElements;

        private void Awake()
        {
            loadingBar.SetActive(false);
        }

        public void MoveToScene(int sceneID)
        {
            //transition.SetTrigger("Start 0");
            //transition.SetBool("startTran", true);

            //SceneManager.LoadScene(sceneID);

            StartCoroutine(LoadAsyncScene(sceneID));
        }

        //IEnumerator LoadLevel(int sceneID)
        //{

        //    yield return new WaitForSeconds(transitionTime);

        //    SceneManager.LoadScene(sceneID);
        //}

        public IEnumerator LoadAsyncScene(int sceneID)
        {
            loadingCam.enabled = true;
            Camera.main.enabled = false;

            AsyncOperation async = SceneManager.LoadSceneAsync(sceneID);

            loadingBar.SetActive(true);
            menuElements.SetActive(false);

            while (!async.isDone)
            {

                float progressValue = Mathf.Clamp01(async.progress / 0.9f);

                loadingBarFill.fillAmount = progressValue;

                yield return null;
            }
        }

    }
}
