/* DialogTree.cs
 * The dialog tree structure for managing NPC dialogues and player choices. It needs to be a prefab for Unity to serialize it.
 * Supports branching conversations based on player selections.
 * Includes a list of action choices to be fed to the DialogManager class. Within this list should be the capability to call functions in other scripts.
 * 
 * How to use in Unity:
 * 1. Create a new script that inherits from DialogTree.
 * 2. Define the dialog nodes and choices within the new script.
 * 3. Attach the script to an NPC GameObject.
 * 
 */

using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains a complete dialog tree structure starting from one node.
/// Enhanced with automatic node creation and tree building capabilities.
/// </summary>
[CreateAssetMenu(fileName = "New Dialog Tree", menuName = "Dialog System/Dialog Tree")]
public class DialogTree : ScriptableObject
{
    [Header("Tree Info")]
    public string treeName;
    [TextArea(2, 4)]
    public string description;
    
    [Header("Dialog Flow")]
    [SerializeReference] public DialogNode startingNode;
    
    [Header("All Nodes (Auto-Updated)")]
    [SerializeReference] private List<DialogNode> allNodes = new List<DialogNode>();
    
    [Header("Quick Add Nodes")]
    [SerializeField] private bool autoUpdateNodeList = true;
    
    #region Tree Management
    
    /// <summary>
    /// Get the first node to start the conversation
    /// </summary>
    public DialogNode GetStartingNode()
    {
        return startingNode;
    }
    
    /// <summary>
    /// Check if this tree has a valid starting point
    /// </summary>
    public bool IsValid()
    {
        return startingNode != null;
    }
    
    /// <summary>
    /// Get all nodes in this tree
    /// </summary>
    public List<DialogNode> GetAllNodes()
    {
        if (autoUpdateNodeList)
        {
            RefreshNodeList();
        }
        return new List<DialogNode>(allNodes);
    }
    
    /// <summary>
    /// Refresh the list of all nodes by traversing the tree
    /// </summary>
    [ContextMenu("Refresh Node List")]
    public void RefreshNodeList()
    {
        allNodes.Clear();
        if (startingNode != null)
        {
            TraverseAndCollectNodes(startingNode, new HashSet<DialogNode>());
        }
    }
    
    private void TraverseAndCollectNodes(DialogNode node, HashSet<DialogNode> visited)
    {
        if (node == null || visited.Contains(node)) return;
        
        visited.Add(node);
        allNodes.Add(node);
        
        // Check next node for auto-advance
        if (node.nextNode != null)
        {
            TraverseAndCollectNodes(node.nextNode, visited);
        }
        
        // Check all choice targets
        if (node.choices != null)
        {
            foreach (var choice in node.choices)
            {
                if (choice != null && choice.targetNode != null)
                {
                    TraverseAndCollectNodes(choice.targetNode, visited);
                }
            }
        }
    }
    
    #endregion
    
    #region Node Creation Methods
    
    /// <summary>
    /// Create the starting node for this tree
    /// </summary>
    public DialogNode CreateStartingNode(string speakerName, string dialogText, bool isPlayerSpeaking = false)
    {
        startingNode = new DialogNode(speakerName, dialogText, isPlayerSpeaking);
        RefreshNodeList();
        return startingNode;
    }
    
    /// <summary>
    /// Add a node as a choice from a parent node
    /// </summary>
    public DialogNode AddChoiceNode(DialogNode parentNode, string choiceText, string speakerName, string dialogText, bool isPlayerSpeaking = false, string customActionId = null)
    {
        if (parentNode == null)
        {
            Debug.LogWarning("Cannot add choice node: parent node is null");
            return null;
        }
        
        // Create the new node
        DialogNode newNode = new DialogNode(speakerName, dialogText, isPlayerSpeaking);
        
        // Add the choice to parent that leads to this new node
        var choice = parentNode.AddChoice(choiceText, customActionId);
        choice.SetTarget(newNode);
        
        RefreshNodeList();
        return newNode;
    }
    
    /// <summary>
    /// Add a node that auto-advances from a parent node
    /// </summary>
    public DialogNode AddSequentialNode(DialogNode parentNode, string speakerName, string dialogText, bool isPlayerSpeaking = false, float autoAdvanceDelay = 0f)
    {
        if (parentNode == null)
        {
            Debug.LogWarning("Cannot add sequential node: parent node is null");
            return null;
        }
        
        // Create the new node
        DialogNode newNode = new DialogNode(speakerName, dialogText, isPlayerSpeaking);
        if (autoAdvanceDelay > 0f)
        {
            newNode.autoAdvanceDelay = autoAdvanceDelay;
        }
        
        // Link it as the next node
        parentNode.SetNextNode(newNode);
        
        RefreshNodeList();
        return newNode;
    }
    
    /// <summary>
    /// Create a simple linear conversation
    /// </summary>
    public void CreateLinearConversation(string[] speakers, string[] dialogTexts, bool[] isPlayerSpeaking = null)
    {
        if (speakers == null || dialogTexts == null || speakers.Length != dialogTexts.Length)
        {
            Debug.LogError("Speakers and dialogTexts arrays must be the same length and not null");
            return;
        }
        
        DialogNode currentNode = null;
        
        for (int i = 0; i < speakers.Length; i++)
        {
            bool playerSpeaking = isPlayerSpeaking != null && i < isPlayerSpeaking.Length && isPlayerSpeaking[i];
            
            if (i == 0)
            {
                // Create starting node
                currentNode = CreateStartingNode(speakers[i], dialogTexts[i], playerSpeaking);
            }
            else
            {
                // Add sequential node
                currentNode = AddSequentialNode(currentNode, speakers[i], dialogTexts[i], playerSpeaking);
            }
        }
    }
    
    #endregion
    
    #region Editor Helper Methods
    
    /// <summary>
    /// Quick method to add a simple dialog node with one "Continue" choice
    /// </summary>
    [ContextMenu("Add Simple Dialog Node")]
    public void AddSimpleDialogNode()
    {
        if (startingNode == null)
        {
            CreateStartingNode("Speaker", "Enter dialog text here", false);
        }
        else
        {
            // Find the last node and add to it
            var lastNode = FindLastNode();
            AddChoiceNode(lastNode, "Continue", "Speaker", "Enter dialog text here", false);
        }
    }
    
    /// <summary>
    /// Add a branching choice to the last node
    /// </summary>
    [ContextMenu("Add Choice Branch")]
    public void AddChoiceBranch()
    {
        var lastNode = FindLastNode();
        if (lastNode != null)
        {
            AddChoiceNode(lastNode, "Choice Option", "Speaker", "Response to choice", false);
        }
    }
    
    /// <summary>
    /// Find the last node in the tree (node with no choices or next node)
    /// </summary>
    private DialogNode FindLastNode()
    {
        if (startingNode == null) return null;
        
        // Simple approach: find first end node
        var allNodes = GetAllNodes();
        foreach (var node in allNodes)
        {
            if (node.IsEndNode)
            {
                return node;
            }
        }
        
        // If no end node found, return starting node
        return startingNode;
    }
    
    #endregion
    
    #region Validation and Debugging
    
    /// <summary>
    /// Validate the tree structure and report any issues
    /// </summary>
    [ContextMenu("Validate Tree")]
    public void ValidateTree()
    {
        if (startingNode == null)
        {
            Debug.LogWarning($"DialogTree '{treeName}': No starting node set");
            return;
        }
        
        RefreshNodeList();
        
        Debug.Log($"DialogTree '{treeName}' validation:");
        Debug.Log($"  - Total nodes: {allNodes.Count}");
        Debug.Log($"  - Starting node: {startingNode.speakerName}: {startingNode.dialogText}");
        
        int choiceCount = 0;
        int endNodes = 0;
        
        foreach (var node in allNodes)
        {
            if (node.HasChoices)
            {
                choiceCount += node.choices.Length;
            }
            if (node.IsEndNode)
            {
                endNodes++;
            }
        }
        
        Debug.Log($"  - Total choices: {choiceCount}");
        Debug.Log($"  - End nodes: {endNodes}");
        
        if (endNodes == 0)
        {
            Debug.LogWarning("No end nodes found - conversation may loop indefinitely");
        }
    }
    
    /// <summary>
    /// Print the tree structure to console
    /// </summary>
    [ContextMenu("Print Tree Structure")]
    public void PrintTreeStructure()
    {
        if (startingNode == null)
        {
            Debug.Log("Tree is empty");
            return;
        }
        
        Debug.Log($"Dialog Tree: {treeName}");
        PrintNodeStructure(startingNode, "", new HashSet<DialogNode>());
    }
    
    private void PrintNodeStructure(DialogNode node, string indent, HashSet<DialogNode> visited)
    {
        if (node == null || visited.Contains(node)) return;
        
        visited.Add(node);
        
        string playerText = node.isPlayerSpeaking ? " (Player)" : "";
        Debug.Log($"{indent}{node.speakerName}{playerText}: {node.dialogText}");
        
        if (node.HasChoices)
        {
            foreach (var choice in node.choices)
            {
                if (choice != null)
                {
                    Debug.Log($"{indent}  Choice: {choice.choiceText}");
                    if (choice.targetNode != null)
                    {
                        PrintNodeStructure(choice.targetNode, indent + "    ", visited);
                    }
                }
            }
        }
        else if (node.nextNode != null)
        {
            Debug.Log($"{indent}  (Auto-advance)");
            PrintNodeStructure(node.nextNode, indent + "  ", visited);
        }
    }
    
    #endregion
}