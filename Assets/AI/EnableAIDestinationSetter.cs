using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnableAIDestinationSetter : MonoBehaviour
{
    public AIDestinationSetter aIDestinationSetter;

    private void Start()
    {
        aIDestinationSetter.enabled = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        aIDestinationSetter.enabled = true;
    }
}
