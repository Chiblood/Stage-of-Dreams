/* DialogNavigator.cs
 * Handles navigation through dialog trees independently from UI display.
 * 
 * How to use in Unity:
 * 1. Create an instance of DialogNavigator in your dialog system or manager class.
 * 2. Use the provided methods to start dialogs, navigate nodes, and handle choices.
 * 3. Subscribe to events for UI updates and custom actions.
 * 4. Ensure NPCContent and DialogNode classes are properly set up for dialog data.
 * 
 */

using UnityEngine;
using System;

/// <summary>
/// Handles navigation through dialog trees independently from UI display.
/// Manages the current state of dialog progression and provides events for UI updates.
/// </summary>
public class DialogNavigator
{
    // Events for UI to subscribe to
    public event Action<DialogNode> OnNodeChanged;
    public event Action<DialogChoice, NPCContent> OnCustomActionTriggered;
    public event Action OnDialogEnded;
    
    // Current navigation state
    private DialogNode currentNode;
    private NPCContent currentNPC;
    private DialogTree currentTree;
    
    /// <summary>
    /// Check if navigation is currently active
    /// </summary>
    public bool IsActive => currentNode != null;
    
    /// <summary>
    /// Get the current dialog node being displayed
    /// </summary>
    public DialogNode CurrentNode => currentNode;
    
    /// <summary>
    /// Get the current NPC being talked to
    /// </summary>
    public NPCContent CurrentNPC => currentNPC;
    
    /// <summary>
    /// Start navigating a dialog tree from an NPC
    /// </summary>
    public bool StartDialog(NPCContent npc, string treeNameOverride = null)
    {
        if (npc == null)
        {
            Debug.LogWarning("Cannot start dialog - NPC is null");
            return false;
        }
        
        // Get the appropriate dialog tree
        DialogTree tree = string.IsNullOrEmpty(treeNameOverride) 
            ? npc.GetMainDialogTree() 
            : npc.GetDialogTree(treeNameOverride);
            
        if (tree == null || !tree.IsValid())
        {
            Debug.LogWarning($"Cannot start dialog - No valid dialog tree found for {npc.npcName}");
            return false;
        }
        
        currentNPC = npc;
        currentTree = tree;
        
        // Notify NPC that dialog started
        currentNPC.OnDialogStarted();
        
        // Navigate to starting node
        NavigateToNode(tree.GetStartingNode());
        
        return true;
    }
    
    /// <summary>
    /// Navigate to a specific dialog node
    /// </summary>
    public void NavigateToNode(DialogNode node)
    {
        if (node == null)
        {
            Debug.LogWarning("Cannot navigate to null node");
            return;
        }
        
        // End previous node if exists
        if (currentNode != null)
        {
            currentNode.onDialogEnd?.Invoke();
        }
        
        // Set new current node
        currentNode = node;
        
        // Trigger node start event
        currentNode.onDialogStart?.Invoke();
        
        // Notify UI about node change
        OnNodeChanged?.Invoke(currentNode);
    }
    
    /// <summary>
    /// Handle a player choice selection
    /// </summary>
    public void SelectChoice(int choiceIndex)
    {
        if (currentNode == null || !currentNode.HasChoices)
        {
            Debug.LogWarning("Cannot select choice - no current node or no choices available");
            return;
        }
        
        if (choiceIndex < 0 || choiceIndex >= currentNode.choices.Length)
        {
            Debug.LogWarning($"Choice index {choiceIndex} is out of range");
            return;
        }
        
        DialogChoice selectedChoice = currentNode.choices[choiceIndex];
        
        // Trigger choice event
        selectedChoice.onChoiceSelected?.Invoke();
        
        // Handle custom action if present
        if (selectedChoice.HasCustomAction && currentNPC != null)
        {
            OnCustomActionTriggered?.Invoke(selectedChoice, currentNPC);
            currentNPC.HandleCustomAction(selectedChoice.customActionId);
        }
        
        // Navigate to target node or end dialog
        if (selectedChoice.targetNode != null)
        {
            NavigateToNode(selectedChoice.targetNode);
        }
        else
        {
            EndDialog();
        }
    }
    
    /// <summary>
    /// Advance dialog for nodes without choices (auto-advance or manual advance)
    /// </summary>
    public void AdvanceDialog()
    {
        if (currentNode == null)
        {
            Debug.LogWarning("Cannot advance - no current node");
            return;
        }
        
        // Only advance if there are no choices
        if (!currentNode.HasChoices)
        {
            EndDialog();
        }
    }
    
    /// <summary>
    /// Jump to a specific named tree within the current NPC
    /// </summary>
    public bool SwitchToTree(string treeName)
    {
        if (currentNPC == null)
        {
            Debug.LogWarning("Cannot switch trees - no current NPC");
            return false;
        }
        
        DialogTree newTree = currentNPC.GetDialogTree(treeName);
        if (newTree == null || !newTree.IsValid())
        {
            Debug.LogWarning($"Cannot switch to tree '{treeName}' - tree not found or invalid");
            return false;
        }
        
        currentTree = newTree;
        NavigateToNode(newTree.GetStartingNode());
        return true;
    }
    
    /// <summary>
    /// Force navigation to a specific node (useful for scripted sequences)
    /// </summary>
    public void ForceNavigateToNode(DialogNode node)
    {
        NavigateToNode(node);
    }
    
    /// <summary>
    /// End the current dialog session
    /// </summary>
    public void EndDialog()
    {
        // Trigger current node end event
        if (currentNode != null)
        {
            currentNode.onDialogEnd?.Invoke();
        }
        
        // Notify NPC that dialog ended
        if (currentNPC != null)
        {
            currentNPC.OnDialogEnded();
        }
        
        // Clear state
        currentNode = null;
        currentNPC = null;
        currentTree = null;
        
        // Notify UI that dialog ended
        OnDialogEnded?.Invoke();
    }
    
    /// <summary>
    /// Get information about the current dialog state
    /// </summary>
    public DialogNavigationState GetCurrentState()
    {
        return new DialogNavigationState
        {
            isActive = IsActive,
            currentNode = currentNode,
            currentNPC = currentNPC,
            currentTree = currentTree,
            hasChoices = currentNode?.HasChoices ?? false,
            shouldAutoAdvance = currentNode?.ShouldAutoAdvance ?? false,
            autoAdvanceDelay = currentNode?.autoAdvanceDelay ?? 0f
        };
    }
}

/// <summary>
/// Snapshot of current dialog navigation state
/// </summary>
public struct DialogNavigationState
{
    public bool isActive;
    public DialogNode currentNode;
    public NPCContent currentNPC;
    public DialogTree currentTree;
    public bool hasChoices;
    public bool shouldAutoAdvance;
    public float autoAdvanceDelay;
}