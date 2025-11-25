/* ScriptReadingData.cs
 * 
 * ScriptableObject container for theatrical script content.
 * Used by ScriptReadingMinigame to display scripts to the player.
 * 
 * How to use in Unity:
 * 1. Right-click in Project window
 * 2. Create > Minigames > Script Reading Data
 * 3. Fill in script title, scene name, and content
 * 4. Assign to ScriptReadingMinigame component
 * 
 * Design Philosophy:
 * - Reusable script content across multiple scenes
 * - Easy to edit in Inspector
 * - Can be referenced by multiple minigames
 * - Supports rich text formatting for theatrical presentation
 */

using UnityEngine;

/// <summary>
/// ScriptableObject data container for theatrical script content.
/// Contains the script that players must study before performing.
/// </summary>
[CreateAssetMenu(fileName = "New Script", menuName = "Minigames/Script Reading Data", order = 1)]
public class ScriptReadingData : ScriptableObject
{
    [Header("Script Identification")]
    [Tooltip("Title of this script (e.g., 'Scene 1: The Angry Patron')")]
    public string scriptTitle = "Untitled Script";
    
    [Tooltip("Scene or act name (e.g., 'Act I, Scene 2')")]
    public string sceneName = "";
    
    [Header("Script Content")]
    [TextArea(10, 20)]
    [Tooltip("The actual script text that the player will study")]
    public string scriptContent = "";
    
    [Header("Presentation Settings")]
    [Tooltip("How long the script should remain on screen (0 = wait for player input)")]
    [Range(0f, 60f)]
    public float displayDuration = 0f;
    
    [Tooltip("Should this script use rich text formatting?")]
    public bool useRichText = true;
    
    [Header("Metadata")]
    [Tooltip("Brief description of what this script teaches (for editor reference)")]
    [TextArea(2, 4)]
    public string editorNotes = "";
    
    [Tooltip("Difficulty rating (1-5, for editor reference)")]
    [Range(1, 5)]
    public int difficultyRating = 1;
    
    [Header("Validation")]
    [SerializeField] private bool validated = false;
    
    /// <summary>
    /// Validate this script data
    /// </summary>
    public bool IsValid()
    {
        if (string.IsNullOrWhiteSpace(scriptTitle))
        {
            Debug.LogError($"[{name}] Script title is empty!");
            return false;
        }
        
        if (string.IsNullOrWhiteSpace(scriptContent))
        {
            Debug.LogError($"[{name}] Script content is empty!");
            return false;
        }
        
        validated = true;
        return true;
    }
    
    /// <summary>
    /// Get formatted script text for display
    /// </summary>
    public string GetFormattedScript()
    {
        string formatted = "";
        
        // Add title
        if (!string.IsNullOrEmpty(scriptTitle))
        {
            if (useRichText)
                formatted += $"<b><size=18>{scriptTitle}</size></b>\n";
            else
                formatted += $"{scriptTitle}\n";
        }
        
        // Add scene name
        if (!string.IsNullOrEmpty(sceneName))
        {
            if (useRichText)
                formatted += $"<i>{sceneName}</i>\n";
            else
                formatted += $"{sceneName}\n";
        }
        
        // Add separator
        formatted += "\n---\n\n";
        
        // Add content
        formatted += scriptContent;
        
        return formatted;
    }
    
    /// <summary>
    /// Get word count for difficulty estimation
    /// </summary>
    public int GetWordCount()
    {
        if (string.IsNullOrWhiteSpace(scriptContent))
            return 0;
        
        return scriptContent.Split(new char[] { ' ', '\n', '\r', '\t' }, 
            System.StringSplitOptions.RemoveEmptyEntries).Length;
    }
    
    /// <summary>
    /// Get estimated reading time in seconds (based on average reading speed)
    /// </summary>
    public float GetEstimatedReadingTime()
    {
        // Average reading speed: ~200 words per minute
        float wordsPerSecond = 200f / 60f;
        return GetWordCount() / wordsPerSecond;
    }
    
    [ContextMenu("Validate Script")]
    private void ValidateScript()
    {
        if (IsValid())
        {
            Debug.Log($"[{name}] Script is valid!");
            Debug.Log($"Word count: {GetWordCount()}");
            Debug.Log($"Estimated reading time: {GetEstimatedReadingTime():F1} seconds");
        }
    }
    
    [ContextMenu("Print Formatted Script")]
    private void PrintFormattedScript()
    {
        Debug.Log($"=== {name} ===\n{GetFormattedScript()}");
    }
}

/* 
 * EXAMPLE SCRIPT CONTENT:
 * 
 * Title: "The Angry Patron"
 * Scene Name: "Act I, Scene 3"
 * 
 * Script Content:
 * ---
 * In this scene, an angry patron confronts you about poor service.
 * Your goal is to CALM THE SITUATION by:
 * 
 * 1. ACKNOWLEDGING their complaint
 * 2. SHOWING EMPATHY for their frustration
 * 3. OFFERING A SOLUTION to make things right
 * 
 * Remember: Stay calm and professional. Don't dismiss their concerns
 * or show indifference. The audience is watching how you handle conflict!
 * 
 * Key phrases to remember:
 * - "I understand your frustration..."
 * - "Let me personally ensure..."
 * - "I apologize for the inconvenience..."
 * ---
 * 
 * USAGE IN DIALOG TREE:
 * 
 * Node 1: "Director: Here's the script for your next scene. Study it carefully!"
 *   └─ End Event: StartScriptReadingEvent (with this ScriptReadingData)
 * 
 * Node 2: "Director: Ready? Break a leg!"
 *   └─ End Event: StartCalmDialogEvent (tests knowledge of this script)
 */
