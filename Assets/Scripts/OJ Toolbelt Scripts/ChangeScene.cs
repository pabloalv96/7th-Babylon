using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Toolbelt_OJ
{
    public class ChangeScene : MonoBehaviour
    {

        public Animator transition;

        public float transitionTime = 2f;

        public void MoveToScene(int sceneID)
        {
            transition.SetTrigger("Start 0");
            transition.SetBool("startTran", true);

            StartCoroutine(LoadLevel(sceneID));
        }

        IEnumerator LoadLevel(int sceneID)
        {
            

            yield return new WaitForSeconds(transitionTime);

            SceneManager.LoadScene(sceneID);
        }

    }
}
