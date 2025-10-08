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
    [SerializeField] private float retriggerDelay = 1f; // Delay between retriggering
    #endregion

    // Internal state
    private DialogManager dialogManager;
    private PlayerScript player;
    private bool hasTriggered = false;
    private bool isWaitingForInput = false;
    private float lastTriggerTime = 0f;
    
    // Events for external systems
    public System.Action OnDialogTriggered;
    public System.Action OnDialogEnded;
    
    private void Start()
    {
        dialogManager = DialogManager.Instance;
        player = FindFirstObjectByType<PlayerScript>();
        
        // Get NPC from this GameObject if not assigned
        if (targetNPC == null)
        {
            targetNPC = GetComponent<NPCContent>();
        }
        
        // Validate setup
        if (targetNPC == null)
        {
            Debug.LogWarning($"DialogueTrigger on {gameObject.name} has no NPC content assigned!");
        }
        
        if (dialogManager == null)
        {
            Debug.LogWarning($"DialogueTrigger on {gameObject.name} cannot find DialogManager instance!");
        }
    }
    
    private void Update()
    {
        // Don't trigger if dialog is already active
        if (dialogManager != null && dialogManager.IsDialogActive()) return;
        
        // Check if we can trigger again
        if (hasTriggered && !canRetrigger) return;
        
        // Check retrigger delay
        if (hasTriggered && Time.time - lastTriggerTime < retriggerDelay) return;
        
        // Check different trigger conditions
        CheckSpotlightTrigger();
        CheckInteractionTrigger();
        CheckProximityTrigger();
        CheckWaitingForInput();
    }
    
    private void CheckSpotlightTrigger()
    {
        if (!triggerOnSpotlight || player == null) return;
        
        if (player.inSpotlight)
        {
            if (requireInteractionInput && !isWaitingForInput)
            {
                // Start waiting for input
                isWaitingForInput = true;
                ShowInteractionPrompt(true);
                Debug.Log($"Press {interactionKey} to start dialog with {targetNPC?.npcName}");
            }
            else if (!requireInteractionInput)
            {
                // Immediate trigger
                TriggerDialog();
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
                TriggerDialog();
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
            TriggerDialog();
        }
    }
    
    private void CheckWaitingForInput()
    {
        if (!isWaitingForInput) return;
        
        if (GetInteractInput())
        {
            TriggerDialog();
        }
    }
    
    /// <summary>
    /// Start the dialog with the target NPC
    /// </summary>
    private void TriggerDialog()
    {
        if (dialogManager == null || targetNPC == null)
        {
            Debug.LogWarning($"Cannot trigger dialog on {gameObject.name} - DialogManager or NPC is missing");
            return;
        }
        
        // Clear waiting state
        isWaitingForInput = false;
        ShowInteractionPrompt(false);
        
        // Start the dialog
        if (!string.IsNullOrEmpty(specificDialogTree))
        {
            dialogManager.StartDialog(targetNPC, specificDialogTree);
        }
        else
        {
            dialogManager.StartDialog(targetNPC);
        }
        
        // Update trigger state
        hasTriggered = true;
        lastTriggerTime = Time.time;
        
        // Fire events
        OnDialogTriggered?.Invoke();
        
        Debug.Log($"Dialog triggered with {targetNPC.npcName}");
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
            Debug.Log($"[UI Prompt] Press {interactionKey} to interact with {targetNPC?.npcName}");
        }
        else
        {
            Debug.Log("[UI Prompt] Hidden");
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
        Debug.Log($"DialogueTrigger on {gameObject.name} reset");
    }
    
    /// <summary>
    /// Manually trigger the dialog (for external calls)
    /// </summary>
    public void ManualTrigger()
    {
        TriggerDialog();
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
    }
    
    /// <summary>
    /// Change the target NPC at runtime
    /// </summary>
    public void SetTargetNPC(NPCContent newNPC, string treeNameOverride = "")
    {
        targetNPC = newNPC;
        specificDialogTree = treeNameOverride;
        ResetTrigger(); // Reset when changing target
    }
    
    /// <summary>
    /// Called when dialog ends (can be called by external systems)
    /// </summary>
    public void OnDialogEndedCallback()
    {
        OnDialogEnded?.Invoke();
    }
    
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
    }
}