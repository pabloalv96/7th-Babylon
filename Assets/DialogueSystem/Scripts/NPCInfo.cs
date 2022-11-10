using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("DialogueSystem/NPCInfo"))]

[System.Serializable]
public class NPCInfo : ScriptableObject
{
    public string npcName;
    public string npcGender;

    public string npcProfileID;

    //public NPCMood npcMood;
    //public StatContainer stats;

    public NPCDialogue npcDialogue;

    public NPCDialogueOption RespondBasedOnStat(StatContainer stats, PlayerDialogueOption playerDialogueInput)
    {
        for (int i = 0; i < npcDialogue.dialogueConnections.Count; i++) //cycle through dialogue options
        {
            if (npcDialogue.dialogueConnections[i].playerDialogueInput == playerDialogueInput) //find current player dialogue
            {
                for (int d = 0; d < npcDialogue.dialogueConnections[i].npcResponses.Count; d++) //scroll through current dialogue responses
                {
                    if (npcDialogue.dialogueConnections[i].npcResponses[d].requiredStat == stats.highestStat)
                    {
                        return npcDialogue.dialogueConnections[i].npcResponses[d].response;
                    }
                    else if (d + 1 >= npcDialogue.dialogueConnections[i].npcResponses.Count)
                    {
                        return npcDialogue.dialogueConnections[i].npcResponses[0].response;
                    }
                }
            }
        }

        return DefaultResponse(playerDialogueInput);
    }

    public NPCDialogueOption DefaultResponse(PlayerDialogueOption playerDialogueInput)
    {
        for (int i = 0; i < npcDialogue.dialogueConnections.Count; i++) //cycle through dialogue options
        {
            if (npcDialogue.dialogueConnections[i].playerDialogueInput == playerDialogueInput) //find current player dialogue
            {
                return npcDialogue.dialogueConnections[i].npcResponses[0].response;
            }
        }

        return npcDialogue.nothingToSayDialogue;
    }

    //public NPCDialogueOption RespondBasedOnMood(PlayerDialogueOption playerDialogueInput)
    //{
    //    for (int i = 0; i < npcDialogue.dialogueConnections.Count; i++) //cycle through dialogue options
    //    {
    //        if (npcDialogue.dialogueConnections[i].playerDialogueInput == playerDialogueInput) //find current player dialogue
    //        {
    //            for (int d = 0; d < npcDialogue.dialogueConnections[i].npcResponses.Count; d++) //scroll through current dialogue responses
    //            {
    //                if (npcDialogue.dialogueConnections[i].npcResponses[d].requiredEmotion == npcMood.currentEmotion.emotionName)
    //                {
    //                    return npcDialogue.dialogueConnections[i].npcResponses[d].response;
    //                }
    //            }
    //        }
    //    }

    //    return RespondBasedOnClosestMood(playerDialogueInput);
    //}

    //public NPCDialogueOption RespondBasedOnClosestMood(PlayerDialogueOption playerDialogueInput)
    //{

    //    for (int i = 0; i < npcDialogue.dialogueConnections.Count; i++) //cycle through dialogue options
    //    {
    //        if (npcDialogue.dialogueConnections[i].playerDialogueInput == playerDialogueInput) //find current player dialogue
    //        {
    //            for (int d = 0; d < npcDialogue.dialogueConnections[i].npcResponses.Count; d++) //scroll through current dialogue responses
    //            {
    //                if (npcDialogue.dialogueConnections[i].npcResponses[d].requiredEmotion == npcMood.currentEmotion.emotionName)
    //                {
    //                    return npcDialogue.dialogueConnections[i].npcResponses[d].response;
    //                }
    //            }
    //        }
    //    }

    //    return default;
    //}
}