using UnityEngine;

/// <summary>
/// Example script showing how to use the enhanced DialogTree system
/// to create dialog trees programmatically and in the editor.
/// </summary>
public class DialogTreeExample : MonoBehaviour
{
    [Header("Dialog Tree Examples")]
    [SerializeField] private DialogTree exampleTree;
    
    [ContextMenu("Create Simple Performance Dialog")]
    public void CreatePerformanceDialog()
    {
        if (exampleTree == null)
        {
            Debug.LogError("Please assign a DialogTree asset first!");
            return;
        }
        
        // Create the starting node
        var startNode = exampleTree.CreateStartingNode("Director", "The spotlight is waiting for you. Are you ready to begin your performance?");
        
        // Add choice branches
        var readyNode = exampleTree.AddChoiceNode(startNode, "I'm ready!", "Director", 
            "Excellent! Remember, this scene is about overcoming fear. Give it everything you've got!");
        
        var practiceNode = exampleTree.AddChoiceNode(startNode, "I need more practice", "Director", 
            "That's wise. A good actor always seeks to improve. What would you like to work on?");
        
        var scaredNode = exampleTree.AddChoiceNode(startNode, "I'm too scared...", "Director", 
            "Fear is natural, but don't let it control you. The stage is where we transform fear into art.");
        
        // Continue the ready branch
        var performanceNode = exampleTree.AddChoiceNode(readyNode, "Let's do this!", "Director", 
            "Perfect! Break a leg out there!", false, "start_performance");
        
        // Continue the practice branch  
        var linesNode = exampleTree.AddChoiceNode(practiceNode, "My lines", "Director", 
            "Let's run through them once more. Remember, speak from the heart, not just memory.");
        
        var movementNode = exampleTree.AddChoiceNode(practiceNode, "My movement", "Director", 
            "Good choice. Movement should feel natural. Let your emotions guide your body.");
        
        // Continue the scared branch
        var encourageNode = exampleTree.AddChoiceNode(scaredNode, "Tell me it will be okay", "Director", 
            "It will be more than okay - it will be magical. Every great performance starts with courage.");
        
        var breathingNode = exampleTree.AddChoiceNode(scaredNode, "Help me calm down", "Director", 
            "Of course. Let's do some breathing exercises together. In through the nose, out through the mouth...");
        
        // Add some final nodes
        exampleTree.AddSequentialNode(linesNode, "Player", "I think I've got it now!", true);
        exampleTree.AddSequentialNode(movementNode, "Player", "That feels much better!", true);
        exampleTree.AddChoiceNode(encourageNode, "You're right, I can do this!", "Director", 
            "That's the spirit! Now get out there and shine!", false, "start_performance");
        exampleTree.AddSequentialNode(breathingNode, "Player", "I feel much calmer now. Thank you.", true);
        
        Debug.Log("Performance dialog tree created successfully!");
    }
    
    [ContextMenu("Create Linear Story Dialog")]
    public void CreateLinearStoryDialog()
    {
        if (exampleTree == null)
        {
            Debug.LogError("Please assign a DialogTree asset first!");
            return;
        }
        
        // Create a linear conversation using the helper method
        string[] speakers = {
            "Narrator",
            "Player", 
            "Ghost of the Theater",
            "Player",
            "Ghost of the Theater",
            "Player",
            "Ghost of the Theater"
        };
        
        string[] dialogTexts = {
            "As you step onto the old stage, a mysterious figure appears in the shadows...",
            "Who... who are you?",
            "I am the spirit of this theater, bound to these boards for eternity...",
            "Why are you showing yourself to me?",
            "Because you have the gift, young one. The ability to see beyond the veil of performance.",
            "What do you want from me?",
            "Help me put on one final show, and I shall grant you the greatest performance of your life."
        };
        
        bool[] isPlayerSpeaking = { false, true, false, true, false, true, false };
        
        exampleTree.CreateLinearConversation(speakers, dialogTexts, isPlayerSpeaking);
        
        Debug.Log("Linear story dialog created successfully!");
    }
    
    [ContextMenu("Create Complex Branching Dialog")]
    public void CreateComplexBranchingDialog()
    {
        if (exampleTree == null)
        {
            Debug.LogError("Please assign a DialogTree asset first!");
            return;
        }
        
        // Create a complex dialog with multiple branches and custom actions
        var startNode = exampleTree.CreateStartingNode("Casting Director", 
            "Welcome to the audition! What type of role are you hoping to land?");
        
        // Main role branches
        var leadNode = exampleTree.AddChoiceNode(startNode, "Lead role", "Casting Director",
            "Ambitious! Lead roles require confidence, skill, and stage presence. Show me what you've got!");
        
        var supportNode = exampleTree.AddChoiceNode(startNode, "Supporting role", "Casting Director", 
            "Smart choice! Supporting roles are the backbone of any good production. They require versatility.");
        
        var ensembleNode = exampleTree.AddChoiceNode(startNode, "Ensemble", "Casting Director",
            "Excellent! Ensemble work is all about collaboration and being part of something bigger.");
        
        // Lead role sub-branches
        var heroNode = exampleTree.AddChoiceNode(leadNode, "Heroic character", "Casting Director",
            "I can see the determination in your eyes. Heroes must inspire hope in others.");
        
        var villainNode = exampleTree.AddChoiceNode(leadNode, "Villain", "Casting Director",
            "Interesting choice! The best villains believe they're the hero of their own story.");
        
        var comedyNode = exampleTree.AddChoiceNode(leadNode, "Comedy lead", "Casting Director",
            "Wonderful! Comedy is one of the hardest forms of acting. Timing is everything.");
        
        // Add performance tests with custom actions
        exampleTree.AddChoiceNode(heroNode, "Perform a heroic monologue", "Casting Director",
            "Magnificent! You have the presence of a true hero. The role is yours!", false, "grant_hero_role");
        
        exampleTree.AddChoiceNode(villainNode, "Show your villainous side", "Casting Director", 
            "Deliciously evil! You understand the complexity of villainy. Welcome to the dark side!", false, "grant_villain_role");
        
        exampleTree.AddChoiceNode(comedyNode, "Tell a joke", "Casting Director",
            "Ha! Perfect timing and delivery. You'll have the audience in stitches!", false, "grant_comedy_role");
        
        // Supporting role outcomes
        exampleTree.AddChoiceNode(supportNode, "I'm ready for any supporting role", "Casting Director",
            "That flexibility is exactly what we need. You're hired!", false, "grant_support_role");
        
        // Ensemble outcome
        exampleTree.AddChoiceNode(ensembleNode, "I love working with others", "Casting Director",
            "Perfect team spirit! Welcome to our ensemble cast!", false, "grant_ensemble_role");
        
        Debug.Log("Complex branching dialog created successfully!");
    }
    
    [ContextMenu("Validate Current Tree")]
    public void ValidateCurrentTree()
    {
        if (exampleTree != null)
        {
            exampleTree.ValidateTree();
        }
        else
        {
            Debug.LogWarning("No DialogTree assigned!");
        }
    }
    
    [ContextMenu("Print Tree Structure")]
    public void PrintTreeStructure()
    {
        if (exampleTree != null)
        {
            exampleTree.PrintTreeStructure();
        }
        else
        {
            Debug.LogWarning("No DialogTree assigned!");
        }
    }
}