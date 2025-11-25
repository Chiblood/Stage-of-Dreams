/* CalmDialogTree.cs
 * 
 * ScriptableObject data container for a CalmDialog minigame challenge.
 * Contains: script text, question dialog tree, correct answer index, and success/failure trees.
 * 
 * How to use in Unity:
 * 1. Right-click in Project window
 * 2. Create > Minigames > CalmDialog Tree
 * 3. Assign a DialogTree with exactly 3 choices for the question
 * 4. Set the correct choice index (0-2)
 * 5. Optionally assign success/failure follow-up dialog trees
 * 6. Assign to CalmDialogMinigame component
 * 
 * Design Philosophy:
 * - Reusable challenge content
 * - Easy to create and test in Inspector
 * - Integrates with existing DialogTree system
 * - Validation ensures correct setup
 */

using UnityEngine;

/// <summary>
/// ScriptableObject data container for a CalmDialog minigame challenge.
/// Contains: script text, question node, 3 choices (1 correct), and success/failure trees.
/// </summary>
[CreateAssetMenu(fileName = "New CalmDialog Tree", menuName = "Minigames/CalmDialog Tree", order = 2)]
public class CalmDialogTree : ScriptableObject
{
    [Header("Script (shown before minigame)")]
    [Tooltip("The script text that player studies before the challenge")]
    [TextArea(5, 10)]
    public string scriptText = "";
    
    [Tooltip("Optional: Reference to ScriptReadingData asset (alternative to scriptText field)")]
    public ScriptReadingData scriptData;
    
    [Header("Minigame Question")]
    [Tooltip("The dialog tree containing the question and exactly 3 choices")]
    public DialogTree questionTree;
    
    [Header("Correct Answer")]
    [Tooltip("Index of the correct choice (0 = first choice, 1 = second, 2 = third)")]
    [Range(0, 2)]
    public int correctChoiceIndex = 0;
    
    [Header("Performance Rewards/Penalties")]
    [Tooltip("Applause points awarded for correct choice")]
    public int applauseReward = 10;
    
    [Tooltip("Boo points added for incorrect choice")]
    public int booPenalty = 5;
    
    [Header("Follow-up Dialog Trees (Optional)")]
    [Tooltip("Dialog to play after correct choice (optional)")]
    public DialogTree successTree;
    
    [Tooltip("Dialog to play after incorrect choice (optional)")]
    public DialogTree failureTree;
    
    [Header("Metadata")]
    [Tooltip("Brief description of this challenge (for editor reference)")]
    [TextArea(2, 4)]
    public string editorNotes = "";
    
    [Tooltip("Difficulty rating (1-5, for editor reference)")]
    [Range(1, 5)]
    public int difficultyRating = 1;
    
    [Header("Validation")]
    [SerializeField] private bool validated = false;
    
    /// <summary>
    /// Validate this CalmDialogTree configuration
    /// </summary>
    public bool IsValid()
    {
        // Check script text or script data
        if (string.IsNullOrWhiteSpace(scriptText) && scriptData == null)
        {
            Debug.LogError($"[{name}] No script text or script data assigned!");
            return false;
        }
        
        // Validate script data if present
        if (scriptData != null && !scriptData.IsValid())
        {
            Debug.LogError($"[{name}] Script data is not valid!");
            return false;
        }
        
        // Check question tree
        if (questionTree == null)
        {
            Debug.LogError($"[{name}] Question tree is null!");
            return false;
        }
        
        if (!questionTree.IsValid())
        {
            Debug.LogError($"[{name}] Question tree is not valid!");
            return false;
        }
        
        // Validate that question tree has exactly 3 choices
        var startNode = questionTree.GetStartingNode();
        if (startNode == null)
        {
            Debug.LogError($"[{name}] Question tree has no starting node!");
            return false;
        }
        
        if (startNode.Choices.Count != 3)
        {
            Debug.LogError($"[{name}] Question tree must have exactly 3 choices! Currently has {startNode.Choices.Count}");
            return false;
        }
        
        // Validate correct choice index
        if (correctChoiceIndex < 0 || correctChoiceIndex > 2)
        {
            Debug.LogError($"[{name}] Correct choice index must be 0, 1, or 2! Currently: {correctChoiceIndex}");
            return false;
        }
        
        // Validate success tree if present
        if (successTree != null && !successTree.IsValid())
        {
            Debug.LogWarning($"[{name}] Success tree is not valid!");
        }
        
        // Validate failure tree if present
        if (failureTree != null && !failureTree.IsValid())
        {
            Debug.LogWarning($"[{name}] Failure tree is not valid!");
        }
        
        validated = true;
        return true;
    }
    
    /// <summary>
    /// Get the index of the correct choice
    /// </summary>
    public int GetCorrectChoiceIndex()
    {
        return correctChoiceIndex;
    }
    
    /// <summary>
    /// Get the script text (from scriptText field or scriptData asset)
    /// </summary>
    public string GetScriptText()
    {
        if (scriptData != null)
        {
            return scriptData.GetFormattedScript();
        }
        
        return scriptText;
    }
    
    /// <summary>
    /// Get this as an NPCContent wrapper (for DialogManager compatibility)
    /// </summary>
    public NPCContent GetAsNPCContent()
    {
        // Create a temporary GameObject with NPCContent component
        var tempObj = new GameObject("CalmDialogNPC_Temp");
        var npc = tempObj.AddComponent<NPCContent>();
        npc.npcName = "CalmDialog NPC";
        npc.mainDialogTree = questionTree;
        
        // Hide the temporary object
        tempObj.hideFlags = HideFlags.HideInHierarchy;
        
        return npc;
    }
    
    /// <summary>
    /// Get the question text from the starting node
    /// </summary>
    public string GetQuestionText()
    {
        if (questionTree == null)
            return "";
        
        var startNode = questionTree.GetStartingNode();
        if (startNode == null)
            return "";
        
        return startNode.DialogText;
    }
    
    /// <summary>
    /// Get all choice texts as an array
    /// </summary>
    public string[] GetChoiceTexts()
    {
        if (questionTree == null)
            return new string[0];
        
        var startNode = questionTree.GetStartingNode();
        if (startNode == null || startNode.Choices.Count != 3)
            return new string[0];
        
        string[] choices = new string[3];
        for (int i = 0; i < 3; i++)
        {
            choices[i] = startNode.Choices[i]?.ChoiceText ?? "";
        }
        
        return choices;
    }
    
    [ContextMenu("Validate Setup")]
    private void ValidateSetup()
    {
        if (IsValid())
        {
            Debug.Log($"[{name}] CalmDialogTree is valid!");
            Debug.Log($"Question: {GetQuestionText()}");
            Debug.Log($"Correct answer: Choice {correctChoiceIndex}");
            
            var choices = GetChoiceTexts();
            for (int i = 0; i < choices.Length; i++)
            {
                string marker = (i == correctChoiceIndex) ? " [CORRECT]" : "";
                Debug.Log($"  Choice {i}: {choices[i]}{marker}");
            }
        }
    }
    
    [ContextMenu("Print Challenge Details")]
    private void PrintChallengeDetails()
    {
        Debug.Log($"=== {name} ===");
        Debug.Log($"Difficulty: {difficultyRating}/5");
        Debug.Log($"Rewards: +{applauseReward} applause, +{booPenalty} boo");
        Debug.Log($"\nScript:\n{GetScriptText()}");
        Debug.Log($"\nQuestion: {GetQuestionText()}");
        
        var choices = GetChoiceTexts();
        for (int i = 0; i < choices.Length; i++)
        {
            string marker = (i == correctChoiceIndex) ? " [CORRECT]" : "";
            Debug.Log($"  [{i}] {choices[i]}{marker}");
        }
        
        Debug.Log($"\nSuccess Dialog: {(successTree != null ? successTree.name : "None")}");
        Debug.Log($"Failure Dialog: {(failureTree != null ? failureTree.name : "None")}");
    }
}

/* 
 * EXAMPLE SETUP:
 * 
 * === CalmDialog Tree: "The Angry Patron Challenge" ===
 * 
 * Script Text:
 * ---
 * Remember, in this scene you must calm the angry patron by:
 * 1. Acknowledging their complaint
 * 2. Showing empathy
 * 3. Offering a solution
 * 
 * Stay calm and professional!
 * ---
 * 
 * Question Tree:
 * Starting Node:
 *   Speaker: "Angry Patron"
 *   Text: "This is unacceptable! I've been waiting for an hour!"
 *   Choices:
 *     [0] "Sir, I understand your frustration. Let me personally ensure your order is prioritized."
 *     [1] "Well, we're very busy tonight, you'll just have to wait."
 *     [2] "I don't know what you want me to do about it."
 * 
 * Correct Choice Index: 0
 * 
 * Applause Reward: 10
 * Boo Penalty: 5
 * 
 * Success Tree: "Patron_Calmed_Success"
 * Failure Tree: "Patron_Still_Angry_Failure"
 * 
 * === USAGE IN SCENE ===
 * 
 * 1. Create CalmDialogTree asset (this file)
 * 2. Create/assign DialogTree with question and 3 choices
 * 3. Add CalmDialogMinigame component to scene GameObject
 * 4. Assign this CalmDialogTree to the component
 * 5. Trigger via DialogEvent or spotlight entry
 * 
 * === USAGE IN DIALOG TREE ===
 * 
 * Node: "Director: Time to test your skills!"
 *   └─ End Event: StartCalmDialogEvent
 *      └─ Minigame Instance: [CalmDialogMinigame in scene]
 */
