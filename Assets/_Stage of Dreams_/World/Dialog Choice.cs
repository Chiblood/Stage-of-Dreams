/* Dialog Choice.cs
 * Defines a single choice in a dialog tree, including the choice text, target Dialog Node, and any associated events.
 */

using UnityEngine;
using System;
using UnityEngine.Events;
using System.Collections.Generic;

/// <summary> Represents a choice the player can make in dialog </summary>
[System.Serializable]
public class DialogChoice
{
    // Choice Display - no headers, PropertyDrawer handles organization
    public string choiceText;
    
    // Target - no headers, PropertyDrawer groups these
    [SerializeReference] public DialogNode targetNode; // Where this choice leads
    [Tooltip("Name of target node (for convergent paths) - will be resolved at runtime")]
    public string targetNodeName; // For referencing nodes by name
    
    // Actions - no headers, PropertyDrawer organizes these
    public string customActionId; // For triggering game events
    [SerializeReference] public UnityEngine.Events.UnityEvent onChoiceSelected;

    // Default constructor for serialization
    public DialogChoice()
    {
        onChoiceSelected = new UnityEvent();
    }

    // Constructor with parameters
    public DialogChoice(string text, string actionId = null)
    {
        choiceText = text;
        customActionId = actionId;
        onChoiceSelected = new UnityEvent();
    }

    // Constructor with event
    public DialogChoice(UnityEvent onChoiceSelected)
    {
        this.onChoiceSelected = onChoiceSelected ?? new UnityEvent();
    }

    /// <summary>
    /// Check if this choice has a custom action to execute
    /// </summary>
    public bool HasCustomAction => !string.IsNullOrEmpty(customActionId);

    /// <summary> Check if this choice references a node by name </summary>
    public bool HasNamedTarget => !string.IsNullOrEmpty(targetNodeName);

    /// <summary> Create and set a target node for this choice </summary>
    public DialogNode CreateTargetNode(string speaker, string text, bool playerSpeaking = false, string nodeId = null)
    {
        targetNode = new DialogNode(speaker, text, playerSpeaking, nodeId);
        if (targetNode != null)
        {
            targetNode.AddIncomingReference(null); // Will be set by parent when choice is added
        }
        return targetNode;
    }

    /// <summary> Set the target node for this choice </summary>
    public void SetTarget(DialogNode target)
    {
        targetNode = target;
        if (target != null)
        {
            target.AddIncomingReference(null); // Will be set by parent when choice is added
        }
    }

    /// <summary> Set target by node name (for convergent paths) </summary>
    public void SetTargetByName(string nodeName)
    {
        targetNodeName = nodeName;
    }

    /// <summary> Resolve named target to actual node reference </summary>
    public bool ResolveNamedTarget(DialogTree tree)
    {
        if (!HasNamedTarget) return targetNode != null;
        
        var namedNode = tree.FindNodeByName(targetNodeName);
        if (namedNode != null)
        {
            targetNode = namedNode;
            return true;
        }
        
        Debug.LogWarning($"Could not resolve target node name '{targetNodeName}' in tree '{tree.treeName}'");
        return false;
    }
}