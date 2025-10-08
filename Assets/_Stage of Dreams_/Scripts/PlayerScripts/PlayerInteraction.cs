/* PlayerInteraction.cs
 *
 *  Enables the player to interact with objects in the game world, such as items or inventories.
 *  Key Note: NPC's and Dialogs are managed with DialogueTrigger and DialogManager instead.
 *  
 *  How to use in Unity:
 *  
 */

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

/// <summary>
/// Handles player interaction with objects in the 2D game world.
/// Focuses on 2D physics and interactables. NPCs and Dialogs are managed with DialogueTrigger and DialogManager.
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    #region Editor Data
    [Header("2D Interaction Settings")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private LayerMask interactableLayer = -1;
    
    [Header("UI Elements")]
    [SerializeField] private GameObject interactionPrompt; // UI element showing "Press E to interact"
    [SerializeField] private TextMeshProUGUI promptText;
    
    [Header("Dependencies")]
    [SerializeField] private PlayerScript playerController; // Reference to player controller
    #endregion

    // Current state
    private Interactable currentInteractable;
    private PlayerInput playerInput; // For input system integration
    
    private void Start()
    {
        // Get player controller if not assigned
        if (playerController == null)
            playerController = GetComponent<PlayerScript>();
            
        // Get PlayerInput component
        playerInput = GetComponent<PlayerInput>();
            
        // Hide interaction prompt initially
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }
    
    private void Update()
    {
        // Check for interactables in 2D space
        CheckForInteractables2D();
        
        // Handle interaction input - support both legacy Input and Input System
        bool interactPressed = GetInteractInput();
        
        // Handle object interaction if no dialogue is active
        if (interactPressed && currentInteractable != null)
        {
            // Don't interact if dialogue is already active
            if (DialogManager.Instance != null && 
                (DialogManager.Instance.IsDialogActive()))
            {
                return;
            }
            
            // Check if the interactable has a DialogueTrigger
            var dialogTrigger = currentInteractable.GetComponent<DialogueTrigger>();
            if (dialogTrigger != null)
            {
                // Let the DialogueTrigger handle the interaction
                dialogTrigger.ManualTrigger();
                
                // Disable player movement during interaction
                if (playerController != null)
                    playerController.DisableMovement();
                    
                return;
            }
            
            // For non-dialog interactables, use the old system
            // Disable player movement during interaction
            if (playerController != null)
                playerController.DisableMovement();
            
            currentInteractable.Interact();
        }
    }
    
    /// <summary>
    /// Check for interactables using 2D physics
    /// </summary>
    private void CheckForInteractables2D()
    {
        Interactable nearestInteractable = null;
        float nearestDistance = float.MaxValue;
        
        // Use 2D physics to find all interactables in range
        Collider2D[] colliders2D = Physics2D.OverlapCircleAll(
            transform.position, 
            interactionRange, 
            interactableLayer
        );
        
        foreach (Collider2D col in colliders2D)
        {
            Interactable interactable = col.GetComponent<Interactable>();
            if (interactable != null)
            {
                // Use 2D distance calculation
                float distance = Vector2.Distance(transform.position, col.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestInteractable = interactable;
                }
            }
        }
        
        // Update current interactable
        if (nearestInteractable != currentInteractable)
        {
            currentInteractable = nearestInteractable;
            UpdateInteractionPrompt();
        }
    }
    
    /// <summary>
    /// Get interaction input from multiple sources (Input System + Legacy)
    /// </summary>
    private bool GetInteractInput()
    {
        bool interactPressed = false;
        
        if (playerInput != null)
        {
            // Try to get interact action from Input System
            var interactAction = playerInput.actions["Interact"];
            if (interactAction != null)
            {
                interactPressed = interactAction.WasPressedThisFrame();
            }
            else
            {
                // Fallback to legacy input
                interactPressed = Input.GetKeyDown(interactKey);
            }
        }
        else
        {
            // Legacy input only
            interactPressed = Input.GetKeyDown(interactKey);
        }
        
        return interactPressed;
    }
    
    /// <summary>
    /// Update the interaction prompt UI based on current interactable
    /// </summary>
    private void UpdateInteractionPrompt()
    {
        if (interactionPrompt == null) return;

        bool shouldShowPrompt = currentInteractable != null &&
                              DialogManager.Instance != null &&
                              !DialogManager.Instance.IsDialogActive();
        
        interactionPrompt.SetActive(shouldShowPrompt);
        
        if (shouldShowPrompt && promptText != null)
        {
            // Get the correct key name based on input method
            string keyName = GetInteractionKeyDisplayName();
            
            // Check if it's a DialogueTrigger (NPC) or regular interactable
            var dialogTrigger = currentInteractable.GetComponent<DialogueTrigger>();
            if (dialogTrigger != null)
            {
                // Get NPC name from the DialogueTrigger's target NPC
                string npcName = "someone";
                var npcContent = currentInteractable.GetComponent<NPCContent>();
                if (npcContent != null && !string.IsNullOrEmpty(npcContent.npcName))
                {
                    npcName = npcContent.npcName;
                }
                
                promptText.text = $"Press {keyName} to talk to {npcName}";
            }
            else
            {
                // Regular interactable
                string interactionText = "interact";
                if (currentInteractable != null && !string.IsNullOrEmpty(currentInteractable.name))
                {
                    interactionText = $"interact with {currentInteractable.name}";
                }
                
                promptText.text = $"Press {keyName} to {interactionText}";
            }
        }
    }
    
    /// <summary>
    /// Get the display name for the interaction key
    /// </summary>
    private string GetInteractionKeyDisplayName()
    {
        string keyName = interactKey.ToString();
        
        if (playerInput != null)
        {
            var interactAction = playerInput.actions["Interact"];
            if (interactAction != null && interactAction.bindings.Count > 0)
            {
                keyName = interactAction.GetBindingDisplayString();
            }
        }
        
        return keyName;
    }
    
    /// <summary>
    /// Draw the 2D interaction range in Scene view
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        // Draw 2D interaction range as a circle
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
        
        // For 2D, we can also draw it as a flat circle
        var originalMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, Quaternion.identity, Vector3.one);
        
        // Draw a flattened circle for 2D visualization
        const int segments = 32;
        float angleStep = 360f / segments;
        Vector3 prevPoint = Vector3.right * interactionRange;
        
        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 newPoint = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * interactionRange;
            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
        
        Gizmos.matrix = originalMatrix;
        
        // Show current interactable connection
        if (currentInteractable != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, currentInteractable.transform.position);
        }
    }
    
    #region Public Interface
    /// <summary>
    /// Check if there's currently an interactable in range
    /// </summary>
    public bool HasCurrentInteractable()
    {
        return currentInteractable != null;
    }
    
    /// <summary>
    /// Get the current interactable object
    /// </summary>
    public Interactable GetCurrentInteractable()
    {
        return currentInteractable;
    }
    
    /// <summary>
    /// Force interaction with current interactable (for external calls)
    /// </summary>
    public void ForceInteract()
    {
        if (currentInteractable != null)
        {
            // Check for DialogueTrigger first
            var dialogTrigger = currentInteractable.GetComponent<DialogueTrigger>();
            if (dialogTrigger != null)
            {
                dialogTrigger.ManualTrigger();
            }
            else
            {
                currentInteractable.Interact();
            }
        }
    }
    
    /// <summary>
    /// Check if player is within interaction range of a specific object
    /// </summary>
    public bool IsInRangeOf(Transform target)
    {
        if (target == null) return false;
        return Vector2.Distance(transform.position, target.position) <= interactionRange;
    }
    
    /// <summary>
    /// Set the interaction range at runtime
    /// </summary>
    public void SetInteractionRange(float newRange)
    {
        interactionRange = Mathf.Max(0f, newRange);
    }
    
    /// <summary>
    /// Enable or disable the interaction system
    /// </summary>
    public void SetInteractionEnabled(bool enabled)
    {
        this.enabled = enabled;
        
        if (!enabled && interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }
    #endregion
}