using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    public List<GameObject> brokenPartsPrefabs;
    public List<StatContainer.Stat> relatedSin;


    public void BreakObject()
    {
        foreach(GameObject part in brokenPartsPrefabs)
        {
            Instantiate(part, new Vector3(transform.position.x + Random.Range(-1, 1), transform.position.y + Random.Range(-1, 1), transform.position.z + Random.Range(-1, 1)), Quaternion.identity);
        }

        FindObjectOfType<PlayerInfoController>().AffectStatValues(relatedSin);


        Destroy(gameObject);


    }
}
