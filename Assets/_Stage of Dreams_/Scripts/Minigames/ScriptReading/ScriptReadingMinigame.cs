/* ScriptReadingMinigame.cs
 * 
 * Displays a theatrical script for the player to study before performing.
 * Can be triggered from dialog events or scene start.
 * 
 * How to use in Unity:
 * 1. Attach to a GameObject in your scene
 * 2. Assign a ScriptReadingData asset with the script content
 * 3. Configure UI references (will use UI Toolkit)
 * 4. Call ShowScript() to display, or trigger from DialogEvent
 * 
 * Integration:
 * - Can be called from StartScriptReadingEvent in dialog trees
 * - Blocks dialog progression until player dismisses (optional)
 * - Tracks whether script has been read
 * - Can be reused for multiple scripts in a scene
 */

using UnityEngine;
using UnityEngine.InputSystem;
using System;

/// <summary>
/// Manages the Script Reading minigame - displays theatrical scripts for player to study.
/// Integrates with Dialog System via MinigameDialogEvents.
/// </summary>
public class ScriptReadingMinigame : MonoBehaviour
{
    #region Inspector Fields
    [Header("Script Configuration")]
    [SerializeField] private ScriptReadingData scriptData;
    [SerializeField] private bool showOnSceneStart = false;
    [SerializeField] private float autoShowDelay = 0.5f;
    
    [Header("UI References (TBD - Will use UI Toolkit)")]
    // [SerializeField] private UIDocument scriptUIDocument;
    // [SerializeField] private VisualTreeAsset scriptVisualTree;
    
    [Header("Input Configuration")]
    [SerializeField] private string dismissInputAction = "Interact"; // E key or similar
    [SerializeField] private bool allowSkipWithAnyKey = false;
    
    [Header("Player Movement")]
    [SerializeField] private bool disablePlayerMovement = true;
    
    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = true;
    #endregion
    
    #region State
    private bool isDisplaying = false;
    private bool hasBeenRead = false;
    private bool blockingDialog = false;
    private PlayerScript playerScript;
    private PlayerInput playerInput;
    private InputAction dismissAction;
    #endregion
    
    #region Events
    public event Action<ScriptReadingData> OnScriptShown;
    public event Action<ScriptReadingData> OnScriptDismissed;
    public event Action<ScriptReadingData> OnScriptRead; // Fires first time script is read
    #endregion
    
    #region Properties
    public bool IsDisplaying => isDisplaying;
    public bool HasBeenRead => hasBeenRead;
    public ScriptReadingData CurrentScript => scriptData;
    #endregion
    
    #region Initialization
    private void Start()
    {
        ValidateSetup();
        InitializeReferences();
        
        if (showOnSceneStart && scriptData != null)
        {
            StartCoroutine(ShowScriptAfterDelay(autoShowDelay));
        }
    }
    
    private void ValidateSetup()
    {
        if (scriptData == null)
        {
            LogWarning("No ScriptReadingData assigned! Script will not display.");
        }
        
        LogDebug("ScriptReadingMinigame initialized");
    }
    
    private void InitializeReferences()
    {
        // Find player references
        if (disablePlayerMovement)
        {
            playerScript = FindFirstObjectByType<PlayerScript>();
            if (playerScript == null)
            {
                LogWarning("PlayerScript not found - cannot disable movement");
            }
        }
        
        // Find input references
        playerInput = FindFirstObjectByType<PlayerInput>();
        if (playerInput != null)
        {
            dismissAction = playerInput.actions[dismissInputAction];
            if (dismissAction == null)
            {
                LogWarning($"Input action '{dismissInputAction}' not found");
            }
        }
        else
        {
            LogWarning("PlayerInput not found - will use fallback input");
        }
    }
    #endregion
    
    #region Script Display Control
    /// <summary>
    /// Show the script to the player
    /// </summary>
    /// <param name="blockDialog">If true, blocks dialog progression until dismissed</param>
    public void ShowScript(bool blockDialog = false)
    {
        if (isDisplaying)
        {
            LogWarning("Script is already displaying");
            return;
        }
        
        if (scriptData == null)
        {
            LogError("Cannot show script - no ScriptReadingData assigned!");
            return;
        }
        
        LogDebug($"Showing script: '{scriptData.scriptTitle}' (Block dialog: {blockDialog})");
        
        isDisplaying = true;
        blockingDialog = blockDialog;
        
        // Disable player movement
        if (disablePlayerMovement && playerScript != null)
        {
            playerScript.DisableMovement();
            LogDebug("Player movement disabled");
        }
        
        // Display UI (TODO: Implement UI Toolkit version)
        DisplayScriptUI();
        
        // Fire event
        OnScriptShown?.Invoke(scriptData);
    }
    
    /// <summary>
    /// Show a specific script (override current ScriptReadingData)
    /// </summary>
    public void ShowScript(ScriptReadingData data, bool blockDialog = false)
    {
        if (data == null)
        {
            LogError("Cannot show script - data is null!");
            return;
        }
        
        scriptData = data;
        ShowScript(blockDialog);
    }
    
    /// <summary>
    /// Dismiss the current script
    /// </summary>
    public void DismissScript()
    {
        if (!isDisplaying)
        {
            LogWarning("No script is currently displaying");
            return;
        }
        
        LogDebug("Dismissing script");
        
        isDisplaying = false;
        
        // Mark as read (first time only)
        if (!hasBeenRead)
        {
            hasBeenRead = true;
            OnScriptRead?.Invoke(scriptData);
            LogDebug("Script marked as read for the first time");
        }
        
        // Hide UI
        HideScriptUI();
        
        // Re-enable player movement
        if (disablePlayerMovement && playerScript != null)
        {
            playerScript.EnableMovement();
            LogDebug("Player movement re-enabled");
        }
        
        // Fire event
        OnScriptDismissed?.Invoke(scriptData);
        
        // Clear blocking flag
        blockingDialog = false;
    }
    
    /// <summary>
    /// Check if the script is currently blocking dialog progression
    /// </summary>
    public bool IsBlockingDialog()
    {
        return isDisplaying && blockingDialog;
    }
    #endregion
    
    #region UI Display (Placeholder - Will use UI Toolkit)
    private void DisplayScriptUI()
    {
        // TODO: Implement UI Toolkit version in Phase 2
        // For now, just log to console
        
        LogDebug("=== SCRIPT DISPLAY ===");
        LogDebug($"Title: {scriptData.scriptTitle}");
        LogDebug($"Scene: {scriptData.sceneName}");
        LogDebug("---");
        LogDebug(scriptData.scriptContent);
        LogDebug("---");
        LogDebug($"Press {dismissInputAction} to continue");
        LogDebug("======================");
        
        // Show placeholder UI message
        Debug.Log($"[SCRIPT]\n{scriptData.GetFormattedScript()}");
    }
    
    private void HideScriptUI()
    {
        // TODO: Implement UI Toolkit version
        LogDebug("Script UI hidden");
    }
    #endregion
    
    #region Input Handling
    private void Update()
    {
        if (!isDisplaying) return;
        
        // Check for dismiss input
        bool shouldDismiss = false;
        
        // Primary dismiss action
        if (dismissAction != null && dismissAction.WasPressedThisFrame())
        {
            shouldDismiss = true;
        }
        
        // Allow any key to skip (if enabled)
        if (allowSkipWithAnyKey && Input.anyKeyDown)
        {
            shouldDismiss = true;
        }
        
        if (shouldDismiss)
        {
            DismissScript();
        }
    }
    #endregion
    
    #region Utility Methods
    /// <summary>
    /// Reset the "has been read" flag (useful for testing or replaying)
    /// </summary>
    public void ResetReadStatus()
    {
        hasBeenRead = false;
        LogDebug("Read status reset");
    }
    
    /// <summary>
    /// Change the script data at runtime
    /// </summary>
    public void SetScriptData(ScriptReadingData data)
    {
        if (isDisplaying)
        {
            LogWarning("Cannot change script while it's being displayed");
            return;
        }
        
        scriptData = data;
        hasBeenRead = false; // Reset read status for new script
        LogDebug($"Script data changed to: {data?.scriptTitle ?? "null"}");
    }
    
    private System.Collections.IEnumerator ShowScriptAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowScript(false);
    }
    #endregion
    
    #region Logging
    private void LogDebug(string message)
    {
        if (enableDebugLogs)
            Debug.Log($"[ScriptReadingMinigame] {message}");
    }
    
    private void LogWarning(string message)
    {
        Debug.LogWarning($"[ScriptReadingMinigame] {message}");
    }
    
    private void LogError(string message)
    {
        Debug.LogError($"[ScriptReadingMinigame] {message}");
    }
    #endregion
    
    #region Editor Support
    [ContextMenu("Show Script (Test)")]
    private void EditorShowScript()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("Script display only works in play mode");
            return;
        }
        
        ShowScript(false);
    }
    
    [ContextMenu("Dismiss Script (Test)")]
    private void EditorDismissScript()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("Script dismiss only works in play mode");
            return;
        }
        
        DismissScript();
    }
    
    [ContextMenu("Reset Read Status")]
    private void EditorResetReadStatus()
    {
        ResetReadStatus();
        Debug.Log("Read status reset via context menu");
    }
    #endregion
    
    #region Cleanup
    private void OnDestroy()
    {
        // Clean up any lingering state
        if (isDisplaying)
        {
            DismissScript();
        }
    }
    #endregion
}
