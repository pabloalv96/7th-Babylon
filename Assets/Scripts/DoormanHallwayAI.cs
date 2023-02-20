using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class DoormanHallwayAI : MonoBehaviour
{
    public enum AIState { patrolling, chasing, /*searching,*/ idling}

    public AIState doormanState;

    public GameObject doormanEyes;

    [SerializeField] private float viewDistance /*, viewRadius*/;

    [SerializeField] private bool playerInSight;
    private int playerLayer;

    public List<Transform> waypointsList;

    public AIDestinationSetter destinationSetter;

    public float changeWaypointDistance;

    private GameObject player;

    public Transform closestWaypointToPlayer;

    public float searchTimer = 5f;

    [SerializeField] private Transform lobbyWaypoint;
    private float searchTimerReset;

    private Color gizmoColour;

    private DialogueInitiator dialogueInitiator;

    [SerializeField] private List<NPCDialogueOption> chasingDialogue, searchingDialogue, caughtDialogue;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        searchTimerReset = searchTimer;
        dialogueInitiator = GameObject.FindObjectOfType<DialogueInitiator>();
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

            //case AIState.searching:
            //    Search();
            //    gizmoColour = Color.magenta;

            //    break;

            case AIState.idling:
                Idle();
                gizmoColour = Color.cyan;

                break;
        }

       
       
    }

    public void FixedUpdate()
    {
        RaycastHit hit;
        playerLayer = 1 << 7;

        if (Physics.Raycast(doormanEyes.transform.position, transform.forward, out hit, Mathf.Infinity, playerLayer))
        {
            //Debug.DrawLine(doormanEyes.transform.position, transform.TransformDirection(Vector3.forward), Color.magenta);
            //if ((hit.collider.CompareTag("Wall") || hit.collider.CompareTag("Wall")) && !hit.collider.CompareTag("Player"))
            //{
            //    return;
            //}
            //else
            if (hit.collider != null)
            {
                Debug.DrawLine(transform.position, hit.point, Color.red);

                doormanState = AIState.chasing;
                //searchTimer = searchTimerReset;
                //Chase();

                if (!FindObjectOfType<DialogueListSystem>().enabled)
                    dialogueInitiator.BeginSubtitleSequence(gameObject.GetComponent<NPCBrain>().npcInfo, chasingDialogue[Random.Range(0, chasingDialogue.Count)]);

                //Debug.Log("Player in sight");
            }
            else
            {
                doormanState = AIState.patrolling;

                if (!FindObjectOfType<DialogueListSystem>().enabled)
                    dialogueInitiator.BeginSubtitleSequence(gameObject.GetComponent<NPCBrain>().npcInfo, searchingDialogue[Random.Range(0, searchingDialogue.Count)]);
            }
            //else
            //{
            //    Debug.DrawLine(transform.position, hit.point, Color.white);

            //    if (doormanState == AIState.chasing)
            //    {
            //        searchTimer -= Time.deltaTime;

            //        if (searchTimer <= 0)
            //        {
            //            //doormanState = AIState.searching;
            //            doormanState = AIState.patrolling;

            //            //Patrol();

            //            dialogueInitiator.BeginSubtitleSequence(gameObject.GetComponent<NPCBrain>().npcInfo, searchingDialogue[Random.Range(0, searchingDialogue.Count)]);


            //            Debug.DrawRay(transform.position, hit.point, Color.magenta);

            //            searchTimer = searchTimerReset;
            //        }
            //    }
            //}
        }

        //if (doormanState == AIState.searching)
        //{
        //    searchTimer -= Time.deltaTime;

        //    if (searchTimer <= 0f)
        //    {
        //        doormanState = AIState.patrolling;
        //        searchTimer = searchTimerReset;
        //    }
        //}

        if (Vector3.Distance(transform.position, player.transform.position) < 2f)
        {
            if (doormanState == AIState.chasing)
            {
                doormanState = AIState.idling;

                // fail state here
                dialogueInitiator.BeginSubtitleSequence(gameObject.GetComponent<NPCBrain>().npcInfo, caughtDialogue[Random.Range(0, caughtDialogue.Count)]);
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
        if (waypointsList != null)
        {
            if (destinationSetter.target == null || Vector3.Distance(transform.position, destinationSetter.target.transform.position) <= changeWaypointDistance)
            {
                destinationSetter.target = waypointsList[Random.Range(0, waypointsList.Count)];
            }
        }
        else
        {
            if (destinationSetter.target == null || Vector3.Distance(transform.position, destinationSetter.target.transform.position) <= changeWaypointDistance)
            {
                destinationSetter.target = GameObject.FindGameObjectsWithTag("HallWaypoint")[Random.Range(0, GameObject.FindGameObjectsWithTag("HallWaypoint").Length)].transform;
            }
        }
    }

    public void Chase()
    {
        destinationSetter.target = player.transform; 
    }

    //public void Search()
    //{
    //    destinationSetter.target = destinationSetter.target = closestWaypointToPlayer;

    //    if (Vector3.Distance(transform.position, destinationSetter.target.transform.position) <= changeWaypointDistance)
    //    {
    //        destinationSetter.target = waypointsList[Random.Range(0, waypointsList.Count)];
    //    }

    //}

    public void Idle()
    {
        destinationSetter.enabled = false;
        transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));

        FailState();

    }

    public void FailState()
    {
        //enable cross fade
        //player.GetComponent<Rigidbody>().position = lobbyWaypoint.position;
        player.transform.position = lobbyWaypoint.position;
        //destinationSetter.enabled = true;

        //gameObject.SetActive(false);
        doormanState = AIState.patrolling;
    }


    //void OnDrawGizmosSelected()
    //{
    //    // Draw a yellow sphere at the transform's position
    //    Gizmos.color = gizmoColour;
    //    Gizmos.DrawWireSphere(transform.position, viewDistance);
    //}
    
}


