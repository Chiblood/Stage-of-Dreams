/* DialogueTrigger.cs
 * Triggers the call to the DialogManager instance to display a sequence of dialogues based on certain conditions.
 * Passes NPC Content to the DialogManager which handles the display and flow of dialogues.
 * Trigger conditions can include player proximity, spotlight detection, or interaction input.
 * 
 * How to use in Unity:
 * 1. Attach this script to a GameObject that represents an NPC or interactive object.
 * 2. Assign an NPCContent scriptable object to the 'targetNPC' field in the inspector.
 * 3. Configure trigger conditions (spotlight, interaction, proximity) as needed.
 * 4. Optionally set a specific dialog tree name if the NPC has multiple trees.
 * 5. Ensure there is a DialogManager instance in the scene to handle dialog display.
 * 6. Customize interaction prompts by overriding ShowInteractionPrompt if desired.
 * 
 */

using UnityEngine;

/// <summary>
/// Unified dialog trigger that handles multiple trigger conditions (spotlight, interaction, proximity).
/// This replaces separate trigger systems and provides a single point for dialog triggering.
/// </summary>
public class DialogueTrigger : MonoBehaviour
{
    #region Editor Fields
    [Header("Trigger Settings")]
    [SerializeField] private bool triggerOnSpotlight = false;
    [SerializeField] private bool triggerOnInteraction = false;
    [SerializeField] private bool triggerOnProximity = false;
    [SerializeField] private float interactionRange = 2f;
    [SerializeField] private float proximityRange = 1.5f;
    
    [Header("Interaction Settings")]
    [SerializeField] private bool requireInteractionInput = true;
    [SerializeField] private KeyCode interactionKey = KeyCode.E;
    
    [Header("NPC Reference")]
    [SerializeField] private NPCContent targetNPC;
    [SerializeField] private string specificDialogTree = ""; // Optional specific tree name
    
    [Header("Trigger Behavior")]
    [SerializeField] private bool canRetrigger = false; // Can trigger multiple times
    [SerializeField] private float retriggerDelay = 10f; // Delay between retriggering
    
    [Header("Debug Settings")]
    [SerializeField] private bool enableDebugLogs = true;
    #endregion

    #region Internal Fields
    // Internal state
    private DialogManager dialogManager;
    private PlayerScript player;
    private bool hasTriggered = false;
    private bool isWaitingForInput = false;
    private float lastTriggerTime = 0f;
    
    // Validation cache
    private bool isSetupValid = false;
    private string lastValidationError = "";
    
    // Events for external systems
    public System.Action OnDialogTriggered;
    public System.Action OnDialogEnded;
    #endregion

    #region Initialization

    /// <summary> Initialize references and validate setup </summary>
    private void Start()
    {
        InitializeReferences();
        ValidateSetup();
    }
    
    /// <summary>
    /// Initialize references to DialogManager and PlayerScript
    /// </summary>
    private void InitializeReferences()
    {
        dialogManager = DialogManager.Instance;
        player = FindFirstObjectByType<PlayerScript>();
        
        // Get NPC from this GameObject if not assigned
        if (targetNPC == null)
        {
            targetNPC = GetComponent<NPCContent>();
        }
    }
    #endregion

    #region Validation Methods
    /// <summary> Validate the complete setup for dialog triggering </summary>
    private void ValidateSetup()
    {
        isSetupValid = true;
        lastValidationError = "";
        
        // Check DialogManager
        if (dialogManager == null)
        {
            isSetupValid = false;
            lastValidationError = "DialogManager instance not found in scene";
            LogError($"DialogueTrigger on {gameObject.name}: {lastValidationError}");
            return;
        }
        
        // Check NPCContent
        if (targetNPC == null)
        {
            isSetupValid = false;
            lastValidationError = "No NPCContent assigned or found on GameObject";
            LogError($"DialogueTrigger on {gameObject.name}: {lastValidationError}");
            return;
        }
        
        // Validate NPC has dialog content
        if (!ValidateNPCContent())
        {
            isSetupValid = false;
            return; // Error message already set in ValidateNPCContent
        }
        
        // Check trigger conditions
        if (!triggerOnSpotlight && !triggerOnInteraction && !triggerOnProximity)
        {
            isSetupValid = false;
            lastValidationError = "No trigger conditions enabled";
            LogError($"DialogueTrigger on {gameObject.name}: {lastValidationError}");
            return;
        }
        
        // Check player reference for proximity/interaction triggers
        if ((triggerOnProximity || triggerOnInteraction) && player == null)
        {
            LogWarning($"DialogueTrigger on {gameObject.name}: PlayerScript not found - proximity/interaction triggers will not work");
        }
        
        if (enableDebugLogs)
        {
            LogDebug($"DialogueTrigger on {gameObject.name}: Setup validation passed");
        }
    }
    
    /// <summary> Validate that the NPC has the required dialog content </summary>
    private bool ValidateNPCContent()
    {
        if (targetNPC == null) return false;
        
        // Check if specific dialog tree is requested
        if (!string.IsNullOrEmpty(specificDialogTree))
        {
            var requestedTree = targetNPC.GetDialogTree(specificDialogTree);
            if (requestedTree == null)
            {
                lastValidationError = $"Specific dialog tree '{specificDialogTree}' not found on NPC '{targetNPC.npcName}'";
                LogError($"DialogueTrigger on {gameObject.name}: {lastValidationError}");
                return false;
            }
            
            if (!requestedTree.IsValid())
            {
                lastValidationError = $"Specific dialog tree '{specificDialogTree}' is not valid (no starting node)";
                LogError($"DialogueTrigger on {gameObject.name}: {lastValidationError}");
                return false;
            }
        }
        else
        {
            // Check main dialog tree
            var mainTree = targetNPC.GetMainDialogTree();
            if (mainTree == null)
            {
                lastValidationError = $"NPC '{targetNPC.npcName}' has no main dialog tree assigned";
                LogError($"DialogueTrigger on {gameObject.name}: {lastValidationError}");
                return false;
            }
            
            if (!mainTree.IsValid())
            {
                lastValidationError = $"NPC '{targetNPC.npcName}' main dialog tree is not valid (no starting node)";
                LogError($"DialogueTrigger on {gameObject.name}: {lastValidationError}");
                return false;
            }
        }
        
        return true;
    }
    
    /// <summary> Get the dialog tree that will be used for this trigger </summary>
    public DialogTree GetTargetDialogTree()
    {
        if (targetNPC == null) return null;
        
        return !string.IsNullOrEmpty(specificDialogTree) 
            ? targetNPC.GetDialogTree(specificDialogTree) 
            : targetNPC.GetMainDialogTree();
    }

    #endregion

    #region Main update loop to check trigger conditions and Passing DialogTree to DialogManager

    /// <summary> Check if the trigger is ready to fire </summary>
    public bool IsReadyToTrigger()
    {
        return isSetupValid && 
               dialogManager != null && 
               targetNPC != null && 
               !dialogManager.IsDialogActive() &&
               (!hasTriggered || canRetrigger) &&
               (!hasTriggered || Time.time - lastTriggerTime >= retriggerDelay);
    }
    private void Update()
    {
        // Early exit if not ready
        if (!IsReadyToTrigger()) return;
        
        // Check different trigger conditions
        CheckSpotlightTrigger();
        CheckInteractionTrigger();
        CheckProximityTrigger();
        CheckWaitingForInput();
    }
    
    private void CheckSpotlightTrigger()
    {
        if (!triggerOnSpotlight || player == null) return;
        
        // Check if player is specifically in THIS spotlight, not just any spotlight
        bool playerInThisSpotlight = IsPlayerInThisSpotlight();
        
        if (playerInThisSpotlight)
        {
            if (requireInteractionInput && !isWaitingForInput)
            {
                // Start waiting for input
                isWaitingForInput = true;
                ShowInteractionPrompt(true);
                LogDebug($"Press {interactionKey} to start dialog with {targetNPC?.npcName}");
            }
            else if (!requireInteractionInput)
            {
                // Immediate trigger
                PassNPCContent();
            }
        }
        else
        {
            // Player left spotlight
            if (isWaitingForInput)
            {
                isWaitingForInput = false;
                ShowInteractionPrompt(false);
            }
        }
    }
    
    /// <summary>
    /// Check if the player is specifically in this spotlight's radius
    /// </summary>
    private bool IsPlayerInThisSpotlight()
    {
        if (player == null) return false;
        
        // Get the Spotlight component on this GameObject
        Spotlight thisSpotlight = GetComponent<Spotlight>();
        if (thisSpotlight == null) return false;
        
        // Check if player is within this specific spotlight's radius
        float distance = Vector2.Distance(transform.position, player.transform.position);
        return distance <= thisSpotlight.radius;
    }
    
    private void CheckInteractionTrigger()
    {
        if (!triggerOnInteraction || player == null) return;
        
        if (IsPlayerInRange(interactionRange))
        {
            if (!isWaitingForInput)
            {
                isWaitingForInput = true;
                ShowInteractionPrompt(true);
            }
            
            if (GetInteractInput())
            {
                PassNPCContent();
            }
        }
        else
        {
            if (isWaitingForInput && triggerOnInteraction)
            {
                isWaitingForInput = false;
                ShowInteractionPrompt(false);
            }
        }
    }
    
    private void CheckProximityTrigger()
    {
        if (!triggerOnProximity || player == null) return;
        
        if (IsPlayerInRange(proximityRange))
        {
            PassNPCContent();
        }
    }
    
    private void CheckWaitingForInput()
    {
        if (!isWaitingForInput) return;
        
        if (GetInteractInput())
        {
            PassNPCContent();
        }
    }

    #endregion

    /// <summary> 
    /// Pass the NPC Content from the target NPC to the DialogManager to start the dialog
    /// This is the core method that connects DialogueTrigger to DialogManager
    /// </summary>
    private void PassNPCContent()
    {
        // Pre-validation of setup
        if (!isSetupValid)
        {
            LogError($"Cannot trigger dialog on {gameObject.name} - Setup validation failed: {lastValidationError}");
            return;
        }
        
        // Runtime validation
        if (dialogManager == null || targetNPC == null)
        {
            LogError($"Cannot trigger dialog on {gameObject.name} - DialogManager or NPC is missing");
            return;
        }
        
        // Final validation of dialog content
        if (!ValidateNPCContent())
        {
            LogError($"Cannot trigger dialog on {gameObject.name} - NPC content validation failed: {lastValidationError}");
            return;
        }
        
        // Clear waiting state
        isWaitingForInput = false;
        ShowInteractionPrompt(false);

        // Prepare NPCContent for dialog
        PrepareNPCForDialog();

        // Pass to the dialog manager with appropriate tree selection
        bool dialogStarted = false;
        if (!string.IsNullOrEmpty(specificDialogTree))
        {
            dialogStarted = TryStartDialog(targetNPC, specificDialogTree);
        }
        else
        {
            dialogStarted = TryStartDialog(targetNPC);
        }
        
        if (dialogStarted)
        {
            // Update trigger state
            hasTriggered = true;
            lastTriggerTime = Time.time;
            
            // Subscribe to dialog end event to handle cleanup
            SubscribeToDialogEvents();
            
            // Fire events
            OnDialogTriggered?.Invoke();
            
            LogDebug($"Dialog successfully triggered with {targetNPC.npcName}");
        }
        else
        {
            LogError($"Failed to start dialog with {targetNPC.npcName}");
        }
    }
    
    /// <summary>
    /// Subscribe to DialogManager events for proper integration
    /// </summary>
    private void SubscribeToDialogEvents()
    {
        if (dialogManager != null)
        {
            // Subscribe to dialog ended event to handle cleanup
            dialogManager.OnDialogEnded += HandleDialogManagerEnded;
        }
    }
    
    /// <summary>
    /// Unsubscribe from DialogManager events
    /// </summary>
    private void UnsubscribeFromDialogEvents()
    {
        if (dialogManager != null)
        {
            dialogManager.OnDialogEnded -= HandleDialogManagerEnded;
        }
    }
    
    /// <summary>
    /// Handle when DialogManager signals that dialog has ended
    /// </summary>
    private void HandleDialogManagerEnded(NPCContent endedNPC)
    {
        // Verify this is our NPC
        if (endedNPC == targetNPC)
        {
            LogDebug($"Dialog ended notification received for {endedNPC?.npcName}");
            
            // Call our dialog ended callback
            OnDialogEndedCallback();
            
            // Unsubscribe from events
            UnsubscribeFromDialogEvents();
        }
    }
    
    /// <summary>
    /// Prepare the NPC for dialog by calling its OnDialogStarted method
    /// </summary>
    private void PrepareNPCForDialog()
    {
        if (targetNPC != null)
        {
            // This allows the NPC to prepare for dialog (save state, pause AI, etc.)
            targetNPC.OnDialogStarted();
        }
    }
    
    /// <summary>
    /// Safely attempt to start dialog with the DialogManager
    /// </summary>
    private bool TryStartDialog(NPCContent npc)
    {
        try
        {
            dialogManager.StartDialog(npc);
            return true;
        }
        catch (System.Exception ex)
        {
            LogError($"Exception starting dialog: {ex.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// Safely attempt to start dialog with specific tree
    /// </summary>
    private bool TryStartDialog(NPCContent npc, string treeName)
    {
        try
        {
            dialogManager.StartDialog(npc, treeName);
            return true;
        }
        catch (System.Exception ex)
        {
            LogError($"Exception starting dialog with tree '{treeName}': {ex.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// Check if player is within specified range
    /// </summary>
    private bool IsPlayerInRange(float range)
    {
        if (player == null) return false;
        float distance = Vector2.Distance(transform.position, player.transform.position);
        return distance <= range;
    }
    
    /// <summary>
    /// Check for interaction input from multiple sources
    /// </summary>
    private bool GetInteractInput()
    {
        // Input System check
        if (player != null)
        {
            var playerInput = player.GetComponent<UnityEngine.InputSystem.PlayerInput>();
            if (playerInput != null)
            {
                var interactAction = playerInput.actions["Interact"];
                if (interactAction != null && interactAction.WasPressedThisFrame())
                {
                    return true;
                }
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// Show/hide interaction prompt (override this for custom UI)
    /// </summary>
    protected virtual void ShowInteractionPrompt(bool show)
    {
        if (show)
        {
            LogDebug($"[UI Prompt] Press {interactionKey} to interact with {targetNPC?.npcName}");
        }
        else
        {
            LogDebug("[UI Prompt] Hidden");
        }
        
        // You can implement actual UI prompts here
        // Example: interactionPromptUI?.SetActive(show);
    }
    
    /// <summary>
    /// Reset the trigger state (useful for repeatable dialogs)
    /// </summary>
    public void ResetTrigger()
    {
        hasTriggered = false;
        isWaitingForInput = false;
        lastTriggerTime = 0f;
        ShowInteractionPrompt(false);
        LogDebug($"DialogueTrigger on {gameObject.name} reset");
    }
    
    /// <summary>
    /// Manually trigger the dialog (for external calls)
    /// </summary>
    public void ManualTrigger()
    {
        if (IsReadyToTrigger())
        {
            PassNPCContent();
        }
        else
        {
            LogWarning($"Manual trigger failed - trigger not ready: {lastValidationError}");
        }
    }
    
    /// <summary>
    /// Enable/disable specific trigger types at runtime
    /// </summary>
    public void SetTriggerType(bool spotlight, bool interaction, bool proximity)
    {
        triggerOnSpotlight = spotlight;
        triggerOnInteraction = interaction;
        triggerOnProximity = proximity;
        
        // Reset state when changing trigger types
        if (!spotlight && !interaction && !proximity)
        {
            isWaitingForInput = false;
            ShowInteractionPrompt(false);
        }
        
        // Re-validate setup
        ValidateSetup();
    }
    
    /// <summary>
    /// Change the target NPC at runtime
    /// </summary>
    public void SetTargetNPC(NPCContent newNPC, string treeNameOverride = "")
    {
        targetNPC = newNPC;
        specificDialogTree = treeNameOverride;
        
        // Re-validate with new NPC
        ValidateSetup();
        
        // Reset trigger state
        ResetTrigger();
    }
    
    /// <summary>
    /// Called when dialog ends (can be called by external systems)
    /// </summary>
    public void OnDialogEndedCallback()
    {
        // Notify the NPC that dialog ended
        if (targetNPC != null)
        {
            targetNPC.OnDialogEnded();
        }
        
        OnDialogEnded?.Invoke();
    }
    
    /// <summary>
    /// Get current setup status for debugging
    /// </summary>
    public string GetSetupStatus()
    {
        if (isSetupValid)
        {
            return "Setup Valid";
        }
        else
        {
            return $"Setup Invalid: {lastValidationError}";
        }
    }
    
    #region Logging Methods
    private void LogDebug(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"[DialogueTrigger] {message}");
        }
    }
    
    private void LogWarning(string message)
    {
        Debug.LogWarning($"[DialogueTrigger] {message}");
    }
    
    private void LogError(string message)
    {
        Debug.LogError($"[DialogueTrigger] {message}");
    }
    #endregion
    
    private void OnDrawGizmosSelected()
    {
        // Draw interaction range
        if (triggerOnInteraction)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, interactionRange);
        }
        
        // Draw proximity range
        if (triggerOnProximity)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, proximityRange);
        }
        
        // Draw validation status
        if (Application.isPlaying)
        {
            Gizmos.color = isSetupValid ? Color.green : Color.red;
            Gizmos.DrawWireCube(transform.position + Vector3.up * 0.5f, Vector3.one * 0.2f);
        }
    }
    
    #region Lifecycle Management
    private void OnDestroy()
    {
        // Clean up event subscriptions
        UnsubscribeFromDialogEvents();
        
        LogDebug($"DialogueTrigger on {gameObject.name} destroyed");
    }
    
    private void OnDisable()
    {
        // Clean up state when disabled
        isWaitingForInput = false;
        ShowInteractionPrompt(false);
        
        // Clean up event subscriptions
        UnsubscribeFromDialogEvents();
    }
    #endregion
}