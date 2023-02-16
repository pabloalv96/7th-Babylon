using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class DoormanHallwayAI : MonoBehaviour
{
    public enum AIState { patrolling, chasing, searching, idling}

    public AIState doormanState;

    public GameObject doormanEyes;

    [SerializeField] private float viewDistance, viewRadius;

    public List<Transform> waypointsList;

    public AIDestinationSetter destinationSetter;

    public float changeWaypointDistance;

    private GameObject player;

    public Transform closestWaypointToPlayer;

    public float searchTimer = 5f;
    private float searchTimerReset;

    private Color gizmoColour;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        searchTimerReset = searchTimer;
    }
    public void Update()
    {
        switch (doormanState)
        {
            case AIState.patrolling:
                Patrol();
                gizmoColour = Color.yellow;
                break;

            case AIState.chasing:
                Chase();
                gizmoColour = Color.red;

                break;

            case AIState.searching:
                Search();
                gizmoColour = Color.magenta;

                break;

            case AIState.idling:
                Idle();
                gizmoColour = Color.cyan;

                break;
        }
    }

    public void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.SphereCast(doormanEyes.transform.position, viewRadius, transform.forward, out hit, viewDistance))
        {
            //Debug.Dra(doormanEyes.transform.position, transform.TransformDirection(Vector3.forward), Color.magenta, viewDistance);
            //if ((hit.collider.CompareTag("Wall") || hit.collider.CompareTag("Wall")) && !hit.collider.CompareTag("Player"))
            //{
            //    return;
            //}
            //else
            if (hit.collider.CompareTag("Player"))
            {
                doormanState = AIState.chasing;
                searchTimer = searchTimerReset;
                Debug.Log("Player in sight");
            }
            else
            {
                if (doormanState == AIState.chasing)
                {
                    searchTimer -= Time.deltaTime;

                    if (searchTimer <= 0)
                    {
                        doormanState = AIState.searching;
                        searchTimer = searchTimerReset;
                    }
                }
            }
        }

        if (doormanState == AIState.searching)
        {
            searchTimer -= Time.deltaTime;

            if (searchTimer <= 0f)
            {
                doormanState = AIState.patrolling;
                searchTimer = searchTimerReset;
            }
        }

        if (Vector3.Distance(transform.position, destinationSetter.target.transform.position) < 1f)
        {
            if (doormanState == AIState.chasing)
            {
                doormanState = AIState.idling;

                // fail state here
            }
        }
        else if (doormanState == AIState.idling)
        {
            destinationSetter.enabled = true;

            doormanState = AIState.chasing;
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

    public void Search()
    {
        destinationSetter.target = closestWaypointToPlayer;

        if (Vector3.Distance(transform.position, destinationSetter.target.transform.position) <= changeWaypointDistance)
        {
            transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));
        }

    }

    public void Idle()
    {
        destinationSetter.enabled = false;
        transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));

    }


    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = gizmoColour;
        Gizmos.DrawWireSphere(transform.position, 5);
    }
    
}


