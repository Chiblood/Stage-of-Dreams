using UnityEngine;

/// <summary>
/// Example script showing how to properly set up the integrated dialog system
/// with Spotlight, DialogueTrigger, and NPCContent working together.
/// </summary>
public class IntegratedDialogSystemExample : MonoBehaviour
{
    [Header("Setup Example")]
    [SerializeField] private Spotlight spotlight;
    [SerializeField] private DialogueTrigger dialogTrigger;
    [SerializeField] private NPCContent npcContent;
    
    [ContextMenu("Auto-Setup Integrated System")]
    public void AutoSetupIntegratedSystem()
    {
        // Get or create components
        if (spotlight == null)
            spotlight = GetComponent<Spotlight>();
            
        if (dialogTrigger == null)
            dialogTrigger = GetComponent<DialogueTrigger>();
            
        if (npcContent == null)
            npcContent = GetComponent<NPCContent>();
        
        // Create components if they don't exist
        if (spotlight == null)
        {
            spotlight = gameObject.AddComponent<Spotlight>();
            Debug.Log("Added Spotlight component");
        }
        
        if (dialogTrigger == null)
        {
            dialogTrigger = gameObject.AddComponent<DialogueTrigger>();
            Debug.Log("Added DialogueTrigger component");
        }
        
        // Configure the DialogueTrigger for spotlight triggering
        if (dialogTrigger != null)
        {
            // Use reflection to set private fields (for this example)
            var triggerType = typeof(DialogueTrigger);
            var triggerOnSpotlightField = triggerType.GetField("triggerOnSpotlight", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var requireInteractionField = triggerType.GetField("requireInteractionInput", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var targetNPCField = triggerType.GetField("targetNPC", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            triggerOnSpotlightField?.SetValue(dialogTrigger, true);
            requireInteractionField?.SetValue(dialogTrigger, true);
            if (npcContent != null)
                targetNPCField?.SetValue(dialogTrigger, npcContent);
            
            Debug.Log("Configured DialogueTrigger for spotlight interaction");
        }
        
        // Link spotlight to dialog trigger
        if (spotlight != null && dialogTrigger != null)
        {
            spotlight.SetDialogTrigger(dialogTrigger);
            Debug.Log("Linked Spotlight to DialogueTrigger");
        }
        
        Debug.Log("Integrated dialog system setup complete!");
        Debug.Log("Now create a DialogTree asset and assign it to your NPCContent component.");
    }
    
    [ContextMenu("Create Example Dialog Tree")]
    public void CreateExampleDialogTreeInstructions()
    {
        Debug.Log("To create an example dialog tree:");
        Debug.Log("1. Right-click in Project ? Create ? Dialog System ? Dialog Tree");
        Debug.Log("2. Name it 'SpotlightDialog' or similar");
        Debug.Log("3. Create DialogNodes in the inspector:");
        Debug.Log("   - Node 1: 'The spotlight finds you! What will you do?'");
        Debug.Log("     - Choice 1: 'Begin the performance' ? Custom Action: 'start_performance'");
        Debug.Log("     - Choice 2: 'I need more practice' ? End Dialog");
        Debug.Log("     - Choice 3: 'Tell me about the scene' ? Node 2");
        Debug.Log("   - Node 2: 'This scene represents the actor's fears...' ? End Dialog");
        Debug.Log("4. Assign the tree to your NPCContent component");
        Debug.Log("5. Test by entering the spotlight and pressing E");
    }
    
    private void Start()
    {
        // Subscribe to dialog trigger events
        if (dialogTrigger != null)
        {
            dialogTrigger.OnDialogTriggered += HandleDialogTriggered;
            dialogTrigger.OnDialogEnded += HandleDialogEnded;
        }
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from events
        if (dialogTrigger != null)
        {
            dialogTrigger.OnDialogTriggered -= HandleDialogTriggered;
            dialogTrigger.OnDialogEnded -= HandleDialogEnded;
        }
    }
    
    private void HandleDialogTriggered()
    {
        Debug.Log($"[{gameObject.name}] Dialog was triggered!");
        // Handle what happens when dialog starts
        // Example: Pause game, change lighting, play sound
    }
    
    private void HandleDialogEnded()
    {
        Debug.Log($"[{gameObject.name}] Dialog ended!");
        // Handle what happens when dialog ends
        // Example: Resume game, restore lighting, enable player movement
    }
}

/// <summary>
/// Example NPC implementation that works with the new integrated system.
/// </summary>
public class ExampleSpotlightNPC : NPCContent
{
    [Header("Performance Settings")]
    [SerializeField] private bool hasCompletedPerformance = false;
    
    public override void HandleCustomAction(string actionId)
    {
        switch (actionId.ToLower())
        {
            case "start_performance":
                StartPerformanceMode();
                break;
                
            case "give_encouragement":
                GiveEncouragement();
                break;
                
            case "explain_scene":
                ExplainScene();
                break;
                
            case "mark_complete":
                MarkPerformanceComplete();
                break;
                
            default:
                Debug.Log($"{npcName} doesn't know how to handle action: {actionId}");
                break;
        }
    }
    
    public override void OnDialogStarted()
    {
        Debug.Log($"Started talking to {npcName} - spotlight performance director");
        // Could pause player movement, change camera, etc.
    }
    
    public override void OnDialogEnded()
    {
        Debug.Log($"Finished talking to {npcName}");
        // Could restore player control, etc.
    }
    
    private void StartPerformanceMode()
    {
        Debug.Log($"{npcName}: Starting performance mode!");
        // This would integrate with your turn-based performance system
        // Example: PerformanceManager.Instance?.StartPerformance();
        
        // For now, just mark as complete
        hasCompletedPerformance = true;
    }
    
    private void GiveEncouragement()
    {
        Debug.Log($"{npcName}: You've got this! Trust your instincts!");
        // Could show encouraging UI, play sound, etc.
    }
    
    private void ExplainScene()
    {
        Debug.Log($"{npcName}: This scene is about overcoming stage fright...");
        // Could show tutorial UI, highlight stage elements, etc.
    }
    
    private void MarkPerformanceComplete()
    {
        hasCompletedPerformance = true;
        Debug.Log($"{npcName}: Excellent performance! You've mastered this scene.");
        // Could unlock next area, save progress, etc.
    }
}

/// <summary>
/// Example script for setting up a complete spotlight dialog scene.
/// </summary>
[System.Serializable]
public class SpotlightSceneSetup
{
    [Header("Scene Components")]
    public GameObject spotlightPrefab;
    public GameObject playerPrefab;
    public DialogTree spotlightDialogTree;
    
    [Header("Scene Settings")]
    public Vector3 spotlightPosition = Vector3.zero;
    public float spotlightRadius = 1.5f;
    public string sceneName = "Performance Scene";
    
    public void CreateScene()
    {
        // This method could be used to programmatically create complete scenes
        Debug.Log($"Creating {sceneName} with integrated dialog system...");
        
        // Implementation would create GameObjects, assign components, 
        // configure dialog trees, etc.
    }
}