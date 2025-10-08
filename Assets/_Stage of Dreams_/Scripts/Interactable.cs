using UnityEngine;

public class Interactable : MonoBehaviour
{
    [Header("Character Settings")]
    public string speakerName = "Director";
    public Sprite characterSprite; // Optional character sprite
    public bool isPlayerCharacter = false; // Whether this represents the player
    
    [Header("Dialogue Content")]
    [TextArea(3, 10)]
    public string dialogueText = "Your cue is coming up. Are you ready?";
    
    [Header("Choice System")]
    public string[] choices = { "Yes, I'm ready!", "Give me a moment." };
    public bool showChoices = true; // Whether to show choices after dialogue
    
    [Header("Settings")]
    public float dialogueDisplayDuration = 0f; // 0 = wait for input or choices
    
    public void Interact()
    {
        // Ensure DialogManager exists
        if (DialogManager.Instance == null)
        {
            Debug.LogError("DialogManager instance not found! Make sure there's a DialogManager in the scene.");
            return;
        }
        
        // Show the dialogue
        DialogManager.Instance.ShowDialogue(
            speakerName, 
            dialogueText, 
            characterSprite, 
            isPlayerCharacter, 
            dialogueDisplayDuration
        );
        
        // Show choices if enabled and we have choices
        if (showChoices && choices != null && choices.Length > 0)
        {
            DialogManager.Instance.ShowChoices(choices, OnChoiceSelected);
        }
    }
    
    // Handle what happens when a choice is selected
    private void OnChoiceSelected(int choiceIndex)
    {
        Debug.Log($"Player selected choice {choiceIndex}: {choices[choiceIndex]}");
        
        // Add your custom logic here based on the choice
        switch (choiceIndex)
        {
            case 0: // First choice
                HandleFirstChoice();
                break;
            case 1: // Second choice
                HandleSecondChoice();
                break;
            default:
                Debug.Log($"Unhandled choice index: {choiceIndex}");
                break;
        }
    }
    
    private void HandleFirstChoice()
    {
        // Example: Player is ready
        Debug.Log("Player is ready to proceed!");
        // Add your game logic here
    }
    
    private void HandleSecondChoice()
    {
        // Example: Player needs more time
        Debug.Log("Player needs more time.");
        // Add your game logic here
    }
    
    // Optional: Method to interact without showing choices
    public void InteractWithoutChoices()
    {
        if (DialogManager.Instance == null)
        {
            Debug.LogError("DialogManager instance not found!");
            return;
        }
        
        DialogManager.Instance.ShowDialogue(
            speakerName, 
            dialogueText, 
            characterSprite, 
            isPlayerCharacter, 
            dialogueDisplayDuration
        );
    }
}