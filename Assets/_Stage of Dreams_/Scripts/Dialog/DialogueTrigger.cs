/* DialogueTrigger.cs
 * Triggers the call to the DialogManager instance to display a sequence of dialogues based on certain conditions.
 * Passes NPC Content to the DialogManager which handles the display and flow of dialogues.
 * Trigger conditions can include player proximity, spotlight detection, or interaction input.
 * 
 * How to use in Unity:
 * 
 */

using UnityEngine;

/// <summary>
/// Simple dialog trigger that starts conversations with NPCs.
/// Handles player proximity and interaction input.
/// </summary>
public class DialogueTrigger : MonoBehaviour
{
    [Header("Trigger Settings")]
    [SerializeField] private bool triggerOnSpotlight = true;
    [SerializeField] private bool triggerOnInteraction = false;
    [SerializeField] private float interactionRange = 2f;
    
    [Header("NPC Reference")]
    [SerializeField] private NPCContent targetNPC;
    
    private DialogManager dialogManager;
    private PlayerScript player;
    private bool hasTriggered = false;
    
    private void Start()
    {
        dialogManager = FindFirstObjectByType<DialogManager>();
        player = FindFirstObjectByType<PlayerScript>();
        
        // Get NPC from this GameObject if not assigned
        if (targetNPC == null)
        {
            targetNPC = GetComponent<NPCContent>();
        }
    }
    
    private void Update()
    {
        // Don't trigger if dialog is already active
        if (dialogManager != null && dialogManager.IsDialogActive()) return;
        
        // Spotlight trigger
        if (triggerOnSpotlight && player != null && player.inSpotlight && !hasTriggered)
        {
            TriggerDialog();
        }
        
        // Interaction trigger
        if (triggerOnInteraction && IsPlayerNearby() && GetInteractInput())
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
            Debug.LogWarning("Cannot trigger dialog - DialogManager or NPC is missing");
            return;
        }
        
        dialogManager.StartDialog(targetNPC);
        
        if (triggerOnSpotlight)
        {
            hasTriggered = true; // Prevent re-triggering for spotlight
        }
    }
    
    /// <summary>
    /// Check if player is close enough to interact
    /// </summary>
    private bool IsPlayerNearby()
    {
        if (player == null) return false;
        float distance = Vector2.Distance(transform.position, player.transform.position);
        return distance <= interactionRange;
    }
    
    /// <summary>
    /// Check for interaction input
    /// </summary>
    private bool GetInteractInput()
    {
        // Simple input check - you can expand this based on your input system
        return Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space);
    }
    
    /// <summary>
    /// Reset the trigger (useful for repeatable dialogs)
    /// </summary>
    public void ResetTrigger()
    {
        hasTriggered = false;
    }
    
    /// <summary>
    /// Manually trigger the dialog (for external calls)
    /// </summary>
    public void ManualTrigger()
    {
        TriggerDialog();
    }
}