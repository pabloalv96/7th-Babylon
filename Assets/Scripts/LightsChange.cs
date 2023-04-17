using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightsChange : MonoBehaviour
{
    [SerializeField] Light directionalLight;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            directionalLight.intensity = 0;
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            directionalLight.intensity = 1;
        }
    }
}
