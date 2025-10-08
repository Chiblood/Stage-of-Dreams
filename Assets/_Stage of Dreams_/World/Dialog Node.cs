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
    public DialogChoice[] choices; // Empty array = auto-advance or end
    
    [Header("Events")]
    public UnityEngine.Events.UnityEvent onDialogStart;
    public UnityEngine.Events.UnityEvent onDialogEnd;
    
    /// <summary>
    /// Check if this node has choices for the player to make
    /// </summary>
    public bool HasChoices => choices != null && choices.Length > 0;
    
    /// <summary>
    /// Check if this node should auto-advance after a delay
    /// </summary>
    public bool ShouldAutoAdvance => autoAdvanceDelay > 0f;
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
    public DialogNode targetNode; // Where this choice leads
    
    [Header("Actions")]
    public string customActionId; // For triggering game events
    public UnityEngine.Events.UnityEvent onChoiceSelected;
    
    /// <summary>
    /// Check if this choice has a custom action to execute
    /// </summary>
    public bool HasCustomAction => !string.IsNullOrEmpty(customActionId);
}