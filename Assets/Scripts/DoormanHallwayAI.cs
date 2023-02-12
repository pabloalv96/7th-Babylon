using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class DoormanHallwayAI : MonoBehaviour
{
    public enum AIState { patrolling, chasing}

    public AIState doormanState;

    public GameObject doormanHead;

    [SerializeField] private float viewDistance;

    public List<Transform> waypointsList;

    public AIDestinationSetter destinationSetter;

    public float changeWaypointDistance;

    private GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    public void Update()
    {
        switch (doormanState)
        {
            case AIState.patrolling:
                Patrol();
                break;

            case AIState.chasing:
                Chase();
                break;
        }
    }

    public void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(doormanHead.transform.position, transform.TransformDirection(Vector3.forward), out hit, viewDistance))
        {
            if (hit.collider.CompareTag("Default"))
            {
                return;
            }
            else if (hit.collider.CompareTag("Player"))
            {
                doormanState = AIState.chasing;
            }
        }
    }

    public void Patrol()
    {
        if (destinationSetter.target == null || Vector3.Distance(transform.position, destinationSetter.target.transform.position) <= changeWaypointDistance)
        {
            destinationSetter.target = waypointsList[Random.Range(0, waypointsList.Count)];
        }
    }

    public void Chase()
    {
        destinationSetter.target = player.transform; 
    }
}
