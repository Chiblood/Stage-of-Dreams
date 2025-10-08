using UnityEngine;

/// <summary>
/// Example NPC implementation showing how to use the dialog system.
/// This is a concrete example of how to inherit from NPCContent.
/// </summary>
public class ExampleNPC : NPCContent
{
    [Header("NPC Specific Settings")]
    [SerializeField] private bool giveReward = false;
    [SerializeField] private string rewardItem = "Stage Prop";
    
    /// <summary>
    /// Handle custom actions triggered by dialog choices
    /// </summary>
    public override void HandleCustomAction(string actionId)
    {
        switch (actionId.ToLower())
        {
            case "give_reward":
                GiveReward();
                break;
                
            case "start_practice":
                StartPracticeSession();
                break;
                
            case "explain_rules":
                ExplainStageRules();
                break;
                
            default:
                Debug.Log($"{npcName} doesn't know how to handle action: {actionId}");
                break;
        }
    }
    
    /// <summary>
    /// Called when dialog starts with this NPC
    /// </summary>
    public override void OnDialogStarted()
    {
        Debug.Log($"Started talking to {npcName}");
        // You could pause game, play sound, change NPC animation, etc.
    }
    
    /// <summary>
    /// Called when dialog ends with this NPC
    /// </summary>
    public override void OnDialogEnded()
    {
        Debug.Log($"Finished talking to {npcName}");
        // You could resume game, restore NPC state, etc.
    }
    
    // Example custom action methods
    private void GiveReward()
    {
        if (!giveReward)
        {
            Debug.Log($"{npcName}: I don't have anything for you right now.");
            return;
        }
        
        Debug.Log($"{npcName}: Here, take this {rewardItem}!");
        giveReward = false; // Give reward only once
        
        // Add item to player inventory here
        // InventorySystem.AddItem(rewardItem);
    }
    
    private void StartPracticeSession()
    {
        Debug.Log($"{npcName}: Let's start practicing your lines!");
        // Start practice mini-game or scene
        // PracticeManager.StartSession();
    }
    
    private void ExplainStageRules()
    {
        Debug.Log($"{npcName}: The stage has many rules you must follow...");
        // Show tutorial or rules UI
        // TutorialManager.ShowRules();
    }
}