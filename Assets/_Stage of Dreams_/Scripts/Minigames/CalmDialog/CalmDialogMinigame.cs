/* CalmDialogMinigame.cs
 * 
 * Manages the CalmDialog minigame - player chooses correct dialog option (1 of 3)
 * based on a script they studied.
 * 
 * How to use in Unity:
 * 1. Attach to a GameObject in your scene
 * 2. Assign a CalmDialogTree with the challenge data
 * 3. Configure spotlight trigger (optional)
 * 4. Link to DialogManager and AudienceManager
 * 5. Call StartMinigame() or trigger from DialogEvent
 * 
 * Integration:
 * - Can be triggered by spotlight entry
 * - Can be called from StartCalmDialogEvent in dialog trees
 * - Uses existing DialogManager for UI display
 * - Integrates with AudienceManager for reactions
 * - Supports retry on failure
 */

using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Manages the CalmDialog minigame - player chooses correct dialog option (1 of 3)
/// based on a script they studied at scene start.
/// Integrates with Dialog System, Spotlight System, and Audience System.
/// </summary>
public class CalmDialogMinigame : MonoBehaviour
{
    #region Inspector Fields
    [Header("Minigame Configuration")]
    [SerializeField] private CalmDialogTree minigameTree;
    [SerializeField] private bool startOnSpotlightEntry = false;
    [SerializeField] private Spotlight triggerSpotlight;
    
    [Header("Script Display")]
    [SerializeField] private ScriptReadingMinigame scriptReadingMinigame;
    [SerializeField] private bool showScriptAtSceneStart = false;
    [SerializeField] private bool requireScriptRead = true;
    
    [Header("System References")]
    [SerializeField] private DialogManager dialogManager;
    [SerializeField] private AudienceManager audienceManager;
    
    [Header("Performance Tracking (TBD)")]
    // [SerializeField] private ApplauseMeter applauseMeter;
    // [SerializeField] private BooMeter booMeter;
    
    [Header("Retry Configuration")]
    [SerializeField] private int maxRetries = 3;
    [SerializeField] private float retryDelay = 2f;
    [SerializeField] private bool allowUnlimitedRetries = true;
    
    [Header("Player Movement")]
    [SerializeField] private bool disablePlayerMovement = true;
    
    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = true;
    #endregion
    
    #region State
    private bool minigameActive = false;
    private bool minigameCompleted = false;
    private int attemptCount = 0;
    private int correctChoiceIndex = -1;
    private bool scriptHasBeenRead = false;
    private PlayerScript playerScript;
    #endregion
    
    #region Events
    public event Action<bool> OnMinigameAttempted; // bool = success
    public event Action OnMinigameCompleted;
    public event Action OnMinigameFailed;
    public event Action OnMinigameStarted;
    #endregion
    
    #region Properties
    public bool IsActive => minigameActive;
    public bool IsCompleted => minigameCompleted;
    public int AttemptCount => attemptCount;
    public CalmDialogTree CurrentTree => minigameTree;
    #endregion
    
    #region Initialization
    private void Start()
    {
        ValidateSetup();
        InitializeReferences();
        
        // Show script at scene start if configured
        if (showScriptAtSceneStart && scriptReadingMinigame != null)
        {
            scriptReadingMinigame.ShowScript(false);
        }
        
        // Subscribe to spotlight if configured
        if (startOnSpotlightEntry && triggerSpotlight != null)
        {
            triggerSpotlight.OnCharacterEnteredSpotlight += HandleSpotlightEntry;
        }
    }
    
    private void ValidateSetup()
    {
        if (minigameTree == null)
        {
            LogError("No CalmDialogTree assigned!");
            return;
        }
        
        if (!minigameTree.IsValid())
        {
            LogError("CalmDialogTree is not valid!");
            return;
        }
        
        if (requireScriptRead && scriptReadingMinigame == null)
        {
            LogWarning("Script reading required but no ScriptReadingMinigame assigned!");
        }
        
        LogDebug("CalmDialogMinigame setup validated");
    }
    
    private void InitializeReferences()
    {
        // Find DialogManager
        if (dialogManager == null)
        {
            dialogManager = DialogManager.Instance;
        }
        
        if (dialogManager == null)
        {
            LogError("DialogManager not found!");
        }
        
        // Find AudienceManager
        if (audienceManager == null)
        {
            audienceManager = FindFirstObjectByType<AudienceManager>();
        }
        
        // Find PlayerScript for movement control
        if (disablePlayerMovement)
        {
            playerScript = FindFirstObjectByType<PlayerScript>();
            if (playerScript == null)
            {
                LogWarning("PlayerScript not found - cannot disable movement");
            }
        }
        
        LogDebug("System references initialized");
    }
    #endregion
    
    #region Minigame Flow
    private void HandleSpotlightEntry(ISpotlightCharacter character)
    {
        if (character is PlayerCharacterWrapper && !minigameCompleted)
        {
            StartMinigame();
        }
    }
    
    /// <summary>
    /// Start the CalmDialog minigame
    /// </summary>
    /// <param name="skipScriptDisplay">If true, skips showing the script (assumes already read)</param>
    public void StartMinigame(bool skipScriptDisplay = false)
    {
        if (minigameActive)
        {
            LogWarning("Minigame already active");
            return;
        }
        
        if (minigameCompleted)
        {
            LogDebug("Minigame already completed");
            return;
        }
        
        // Check if script has been read (if required)
        if (requireScriptRead && !skipScriptDisplay && scriptReadingMinigame != null)
        {
            if (!scriptReadingMinigame.HasBeenRead)
            {
                LogWarning("Script must be read before starting minigame!");
                scriptReadingMinigame.ShowScript(true); // Block until read
                return;
            }
        }
        
        LogDebug($"Starting CalmDialog minigame (Attempt #{attemptCount + 1})");
        
        minigameActive = true;
        attemptCount++;
        
        // Disable player movement
        if (disablePlayerMovement && playerScript != null)
        {
            playerScript.DisableMovement();
            LogDebug("Player movement disabled");
        }
        
        // Subscribe to dialog events
        if (dialogManager != null)
        {
            dialogManager.OnDialogEnded += HandleDialogEnded;
        }
        
        // Get the correct choice index from the tree
        correctChoiceIndex = minigameTree.GetCorrectChoiceIndex();
        
        // Fire started event
        OnMinigameStarted?.Invoke();
        
        // Start the dialog with the question
        StartMinigameDialog();
    }
    
    private void StartMinigameDialog()
    {
        if (dialogManager == null || minigameTree == null)
        {
            LogError("Cannot start minigame dialog - missing references!");
            return;
        }
        
        // Get NPC wrapper from tree
        var tempNPC = minigameTree.GetAsNPCContent();
        
        if (tempNPC == null)
        {
            LogError("Failed to create NPC wrapper from CalmDialogTree!");
            return;
        }
        
        // Start the dialog
        bool success = dialogManager.StartDialog(tempNPC);
        
        if (!success)
        {
            LogError("Failed to start dialog with DialogManager!");
            minigameActive = false;
        }
    }
    
    private void HandleDialogEnded(NPCContent npc)
    {
        // Unsubscribe
        if (dialogManager != null)
        {
            dialogManager.OnDialogEnded -= HandleDialogEnded;
        }
        
        minigameActive = false;
        
        // The choice validation happens in HandleChoiceSelected
        // This is just cleanup after dialog closes
        LogDebug("Dialog ended, minigame cleaning up");
    }
    
    /// <summary>
    /// Handle player's choice selection (called externally, e.g., from DialogManager event)
    /// </summary>
    public void HandleChoiceSelected(int choiceIndex)
    {
        if (!minigameActive)
        {
            LogWarning("HandleChoiceSelected called but minigame not active!");
            return;
        }
        
        LogDebug($"Player selected choice {choiceIndex}, correct is {correctChoiceIndex}");
        
        bool isCorrect = (choiceIndex == correctChoiceIndex);
        
        OnMinigameAttempted?.Invoke(isCorrect);
        
        if (isCorrect)
        {
            HandleSuccess();
        }
        else
        {
            HandleFailure();
        }
    }
    
    private void HandleSuccess()
    {
        LogDebug("[SUCCESS] Player chose the correct dialog!");
        
        minigameCompleted = true;
        
        // Trigger positive audience reaction
        if (audienceManager != null)
        {
            audienceManager.AudienceApplause(8); // Heavy applause
            audienceManager.AudienceReaction("cheer");
        }
        
        // Update meters (when implemented)
        // applauseMeter?.AddApplause(minigameTree.applauseReward);
        
        // Fire completed event
        OnMinigameCompleted?.Invoke();
        
        // Re-enable player movement
        if (disablePlayerMovement && playerScript != null)
        {
            playerScript.EnableMovement();
            LogDebug("Player movement re-enabled");
        }
        
        // Optionally play success dialog
        if (minigameTree.successTree != null)
        {
            var successNPC = CreateNPCForTree(minigameTree.successTree);
            if (successNPC != null && dialogManager != null)
            {
                dialogManager.StartDialog(successNPC);
            }
        }
    }
    
    private void HandleFailure()
    {
        LogDebug("[FAILURE] Player chose incorrect dialog!");
        
        // Trigger negative audience reaction
        if (audienceManager != null)
        {
            audienceManager.AudienceReaction("boo");
        }
        
        // Update meters (when implemented)
        // booMeter?.AddBoo(minigameTree.booPenalty);
        
        // Fire failed event
        OnMinigameFailed?.Invoke();
        
        // Check retry limit
        if (!allowUnlimitedRetries && attemptCount >= maxRetries)
        {
            LogWarning($"Max retries ({maxRetries}) reached! Minigame failed permanently.");
            HandlePermanentFailure();
            return;
        }
        
        // Optionally play failure dialog
        if (minigameTree.failureTree != null)
        {
            var failureNPC = CreateNPCForTree(minigameTree.failureTree);
            if (failureNPC != null && dialogManager != null)
            {
                dialogManager.StartDialog(failureNPC);
            }
        }
        
        // Schedule retry
        StartCoroutine(RetryAfterDelay());
    }
    
    private void HandlePermanentFailure()
    {
        minigameCompleted = false; // Permanently failed
        
        // Re-enable player movement
        if (disablePlayerMovement && playerScript != null)
        {
            playerScript.EnableMovement();
        }
        
        LogError("Minigame permanently failed!");
        // TODO: Handle permanent failure (game over, skip minigame, etc.)
    }
    
    private IEnumerator RetryAfterDelay()
    {
        LogDebug($"Retrying minigame in {retryDelay} seconds...");
        yield return new WaitForSeconds(retryDelay);
        
        // Restart minigame (skip script display on retry)
        StartMinigame(true);
    }
    #endregion
    
    #region Utility Methods
    /// <summary>
    /// Create an NPCContent wrapper for a DialogTree
    /// </summary>
    private NPCContent CreateNPCForTree(DialogTree tree)
    {
        if (tree == null)
        {
            LogError("Cannot create NPC for null tree!");
            return null;
        }
        
        // Create a temporary GameObject with NPCContent component
        var tempObj = new GameObject("MinigameNPC_Temp");
        var npc = tempObj.AddComponent<NPCContent>();
        npc.npcName = "Minigame NPC";
        npc.mainDialogTree = tree;
        
        // Hide the temporary object
        tempObj.hideFlags = HideFlags.HideInHierarchy;
        
        return npc;
    }
    
    /// <summary>
    /// Reset the minigame state (for testing or replaying)
    /// </summary>
    public void ResetMinigame()
    {
        minigameCompleted = false;
        minigameActive = false;
        attemptCount = 0;
        correctChoiceIndex = -1;
        
        LogDebug("Minigame reset");
    }
    
    /// <summary>
    /// Change the minigame tree at runtime
    /// </summary>
    public void SetMinigameTree(CalmDialogTree tree)
    {
        if (minigameActive)
        {
            LogWarning("Cannot change minigame tree while minigame is active!");
            return;
        }
        
        minigameTree = tree;
        ResetMinigame();
        
        LogDebug($"Minigame tree changed to: {tree?.name ?? "null"}");
    }
    #endregion
    
    #region Logging
    private void LogDebug(string message)
    {
        if (enableDebugLogs)
            Debug.Log($"[CalmDialogMinigame] {message}");
    }
    
    private void LogWarning(string message)
    {
        Debug.LogWarning($"[CalmDialogMinigame] {message}");
    }
    
    private void LogError(string message)
    {
        Debug.LogError($"[CalmDialogMinigame] {message}");
    }
    #endregion
    
    #region Editor Support
    [ContextMenu("Start Minigame (Test)")]
    private void EditorStartMinigame()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("Minigame test only works in play mode");
            return;
        }
        
        StartMinigame();
    }
    
    [ContextMenu("Reset Minigame")]
    private void EditorResetMinigame()
    {
        ResetMinigame();
        Debug.Log("Minigame reset via context menu");
    }
    
    [ContextMenu("Simulate Correct Choice")]
    private void EditorSimulateCorrectChoice()
    {
        if (!Application.isPlaying || !minigameActive)
        {
            Debug.LogWarning("Must be in play mode with active minigame");
            return;
        }
        
        HandleChoiceSelected(correctChoiceIndex);
    }
    
    [ContextMenu("Simulate Wrong Choice")]
    private void EditorSimulateWrongChoice()
    {
        if (!Application.isPlaying || !minigameActive)
        {
            Debug.LogWarning("Must be in play mode with active minigame");
            return;
        }
        
        int wrongChoice = (correctChoiceIndex + 1) % 3;
        HandleChoiceSelected(wrongChoice);
    }
    #endregion
    
    #region Cleanup
    private void OnDestroy()
    {
        // Unsubscribe from events
        if (triggerSpotlight != null)
        {
            triggerSpotlight.OnCharacterEnteredSpotlight -= HandleSpotlightEntry;
        }
        
        if (dialogManager != null)
        {
            dialogManager.OnDialogEnded -= HandleDialogEnded;
        }
        
        // Re-enable player movement if still disabled
        if (minigameActive && disablePlayerMovement && playerScript != null)
        {
            playerScript.EnableMovement();
        }
    }
    #endregion
}
