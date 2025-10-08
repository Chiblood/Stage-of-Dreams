using UnityEngine;

/// <summary>
/// Example showing advanced usage of the DialogNavigator system.
/// Demonstrates direct navigation control, event handling, and tree switching.
/// </summary>
public class DialogNavigationExample : MonoBehaviour
{
    [Header("Example Settings")]
    [SerializeField] private DialogNode testNode;
    [SerializeField] private NPCContent testNPC;
    
    private DialogNavigator navigator;
    
    private void Start()
    {
        // Create our own navigator instance for advanced control
        navigator = new DialogNavigator();
        
        // Subscribe to all navigation events
        navigator.OnNodeChanged += HandleNodeChanged;
        navigator.OnCustomActionTriggered += HandleCustomAction;
        navigator.OnDialogEnded += HandleDialogEnded;
    }
    
    [ContextMenu("Test Direct Navigation")]
    public void TestDirectNavigation()
    {
        if (testNPC != null)
        {
            // Start dialog normally
            navigator.StartDialog(testNPC);
        }
    }
    
    [ContextMenu("Test Tree Switching")]
    public void TestTreeSwitching()
    {
        if (navigator.IsActive)
        {
            // Switch to a different tree during conversation
            navigator.SwitchToTree("AlternateConversation");
        }
        else
        {
            Debug.Log("No active dialog to switch trees");
        }
    }
    
    [ContextMenu("Test Force Navigation")]
    public void TestForceNavigation()
    {
        if (testNode != null)
        {
            // Force jump to a specific node (useful for cutscenes)
            navigator.ForceNavigateToNode(testNode);
        }
    }
    
    [ContextMenu("Get Navigation State")]
    public void LogNavigationState()
    {
        var state = navigator.GetCurrentState();
        
        Debug.Log($"Navigation State:");
        Debug.Log($"  Active: {state.isActive}");
        Debug.Log($"  Current NPC: {state.currentNPC?.npcName ?? "None"}");
        Debug.Log($"  Current Tree: {state.currentTree?.treeName ?? "None"}");
        Debug.Log($"  Has Choices: {state.hasChoices}");
        Debug.Log($"  Should Auto Advance: {state.shouldAutoAdvance}");
        
        if (state.currentNode != null)
        {
            Debug.Log($"  Current Text: {state.currentNode.dialogText}");
        }
    }
    
    // Event handlers demonstrating how to respond to navigation events
    
    private void HandleNodeChanged(DialogNode node)
    {
        Debug.Log($"[Navigation] Moved to new node: {node.speakerName}: {node.dialogText}");
        
        // Example: Change music based on speaker
        if (node.isPlayerSpeaking)
        {
            // Play player dialog music
            Debug.Log("Playing player dialog music");
        }
        else
        {
            // Play NPC dialog music  
            Debug.Log("Playing NPC dialog music");
        }
        
        // Example: Update character animations
        if (!string.IsNullOrEmpty(node.speakerName))
        {
            Debug.Log($"Setting {node.speakerName} to talking animation");
        }
    }
    
    private void HandleCustomAction(DialogChoice choice, NPCContent npc)
    {
        Debug.Log($"[Navigation] Custom action triggered: {choice.customActionId} from {npc.npcName}");
        
        // Example: Handle game-specific actions
        switch (choice.customActionId.ToLower())
        {
            case "start_minigame":
                Debug.Log("Starting minigame...");
                // MinigameManager.Instance.StartGame();
                break;
                
            case "save_game":
                Debug.Log("Saving game...");
                // SaveSystem.SaveGame();
                break;
                
            case "change_scene":
                Debug.Log("Changing scene...");
                // SceneManager.LoadScene("NextLevel");
                break;
        }
    }
    
    private void HandleDialogEnded()
    {
        Debug.Log("[Navigation] Dialog ended - returning to gameplay");
        
        // Example: Restore game state
        // PlayerController.EnableMovement();
        // GameManager.ResumeGame();
        // AudioManager.RestoreBackgroundMusic();
    }
    
    /// <summary>
    /// Example of using navigation for a scripted cutscene
    /// </summary>
    [ContextMenu("Test Scripted Sequence")]
    public void TestScriptedSequence()
    {
        StartCoroutine(ScriptedSequenceExample());
    }
    
    private System.Collections.IEnumerator ScriptedSequenceExample()
    {
        if (testNPC == null) yield break;
        
        Debug.Log("Starting scripted sequence...");
        
        // Start the dialog
        navigator.StartDialog(testNPC);
        
        // Wait a bit
        yield return new WaitForSeconds(2f);
        
        // Force advance to next part of sequence
        if (navigator.IsActive)
        {
            navigator.AdvanceDialog();
        }
        
        // Wait again
        yield return new WaitForSeconds(1f);
        
        // End the sequence
        navigator.EndDialog();
        
        Debug.Log("Scripted sequence complete!");
    }
    
    private void OnDestroy()
    {
        // Clean up event subscriptions
        if (navigator != null)
        {
            navigator.OnNodeChanged -= HandleNodeChanged;
            navigator.OnCustomActionTriggered -= HandleCustomAction;
            navigator.OnDialogEnded -= HandleDialogEnded;
        }
    }
}