/* Dialog Node.cs
 * Defines a single node in a dialog tree, including the dialog text, method calls (if applicable), choices, and references to subsequent nodes.
 * 
 * How to use in Unity: 
 * 1. Inherit from this class to create specific dialog nodes for your NPCs or dialog systems.
 * 2. Define the dialog text, speaker info, choices, and any events to trigger.
 * 
 */

using UnityEngine;
using System;
using UnityEngine.Events;

/// <summary>
/// Represents a single dialog node containing text, speaker info, and possible choices.
/// Each node can link to other nodes through choices or automatically advance.
/// </summary>
[System.Serializable]
public class DialogNode
{
    [Header("Dialog Content")]
    public string speakerName;
    [TextArea(3, 6)]
    public string dialogText;
    public bool isPlayerSpeaking = false;
    
    [Header("Flow Control")]
    public float autoAdvanceDelay = 0f; // 0 = wait for input
    [SerializeReference] public DialogChoice[] choices; // Empty array = auto-advance or end
    [SerializeReference] public DialogNode nextNode; // For auto-advance without choices

    [Header("Events")]
    public UnityEngine.Events.UnityEvent onDialogStart;
    public UnityEngine.Events.UnityEvent onDialogEnd;

    // Default constructor for serialization
    public DialogNode()
    {
        onDialogStart = new UnityEvent();
        onDialogEnd = new UnityEvent();
        choices = new DialogChoice[0];
    }

    // Constructor with parameters
    public DialogNode(string speaker, string text, bool playerSpeaking = false)
    {
        speakerName = speaker;
        dialogText = text;
        isPlayerSpeaking = playerSpeaking;
        onDialogStart = new UnityEvent();
        onDialogEnd = new UnityEvent();
        choices = new DialogChoice[0];
    }

    // Constructor with event
    public DialogNode(UnityEvent onDialogEnd)
    {
        this.onDialogEnd = onDialogEnd ?? new UnityEvent();
        onDialogStart = new UnityEvent();
        choices = new DialogChoice[0];
    }

    public bool HasChoices => choices != null && choices.Length > 0;
    public bool ShouldAutoAdvance => autoAdvanceDelay > 0f;
    
    /// <summary>
    /// Check if this node is the last in the dialog chain
    /// </summary>
    public bool IsEndNode => nextNode == null && (!HasChoices || choices.Length == 0);

    /// <summary>
    /// Add a choice to this node and optionally create/link a target node
    /// </summary>
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

    /// <summary>
    /// Add a choice that leads to a specific node
    /// </summary>
    public DialogChoice AddChoiceToNode(string choiceText, DialogNode targetNode, string customActionId = null)
    {
        var choice = AddChoice(choiceText, customActionId);
        choice.targetNode = targetNode;
        return choice;
    }

    /// <summary>
    /// Set the next node for auto-advance
    /// </summary>
    public void SetNextNode(DialogNode next)
    {
        nextNode = next;
    }

    /// <summary>
    /// Create and link a new node that this one will auto-advance to
    /// </summary>
    public DialogNode CreateNextNode(string speaker, string text, bool playerSpeaking = false)
    {
        var newNode = new DialogNode(speaker, text, playerSpeaking);
        nextNode = newNode;
        return newNode;
    }

    /// <summary>
    /// Remove a choice by index
    /// </summary>
    public void RemoveChoice(int index)
    {
        if (index < 0 || index >= choices.Length) return;

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
}

/// <summary>
/// Represents a choice the player can make in dialog
/// </summary>
[System.Serializable]
public class DialogChoice
{
    [Header("Choice Display")]
    public string choiceText;
    
    [Header("Target")]
    [SerializeReference] public DialogNode targetNode; // Where this choice leads
    
    [Header("Actions")]
    public string customActionId; // For triggering game events
    public UnityEngine.Events.UnityEvent onChoiceSelected;

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

    /// <summary>
    /// Create and set a target node for this choice
    /// </summary>
    public DialogNode CreateTargetNode(string speaker, string text, bool playerSpeaking = false)
    {
        targetNode = new DialogNode(speaker, text, playerSpeaking);
        return targetNode;
    }

    /// <summary>
    /// Set the target node for this choice
    /// </summary>
    public void SetTarget(DialogNode target)
    {
        targetNode = target;
    }
}