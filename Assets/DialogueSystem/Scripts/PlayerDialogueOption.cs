using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[CreateAssetMenu (menuName = "DialogueSystem/PlayerDialogueOption")]
public class PlayerDialogueOption : ScriptableObject
{
    [TextArea(3, 10)]
    public string dialogue;

    //public List<NPCMood.Emotion> emotionsToAffectList;
    public List<StatContainer.Stat> statsToEffectList;

    //public float npcWillRemember;

    public bool isResponseToNPCDialogue;

    public bool isGoodbyeOption, isChangeTopicOption;

    public bool isLocked = false;

    public List<UnityEvent> conditionalEvents;

    //public bool isQuestDialogue;
    public OJQuest relatedQuest;

    //public bool giveItems;
    public List<InventoryItem> itemsToGive;

    //public bool recieveItems;
    public List<InventoryItem> itemsToRecieve;

    //public void AffectEmotionValues()
    //{
    //    foreach (NPCMood.Emotion emotionToAffect in emotionsToAffectList)
    //    {
    //        foreach (NPCMood.Emotion emotion in FindObjectOfType<DialogueListSystem>().npc.npcMood.listOfEmotions)
    //        {
    //            if (emotion.emotionName == emotionToAffect.emotionName)
    //            {
    //                emotion.emotionValue += emotionToAffect.emotionValue;
    //            }
    //        }
    //    }

    //}

    //public void AffectStatValues()
    //{
    //    foreach (StatContainer.Stat statToEffect in statsToEffectList)
    //    {
    //        foreach (StatContainer.Stat stat in FindObjectOfType<PlayerInfoController>().playerStats.listOfStats)
    //        {
    //            if (stat.statName == statToEffect.statName)
    //            {
    //                stat.statValue += statToEffect.statValue;
    //            }
    //        }
    //    }

    //    CheckStatValues();
    //}

    //static int SortStatByValues(StatContainer.Stat s1, StatContainer.Stat s2)
    //{
    //    return s2.statValue.CompareTo(s1.statValue);
    //}


    //public void CheckStatValues()
    //{

    //    FindObjectOfType<PlayerInfoController>().playerStats.listOfStats.Sort(SortStatByValues);

    //    FindObjectOfType<PlayerInfoController>().playerStats.highestStat = FindObjectOfType<PlayerInfoController>().playerStats.listOfStats[0];

    //}

    //public NPCEmotions.NPCFeelings AffectEmotionValues(NPCEmotions.NPCFeelings npcEmotions)
    //{
    //    npcEmotions.happiness += happinessEffect;
    //    npcEmotions.stress += stressEffect;
    //    npcEmotions.shock += shockEffect;

    //    Debug.Log("Emotions have been affected");

    //    return npcEmotions;
    //}
}
