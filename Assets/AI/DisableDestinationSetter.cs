using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class DisableDestinationSetter : MonoBehaviour
{
    public AIDestinationSetter destinationSetter;
    private RichAI richAI;

    private void Start()
    {
        richAI = GetComponent<RichAI>();
    }
    void Update()
    {
        if (richAI.reachedEndOfPath)
        {
            destinationSetter.enabled = false;
        }
        else
        {
            destinationSetter.enabled = true;
        }
    }
}
