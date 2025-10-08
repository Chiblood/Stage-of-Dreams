using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

/// <summary>
/// Handles player interaction with objects in the game world.
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    #region Editor Data
    [Header("Interaction Settings")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private LayerMask interactableLayer = -1;
    
    [Header("UI Elements")]
    [SerializeField] private GameObject interactionPrompt; // UI element showing "Press E to interact"
    [SerializeField] private TextMeshProUGUI promptText;
    
    [Header("Dependencies")]
    [SerializeField] private PlayerScript playerController; // Reference to player controller
    #endregion

    private Interactable currentInteractable;
    private Camera playerCamera;
    private PlayerInput playerInput; // For input system integration
    
    private void Start()
    {
        // Get the camera - adjust this based on your camera setup
        playerCamera = Camera.main;
        if (playerCamera == null)
            playerCamera = FindAnyObjectByType<Camera>();
        
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
        // Check for interactables
        CheckForInteractables();
        
        // Handle interaction input - support both legacy Input and Input System
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
        
        // Handle dialog advancement/closing first
        if (interactPressed && DialogManager.Instance != null)
        {
            if (DialogManager.Instance.IsDialogueActive())
            {
                // If choices are active, don't advance - let the choice system handle it
                if (!DialogManager.Instance.IsChoiceActive())
                {
                    // Advance/close dialogue
                    DialogManager.Instance.HideDialogue();
                    
                    // Re-enable movement when dialogue closes
                    if (playerController != null)
                        playerController.EnableMovement();
                    
                    return; // Don't process other interactions this frame
                }
                else
                {
                    // Choices are active, don't process other interactions
                    return;
                }
            }
        }
        
        // Handle object interaction if no dialogue is active
        if (interactPressed && currentInteractable != null)
        {
            // Don't interact if dialogue is already active
            if (DialogManager.Instance != null && 
                (DialogManager.Instance.IsDialogueActive() || DialogManager.Instance.IsChoiceActive()))
            {
                return;
            }
            
            // Disable player movement during interaction
            if (playerController != null)
                playerController.DisableMovement();
            
            currentInteractable.Interact();
        }
    }
    private void CheckForInteractables()
    {
        Interactable nearestInteractable = null;
        float nearestDistance = float.MaxValue;
        
        // Find all interactables in range
        Collider[] colliders = Physics.OverlapSphere(transform.position, interactionRange, interactableLayer);
        
        foreach (Collider col in colliders)
        {
            Interactable interactable = col.GetComponent<Interactable>();
            if (interactable != null)
            {
                float distance = Vector3.Distance(transform.position, col.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestInteractable = interactable;
                }
            }
        }
        
        // For 2D games, you might prefer this approach instead:
        /*
        Collider2D[] colliders2D = Physics2D.OverlapCircleAll(transform.position, interactionRange, interactableLayer);
        
        foreach (Collider2D col in colliders2D)
        {
            Interactable interactable = col.GetComponent<Interactable>();
            if (interactable != null)
            {
                float distance = Vector2.Distance(transform.position, col.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestInteractable = interactable;
                }
            }
        }
        */
        
        // Update current interactable
        if (nearestInteractable != currentInteractable)
        {
            currentInteractable = nearestInteractable;
            UpdateInteractionPrompt();
        }
    }
    
    private void UpdateInteractionPrompt()
    {
        if (interactionPrompt == null) return;
        
        bool shouldShowPrompt = currentInteractable != null && 
                              DialogManager.Instance != null && 
                              !DialogManager.Instance.IsDialogueActive() && 
                              !DialogManager.Instance.IsChoiceActive();
        
        interactionPrompt.SetActive(shouldShowPrompt);
        
        if (shouldShowPrompt && promptText != null)
        {
            // Get the correct key name based on input method
            string keyName = interactKey.ToString();
            if (playerInput != null)
            {
                var interactAction = playerInput.actions["Interact"];
                if (interactAction != null && interactAction.bindings.Count > 0)
                {
                    keyName = interactAction.GetBindingDisplayString();
                }
            }
            
            promptText.text = $"Press {keyName} to talk to {currentInteractable.speakerName}";
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        // Draw interaction range in Scene view
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
        
        // For 2D games, you might prefer this:
        // Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
    
    #region Public Interface
    public bool HasCurrentInteractable()
    {
        return currentInteractable != null;
    }
    
    public Interactable GetCurrentInteractable()
    {
        return currentInteractable;
    }
    
    public void ForceInteract()
    {
        if (currentInteractable != null)
        {
            currentInteractable.Interact();
        }
    }
    #endregion
}