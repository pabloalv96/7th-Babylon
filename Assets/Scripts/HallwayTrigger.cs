using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HallwayTrigger : MonoBehaviour
{
    public List<UnityEvent> eventsToTrigger;
    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            foreach(UnityEvent hallwayEvent in eventsToTrigger)
            {
                hallwayEvent.Invoke();
            }
        }
    }

    public void GivePositionToDoorman()
    {
        FindObjectOfType<DoormanHallwayAI>().closestWaypointToPlayer = transform;
    }
}
