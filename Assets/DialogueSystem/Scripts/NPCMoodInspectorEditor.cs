//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;

////Create and edit npc emotions in the inspector
//# if UNITY_EDITOR
//[CustomEditor(typeof(NPCInfo))]
//public class NPCMoodInspectorEditor : Editor
//{
//    NPCMood npcMood;

//    public NPCInfo[] npcPresets;

//    public void Awake()
//    {
//        ManageNPCPresetsList();
//    }

//    public override void OnInspectorGUI()
//    {
//        base.OnInspectorGUI();

//        NPCInfo npc = (NPCInfo)target;
//        if (npc == null) return;

//        npcMood = npc.npcMood;

//        GUILayout.Space(5f);


//        GUILayout.Label("Create New Emotions for Each NPC");

//        GUILayout.BeginHorizontal();

//        npcMood.newEmotionName = GUILayout.TextField(npcMood.newEmotionName, 25);

//        if (GUILayout.Button("Create New Emotion"))
//        {
//            // Create a new emotion and add it to the list
//            NPCMood.Emotion newEmotion = CreateNewEmotion(npcMood.newEmotionName);
//            AddEmotionToList(newEmotion);

//            //Add the emotion for all other npc's
//            SetCreatedEmotionsToAllOtherNPC();

//            NPCMood.Personality newPersoanlityValues = CreatePersonalityThresholdsPerEmotion(newEmotion);
//            AddPersonalityThresholdsToList(newPersoanlityValues);

//            AddPersonalityThresholdsForAllOtherNPC();

//            UpdateCurrentEmotion();

//            // reset text field
//            npcMood.newEmotionName = "New Emotion";

//        }
//        GUILayout.EndHorizontal();

//        GUILayout.Space(5f);

//        GUILayout.BeginHorizontal();

//        GUILayout.Label("Set Emotions & Personality for all NPCs");

//        if (GUILayout.Button("Set Emotions"))
//        {
//            SetCreatedEmotionsToAllOtherNPC();

//            AddPersonalityThresholdsForAllOtherNPC();
//        }
//        GUILayout.EndHorizontal();

//        GUILayout.Space(5f);


//        GUILayout.BeginHorizontal();

//        GUILayout.Label("Reset Personality values to current Emotions");

//        if (GUILayout.Button("Reset Personality Range"))
//        {
//            for (int i = 0; i < npcPresets.Length; i++)
//            {
//                npcPresets[i].npcMood.listOfPersonalityThresholds.Clear();

//                if (npcPresets[i] == target)
//                {
//                    foreach (NPCMood.Emotion emotion in npcPresets[i].npcMood.listOfEmotions)
//                    {
//                        NPCMood.Personality newPersoanlityValues = CreatePersonalityThresholdsPerEmotion(emotion);
//                        AddPersonalityThresholdsToList(newPersoanlityValues);


//                    }
//                }
//            }

//            AddPersonalityThresholdsForAllOtherNPC();
//        }
//        GUILayout.EndHorizontal();

//        GUILayout.Space(5f);

//        GUILayout.BeginHorizontal();

//        GUILayout.Label("Send Emotion Values To Personality Thresholds");

//        if (GUILayout.Button("Set Personality Emotion Values"))
//        {
//            UpdateCurrentEmotion();

//        }
//        GUILayout.EndHorizontal();

//        //GUILayout.BeginVertical();

//        GUILayout.Label("Increase & Decrease Emotions");

//        foreach (NPCMood.Emotion emotion1 in npc.npcMood.listOfEmotions)
//        {
//            GUILayout.BeginHorizontal();

//            if (GUILayout.Button("Increase value"))
//            {
//                emotion1.emotionValue++;

//                UpdateCurrentEmotion();

//            }

//            GUILayout.Label(emotion1.emotionName);

//            if (GUILayout.Button("Decrease value"))
//            {
//                emotion1.emotionValue--;

//                UpdateCurrentEmotion();
//            }
//            GUILayout.EndHorizontal();


//        }
//       // GUILayout.EndVertical();
//    }

//    public void UpdateCurrentEmotion()
//    {
//        UpdateEmotionThresholdValues();

//        CheckEmotionThresholds();
//    }
//    public NPCMood.Emotion CreateNewEmotion(string emotionName)
//    {
//        float emotionValue = 0;

//        NPCMood.Emotion newEmotion = new NPCMood.Emotion();
//        newEmotion.emotionName = emotionName;
//        newEmotion.emotionValue = emotionValue;

//        return newEmotion;
//    }

//    public void AddEmotionToList(NPCMood.Emotion newEmotion)
//    {
//        npcMood.listOfEmotions.Add(newEmotion);

//    }

//    public void ManageNPCPresetsList()
//    {
//        NPCInfo[] npcPresetsInFolder = Resources.FindObjectsOfTypeAll<NPCInfo>();

//        npcPresets = npcPresetsInFolder;
//    }

//    public void SetCreatedEmotionsToAllOtherNPC()
//    {
//        foreach (NPCInfo npc in npcPresets)
//        {
//            for (int i = 0; i < npcPresets.Length; i++)
//            {
//                foreach (NPCMood.Emotion emotion in npcPresets[i].npcMood.listOfEmotions)
//                {
//                    if (!npc.npcMood.listOfEmotions.Contains(emotion))
//                    {
//                        npc.npcMood.listOfEmotions.Add(emotion);
//                    }
//                }
//            }
//            RemoveDuplicateEmotions(npc);
//        }
//    }

//    public void RemoveDuplicateEmotions(NPCInfo npc)
//    {
//        // cycle through the list of emotions twice
//        for (int i = 0; i < npc.npcMood.listOfEmotions.Count; i++)
//        {
//            for (int e = 0; e < npc.npcMood.listOfEmotions.Count; e++)
//            {
//                if (e != i && npc.npcMood.listOfEmotions[i].emotionName == npc.npcMood.listOfEmotions[e].emotionName) 
//                {
//                    // if two elements have the same name remove the last
//                    npc.npcMood.listOfEmotions.Remove(npc.npcMood.listOfEmotions[e]);
//                }

//            }
//        }

//    }

//    public NPCMood.Personality CreatePersonalityThresholdsPerEmotion(NPCMood.Emotion emotion)
//    {
//        NPCMood.Personality personalityValues = new NPCMood.Personality();
//        personalityValues.emotion = emotion;
//        personalityValues.baseValue = 0f;
//        personalityValues.minThreshold = 10f;
//        personalityValues.midThreshold = 25f;
//        personalityValues.maxThreshold = 50f;

//        return personalityValues;
//    }

//    public void AddPersonalityThresholdsToList(NPCMood.Personality newPersonalityValues)
//    {
//        npcMood.listOfPersonalityThresholds.Add(newPersonalityValues);
//    }

//    public void AddPersonalityThresholdsForAllOtherNPC()
//    {
//        foreach (NPCInfo npc in npcPresets)
//        {
//            for (int i = 0; i < npcPresets.Length; i++)
//            {
//                foreach (NPCMood.Personality personalityValue in npcPresets[i].npcMood.listOfPersonalityThresholds)
//                {
//                    if (!npc.npcMood.listOfPersonalityThresholds.Contains(personalityValue))
//                    {
//                        npc.npcMood.listOfPersonalityThresholds.Add(personalityValue);
//                    }
//                }
//            }
//            RemoveDuplicatePersonalityValues(npc);
//        }
//    }

//    public void RemoveDuplicatePersonalityValues(NPCInfo npc)
//    {

//        for (int i = 0; i < npc.npcMood.listOfPersonalityThresholds.Count; i++)
//        {
//            for (int e = 0; e < npc.npcMood.listOfPersonalityThresholds.Count; e++)
//            {
//                if (e != i && npc.npcMood.listOfPersonalityThresholds[i].emotion == npc.npcMood.listOfPersonalityThresholds[e].emotion)
//                {
//                    npc.npcMood.listOfPersonalityThresholds.Remove(npc.npcMood.listOfPersonalityThresholds[e]);
//                }

//            }
//        }

//    }

//    public void CheckEmotionThresholds()
//    {
        
//        foreach (NPCInfo npc in npcPresets)
//        {
//            for (int i = 0; i < npc.npcMood.listOfPersonalityThresholds.Count; i++)
//            {
//                npc.npcMood.listOfPersonalityThresholds[i].currentElementNum = i;
//            }

//            npc.npcMood.listOfPersonalityThresholds.Sort(new EmotionComparer());


//            npc.npcMood.currentEmotion = npc.npcMood.listOfPersonalityThresholds[0].emotion;
//        }

//    }

//    public void UpdateEmotionThresholdValues()
//    {
//        for (int i = 0; i < npcPresets.Length; i++)
//        {
//            foreach (NPCMood.Personality personality in npcPresets[i].npcMood.listOfPersonalityThresholds)
//            {
//                foreach (NPCMood.Emotion emotion in npcPresets[i].npcMood.listOfEmotions)
//                {
//                    if (personality.emotion.emotionName == emotion.emotionName)
//                    {
//                        personality.emotion.emotionValue = emotion.emotionValue;
//                    }
//                }

//                UpdatePersonalityThreshold(personality);   
//            }
//        }
//    }

//    static int UpdatePersonalityThreshold(NPCMood.Personality personality)
//    {
//        if (personality.emotion.emotionValue < personality.minThreshold)
//        {
//            return personality.emotion.currentThreshold = 0;
//        }
//        else if (personality.emotion.emotionValue >= personality.minThreshold && personality.emotion.emotionValue < personality.midThreshold) // check if emotion 1 is in it's min threshold
//        {
//            return personality.emotion.currentThreshold = 1;
//        }
//        else if (personality.emotion.emotionValue >= personality.midThreshold && personality.emotion.emotionValue < personality.maxThreshold) // check if emotion 1 is in it's mid threshold
//        {
//            return personality.emotion.currentThreshold = 2;
//        }
//        else if (personality.emotion.emotionValue >= personality.maxThreshold)
//        {
//            return personality.emotion.currentThreshold = 3;
//        }

//        return personality.emotion.currentThreshold = 0;
//    }

//    public void DoubleCheckHighestEmotion()
//    {
//        foreach (NPCInfo npc in npcPresets)
//        {
//            if (npc.npcMood.listOfEmotions[0].currentThreshold < npc.npcMood.listOfEmotions[1].currentThreshold ||
//                npc.npcMood.listOfEmotions[0].currentThreshold == npc.npcMood.listOfEmotions[1].currentThreshold &&
//                npc.npcMood.listOfEmotions[0].emotionValue < npc.npcMood.listOfEmotions[0].emotionValue)
//            {
//                UpdateEmotionThresholdValues();

//                CheckEmotionThresholds();
//            }
//        }
//    }

//    static NPCMood.Emotion SetCurrentEmotionBasedOnThreshold(NPCMood.Personality p1, NPCMood.Personality p2)
//    {
//        float p1EmotionValue = p1.emotion.emotionValue;
//        float p2EmotionValue = p2.emotion.emotionValue;

//        if (p1.emotion.currentThreshold == 1) // check if emotion 1 is in it's min threshold
//        {
//            if (p2.emotion.currentThreshold == 1) // check if emotion 2 is in it's min threshold
//            {
//                // Check which has higher emotion value
//                if (p1.emotion.emotionValue > p2.emotion.emotionValue)
//                {
//                    return p1.emotion;
//                }
//                else if (p1.emotion.emotionValue > p2.emotion.emotionValue)
//                {
//                    return p2.emotion;
//                }
//                else
//                {
//                    return p1.emotion;
//                }
//            }
//            else if (p2EmotionValue <= p2.minThreshold) // if emotion 2 isnt in it's min threshold return emotion 1
//            {
//                return p2.emotion;
//            }
//            else if (p2EmotionValue >= p2.midThreshold) // if emotion 2 is above it's min threshold return emotion 2
//            {
//                return p1.emotion;
//            }

//        }
//        else if (p1EmotionValue >= p1.midThreshold && p1EmotionValue < p1.maxThreshold) // check if emotion 1 is in it's mid threshold
//        {
//            p1.emotion.currentThreshold = 2;

//            if (p2EmotionValue >= p2.midThreshold && p2EmotionValue < p2.maxThreshold) // check if emotion 2 is in it's mid threshold
//            {
//                // Check which has higher emotion value
//                if (p1.emotion.emotionValue > p2.emotion.emotionValue)
//                {
//                    return p1.emotion;
//                }
//                else if (p1.emotion.emotionValue > p2.emotion.emotionValue)
//                {
//                    return p2.emotion;
//                }
//                else
//                {
//                    return p1.emotion;
//                }
//            }
//            else if (p2EmotionValue <= p2.midThreshold) // if emotion 2 isnt in it's mid threshold return emotion 1
//            {
//                return p2.emotion;

//            }
//            else if (p2EmotionValue >= p2.maxThreshold) // if emotion 2 is above it's mid threshold return emotion 2
//            {
//                return p1.emotion;

//            }
//        }
//        else // otherwise if emotion 1 is in it's max threshold
//        {
//            p1.emotion.currentThreshold = 3;

//            if (p2EmotionValue >= p2.maxThreshold) // check if emotion 2 is in it's max threshold
//            {
//                // Check which has higher emotion value
//                if (p1.emotion.emotionValue > p2.emotion.emotionValue)
//                {
//                    return p1.emotion;
//                }
//                else if (p1.emotion.emotionValue > p2.emotion.emotionValue)
//                {
//                    return p2.emotion;
//                }
//                else
//                {
//                    return p1.emotion;
//                }
//            }
//            else
//            {
//                return p1.emotion;
//            }
//        }

//        return p1.emotion;
//        //return p2.emotion.emotionValue.CompareTo(p1.emotion.emotionValue);
//    }

//    static int SortEmotionsByValue(NPCMood.Emotion e1, NPCMood.Emotion e2)
//    {
//        return e1.emotionValue.CompareTo(e2.emotionValue);
//    }

//    static int SortPersonalityByValue(NPCMood.Personality p1, NPCMood.Personality p2)
//    {
//        return p1.emotion.emotionValue.CompareTo(p2.emotion.emotionValue);
//    }

//    static int SortPersonalityByThreshold(NPCMood.Personality p1, NPCMood.Personality p2)
//    {
//        float p1EmotionValue = p1.emotion.emotionValue;
//        float p2EmotionValue = p2.emotion.emotionValue;

//        if (p1.emotion.currentThreshold == 1) // check if emotion 1 is in it's min threshold
//        {
//            if (p2.emotion.currentThreshold == 1) // check if emotion 2 is in it's min threshold
//            {
//                return p2.emotion.emotionValue.CompareTo(p1.emotion.emotionValue); // if both are in same threshold then compare the two
//            }
//            else if (p2EmotionValue <= p2.minThreshold) // if emotion 2 isnt in it's min threshold return emotion 1
//            {
//                return (int)p1.emotion.emotionValue;
//            }
//            else if (p2EmotionValue >= p2.midThreshold) // if emotion 2 is above it's min threshold return emotion 2
//            {
//                return (int)p2.emotion.emotionValue;
//            }

//        }
//        else if (p1EmotionValue >= p1.midThreshold && p1EmotionValue < p1.maxThreshold) // check if emotion 1 is in it's mid threshold
//        {
//            p1.emotion.currentThreshold = 2;

//            if (p2EmotionValue >= p2.midThreshold && p2EmotionValue < p2.maxThreshold) // check if emotion 2 is in it's mid threshold
//            {
//                return p2.emotion.emotionValue.CompareTo(p1.emotion.emotionValue); // if both are in same threshold then compare the two
//            }
//            else if (p2EmotionValue <= p2.midThreshold) // if emotion 2 isnt in it's mid threshold return emotion 1
//            {
//                return (int)p1.emotion.emotionValue;
//            }
//            else if (p2EmotionValue >= p2.maxThreshold) // if emotion 2 is above it's mid threshold return emotion 2
//            {
//                return (int)p2.emotion.emotionValue;
//            }
//        }
//        else // otherwise if emotion 1 is in it's max threshold
//        {
//            p1.emotion.currentThreshold = 3;

//            if (p2EmotionValue >= p2.maxThreshold) // check if emotion 2 is in it's max threshold
//            {
//                return p2.emotion.emotionValue.CompareTo(p1.emotion.emotionValue); // if both are in same threshold then compare the two
//            }
//            else
//            {
//                return (int)p1.emotion.emotionValue;
//            }
//        }


//        return p2.emotion.emotionValue.CompareTo(p1.emotion.emotionValue);
//    }

//}
//#endif

//public class EmotionComparer : IComparer<NPCMood.Personality>
//{
//    public int Compare(NPCMood.Personality p1, NPCMood.Personality p2)
//    {
//        float p1EmotionValue = p1.emotion.emotionValue;
//        float p2EmotionValue = p2.emotion.emotionValue;

//        if (p1.emotion.currentThreshold == 0)
//        {
//            if (p2.emotion.currentThreshold == 0) // check if emotion 2 is in it's min threshold
//            {
//                int emotionComparison = p2.emotion.emotionValue.CompareTo(p1.emotion.emotionValue);
//                if (emotionComparison != 0)
//                {
//                    return emotionComparison;
//                } // if both are in same threshold then compare the two
//                else
//                {
//                    return p1.currentElementNum;
//                }
//            }
//            else if (p2EmotionValue >= p2.minThreshold) // if emotion 2 is above it's min threshold return emotion 2
//            {
//                return p2.currentElementNum;
//            }
//        }
//        else if (p1.emotion.currentThreshold == 1) // check if emotion 1 is in it's min threshold
//        {
//            if (p2.emotion.currentThreshold == 1) // check if emotion 2 is in it's min threshold
//            {
//                int emotionComparison = p2.emotion.emotionValue.CompareTo(p1.emotion.emotionValue);
//                if (emotionComparison != 0)
//                {
//                    return emotionComparison;
//                } // if both are in same threshold then compare the two
//                else
//                {
//                    return p1.currentElementNum;
//                }
//            }
//            else if (p2EmotionValue <= p2.minThreshold) // if emotion 2 isnt in it's min threshold return emotion 1
//            {
//                return p1.currentElementNum;
//            }
//            else if (p2EmotionValue >= p2.midThreshold) // if emotion 2 is above it's min threshold return emotion 2
//            {
//                return p2.currentElementNum;
//            }

//        }
//        else if (p1EmotionValue >= p1.midThreshold && p1EmotionValue < p1.maxThreshold) // check if emotion 1 is in it's mid threshold
//        {

//            if (p2EmotionValue >= p2.midThreshold && p2EmotionValue < p2.maxThreshold) // check if emotion 2 is in it's mid threshold
//            {
//                int emotionComparison = p2.emotion.emotionValue.CompareTo(p1.emotion.emotionValue);
//                if (emotionComparison != 0)
//                {
//                    return emotionComparison;
//                } // if both are in same threshold then compare the
//                else
//                {
//                    return p1.currentElementNum;
//                }
//            }
//            else if (p2EmotionValue <= p2.midThreshold) // if emotion 2 isnt in it's mid threshold return emotion 1
//            {
//                return p1.currentElementNum;
//            }
//            else if (p2EmotionValue >= p2.maxThreshold) // if emotion 2 is above it's mid threshold return emotion 2
//            {
//                return p2.currentElementNum;
//            }
//        }
//        else // otherwise if emotion 1 is in it's max threshold
//        {
//            if (p2EmotionValue >= p2.maxThreshold) // check if emotion 2 is in it's max threshold
//            {
//                int emotionComparison = p2.emotion.emotionValue.CompareTo(p1.emotion.emotionValue);
//                if (emotionComparison != 0)
//                {
//                    return emotionComparison;
//                }
//                else
//                {
//                    return p1.currentElementNum;
//                }
//                // if both are in same threshold then compare the two
//            }
//            else
//            {
//                return p1.currentElementNum;
//            }
//        }

//        return p1.currentElementNum;

//    }
//}