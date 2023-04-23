using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class DoormanHallwayAI : MonoBehaviour
{
    public enum AIState { searching, chasing, wandering, idling}

    public AIState doormanState;

    public GameObject doormanEyes;

    [SerializeField] private float viewDistance;

    [SerializeField] private bool playerInSight;
    //private int playerLayer;

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

    public float dialogueCooldownTimer;
    public float dialogueCooldownTimerReset;

    [SerializeField] private List<NPCDialogueOption> chasingDialogue, searchingDialogue, caughtDialogue;

    public CameraFade cameraFade;

    public float teleportDelay = 1;

    public float maxSpeed, minSpeed;

    public float speedChangeTimer;
    public float speedChangeTimerReset;

    public bool increaseSpeed;

    public float increaseSpeedMultiplier;
    //public bool randomWaypoint;

    public float minDistance = 2f;

    private RichAI richAI;

    //public float attentionSpanTimer;

    //public float teleportCount

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        searchTimerReset = searchTimer;
        dialogueInitiator = GameObject.FindObjectOfType<DialogueInitiator>();
        richAI = GetComponent<RichAI>();
    }
    public void Update()
    {
        switch (doormanState)
        {
            //case AIState.wandering:
            //    Wander();
            //    gizmoColour = Color.green;
            //    break;

            case AIState.searching:
                Search();
                gizmoColour = Color.yellow;
                break;

            case AIState.chasing:
                Chase();
                gizmoColour = Color.red;

                break;

            case AIState.idling:
                Idle();
                gizmoColour = Color.cyan;

                break;
        }

       if (dialogueCooldownTimer > 0)
        {
            dialogueCooldownTimer -= Time.deltaTime;
        }

       if (doormanState == AIState.searching)
        {
            searchTimer -= Time.deltaTime;

        }
       

        RaycastHit hit;
        //playerLayer = 1 << 7;

        if (Physics.Raycast(doormanEyes.transform.position, transform.forward, out hit, Mathf.Infinity /*playerLayer*/))
        {
            Debug.DrawLine(doormanEyes.transform.position, transform.TransformDirection(Vector3.forward), Color.magenta);
            if (hit.collider.CompareTag("Wall") || hit.collider.CompareTag("Untagged"))
            {
                doormanState = AIState.searching;

                //if (!FindObjectOfType<DialogueListSystem>().enabled)
                //    dialogueInitiator.BeginSubtitleSequence(gameObject.GetComponent<NPCBrain>().npcInfo, searchingDialogue[Random.Range(0, searchingDialogue.Count)]);
            }
            else
            if (hit.collider.CompareTag("Player"))
            {
                Debug.DrawLine(transform.position, hit.point, Color.red);

                doormanState = AIState.chasing;
                //searchTimer = searchTimerReset;
                //Chase();

                if (!FindObjectOfType<DialogueListSystem>().enabled && dialogueCooldownTimer == 0)
                {
                    dialogueInitiator.BeginSubtitleSequence(gameObject.GetComponent<NPCBrain>().npcInfo, chasingDialogue[Random.Range(0, chasingDialogue.Count)]);
                    dialogueCooldownTimer = dialogueCooldownTimerReset;
                }
                //Debug.Log("Player in sight");
            }
            //else
            //{
            //    doormanState = AIState.wandering;
            //    randomWaypoint = true;

            //    //if (!FindObjectOfType<DialogueListSystem>().enabled && dialogueCooldownTimer == 0)
            //    //{
            //    //    dialogueInitiator.BeginSubtitleSequence(gameObject.GetComponent<NPCBrain>().npcInfo, searchingDialogue[Random.Range(0, searchingDialogue.Count)]);
            //    //    dialogueCooldownTimer = dialogueCooldownTimerReset;
            //    //}
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

                doormanState = AIState.wandering;
            }

            if (speedChangeTimer > 0)
            {
                speedChangeTimer -= Time.deltaTime;
            }
            else
            {
                switch(increaseSpeed)
                {
                    case true:
                        increaseSpeed = false;
                        speedChangeTimer = speedChangeTimerReset;
                        break;

                    case false:
                        increaseSpeed = true;
                        speedChangeTimer = speedChangeTimerReset;
                        break;
                }
            }

            if (!increaseSpeed)
            {
                richAI.maxSpeed = Mathf.Lerp(maxSpeed, minSpeed, speedChangeTimer * increaseSpeedMultiplier);
            }
            else
            {
                richAI.maxSpeed = Mathf.Lerp(minSpeed, maxSpeed, speedChangeTimer * increaseSpeedMultiplier);
            }

        }
    }

    public void Search()
    {
        if (waypointsList != null)
        {
            if (destinationSetter.target == null || Vector3.Distance(transform.position, destinationSetter.target.transform.position) <= changeWaypointDistance || searchTimer <= 0f)
            {
                destinationSetter.target = waypointsList[Random.Range(0, waypointsList.Count)];
                searchTimer = searchTimerReset;
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

        Vector3 targetPostition = new Vector3(player.transform.position.x,
                                      transform.position.y,
                                      player.transform.position.z);
        transform.LookAt(targetPostition);
    }

    //public void Wander()
    //{
    //    //transform.position = waypointsList[Random.Range(0, waypointsList.Count)].position;
    //    if (randomWaypoint)
    //    {
    //        destinationSetter.target = waypointsList[Random.Range(0, waypointsList.Count)];
    //        randomWaypoint = false;
    //    }

    //    if (Vector3.Distance(transform.position, destinationSetter.target.position) <= minDistance)
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
        cameraFade.trapped = true;
        //player.GetComponent<Rigidbody>().position = lobbyWaypoint.position;
        StartCoroutine(TrappedTime(1));
        //destinationSetter.enabled = true;

        //gameObject.SetActive(false);
        doormanState = AIState.searching;
    }

    public IEnumerator TrappedTime(float time) { 

        yield return new WaitForSeconds(time);

        player.transform.position = lobbyWaypoint.position;
    }


    //void OnDrawGizmosSelected()
    //{
    //    // Draw a yellow sphere at the transform's position
    //    Gizmos.color = gizmoColour;
    //    Gizmos.DrawWireSphere(transform.position, viewDistance);
    //}
    
}


