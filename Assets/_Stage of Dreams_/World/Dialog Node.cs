/* Dialog Node.cs
 * Defines a single node in a dialog tree, including the dialog text, method calls (if applicable), choices, and references to subsequent nodes.
 * 
 * Enhanced with true tree structure support including parent references, node naming for convergent paths, and proper tree traversal.
 * Headers and visual organization are handled by DialogNodePropertyDrawer.cs to avoid conflicts in the Inspector.
 * 
 * How to use in Unity: 
 * 1. Create DialogTree assets and build dialog structures using the enhanced editor.
 * 2. Use node names to create convergent dialog paths that can be referenced across branches.
 * 3. Set up UnityEvents to call methods in other scripts during dialog flow.
 * 
 */

using UnityEngine;
using System;
using UnityEngine.Events;
using System.Collections.Generic;

/// <summary>
/// Represents a single dialog node containing text, speaker info, and possible choices.
/// Enhanced with true tree structure including parent references and node identification for convergent paths.
/// Each node can link to other nodes through choices, auto-advance, or convergent references.
/// Visual organization is handled by the PropertyDrawer to avoid Inspector conflicts.
/// </summary>
[System.Serializable]
public class DialogNode
{
    // Node Identification - no header here, PropertyDrawer handles organization
    [Tooltip("Unique name for this node - enables convergent dialog paths and node referencing")]
    public string nodeName;
    
    // Dialog Content - no headers, PropertyDrawer organizes these
    public string speakerName;
    [TextArea(3, 6)]
    public string dialogText;
    public bool isPlayerSpeaking = false;

    // Flow Control - no headers, PropertyDrawer groups these
    public float autoAdvanceDelay = 0f; // 0 = wait for input
    [SerializeReference] public DialogChoice[] choices; // Empty array = auto-advance or end
    [SerializeReference] public DialogNode nextNode; // For auto-advance without choices

    // Tree Structure - no headers, PropertyDrawer handles organization
    [SerializeReference] public DialogNode parentDialog; // Reference to the parent dialog node
    [Tooltip("References to other nodes that can lead to this node (for convergent paths)")]
    [SerializeReference] public List<DialogNode> incomingReferences = new List<DialogNode>();

    // Events - no headers, PropertyDrawer makes these collapsible
    [SerializeReference] public UnityEngine.Events.UnityEvent onDialogStart; // Event triggered when this dialog starts
    [SerializeReference] public UnityEngine.Events.UnityEvent onDialogEnd; // Event triggered when this dialog ends

    // Default constructor for serialization
    public DialogNode()
    {
        onDialogStart = new UnityEvent();
        onDialogEnd = new UnityEvent();
        choices = new DialogChoice[0];
        incomingReferences = new List<DialogNode>();
    }

    // Constructor with parameters
    public DialogNode(string speaker, string text, bool playerSpeaking = false, string nodeId = null)
    {
        speakerName = speaker;
        dialogText = text;
        isPlayerSpeaking = playerSpeaking;
        nodeName = nodeId;
        onDialogStart = new UnityEvent();
        onDialogEnd = new UnityEvent();
        choices = new DialogChoice[0];
        incomingReferences = new List<DialogNode>();
    }

    // Constructor with event
    public DialogNode(UnityEvent onDialogEnd, string nodeId = null)
    {
        this.onDialogEnd = onDialogEnd ?? new UnityEvent();
        onDialogStart = new UnityEvent();
        choices = new DialogChoice[0];
        incomingReferences = new List<DialogNode>();
        nodeName = nodeId;
    }

    public bool HasChoices => choices != null && choices.Length > 0;
    public bool ShouldAutoAdvance => autoAdvanceDelay > 0f;
    public bool IsConvergentNode => incomingReferences != null && incomingReferences.Count > 1;

    /// <summary> Check if this node is the last in the dialog chain </summary>
    public bool IsEndNode => nextNode == null && (!HasChoices || choices.Length == 0);

    /// <summary> Check if this node is a root node (no parent) </summary>
    public bool IsRootNode => parentDialog == null;

    /// <summary> Get all child nodes (from choices and next node) </summary>
    public List<DialogNode> GetChildNodes()
    {
        var children = new List<DialogNode>();
        
        if (nextNode != null)
            children.Add(nextNode);
        
        if (choices != null)
        {
            foreach (var choice in choices)
            {
                if (choice != null && choice.targetNode != null)
                    children.Add(choice.targetNode);
            }
        }
        
        return children;
    }

    /// <summary> Add a choice to this node and optionally create/link a target node </summary>
    public DialogChoice AddChoice(string choiceText, string customActionId = null)
    {
        var newChoice = new DialogChoice(choiceText, customActionId);
        
        // Expand choices array
        var newChoices = new DialogChoice[choices.Length + 1];
        for (int i = 0; i < choices.Length; i++)
        {
            newChoices[i] = choices[i];
        }
        newChoices[choices.Length] = newChoice;
        choices = newChoices;
        
        return newChoice;
    }

    /// <summary> Add a choice that leads to a specific node </summary>
    public DialogChoice AddChoiceToNode(string choiceText, DialogNode targetNode, string customActionId = null)
    {
        var choice = AddChoice(choiceText, customActionId);
        choice.SetTarget(targetNode);
        return choice;
    }

    /// <summary> Add a choice that leads to a node by name (for convergent paths) </summary>
    public DialogChoice AddChoiceToNamedNode(String choiceText, string targetNodeName, DialogTree tree, string customActionId = null)
    {
        var choice = AddChoice(choiceText, customActionId);
        var targetNode = tree.FindNodeByName(targetNodeName);
        if (targetNode != null)
        {
            choice.SetTarget(targetNode);
            targetNode.AddIncomingReference(this);
        }
        else
        {
            Debug.LogWarning($"Could not find node with name '{targetNodeName}' in dialog tree '{tree.treeName}'");
        }
        return choice;
    }

    /// <summary> Set the next node for auto-advance </summary>
    public void SetNextNode(DialogNode next)
    {
        // Remove old parent reference if changing next node
        if (nextNode != null)
        {
            nextNode.RemoveIncomingReference(this);
        }
        
        nextNode = next;
        
        // Set up parent relationship
        if (next != null)
        {
            next.SetParent(this);
            next.AddIncomingReference(this);
        }
    }

    /// <summary> Create and link a new node that this one will auto-advance to </summary>
    public DialogNode CreateNextNode(string speaker, string text, bool playerSpeaking = false, string nodeId = null)
    {
        var newNode = new DialogNode(speaker, text, playerSpeaking, nodeId);
        SetNextNode(newNode);
        return newNode;
    }

    /// <summary> Set the parent node (for tree structure) </summary>
    public void SetParent(DialogNode parent)
    {
        parentDialog = parent;
    }

    /// <summary> Add an incoming reference (for convergent nodes) </summary>
    public void AddIncomingReference(DialogNode sourceNode)
    {
        if (incomingReferences == null)
            incomingReferences = new List<DialogNode>();
        
        if (!incomingReferences.Contains(sourceNode))
            incomingReferences.Add(sourceNode);
    }

    /// <summary> Remove an incoming reference </summary>
    public void RemoveIncomingReference(DialogNode sourceNode)
    {
        if (incomingReferences != null)
            incomingReferences.Remove(sourceNode);
    }

    /// <summary> Remove a choice by index </summary>
    public void RemoveChoice(int index)
    {
        if (index < 0 || index >= choices.Length) return;

        // Remove incoming reference if the choice had a target
        if (choices[index]?.targetNode != null)
        {
            choices[index].targetNode.RemoveIncomingReference(this);
        }

        var newChoices = new DialogChoice[choices.Length - 1];
        int newIndex = 0;
        for (int i = 0; i < choices.Length; i++)
        {
            if (i != index)
            {
                newChoices[newIndex] = choices[i];
                newIndex++;
            }
        }
        choices = newChoices;
    }

    /// <summary> Get the depth of this node in the tree (distance from root) </summary>
    public int GetDepth()
    {
        int depth = 0;
        DialogNode current = this;
        while (current.parentDialog != null)
        {
            depth++;
            current = current.parentDialog;
        }
        return depth;
    }

    /// <summary> Get all ancestor nodes up to the root </summary>
    public List<DialogNode> GetAncestors()
    {
        var ancestors = new List<DialogNode>();
        DialogNode current = parentDialog;
        while (current != null)
        {
            ancestors.Add(current);
            current = current.parentDialog;
        }
        return ancestors;
    }

    /// <summary> Get all descendant nodes in the subtree </summary>
    public List<DialogNode> GetDescendants()
    {
        var descendants = new List<DialogNode>();
        var visited = new HashSet<DialogNode>();
        CollectDescendants(descendants, visited);
        return descendants;
    }

    private void CollectDescendants(List<DialogNode> descendants, HashSet<DialogNode> visited)
    {
        if (visited.Contains(this)) return;
        visited.Add(this);

        var children = GetChildNodes();
        foreach (var child in children)
        {
            if (child != null && !visited.Contains(child))
            {
                descendants.Add(child);
                child.CollectDescendants(descendants, visited);
            }
        }
    }
}