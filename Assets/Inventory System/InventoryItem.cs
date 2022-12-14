using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//To create: Right click in project window -> Create -> Inventory item
[CreateAssetMenu(menuName ="InventoryItem")]
public class InventoryItem : ScriptableObject
{
    //the inventory item model
    public GameObject prefab;
    public string itemName;
    public int numCarried;
    public int maxNumCarried;

    public bool canCollect;
    public bool canConsume;
    public List<OJQuest> relatedQuests;


    public List<StatContainer.Stat> statsToEffectOnCollectionList;
    public List<StatContainer.Stat> statsToEffectOnConsumptionList;

}
