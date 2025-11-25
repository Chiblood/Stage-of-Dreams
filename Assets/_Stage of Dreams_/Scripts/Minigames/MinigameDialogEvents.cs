/* MinigameDialogEvents.cs
 * 
 * Dialog event implementations for triggering minigames from dialog trees.
 * These events can be added to DialogNode start/end events or DialogChoice events.
 * 
 * How to use:
 * 1. Add to a DialogNode's Start Events or End Events list in the Inspector
 * 2. Select "StartMinigameEvent" from the event type dropdown
 * 3. Assign the minigame MonoBehaviour reference
 * 4. Configure optional parameters (skip script display, etc.)
 * 
 * Current minigame types supported:
 * - CalmDialogMinigame (choose correct dialog option based on script)
 * - ScriptReadingMinigame (display script for player to study)
 * - (Future: DancingCombat, DramaticLock, RememberTheScript)
 */

using UnityEngine;
using System;

#region Base Minigame Event

/// <summary>
/// Base class for all minigame start events.
/// Provides common functionality for triggering minigames from dialog events.
/// </summary>
[System.Serializable]
public abstract class MinigameEventBase : DialogEvent
{
    [SerializeField] protected bool pauseDialogDuringMinigame = true;
    [SerializeField] protected bool waitForMinigameCompletion = false;
    
    public bool PauseDialogDuringMinigame
    {
        get => pauseDialogDuringMinigame;
        set => pauseDialogDuringMinigame = value;
    }
    
    public bool WaitForMinigameCompletion
    {
        get => waitForMinigameCompletion;
        set => waitForMinigameCompletion = value;
    }
    
    /// <summary>
    /// Validate that the minigame can be started
    /// </summary>
    public override bool IsValid()
    {
        return base.IsValid() && ValidateMinigameReference();
    }
    
    /// <summary>
    /// Override in derived classes to validate minigame-specific references
    /// </summary>
    protected abstract bool ValidateMinigameReference();
    
    /// <summary>
    /// Get a user-friendly name for this minigame type
    /// </summary>
    protected abstract string GetMinigameName();
    
    public override string GetDisplayName()
    {
        return $"Start Minigame: {GetMinigameName()}";
    }
}

#endregion

#region CalmDialog Minigame Event

/// <summary>
/// Event that starts a CalmDialog minigame.
/// Player must choose the correct dialog option (1 of 3) based on a script they've studied.
/// </summary>
[System.Serializable]
public class StartCalmDialogEvent : MinigameEventBase
{
    [SerializeField] private CalmDialogMinigame minigameInstance;
    [SerializeField] private bool skipScriptDisplay = false;
    
    public CalmDialogMinigame MinigameInstance
    {
        get => minigameInstance;
        set => minigameInstance = value;
    }
    
    public bool SkipScriptDisplay
    {
        get => skipScriptDisplay;
        set => skipScriptDisplay = value;
    }
    
    protected override void OnExecute()
    {
        if (minigameInstance == null)
        {
            Debug.LogError("[StartCalmDialogEvent] No CalmDialogMinigame assigned!");
            return;
        }
        
        Debug.Log($"[StartCalmDialogEvent] Starting CalmDialog minigame (Skip script: {skipScriptDisplay})");
        
        // Start the minigame
        minigameInstance.StartMinigame(skipScriptDisplay);
        
        // Optionally pause dialog system
        if (pauseDialogDuringMinigame)
        {
            // Dialog is already handled by CalmDialogMinigame
            Debug.Log("[StartCalmDialogEvent] Dialog paused during minigame");
        }
    }
    
    protected override bool ValidateMinigameReference()
    {
        if (minigameInstance == null)
        {
            Debug.LogWarning("[StartCalmDialogEvent] No CalmDialogMinigame assigned!");
            return false;
        }
        
        return true;
    }
    
    protected override string GetMinigameName()
    {
        return "CalmDialog";
    }
}

#endregion

#region Script Reading Minigame Event

/// <summary>
/// Event that displays a script for the player to study.
/// Used before CalmDialog or other minigames that require script knowledge.
/// </summary>
[System.Serializable]
public class StartScriptReadingEvent : MinigameEventBase
{
    [SerializeField] private ScriptReadingMinigame minigameInstance;
    [SerializeField] private bool blockUntilRead = true;
    
    public ScriptReadingMinigame MinigameInstance
    {
        get => minigameInstance;
        set => minigameInstance = value;
    }
    
    public bool BlockUntilRead
    {
        get => blockUntilRead;
        set => blockUntilRead = value;
    }
    
    protected override void OnExecute()
    {
        if (minigameInstance == null)
        {
            Debug.LogError("[StartScriptReadingEvent] No ScriptReadingMinigame assigned!");
            return;
        }
        
        Debug.Log($"[StartScriptReadingEvent] Starting Script Reading (Block until read: {blockUntilRead})");
        
        // Start the script reading minigame
        minigameInstance.ShowScript(blockUntilRead);
    }
    
    protected override bool ValidateMinigameReference()
    {
        if (minigameInstance == null)
        {
            Debug.LogWarning("[StartScriptReadingEvent] No ScriptReadingMinigame assigned!");
            return false;
        }
        
        return true;
    }
    
    protected override string GetMinigameName()
    {
        return "Script Reading";
    }
}

#endregion

#region Generic Minigame Event (Future-Proof)

/// <summary>
/// Generic minigame event that can start any minigame with a common interface.
/// Useful for future minigames (DancingCombat, DramaticLock, RememberTheScript).
/// </summary>
[System.Serializable]
public class StartGenericMinigameEvent : MinigameEventBase
{
    public enum MinigameType
    {
        CalmDialog,
        ScriptReading,
        DancingCombat,      // TBD
        DramaticLock,       // TBD
        RememberTheScript   // TBD
    }
    
    [SerializeField] private MinigameType minigameType = MinigameType.CalmDialog;
    [SerializeField] private MonoBehaviour minigameComponent;
    
    public MinigameType Type
    {
        get => minigameType;
        set => minigameType = value;
    }
    
    public MonoBehaviour MinigameComponent
    {
        get => minigameComponent;
        set => minigameComponent = value;
    }
    
    protected override void OnExecute()
    {
        if (minigameComponent == null)
        {
            Debug.LogError($"[StartGenericMinigameEvent] No minigame component assigned for {minigameType}!");
            return;
        }
        
        Debug.Log($"[StartGenericMinigameEvent] Starting {minigameType} minigame");
        
        // Try to start the minigame based on type
        switch (minigameType)
        {
            case MinigameType.CalmDialog:
                if (minigameComponent is CalmDialogMinigame calmDialog)
                {
                    calmDialog.StartMinigame();
                }
                else
                {
                    Debug.LogError("[StartGenericMinigameEvent] Component is not a CalmDialogMinigame!");
                }
                break;
                
            case MinigameType.ScriptReading:
                if (minigameComponent is ScriptReadingMinigame scriptReading)
                {
                    scriptReading.ShowScript(true);
                }
                else
                {
                    Debug.LogError("[StartGenericMinigameEvent] Component is not a ScriptReadingMinigame!");
                }
                break;
                
            case MinigameType.DancingCombat:
            case MinigameType.DramaticLock:
            case MinigameType.RememberTheScript:
                Debug.LogWarning($"[StartGenericMinigameEvent] {minigameType} not yet implemented!");
                break;
                
            default:
                Debug.LogError($"[StartGenericMinigameEvent] Unknown minigame type: {minigameType}");
                break;
        }
    }
    
    protected override bool ValidateMinigameReference()
    {
        if (minigameComponent == null)
        {
            Debug.LogWarning($"[StartGenericMinigameEvent] No minigame component assigned for {minigameType}!");
            return false;
        }
        
        // Validate component type matches minigame type
        switch (minigameType)
        {
            case MinigameType.CalmDialog:
                return minigameComponent is CalmDialogMinigame;
                
            case MinigameType.ScriptReading:
                return minigameComponent is ScriptReadingMinigame;
                
            case MinigameType.DancingCombat:
            case MinigameType.DramaticLock:
            case MinigameType.RememberTheScript:
                Debug.LogWarning($"[StartGenericMinigameEvent] {minigameType} validation not yet implemented!");
                return true; // Allow for future implementation
                
            default:
                return false;
        }
    }
    
    protected override string GetMinigameName()
    {
        return minigameType.ToString();
    }
}

#endregion

/* 
 * USAGE EXAMPLES:
 * 
 * === Example 1: Start CalmDialog from Dialog Node ===
 * 
 * In DialogTree inspector:
 * 1. Expand a DialogNode
 * 2. Go to "Start Events" or "End Events"
 * 3. Add new event → Select "StartCalmDialogEvent"
 * 4. Assign CalmDialogMinigame reference from scene
 * 5. Configure options (skip script display, etc.)
 * 
 * === Example 2: Show Script Before Dialog ===
 * 
 * In DialogTree inspector:
 * 1. Create a node: "Director: Here's your script for the next scene..."
 * 2. Add End Event → Select "StartScriptReadingEvent"
 * 3. Assign ScriptReadingMinigame reference
 * 4. Set "Block Until Read" to true
 * 5. Next node starts after player dismisses script
 * 
 * === Example 3: Chain Script Reading → CalmDialog ===
 * 
 * Node 1: "Director: Study this script carefully!"
 *   └─ End Event: StartScriptReadingEvent (blocks until read)
 * 
 * Node 2: "Director: Now let's see how you perform!"
 *   └─ End Event: StartCalmDialogEvent (skip script display = true)
 * 
 * === Example 4: Future-Proof Generic Event ===
 * 
 * In DialogTree inspector:
 * 1. Add Event → Select "StartGenericMinigameEvent"
 * 2. Choose minigame type from dropdown
 * 3. Assign corresponding MonoBehaviour component
 * 4. Configure common options
 * 
 * WHEN TO USE EACH EVENT TYPE:
 * 
 * StartCalmDialogEvent:
 * - When you know you want CalmDialog specifically
 * - Better type safety in Inspector
 * - Can configure CalmDialog-specific options
 * 
 * StartScriptReadingEvent:
 * - Display script before any minigame
 * - Can be used standalone or chained
 * - Blocks dialog until player dismisses
 * 
 * StartGenericMinigameEvent:
 * - When building flexible dialog trees
 * - Can change minigame type without recreating events
 * - Future-proof for new minigame types
 */
