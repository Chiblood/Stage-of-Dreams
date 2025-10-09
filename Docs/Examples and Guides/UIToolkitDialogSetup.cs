/* UIToolkitDialogSetup.cs
   A prefab class to set up the DialogManager with UI Toolkit components.

    How to use in Unity:
    1. Create an empty GameObject in your scene.
    2. Attach this UIToolkitDialogSetup script to the GameObject.
    3. Optionally assign a VisualTreeAsset and StyleSheet for custom dialog UI.
    4. Use the context menu options to set up the DialogManager and test dialogues.
    5. Ensure you have a DialogManager script in your project to handle the dialogues.

 */

using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Helper script to set up UI Toolkit Dialog System
/// Add this to a GameObject and it will automatically configure the DialogManager
/// </summary>
public class UIToolkitDialogSetup : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private bool autoSetup = true;
    [SerializeField] private VisualTreeAsset dialogVisualTree;
    [SerializeField] private StyleSheet dialogStyleSheet;
    
    private void Start()
    {
        if (autoSetup)
        {
            SetupDialogManager();
        }
    }
    
    [ContextMenu("Setup Dialog Manager")]
    public void SetupDialogManager()
    {
        // Find or create DialogManager
        DialogManager dialogManager = FindFirstObjectByType<DialogManager>();
        if (dialogManager == null)
        {
            GameObject dialogObj = new GameObject("DialogManager");
            dialogManager = dialogObj.AddComponent<DialogManager>();
        }
        
        // Add UIDocument if not present
        UIDocument uiDocument = dialogManager.GetComponent<UIDocument>();
        if (uiDocument == null)
        {
            uiDocument = dialogManager.gameObject.AddComponent<UIDocument>();
        }
        
        // Assign assets if provided
        if (dialogVisualTree != null)
        {
            uiDocument.visualTreeAsset = dialogVisualTree;
        }
        
        Debug.Log("DialogManager setup complete!");
        
        // Force hide dialog after setup
        if (DialogManager.Instance != null)
        {
            DialogManager.Instance.ForceHideDialogue();
        }
    }
    
    [ContextMenu("Test Dialog")]
    public void TestDialog()
    {
        if (DialogManager.Instance != null)
        {
            DialogManager.Instance.TestNPCDialogue();
        }
        else
        {
            Debug.LogError("DialogManager not found! Run Setup Dialog Manager first.");
        }
    }
    
    [ContextMenu("Test Choice Dialog")]
    public void TestChoiceDialog()
    {
        if (DialogManager.Instance != null)
        {
            DialogManager.Instance.TestChoiceDialogue();
        }
        else
        {
            Debug.LogError("DialogManager not found! Run Setup Dialog Manager first.");
        }
    }
    
    [ContextMenu("Test Enhanced Choice Dialog")]
    public void TestEnhancedChoiceDialog()
    {
        if (DialogManager.Instance != null)
        {
            // Create a test enhanced dialogue with various choice actions
            EnhancedDialogueData testDialogue = new EnhancedDialogueData
            {
                characterName = "Test Director",
                dialogueText = "Welcome to the enhanced choice system! What would you like to do?",
                isPlayer = false,
                displayDuration = 0f,
                choices = new DialogueChoice[]
                {
                    new DialogueChoice("Continue to next dialog", ChoiceActionType.ContinueDialog)
                    {
                        nextDialogue = new DialogueData
                        {
                            characterName = "Director",
                            dialogueText = "Great! This is the next dialogue. You can now close this.",
                            isPlayer = false,
                            displayDuration = 0f
                        }
                    },
                    new DialogueChoice("Close and return to game", ChoiceActionType.CloseDialog),
                    new DialogueChoice("Start turn-based mode", ChoiceActionType.StartTurnBasedMode)
                }
            };
            
            DialogManager.Instance.ShowDialogue(testDialogue);
        }
        else
        {
            Debug.LogError("DialogManager not found! Run Setup Dialog Manager first.");
        }
    }
    
    [ContextMenu("Test Choice Sequence")]
    public void TestChoiceSequence()
    {
        if (DialogManager.Instance != null)
        {
            // Create a dialogue that leads to a sequence
            EnhancedDialogueData sequenceStarter = new EnhancedDialogueData
            {
                characterName = "Narrator",
                dialogueText = "Would you like to see a sequence of dialogues?",
                isPlayer = false,
                displayDuration = 0f,
                choices = new DialogueChoice[]
                {
                    new DialogueChoice("Yes, show me the sequence", ChoiceActionType.ContinueSequence)
                    {
                        nextDialogueSequence = new DialogueData[]
                        {
                            new DialogueData
                            {
                                characterName = "Character 1",
                                dialogueText = "This is the first dialogue in the sequence.",
                                isPlayer = false,
                                displayDuration = 2f
                            },
                            new DialogueData
                            {
                                characterName = "Character 2", 
                                dialogueText = "This is the second dialogue in the sequence.",
                                isPlayer = true,
                                displayDuration = 2f
                            },
                            new DialogueData
                            {
                                characterName = "Narrator",
                                dialogueText = "And this is the final dialogue. The sequence is complete!",
                                isPlayer = false,
                                displayDuration = 3f
                            }
                        }
                    },
                    new DialogueChoice("No, close this dialog", ChoiceActionType.CloseDialog)
                }
            };
            
            DialogManager.Instance.ShowDialogue(sequenceStarter);
        }
        else
        {
            Debug.LogError("DialogManager not found!");
        }
    }
    
    [ContextMenu("Test Custom Actions")]
    public void TestCustomActions()
    {
        if (DialogManager.Instance != null)
        {
            EnhancedDialogueData customActionDialogue = new EnhancedDialogueData
            {
                characterName = "Merchant",
                dialogueText = "Welcome to my shop! What can I do for you?",
                isPlayer = false,
                displayDuration = 0f,
                choices = new DialogueChoice[]
                {
                    new DialogueChoice("Open Shop", ChoiceActionType.CustomAction) { customActionID = "open_shop" },
                    new DialogueChoice("Start Combat", ChoiceActionType.CustomAction) { customActionID = "start_combat" },
                    new DialogueChoice("Get Free Item", ChoiceActionType.CustomAction) { customActionID = "give_item" },
                    new DialogueChoice("Leave", ChoiceActionType.CloseDialog)
                }
            };
            
            DialogManager.Instance.ShowDialogue(customActionDialogue);
        }
        else
        {
            Debug.LogError("DialogManager not found!");
        }
    }
}
