using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookSinTimer : MonoBehaviour
{
    public List<StatContainer.Stat> relatedSin;
    public float lookTimer;

    public bool isLooking;

    public void FixedUpdate()
    {
        if (isLooking)
        {
            lookTimer += Time.deltaTime;
            relatedSin[0].statValue = lookTimer;
        }
        else
        {
            if (lookTimer > 0)
            {
                FindObjectOfType<PlayerInfoController>().AffectStatValues(relatedSin);

                lookTimer = 0;
            }
        }
    }
}
