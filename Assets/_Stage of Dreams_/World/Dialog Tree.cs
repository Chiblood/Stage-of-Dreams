/* DialogTree.cs
 * The dialog tree structure for managing NPC dialogues and player choices. 
 * Enhanced with true tree structure, node naming, convergent path support, and advanced tree management.
 * Headers and visual organization are handled by DialogTreeEditor.cs to avoid conflicts in the Inspector.
 * 
 * How to use in Unity:
 * 1. Create DialogTree assets using the Unity Create menu.
 * 2. Use the enhanced editor to build complex dialog structures.
 * 3. Reference nodes by name to create convergent dialog paths.
 * 4. Use DialogEvents and UnityEvents to integrate with other game systems.
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Contains a complete dialog tree structure starting from one node.
/// Enhanced with automatic node creation, tree building capabilities, convergent path support, and DialogEvent integration.
/// </summary>
[CreateAssetMenu(fileName = "New Dialog Tree", menuName = "Dialog System/Dialog Tree")] // Allows creation from Unity Editor

public class DialogTree : ScriptableObject
{
    // Header information and labels are provided by DialogTreeEditor for a cleaner inspector view.
    [SerializeField] public string treeName;
    [SerializeField, TextArea(2, 4)] public string description;
    
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
        return startingNode != null && startingNode.IsValid();
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
        return nodes.FirstOrDefault(node => node.NodeName == nodeName);
    }

    /// <summary> Get all nodes with names (useful for convergent path setup) </summary>
    public List<DialogNode> GetNamedNodes()
    {
        var nodes = GetAllNodes();
        return nodes.Where(node => !string.IsNullOrEmpty(node.NodeName)).ToList();
    }

    /// <summary> Get all convergent nodes (nodes with multiple incoming references) </summary>
    public List<DialogNode> GetConvergentNodes()
    {
        var nodes = GetAllNodes();
        return nodes.Where(node => node.ParentNodes != null && node.ParentNodes.Count > 1).ToList();
    }

    /// <summary> Get all end nodes (nodes with no outgoing connections) </summary>
    public List<DialogNode> GetEndNodes()
    {
        var nodes = GetAllNodes();
        return nodes.Where(node => !node.HasChoices && node.ChildNode == null).ToList();
    }

    /// <summary> Get the maximum depth of the tree </summary>
    public int GetMaxDepth()
    {
        if (startingNode == null) return 0;
        return CalculateDepth(startingNode, new HashSet<DialogNode>());
    }

    private int CalculateDepth(DialogNode node, HashSet<DialogNode> visited)
    {
        if (node == null || visited.Contains(node)) return 0;
        
        visited.Add(node);
        int maxDepth = 0;
        
        // Check child node depth
        if (node.ChildNode != null)
        {
            maxDepth = Math.Max(maxDepth, CalculateDepth(node.ChildNode, visited));
        }
        
        // Check choice target depths
        if (node.HasChoices)
        {
            foreach (var choice in node.Choices)
            {
                if (choice?.TargetNode != null)
                {
                    maxDepth = Math.Max(maxDepth, CalculateDepth(choice.TargetNode, visited));
                }
            }
        }
        
        visited.Remove(node);
        return maxDepth + 1;
    }

    /// <summary> Check if a node name is unique in this tree </summary>
    public bool IsNodeNameUnique(string nodeName, DialogNode excludeNode = null)
    {
        if (string.IsNullOrEmpty(nodeName)) return true; // Empty names are allowed
        
        var nodes = GetAllNodes();
        return !nodes.Any(node => node != excludeNode && node.NodeName == nodeName);
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
        
        // Check child node for auto-advance
        if (node.ChildNode != null)
        {
            TraverseAndCollectNodes(node.ChildNode, visited);
        }
        
        // Check all choice targets
        if (node.HasChoices)
        {
            foreach (var choice in node.Choices)
            {
                if (choice?.TargetNode != null)
                {
                    TraverseAndCollectNodes(choice.TargetNode, visited);
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
            if (node.HasChoices)
            {
                foreach (var choice in node.Choices)
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
        // DialogNode is now a regular class, just use new
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
        
        // DialogNode is now a regular class, just use new
        DialogNode newNode = new DialogNode(speakerName, dialogText, isPlayerSpeaking, nodeName);
        
        // Add the choice to parent that leads to this new node
        // Use customActionId as choiceId parameter
        var choice = parentNode.AddChoice(choiceText, newNode, customActionId ?? "choice_" + parentNode.Choices.Count);
        newNode.AddParentNode(parentNode);
        
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
        
        // DialogChoice is now a regular class, just use new
        var choice = new DialogChoice(choiceText, parentNode);
        choice.TargetNodeName = targetNodeName;

        parentNode.Choices.Add(choice);
        
        // Try to resolve immediately if possible
        choice.ResolveNamedTarget(this);
        
        return choice;
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
        
        // DialogNode is now a regular class, just use new
        DialogNode newNode = new DialogNode(speakerName, dialogText, isPlayerSpeaking, nodeName);
        
        if (autoAdvanceDelay > 0f)
        {
            newNode.AutoAdvanceDelay = autoAdvanceDelay;
        }
        
        // Link it as the child node
        parentNode.SetChildNode(newNode);
        
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
    
    #region Dialog Event Management
    
    /// <summary>
    /// Add a DialogEvent to all nodes in the tree (useful for global events)
    /// </summary>
    public void AddEventToAllNodes(DialogEvent dialogEvent, bool isStartEvent = true)
    {
        if (dialogEvent == null) return;
        
        var nodes = GetAllNodes();
        foreach (var node in nodes)
        {
            if (isStartEvent)
                node.AddStartEvent(dialogEvent);
            else
                node.AddEndEvent(dialogEvent);
        }
    }
    
    /// <summary>
    /// Execute all start events for a given node (called by dialog system)
    /// </summary>
    public void ExecuteNodeStartEvents(DialogNode node)
    {
        
    }
    
    /// <summary>
    /// Execute all end events for a given node (called by dialog system)
    /// </summary>
    public void ExecuteNodeEndEvents(DialogNode node)
    {
        
    }
    
    /// <summary>
    /// Get all nodes that have start events
    /// </summary>
    public List<DialogNode> GetNodesWithStartEvents()
    {
        var nodes = GetAllNodes();
        return nodes.Where(node => node.StartEvents != null && node.StartEvents.Count > 0).ToList();
    }
    
    /// <summary>
    /// Get all nodes that have end events
    /// </summary>
    public List<DialogNode> GetNodesWithEndEvents()
    {
        var nodes = GetAllNodes();
        return nodes.Where(node => node.EndEvents != null && node.EndEvents.Count > 0).ToList();
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

    /// <summary> Find the last node in the tree (node with no choices or child node) </summary>
    private DialogNode FindLastNode()
    {
        if (startingNode == null) return null;
        
        // Simple approach: find first end node
        var endNodes = GetEndNodes();
        return endNodes.FirstOrDefault() ?? startingNode;
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
        Debug.Log($"  - Starting node: {startingNode.CharacterName}: {startingNode.DialogText}");
        
        int choiceCount = 0;
        int endNodes = 0;
        int namedNodes = 0;
        int convergentNodes = 0;
        int nodesWithStartEvents = 0;
        int nodesWithEndEvents = 0;
        
        foreach (var node in allNodes)
        {
            if (!node.IsValid())
            {
                Debug.LogWarning($"Invalid node found: {node.GetDisplayName()}");
            }
            
            if (node.HasChoices)
            {
                choiceCount += node.Choices.Count;
            }
            if (!node.HasChoices && node.ChildNode == null)
            {
                endNodes++;
            }
            if (!string.IsNullOrEmpty(node.NodeName))
            {
                namedNodes++;
            }
            if (node.ParentNodes != null && node.ParentNodes.Count > 1)
            {
                convergentNodes++;
            }
            if (node.StartEvents != null && node.StartEvents.Count > 0)
            {
                nodesWithStartEvents++;
            }
            if (node.EndEvents != null && node.EndEvents.Count > 0)
            {
                nodesWithEndEvents++;
            }
        }
        
        Debug.Log($"  - Total choices: {choiceCount}");
        Debug.Log($"  - End nodes: {endNodes}");
        Debug.Log($"  - Named nodes: {namedNodes}");
        Debug.Log($"  - Convergent nodes: {convergentNodes}");
        Debug.Log($"  - Nodes with start events: {nodesWithStartEvents}");
        Debug.Log($"  - Nodes with end events: {nodesWithEndEvents}");
        Debug.Log($"  - Max depth: {GetMaxDepth()}");
        
        if (endNodes == 0)
        {
            Debug.LogWarning("No end nodes found - conversation may loop indefinitely");
        }

        // Validate node name uniqueness
        ValidateNodeNames();
        
        // Validate named references
        ValidateNamedReferences();
        
        // Validate events
        ValidateDialogEvents();
    }

    private void ValidateNodeNames()
    {
        var nodeNames = new Dictionary<string, List<DialogNode>>();
        
        foreach (var node in allNodes)
        {
            if (!string.IsNullOrEmpty(node.NodeName))
            {
                if (!nodeNames.ContainsKey(node.NodeName))
                    nodeNames[node.NodeName] = new List<DialogNode>();
                
                nodeNames[node.NodeName].Add(node);
            }
        }
        
        foreach (var kvp in nodeNames)
        {
            if (kvp.Value.Count > 1)
            {
                Debug.LogWarning($"Duplicate node names found: {kvp.Key} ( {string.Join(", ", kvp.Value.Select(n => n.GetDisplayName()))} )");
            }
        }
    }

    private void ValidateNamedReferences()
    {
        var nodes = GetAllNodes();
        int unresolvedCount = 0;
        
        foreach (var node in nodes)
        {
            if (node.HasChoices)
            {
                foreach (var choice in node.Choices)
                {
                    if (choice != null && !choice.IsTargetResolved())
                    {
                        unresolvedCount++;
                    }
                }
            }
        }
        
        if (unresolvedCount > 0)
        {
            Debug.LogWarning($"Found {unresolvedCount} unresolved named references in choices");
        }
    }

    private void ValidateDialogEvents()
    {
        var nodes = GetAllNodes();
        int missingStartEvents = 0;
        int missingEndEvents = 0;
        
        foreach (var node in nodes)
        {
            if (node.StartEvents != null && node.StartEvents.Count > 0)
            {
                foreach (var dialogEvent in node.StartEvents)
                {
                    if (dialogEvent == null)
                    {
                        missingStartEvents++;
                    }
                }
            }
            if (node.EndEvents != null && node.EndEvents.Count > 0)
            {
                foreach (var dialogEvent in node.EndEvents)
                {
                    if (dialogEvent == null)
                    {
                        missingEndEvents++;
                    }
                }
            }
        }
        
        if (missingStartEvents > 0 || missingEndEvents > 0)
        {
            Debug.LogWarning($"Found {missingStartEvents} missing start events and {missingEndEvents} missing end events");
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
        
        string playerText = node.IsPlayerSpeaking ? " (Player)" : "";
        string nodeNameText = !string.IsNullOrEmpty(node.NodeName) ? $" [{node.NodeName}]" : "";
        string convergentText = (node.ParentNodes != null && node.ParentNodes.Count > 1) ? " (Convergent)" : "";
        
        Debug.Log($"{indent}{node.CharacterName}{playerText}{nodeNameText}{convergentText}: {node.DialogText}");
        
        if (node.HasChoices)
        {
            foreach (var choice in node.Choices)
            {
                if (choice != null)
                {
                    string targetInfo = "";
                    if (choice.HasNamedTarget)
                    {
                        targetInfo = $" ? [{choice.TargetNodeName}]";
                    }
                    
                    Debug.Log($"{indent}  Choice: {choice.ChoiceText}{targetInfo}");
                    if (choice.TargetNode != null)
                    {
                        PrintNodeStructure(choice.TargetNode, indent + "    ", visited);
                    }
                }
            }
        }
        else if (node.ChildNode != null)
        {
            string autoAdvanceText = node.AutoAdvanceDelay > 0 ? $" ({node.AutoAdvanceDelay}s)" : "";
            Debug.Log($"{indent}  (Auto-advance{autoAdvanceText})");
            PrintNodeStructure(node.ChildNode, indent + "  ", visited);
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