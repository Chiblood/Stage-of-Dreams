/* DialogTree.cs
 * The dialog tree structure for managing NPC dialogues and player choices. 
 * Enhanced with true tree structure, node naming, convergent path support, and advanced tree management.
 * Headers and visual organization are handled by DialogTreeEditor.cs to avoid conflicts in the Inspector.
 * 
 * How to use in Unity:
 * 1. Create DialogTree assets using the Unity Create menu.
 * 2. Use the enhanced editor to build complex dialog structures.
 * 3. Reference nodes by name to create convergent dialog paths.
 * 4. Use UnityEvents to integrate with other game systems.
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Contains a complete dialog tree structure starting from one node.
/// Enhanced with automatic node creation, tree building capabilities, and convergent path support.
/// </summary>
[CreateAssetMenu(fileName = "New Dialog Tree", menuName = "Dialog System/Dialog Tree")] // Allows creation from Unity Editor

public class DialogTree : ScriptableObject
{
    // Header information and labels are provided by DialogTreeEditor for a cleaner inspector view.
    public string treeName;
    [TextArea(2, 4)]
    public string description;
    
    [SerializeReference] public DialogNode startingNode;
    
    [SerializeReference] private List<DialogNode> allNodes = new List<DialogNode>();
    [SerializeField] private bool autoUpdateNodeList = true;
    [SerializeField] private bool validateOnSave = true;
    
    #region Tree Management
    
    /// <summary> Get the first node to start the conversation </summary>
    public DialogNode GetStartingNode()
    {
        return startingNode;
    }
    
    /// <summary> Check if this tree has a valid starting point</summary>
    public bool IsValid()
    {
        return startingNode != null;
    }
    
    /// <summary> Get all nodes in this tree </summary>
    public List<DialogNode> GetAllNodes()
    {
        if (autoUpdateNodeList)
        {
            RefreshNodeList();
        }
        return new List<DialogNode>(allNodes);
    }

    /// <summary> Find a node by its name </summary>
    public DialogNode FindNodeByName(string nodeName)
    {
        if (string.IsNullOrEmpty(nodeName)) return null;
        
        var nodes = GetAllNodes();
        return nodes.FirstOrDefault(node => node.nodeName == nodeName);
    }

    /// <summary> Get all nodes with names (useful for convergent path setup) </summary>
    public List<DialogNode> GetNamedNodes()
    {
        var nodes = GetAllNodes();
        return nodes.Where(node => !string.IsNullOrEmpty(node.nodeName)).ToList();
    }

    /// <summary> Get all convergent nodes (nodes with multiple incoming references) </summary>
    public List<DialogNode> GetConvergentNodes()
    {
        var nodes = GetAllNodes();
        return nodes.Where(node => node.IsConvergentNode).ToList();
    }

    /// <summary> Get all end nodes (nodes with no outgoing connections) </summary>
    public List<DialogNode> GetEndNodes()
    {
        var nodes = GetAllNodes();
        return nodes.Where(node => node.IsEndNode).ToList();
    }

    /// <summary> Get the maximum depth of the tree </summary>
    public int GetMaxDepth()
    {
        var nodes = GetAllNodes();
        return nodes.Count > 0 ? nodes.Max(node => node.GetDepth()) : 0;
    }

    /// <summary> Check if a node name is unique in this tree </summary>
    public bool IsNodeNameUnique(string nodeName, DialogNode excludeNode = null)
    {
        if (string.IsNullOrEmpty(nodeName)) return true; // Empty names are allowed
        
        var nodes = GetAllNodes();
        return !nodes.Any(node => node != excludeNode && node.nodeName == nodeName);
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

    /// <summary> Resolve all named node references in choices </summary>
    [ContextMenu("Resolve Named References")]
    public void ResolveNamedReferences()
    {
        var nodes = GetAllNodes();
        int resolvedCount = 0;
        
        foreach (var node in nodes)
        {
            if (node.choices != null)
            {
                foreach (var choice in node.choices)
                {
                    if (choice != null && choice.ResolveNamedTarget(this))
                    {
                        resolvedCount++;
                    }
                }
            }
        }
        
        Debug.Log($"Resolved {resolvedCount} named node references in '{treeName}'");
    }
    
    #endregion
    
    #region Node Creation Methods
    
    /// <summary>
    /// Create the starting node for this tree
    /// </summary>
    public DialogNode CreateStartingNode(string speakerName, string dialogText, bool isPlayerSpeaking = false, string nodeName = null)
    {
        startingNode = new DialogNode(speakerName, dialogText, isPlayerSpeaking, nodeName);
        RefreshNodeList();
        return startingNode;
    }
    
    /// <summary>
    /// Add a node as a choice from a parent node
    /// </summary>
    public DialogNode AddChoiceNode(DialogNode parentNode, string choiceText, string speakerName, string dialogText, bool isPlayerSpeaking = false, string customActionId = null, string nodeName = null)
    {
        if (parentNode == null)
        {
            Debug.LogWarning("Cannot add choice node: parent node is null");
            return null;
        }
        
        // Create the new node
        DialogNode newNode = new DialogNode(speakerName, dialogText, isPlayerSpeaking, nodeName);
        
        // Add the choice to parent that leads to this new node
        var choice = parentNode.AddChoice(choiceText, customActionId);
        choice.SetTarget(newNode);
        newNode.SetParent(parentNode);
        newNode.AddIncomingReference(parentNode);
        
        RefreshNodeList();
        return newNode;
    }

    /// <summary>
    /// Add a choice that points to an existing named node (for convergent paths)
    /// </summary>
    public DialogChoice AddChoiceToNamedNode(DialogNode parentNode, string choiceText, string targetNodeName, string customActionId = null)
    {
        if (parentNode == null)
        {
            Debug.LogWarning("Cannot add choice: parent node is null");
            return null;
        }
        
        return parentNode.AddChoiceToNamedNode(choiceText, targetNodeName, this, customActionId);
    }
    
    /// <summary>
    /// Add a node that auto-advances from a parent node
    /// </summary>
    public DialogNode AddSequentialNode(DialogNode parentNode, string speakerName, string dialogText, bool isPlayerSpeaking = false, float autoAdvanceDelay = 0f, string nodeName = null)
    {
        if (parentNode == null)
        {
            Debug.LogWarning("Cannot add sequential node: parent node is null");
            return null;
        }
        
        // Create the new node
        DialogNode newNode = new DialogNode(speakerName, dialogText, isPlayerSpeaking, nodeName);
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
    /// Create a simple linear conversation using arrays of speakers and dialog texts
    /// </summary>
    public void CreateLinearConversation(string[] speakers, string[] dialogTexts, bool[] isPlayerSpeaking = null, string[] nodeNames = null)
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
            string nodeName = nodeNames != null && i < nodeNames.Length ? nodeNames[i] : null;
            
            if (i == 0)
            {
                // Create starting node
                currentNode = CreateStartingNode(speakers[i], dialogTexts[i], playerSpeaking, nodeName);
            }
            else
            {
                // Add sequential node
                currentNode = AddSequentialNode(currentNode, speakers[i], dialogTexts[i], playerSpeaking, 0f, nodeName);
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
    
    /// <summary> Add a branching choice to the last node </summary>
    [ContextMenu("Add Choice Branch")]
    public void AddChoiceBranch()
    {
        var lastNode = FindLastNode();
        if (lastNode != null)
        {
            AddChoiceNode(lastNode, "Choice Option", "Speaker", "Response to choice", false);
        }
    }

    /// <summary> Find the last node in the tree (node with no choices or next node) </summary>
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
        int namedNodes = 0;
        int convergentNodes = 0;
        
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
            if (!string.IsNullOrEmpty(node.nodeName))
            {
                namedNodes++;
            }
            if (node.IsConvergentNode)
            {
                convergentNodes++;
            }
        }
        
        Debug.Log($"  - Total choices: {choiceCount}");
        Debug.Log($"  - End nodes: {endNodes}");
        Debug.Log($"  - Named nodes: {namedNodes}");
        Debug.Log($"  - Convergent nodes: {convergentNodes}");
        Debug.Log($"  - Max depth: {GetMaxDepth()}");
        
        if (endNodes == 0)
        {
            Debug.LogWarning("No end nodes found - conversation may loop indefinitely");
        }

        // Validate node name uniqueness
        ValidateNodeNames();
        
        // Validate named references
        ValidateNamedReferences();
    }

    private void ValidateNodeNames()
    {
        var nodeNames = new Dictionary<string, List<DialogNode>>();
        
        foreach (var node in allNodes)
        {
            if (!string.IsNullOrEmpty(node.nodeName))
            {
                if (!nodeNames.ContainsKey(node.nodeName))
                    nodeNames[node.nodeName] = new List<DialogNode>();
                
                nodeNames[node.nodeName].Add(node);
            }
        }
        
        foreach (var kvp in nodeNames)
        {
            if (kvp.Value.Count > 1)
            {
                Debug.LogWarning($"Duplicate node name '{kvp.Key}' found in {kvp.Value.Count} nodes");
            }
        }
    }

    private void ValidateNamedReferences()
    {
        foreach (var node in allNodes)
        {
            if (node.choices != null)
            {
                foreach (var choice in node.choices)
                {
                    if (choice != null && choice.HasNamedTarget)
                    {
                        if (FindNodeByName(choice.targetNodeName) == null)
                        {
                            Debug.LogWarning($"Choice '{choice.choiceText}' references unknown node '{choice.targetNodeName}'");
                        }
                    }
                }
            }
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
        string nodeNameText = !string.IsNullOrEmpty(node.nodeName) ? $" [{node.nodeName}]" : "";
        string convergentText = node.IsConvergentNode ? " (Convergent)" : "";
        
        Debug.Log($"{indent}{node.speakerName}{playerText}{nodeNameText}{convergentText}: {node.dialogText}");
        
        if (node.HasChoices)
        {
            foreach (var choice in node.choices)
            {
                if (choice != null)
                {
                    string targetInfo = "";
                    if (choice.HasNamedTarget)
                    {
                        targetInfo = $" -> [{choice.targetNodeName}]";
                    }
                    
                    Debug.Log($"{indent}  Choice: {choice.choiceText}{targetInfo}");
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

    #region Unity Callbacks

    private void OnValidate()
    {
        if (validateOnSave && Application.isPlaying)
        {
            ValidateTree();
        }
    }

    #endregion
}