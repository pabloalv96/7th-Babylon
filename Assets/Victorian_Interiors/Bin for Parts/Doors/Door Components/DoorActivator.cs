

using UnityEngine;
using System.Collections;

public class DoorActivator : MonoBehaviour
{

    [HideInInspector] public Animator animator;
    public AudioClip openSound;
    public AudioClip closeSound;
    [HideInInspector] public AudioSource source;

    public bool isOpen = false;
    public bool isLocked = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
        source = GetComponent<AudioSource>();

    }

    //void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.tag == "Player")
    //    {
    //        animator.SetBool("Open", true);
    //        source.PlayOneShot(openSound, 1);

    //    }
    //}

    //void OnTriggerExit(Collider other)
    //{
    //    if (other.gameObject.tag == "Player")
    //    {
    //        animator.SetBool("Open", false);
    //        source.PlayOneShot(closeSound, 1);
    //    }
    //}

    private void Update()
    {
        if (animator.GetBool("Open"))
        {
            isOpen = true;
        }
        else
        {
            isOpen = false;
        }
    }
    public void OpenDoor()
    {
        animator.SetBool("Open", true);
        isOpen = true;
        source.PlayOneShot(openSound, 1);
    }

    public void CloseDoor()
    {
        animator.SetBool("Open", false);
        isOpen = false;
        source.PlayOneShot(closeSound, 1);
    }
}
