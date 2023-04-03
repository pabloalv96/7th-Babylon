using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    public List<GameObject> brokenPartsPrefabs;
    public List<StatContainer.Stat> relatedSin;

    public string objectName;

    public float brokenParts = 7;

    public void Awake()
    {
        if (gameObject.GetComponent<ItemInWorld>())
        {
            objectName = gameObject.GetComponent<ItemInWorld>().item.itemName;
        }
    }

    public void BreakObject()
    {
        for (int i = 0; i < brokenParts; i++)
        {
            Instantiate(brokenPartsPrefabs[Random.Range(0, brokenPartsPrefabs.Count)], new Vector3(transform.position.x + Random.Range(-0.1f, 0.1f), transform.position.y + Random.Range(-0.1f, 0.1f), transform.position.z + Random.Range(-0.1f, 0.1f)), Quaternion.identity);
        }

        //foreach(GameObject part in brokenPartsPrefabs)
        //{
        //    Instantiate(part, new Vector3(transform.position.x + Random.Range(-0.1f, 0.1f), transform.position.y + Random.Range(-0.1f, 0.1f), transform.position.z + Random.Range(-0.1f, 0.1f)), Quaternion.identity);
        //}

        FindObjectOfType<PlayerInfoController>().AffectStatValues(relatedSin);


        Destroy(gameObject);

        //StartCoroutine(FindObjectOfType<PlayerInteractionRaycast>().DelaySettingFalseVariables());
    }
}
