/* DialogNavigator.cs
 * Handles navigation through dialog trees independently from UI display. Is automatically used and called by DialogManager and does not need to be attached to any GameObject.
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
    /// Start navigating a dialog tree from an NPC.
    /// </summary>
    public bool StartDialog(NPCContent npc, string treeNameOverride = null)
    {
        if (npc == null)
        {
            Debug.LogWarning("[DialogNavigator] Cannot start dialog - NPC is null");
            return false;
        }
        
        // Get the appropriate dialog tree
        DialogTree tree = string.IsNullOrEmpty(treeNameOverride) 
            ? npc.GetMainDialogTree() 
            : npc.GetDialogTree(treeNameOverride);
            
        if (tree == null || !tree.IsValid())
        {
            Debug.LogWarning($"[DialogNavigator] Cannot start dialog - No valid dialog tree found for {npc.npcName}");
            return false;
        }
        
        currentNPC = npc;
        currentTree = tree;
        
        // Notify NPC that dialog started
        currentNPC.OnDialogStarted();
        
        // Navigate to starting node
        NavigateToNode(tree.GetStartingNode());
        
        Debug.Log($"[DialogNavigator] Started dialog with {npc.npcName} using tree '{tree.treeName}'");
        
        return true;
    }
    
    /// <summary>
    /// Navigate to a specific dialog node
    /// </summary>
    public void NavigateToNode(DialogNode node)
    {
        if (node == null)
        {
            Debug.LogWarning("[DialogNavigator] Cannot navigate to null node - ending dialog");
            EndDialog();
            return;
        }
        
        // Execute end events for previous node
        if (currentNode != null)
        {
            ExecuteNodeEndEvents(currentNode);
        }
        
        // Update current node
        currentNode = node;
        
        // Execute start events for new node
        ExecuteNodeStartEvents(currentNode);
        
        // Notify UI that node changed
        OnNodeChanged?.Invoke(currentNode);
        
        Debug.Log($"[DialogNavigator] Navigated to node: {node.CharacterName}: '{node.DialogText.Substring(0, Mathf.Min(30, node.DialogText.Length))}...?'");
    }
    
    /// <summary>
    /// Execute start events for a node
    /// </summary>
    private void ExecuteNodeStartEvents(DialogNode node)
    {
        if (node == null) return;
        
        // Execute new DialogEvent system
        if (node.StartEvents != null && node.StartEvents.Count > 0)
        {
            Debug.Log($"[DialogNavigator] Executing {node.StartEvents.Count} start events for node");
            foreach (var evt in node.StartEvents)
            {
                if (evt != null && evt.IsValid())
                {
                    try
                    {
                        evt.Execute();
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError($"[DialogNavigator] Error executing start event: {ex.Message}");
                    }
                }
            }
        }
        
        // Execute legacy UnityEvents for backwards compatibility
        if (node.OnDialogStart != null)
        {
            try
            {
                node.OnDialogStart.Invoke();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[DialogNavigator] Error executing OnDialogStart UnityEvent: {ex.Message}");
            }
        }
    }
    
    /// <summary>
    /// Execute end events for a node
    /// </summary>
    private void ExecuteNodeEndEvents(DialogNode node)
    {
        if (node == null) return;
        
        // Execute new DialogEvent system
        if (node.EndEvents != null && node.EndEvents.Count > 0)
        {
            Debug.Log($"[DialogNavigator] Executing {node.EndEvents.Count} end events for node");
            foreach (var evt in node.EndEvents)
            {
                if (evt != null && evt.IsValid())
                {
                    try
                    {
                        evt.Execute();
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError($"[DialogNavigator] Error executing end event: {ex.Message}");
                    }
                }
            }
        }
        
        // Execute legacy UnityEvents for backwards compatibility
        if (node.OnDialogEnd != null)
        {
            try
            {
                node.OnDialogEnd.Invoke();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[DialogNavigator] Error executing OnDialogEnd UnityEvent: {ex.Message}");
            }
        }
    }
    
    /// <summary>
    /// Select a choice by index
    /// </summary>
    public void SelectChoice(int choiceIndex)
    {
        if (currentNode == null)
        {
            Debug.LogWarning("[DialogNavigator] No active node - cannot select choice");
            return;
        }
        
        if (!currentNode.HasChoices)
        {
            Debug.LogWarning("[DialogNavigator] Current node has no choices");
            return;
        }
        
        if (choiceIndex < 0 || choiceIndex >= currentNode.Choices.Count)
        {
            Debug.LogError($"[DialogNavigator] Choice index {choiceIndex} out of range (0-{currentNode.Choices.Count - 1})");
            return;
        }
        
        var selectedChoice = currentNode.Choices[choiceIndex];
        if (selectedChoice == null)
        {
            Debug.LogError($"[DialogNavigator] Choice at index {choiceIndex} is null");
            return;
        }
        
        Debug.Log($"[DialogNavigator] Selected choice: '{selectedChoice.ChoiceText}'");
        
        // Execute choice events
        ExecuteChoiceEvents(selectedChoice);
        
        // Check for custom action ID (for backwards compatibility)
        if (!string.IsNullOrEmpty(selectedChoice.ChoiceId))
        {
            Debug.Log($"[DialogNavigator] Triggering custom action: {selectedChoice.ChoiceId}");
            OnCustomActionTriggered?.Invoke(selectedChoice, currentNPC);
            
            // Also notify NPC
            if (currentNPC != null)
            {
                currentNPC.HandleCustomAction(selectedChoice.ChoiceId);
            }
        }
        
        // Navigate to target node
        if (selectedChoice.TargetNode != null)
        {
            NavigateToNode(selectedChoice.TargetNode);
        }
        else if (selectedChoice.HasNamedTarget)
        {
            // Try to resolve named target
            if (currentTree != null)
            {
                var targetNode = currentTree.FindNodeByName(selectedChoice.TargetNodeName);
                if (targetNode != null)
                {
                    NavigateToNode(targetNode);
                }
                else
                {
                    Debug.LogError($"[DialogNavigator] Could not find named node '{selectedChoice.TargetNodeName}' - ending dialog");
                    EndDialog();
                }
            }
        }
        else
        {
            Debug.LogWarning("[DialogNavigator] Choice has no valid target - ending dialog");
            EndDialog();
        }
    }
    
    /// <summary>
    /// Execute choice events
    /// </summary>
    private void ExecuteChoiceEvents(DialogChoice choice)
    {
        if (choice == null) return;
        
        // Execute choice events
        if (choice.ChoiceEvents != null && choice.ChoiceEvents.Count > 0)
        {
            Debug.Log($"[DialogNavigator] Executing {choice.ChoiceEvents.Count} choice events");
            foreach (var evt in choice.ChoiceEvents)
            {
                if (evt != null && evt.IsValid())
                {
                    try
                    {
                        evt.Execute();
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError($"[DialogNavigator] Error executing choice event: {ex.Message}");
                    }
                }
            }
        }
        
        // Execute legacy UnityEvents for backwards compatibility
        if (choice.OnChoiceSelected != null)
        {
            foreach (var unityEvent in choice.OnChoiceSelected)
            {
                if (unityEvent != null)
                {
                    try
                    {
                        unityEvent.Invoke();
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError($"[DialogNavigator] Error executing choice UnityEvent: {ex.Message}");
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// Advance dialog for nodes without choices (auto-advance or manual advance)
    /// </summary>
    public void AdvanceDialog()
    {
        if (currentNode == null)
        {
            Debug.LogWarning("[DialogNavigator] No active node - cannot advance");
            return;
        }
        
        if (currentNode.HasChoices)
        {
            Debug.LogWarning("[DialogNavigator] Current node has choices - use SelectChoice instead");
            return;
        }
        
        // Check if there's a child node to advance to
        if (currentNode.ChildNode != null)
        {
            Debug.Log("[DialogNavigator] Advancing to child node");
            NavigateToNode(currentNode.ChildNode);
        }
        else
        {
            Debug.Log("[DialogNavigator] No more nodes - ending dialog");
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
            Debug.LogWarning("[DialogNavigator] No current NPC - cannot switch trees");
            return false;
        }
        
        var newTree = currentNPC.GetDialogTree(treeName);
        if (newTree == null || !newTree.IsValid())
        {
            Debug.LogWarning($"[DialogNavigator] Cannot switch to tree '{treeName}' - tree not found or invalid");
            return false;
        }
        
        Debug.Log($"[DialogNavigator] Switching to tree '{treeName}'");
        
        // Execute end events for current node before switching
        if (currentNode != null)
        {
            ExecuteNodeEndEvents(currentNode);
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
        if (node == null)
        {
            Debug.LogWarning("[DialogNavigator] Cannot force navigate to null node");
            return;
        }
        
        Debug.Log($"[DialogNavigator] Force navigating to node: {node.GetDisplayName()}");
        NavigateToNode(node);
    }
    
    /// <summary>
    /// End the current dialog session
    /// </summary>
    public void EndDialog()
    {
        if (currentNode == null && currentNPC == null)
        {
            Debug.LogWarning("[DialogNavigator] No active dialog to end");
            return;
        }
        
        Debug.Log($"[DialogNavigator] Ending dialog with {currentNPC?.npcName ?? "unknown NPC"}");
        
        // Execute end events for current node
        if (currentNode != null)
        {
            ExecuteNodeEndEvents(currentNode);
        }
        
        // Notify NPC that dialog ended
        if (currentNPC != null)
        {
            currentNPC.OnDialogEnded();
        }
        
        // Fire event for UI and external systems
        OnDialogEnded?.Invoke();
        
        // Clear state
        currentNode = null;
        currentNPC = null;
        currentTree = null;
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
            currentTree = currentTree,
            currentNPC = currentNPC,
            hasChoices = currentNode?.HasChoices ?? false,
            shouldAutoAdvance = (currentNode?.HasAutoAdvance ?? false) && (currentNode?.AutoAdvanceDelay > 0),
            autoAdvanceDelay = currentNode?.AutoAdvanceDelay ?? 0f,
            choiceCount = currentNode?.Choices?.Count ?? 0
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
    public DialogTree currentTree;
    public NPCContent currentNPC;
    public bool hasChoices;
    public bool shouldAutoAdvance;
    public float autoAdvanceDelay;
    public int choiceCount;
}