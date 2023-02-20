using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Pathfinding;

public class HallwayTrigger : MonoBehaviour
{
    public List<Transform> nearbyWaypoints;

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
        FindObjectOfType<DoormanHallwayAI>().waypointsList = new List<Transform>();
        FindObjectOfType<DoormanHallwayAI>().waypointsList = nearbyWaypoints;
        FindObjectOfType<DoormanHallwayAI>().closestWaypointToPlayer = nearbyWaypoints[Random.Range(0, nearbyWaypoints.Count)];

    }

    public void StartHallwaySequence(GameObject doorman)
    {
        doorman.GetComponent<RichAI>().enabled = true;
        doorman.GetComponent<AIDestinationSetter>().enabled = true;
    }
}
