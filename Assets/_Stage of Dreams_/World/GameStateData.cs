/* GameStateData.cs
 * Serializable game state data container for Stage of Dreams.
 * 
 * Purpose:
 * - ScriptableObject for easy inspector editing
 * - Default state configuration
 * - Save/load data structure
 * 
 * Usage:
 * 1. Create asset: Right-click → Create → Stage of Dreams → Game State Data
 * 2. Configure default values in inspector
 * 3. Assign to GameStateManager's defaultState field
 * 
 * Architecture:
 * - Data-only class (no logic)
 * - Used by GameStateManager for persistence
 * - Can create multiple configurations for different scenarios
 */

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Serializable game state data for saving/loading.
/// This is a ScriptableObject so you can create default state assets in the editor.
/// </summary>
[CreateAssetMenu(fileName = "GameStateData", menuName = "Stage of Dreams/Game State Data")]
public class GameStateData : ScriptableObject
{
    [Header("Audience Metrics")]
    [Tooltip("Current applause score (0-100 scale)")]
    public float applauseScore = 50f;
    
    [Tooltip("Current boo score (0-100 scale)")]
    public float booScore = 0f;
    
    [Tooltip("Overall audience mood (0.0 = hostile, 1.0 = enthusiastic)")]
    [Range(0f, 1f)]
    public float audienceMood = 0.5f;
    
    [Header("Performance Metrics")]
    [Tooltip("Score for current scene")]
    public int sceneScore = 0;
    
    [Tooltip("Cumulative score for current dream")]
    public int dreamScore = 0;
    
    [Tooltip("Total game score across all dreams")]
    public int totalScore = 0;
    
    [Tooltip("Number of consecutive successes")]
    public int consecutiveSuccesses = 0;
    
    [Tooltip("Number of consecutive failures")]
    public int consecutiveFailures = 0;
    
    [Header("Progression")]
    [Tooltip("IDs of completed scenes")]
    public List<string> completedScenes = new List<string>();
    
    [Tooltip("IDs of unlocked abilities")]
    public List<string> unlockedAbilities = new List<string>();
    
    [Tooltip("IDs of earned achievements")]
    public List<string> earnedAchievements = new List<string>();
    
    [Tooltip("Current dream index (0-based)")]
    public int currentDreamIndex = 0;
    
    [Tooltip("Current act index (0 = Act I, 1 = Act II, 2 = Act III)")]
    [Range(0, 2)]
    public int currentActIndex = 0;
    
    [Header("Session Data")]
    [Tooltip("Total minigames attempted this session")]
    public int totalMinigamesAttempted = 0;
    
    [Tooltip("Total minigames completed successfully this session")]
    public int totalMinigamesCompleted = 0;
    
    /// <summary>
    /// Reset all values to defaults
    /// </summary>
    [ContextMenu("Reset to Defaults")]
    public void ResetToDefaults()
    {
        applauseScore = 50f;
        booScore = 0f;
        audienceMood = 0.5f;
        sceneScore = 0;
        dreamScore = 0;
        totalScore = 0;
        consecutiveSuccesses = 0;
        consecutiveFailures = 0;
        completedScenes.Clear();
        unlockedAbilities.Clear();
        earnedAchievements.Clear();
        currentDreamIndex = 0;
        currentActIndex = 0;
        totalMinigamesAttempted = 0;
        totalMinigamesCompleted = 0;
        
        Debug.Log($"[GameStateData] '{name}' reset to defaults");
    }
    
    /// <summary>
    /// Validate data integrity
    /// </summary>
    [ContextMenu("Validate Data")]
    public void ValidateData()
    {
        bool isValid = true;
        
        // Validate ranges
        if (applauseScore < 0f || applauseScore > 100f)
        {
            Debug.LogWarning($"[GameStateData] '{name}' - Applause score out of range: {applauseScore}");
            isValid = false;
        }
        
        if (booScore < 0f || booScore > 100f)
        {
            Debug.LogWarning($"[GameStateData] '{name}' - Boo score out of range: {booScore}");
            isValid = false;
        }
        
        if (audienceMood < 0f || audienceMood > 1f)
        {
            Debug.LogWarning($"[GameStateData] '{name}' - Audience mood out of range: {audienceMood}");
            isValid = false;
        }
        
        if (currentActIndex < 0 || currentActIndex > 2)
        {
            Debug.LogWarning($"[GameStateData] '{name}' - Act index out of range: {currentActIndex}");
            isValid = false;
        }
        
        // Validate scores
        if (sceneScore < 0 || dreamScore < 0 || totalScore < 0)
        {
            Debug.LogWarning($"[GameStateData] '{name}' - Negative scores detected");
            isValid = false;
        }
        
        // Check for null lists
        if (completedScenes == null)
        {
            Debug.LogWarning($"[GameStateData] '{name}' - completedScenes list is null");
            completedScenes = new List<string>();
            isValid = false;
        }
        
        if (unlockedAbilities == null)
        {
            Debug.LogWarning($"[GameStateData] '{name}' - unlockedAbilities list is null");
            unlockedAbilities = new List<string>();
            isValid = false;
        }
        
        if (earnedAchievements == null)
        {
            Debug.LogWarning($"[GameStateData] '{name}' - earnedAchievements list is null");
            earnedAchievements = new List<string>();
            isValid = false;
        }
        
        if (isValid)
        {
            Debug.Log($"[GameStateData] '{name}' - Validation successful");
        }
        else
        {
            Debug.LogWarning($"[GameStateData] '{name}' - Validation found issues");
        }
    }
    
    /// <summary>
    /// Print current state summary
    /// </summary>
    [ContextMenu("Print Summary")]
    public void PrintSummary()
    {
        Debug.Log($"=== GameStateData: '{name}' ===");
        Debug.Log($"Applause: {applauseScore:F1} | Boo: {booScore:F1} | Mood: {audienceMood:F2}");
        Debug.Log($"Scores - Scene: {sceneScore} | Dream: {dreamScore} | Total: {totalScore}");
        Debug.Log($"Streaks - Successes: {consecutiveSuccesses} | Failures: {consecutiveFailures}");
        Debug.Log($"Progress - Dream: {currentDreamIndex} | Act: {currentActIndex + 1}");
        Debug.Log($"Completed Scenes: {completedScenes.Count}");
        Debug.Log($"Unlocked Abilities: {unlockedAbilities.Count}");
        Debug.Log($"Earned Achievements: {earnedAchievements.Count}");
        Debug.Log($"Minigames - Attempted: {totalMinigamesAttempted} | Completed: {totalMinigamesCompleted}");
    }
    
    private void OnValidate()
    {
        // Clamp values in inspector
        applauseScore = Mathf.Clamp(applauseScore, 0f, 100f);
        booScore = Mathf.Clamp(booScore, 0f, 100f);
        audienceMood = Mathf.Clamp01(audienceMood);
        currentActIndex = Mathf.Clamp(currentActIndex, 0, 2);
        sceneScore = Mathf.Max(0, sceneScore);
        dreamScore = Mathf.Max(0, dreamScore);
        totalScore = Mathf.Max(0, totalScore);
        consecutiveSuccesses = Mathf.Max(0, consecutiveSuccesses);
        consecutiveFailures = Mathf.Max(0, consecutiveFailures);
        currentDreamIndex = Mathf.Max(0, currentDreamIndex);
        totalMinigamesAttempted = Mathf.Max(0, totalMinigamesAttempted);
        totalMinigamesCompleted = Mathf.Max(0, totalMinigamesCompleted);
        
        // Ensure lists are not null
        if (completedScenes == null) completedScenes = new List<string>();
        if (unlockedAbilities == null) unlockedAbilities = new List<string>();
        if (earnedAchievements == null) earnedAchievements = new List<string>();
    }
}
