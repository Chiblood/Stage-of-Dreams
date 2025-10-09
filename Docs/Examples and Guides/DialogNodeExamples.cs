using UnityEngine;

/// <summary>
/// Example showing how to create and link DialogNodes with DialogChoices.
/// This demonstrates different patterns for building dialog trees.
/// </summary>
public class DialogNodeExamples : MonoBehaviour
{
    [Header("Example 1: Simple Linear Dialog")]
    [SerializeField] private DialogNode greetingNode;
    [SerializeField] private DialogNode farewellNode;
    
    [Header("Example 2: Branching Dialog")]
    [SerializeField] private DialogNode questionNode;
    [SerializeField] private DialogNode yesResponseNode;
    [SerializeField] private DialogNode noResponseNode;
    [SerializeField] private DialogNode maybeResponseNode;
    
    [Header("Example 3: Action-Based Choices")]
    [SerializeField] private DialogNode shopKeeperNode;
    
    [ContextMenu("Setup Example 1: Linear Dialog")]
    public void SetupLinearDialog()
    {
        // Create a simple A → B dialog flow
        greetingNode = new DialogNode
        {
            speakerName = "Villager",
            dialogText = "Hello there, traveler!",
            isPlayerSpeaking = false,
            autoAdvanceDelay = 0f,
            choices = new DialogChoice[]
            {
                new DialogChoice
                {
                    choiceText = "Hello!",
                    targetNode = farewellNode // Points to next node
                }
            }
        };
        
        farewellNode = new DialogNode
        {
            speakerName = "Villager", 
            dialogText = "Have a safe journey!",
            isPlayerSpeaking = false,
            autoAdvanceDelay = 2f, // Auto-advance after 2 seconds
            choices = new DialogChoice[0] // No choices = dialog ends
        };
        
        Debug.Log("Linear dialog created: Greeting → Farewell");
    }
    
    [ContextMenu("Setup Example 2: Branching Dialog")]
    public void SetupBranchingDialog()
    {
        // Create responses first
        yesResponseNode = new DialogNode
        {
            speakerName = "Merchant",
            dialogText = "Excellent! Here's your item.",
            isPlayerSpeaking = false,
            choices = new DialogChoice[0] // Ends dialog
        };
        
        noResponseNode = new DialogNode
        {
            speakerName = "Merchant", 
            dialogText = "No problem, come back anytime.",
            isPlayerSpeaking = false,
            choices = new DialogChoice[0] // Ends dialog
        };
        
        maybeResponseNode = new DialogNode
        {
            speakerName = "Merchant",
            dialogText = "Take your time to decide. I'll be here.",
            isPlayerSpeaking = false,
            choices = new DialogChoice[]
            {
                new DialogChoice
                {
                    choiceText = "Actually, yes!",
                    targetNode = yesResponseNode
                },
                new DialogChoice
                {
                    choiceText = "No, thanks.",
                    targetNode = noResponseNode
                }
            }
        };
        
        // Create main question node that branches
        questionNode = new DialogNode
        {
            speakerName = "Merchant",
            dialogText = "Would you like to buy this magical sword?",
            isPlayerSpeaking = false,
            choices = new DialogChoice[]
            {
                new DialogChoice
                {
                    choiceText = "Yes, I'll take it!",
                    targetNode = yesResponseNode
                },
                new DialogChoice
                {
                    choiceText = "No, thank you.",
                    targetNode = noResponseNode
                },
                new DialogChoice
                {
                    choiceText = "Let me think about it...",
                    targetNode = maybeResponseNode
                }
            }
        };
        
        Debug.Log("Branching dialog created: Question → 3 different responses");
    }
    
    [ContextMenu("Setup Example 3: Action-Based Choices")]
    public void SetupActionBasedDialog()
    {
        shopKeeperNode = new DialogNode
        {
            speakerName = "Shop Keeper",
            dialogText = "Welcome to my shop! What can I do for you?",
            isPlayerSpeaking = false,
            choices = new DialogChoice[]
            {
                new DialogChoice
                {
                    choiceText = "Show me your wares",
                    customActionId = "open_shop", // Triggers custom action
                    targetNode = null // Will end dialog after action
                },
                new DialogChoice
                {
                    choiceText = "Tell me about this place",
                    targetNode = CreateInfoNode() // Helper method to create node
                },
                new DialogChoice
                {
                    choiceText = "I need to repair my equipment",
                    customActionId = "repair_equipment",
                    targetNode = null
                },
                new DialogChoice
                {
                    choiceText = "Goodbye",
                    targetNode = null // Ends dialog immediately
                }
            }
        };
        
        Debug.Log("Action-based dialog created with shop interactions");
    }
    
    // Helper method to create nodes programmatically
    private DialogNode CreateInfoNode()
    {
        return new DialogNode
        {
            speakerName = "Shop Keeper",
            dialogText = "This is the town of Silverbrook. We've been here for centuries, serving travelers like yourself.",
            isPlayerSpeaking = false,
            choices = new DialogChoice[]
            {
                new DialogChoice
                {
                    choiceText = "Thanks for the info",
                    targetNode = shopKeeperNode // Loop back to main menu
                }
            }
        };
    }
    
    [ContextMenu("Setup Example 4: Player Speaking Node")]
    public void SetupPlayerSpeakingDialog()
    {
        DialogNode playerThinkingNode = new DialogNode
        {
            speakerName = "You", // Player's name
            dialogText = "I wonder what secrets this old castle holds...",
            isPlayerSpeaking = true, // This makes it clear it's the player
            autoAdvanceDelay = 3f, // Player thoughts auto-advance
            choices = new DialogChoice[0]
        };
        
        DialogNode narratorNode = new DialogNode
        {
            speakerName = "Narrator",
            dialogText = "As you ponder the mysteries ahead, a strange sound echoes from within...",
            isPlayerSpeaking = false,
            choices = new DialogChoice[]
            {
                new DialogChoice
                {
                    choiceText = "Investigate the sound",
                    customActionId = "investigate_sound"
                },
                new DialogChoice
                {
                    choiceText = "Turn back",
                    customActionId = "retreat"
                }
            }
        };
        
        Debug.Log("Player speaking dialog created");
    }
    
    [ContextMenu("Setup Example 5: Complex Looping Dialog")]
    public void SetupLoopingDialog()
    {
        // Create a dialog that can loop back to itself
        DialogNode mainMenuNode = new DialogNode
        {
            speakerName = "Training Master",
            dialogText = "What type of training would you like?",
            isPlayerSpeaking = false,
            choices = new DialogChoice[]
            {
                new DialogChoice
                {
                    choiceText = "Combat Training",
                    targetNode = CreateTrainingNode("Combat", "You practice your sword skills.")
                },
                new DialogChoice
                {
                    choiceText = "Magic Training", 
                    targetNode = CreateTrainingNode("Magic", "You practice casting spells.")
                },
                new DialogChoice
                {
                    choiceText = "I'm done training",
                    targetNode = null // Ends dialog
                }
            }
        };
        
        Debug.Log("Looping dialog created - can return to main menu");
    }
    
    private DialogNode CreateTrainingNode(string trainingType, string description)
    {
        return new DialogNode
        {
            speakerName = "Training Master",
            dialogText = description + " Would you like to train more?",
            isPlayerSpeaking = false,
            choices = new DialogChoice[]
            {
                new DialogChoice
                {
                    choiceText = "Yes, more training!",
                    targetNode = null // Will be set to mainMenuNode when created
                },
                new DialogChoice
                {
                    choiceText = "That's enough for now",
                    targetNode = null // Ends dialog
                }
            }
        };
    }
}

/// <summary>
/// Helper class showing different dialog node patterns you can use
/// </summary>
public static class DialogNodePatterns
{
    /// <summary>
    /// Creates a simple statement node that just ends the dialog
    /// </summary>
    public static DialogNode CreateSimpleStatement(string speaker, string text)
    {
        return new DialogNode
        {
            speakerName = speaker,
            dialogText = text,
            isPlayerSpeaking = false,
            choices = new DialogChoice[0] // No choices = ends dialog
        };
    }
    
    /// <summary>
    /// Creates a node that auto-advances after a delay
    /// </summary>
    public static DialogNode CreateTimedStatement(string speaker, string text, float delay)
    {
        return new DialogNode
        {
            speakerName = speaker,
            dialogText = text,
            isPlayerSpeaking = false,
            autoAdvanceDelay = delay,
            choices = new DialogChoice[0]
        };
    }
    
    /// <summary>
    /// Creates a simple yes/no question node
    /// </summary>
    public static DialogNode CreateYesNoQuestion(string speaker, string question, DialogNode yesTarget, DialogNode noTarget)
    {
        return new DialogNode
        {
            speakerName = speaker,
            dialogText = question,
            isPlayerSpeaking = false,
            choices = new DialogChoice[]
            {
                new DialogChoice
                {
                    choiceText = "Yes",
                    targetNode = yesTarget
                },
                new DialogChoice
                {
                    choiceText = "No", 
                    targetNode = noTarget
                }
            }
        };
    }
    
    /// <summary>
    /// Creates a node that triggers a custom action
    /// </summary>
    public static DialogNode CreateActionNode(string speaker, string text, string actionId)
    {
        return new DialogNode
        {
            speakerName = speaker,
            dialogText = text,
            isPlayerSpeaking = false,
            choices = new DialogChoice[]
            {
                new DialogChoice
                {
                    choiceText = "Continue",
                    customActionId = actionId,
                    targetNode = null // Ends after action
                }
            }
        };
    }
}