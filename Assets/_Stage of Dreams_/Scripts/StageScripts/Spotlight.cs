using UnityEngine;

public class Spotlight : MonoBehaviour
{
    [Header("Spotlight Settings")]
    public float radius = 1.5f;

    [Header("Dependencies")]
    [SerializeField] private PlayerScript _player;

    [Header("Dialog Settings")]
    [SerializeField] private bool triggerDialogOnEnter = true;
    [SerializeField] private bool requireInteractionToStartDialog = true; // New setting
    [SerializeField] private DialogueData[] dialogueSequence; // Array of dialogues to show
    [SerializeField] private EnhancedDialogueData[] enhancedDialogueSequence; // New enhanced sequence
    [SerializeField] private bool useEnhancedDialogue = false; // Toggle between systems
    [SerializeField] private bool showChoicesAfterDialog = false;
    [SerializeField] private string[] choices = { "Continue", "Wait a moment" };

    private bool _wasInSpotlight = false;
    private int currentDialogueIndex = 0;
    private bool hasTriggeredDialog = false;
    private bool playerInSpotlightAndWaiting = false; // New state tracking

    private void Awake()
    {
        // Find player if not assigned
        if (_player == null)
            _player = FindFirstObjectByType<PlayerScript>();

        // Set center position to this object's position
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }

    private void Update()
    {
        if (_player != null)
        {
            CheckPlayerInSpotlight(_player);
            
            // Check for interaction input when player is in spotlight and waiting
            if (playerInSpotlightAndWaiting && !hasTriggeredDialog)
            {
                CheckForInteractionInput();
            }
        }
    }

    private void CheckForInteractionInput()
    {
        // Get the PlayerInput component to check for interaction
        var playerInput = _player.GetComponent<UnityEngine.InputSystem.PlayerInput>();
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
                interactPressed = Input.GetKeyDown(KeyCode.E);
            }
        }
        else
        {
            // Legacy input only
            interactPressed = Input.GetKeyDown(KeyCode.E);
        }

        if (interactPressed)
        {
            Debug.Log("Player pressed E while in spotlight - triggering dialog!");
            TriggerSpotlightDialog();
        }
    }

    public void CheckPlayerInSpotlight(PlayerScript player)
    {
        if (player == null) return;

        Vector2 playerPos = player.transform.position;
        Vector2 centerPosition = transform.position;
        bool isInSpotlight = Vector2.Distance(centerPosition, playerPos) <= radius;

        // Only update if state changed
        if (isInSpotlight != _wasInSpotlight)
        {
            player.inSpotlight = isInSpotlight;
            _wasInSpotlight = isInSpotlight;

            if (isInSpotlight)
            {
                OnPlayerEnteredSpotlight(player);
            }
            else
            {
                OnPlayerExitedSpotlight(player);
            }
        }
    }

    private void OnPlayerEnteredSpotlight(PlayerScript player)
    {
        Debug.Log("Player entered spotlight!");

        // Check if we should trigger dialog immediately or wait for interaction
        if (triggerDialogOnEnter && !hasTriggeredDialog)
        {
            if (requireInteractionToStartDialog)
            {
                // Set state to wait for player interaction
                playerInSpotlightAndWaiting = true;
                Debug.Log("Press E to start the scene...");
                
                // Optionally show a UI prompt here
                ShowInteractionPrompt(true);
            }
            else
            {
                // Immediate trigger (old behavior)
                TriggerSpotlightDialog();
            }
        }
    }

    private void OnPlayerExitedSpotlight(PlayerScript player)
    {
        Debug.Log("Player exited spotlight - Return to exploration mode!");
        
        // Reset waiting state
        playerInSpotlightAndWaiting = false;
        ShowInteractionPrompt(false);
        
        // TODO: Return to normal gameplay
    }

    private void ShowInteractionPrompt(bool show)
    {
        // You can implement UI prompt logic here
        // For now, just log to console
        if (show)
        {
            Debug.Log("UI Prompt: Press E to start scene");
        }
        else
        {
            Debug.Log("UI Prompt: Hidden");
        }
        
        // Example: Show/hide a UI element
        // if (interactionPromptUI != null)
        //     interactionPromptUI.SetActive(show);
    }

    private void TriggerSpotlightDialog()
    {
        // Clear waiting state since dialog is starting
        playerInSpotlightAndWaiting = false;
        ShowInteractionPrompt(false);
        
        if (DialogManager.Instance == null)
        {
            Debug.LogError("DialogManager instance not found! Make sure there's a DialogManager in the scene.");
            return;
        }

        // Use enhanced dialogue system if enabled
        if (useEnhancedDialogue && enhancedDialogueSequence != null && enhancedDialogueSequence.Length > 0)
        {
            if (currentDialogueIndex < enhancedDialogueSequence.Length)
            {
                DialogManager.Instance.ShowDialogue(enhancedDialogueSequence[currentDialogueIndex]);
                currentDialogueIndex++;
                
                if (currentDialogueIndex >= enhancedDialogueSequence.Length)
                {
                    hasTriggeredDialog = true;
                }
            }
        }
        else if (dialogueSequence != null && dialogueSequence.Length > 0)
        {
            // Use basic dialogue system (existing code)
            if (currentDialogueIndex < dialogueSequence.Length)
            {
                // Ensure displayDuration is 0 for manual advancement
                var dialogueData = dialogueSequence[currentDialogueIndex];
                dialogueData.displayDuration = 0f; // Force manual advancement

                DialogManager.Instance.ShowDialogue(dialogueData);

                // Show choices if this is the last dialogue and choices are enabled
                if (showChoicesAfterDialog && choices != null && choices.Length > 0 &&
                    currentDialogueIndex == dialogueSequence.Length - 1)
                {
                    DialogManager.Instance.ShowChoices(choices, OnChoiceSelected);
                }

                currentDialogueIndex++;

                if (currentDialogueIndex >= dialogueSequence.Length)
                {
                    hasTriggeredDialog = true;
                }
            }
        }
        else
        {
            // Fallback: Create an enhanced dialogue with action choices
            CreateDefaultEnhancedDialogue();
        }
    }
    
    private void CreateDefaultEnhancedDialogue()
    {
        // Create a default enhanced dialogue with action choices
        EnhancedDialogueData defaultDialogue = new EnhancedDialogueData
        {
            characterName = "Director",
            dialogueText = "The spotlight reveals your moment. This is your cue! What would you like to do?",
            isPlayer = false,
            displayDuration = 0f,
            choices = new DialogueChoice[]
            {
                new DialogueChoice("Start the scene", ChoiceActionType.StartTurnBasedMode),
                new DialogueChoice("I need more time", ChoiceActionType.CloseDialog),
                new DialogueChoice("Exit to main menu", ChoiceActionType.ExitToMainMenu)
            }
        };
        
        DialogManager.Instance.ShowDialogue(defaultDialogue);
        hasTriggeredDialog = true;
    }

    private void OnChoiceSelected(int choiceIndex)
    {
        Debug.Log($"Player selected spotlight choice {choiceIndex}: {(choices != null && choiceIndex < choices.Length ? choices[choiceIndex] : "Unknown")}");

        // Handle choice logic here
        switch (choiceIndex)
        {
            case 0: // Continue/Ready
                Debug.Log("Player is ready to continue with the scene!");
                // Add your game logic here - maybe start turn-based mode
                break;
            case 1: // Wait
                Debug.Log("Player needs a moment.");
                // Add logic to give player more time or restart dialogue
                ResetDialogue();
                break;
            default:
                Debug.Log($"Unhandled choice index: {choiceIndex}");
                break;
        }
    }

    // Public method to reset the dialogue trigger (useful for testing or multi-use spotlights)
    public void ResetDialogue()
    {
        currentDialogueIndex = 0;
        hasTriggeredDialog = false;
        playerInSpotlightAndWaiting = false;
        ShowInteractionPrompt(false);
    }

    // Public method to force trigger dialog (useful for testing)
    public void ForceStartDialog()
    {
        if (_player != null && _player.inSpotlight && !hasTriggeredDialog)
        {
            TriggerSpotlightDialog();
        }
    }

    // Optional: Draw the spotlight radius in the scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}