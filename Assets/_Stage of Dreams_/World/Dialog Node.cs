/* Dialog Node.cs
 * Defines a single node in a dialog tree. The node will be part of a larger DialogTree asset.
 * A node has the following properties: 
 * Dialog text, method calls for the start or end of the dialog, choices, and references to subsequent nodes.
 *
 * Headers and visual organization are handled by DialogNodePropertyDrawer.cs to avoid conflicts in the Inspector.
 * 
 * How to use in Unity: 
 * 1. Create DialogTree assets and build dialog structures using the enhanced editor.
 * 2. Use node names to create convergent dialog paths that can be referenced across branches.
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
    // Node Identification - no header here, PropertyDrawer handles organization and display in Inspector
    #region Backing Fields - SerializeField for Unity serialization

    [SerializeField, Tooltip("Unique name for this node - enables convergent dialog paths and node referencing")]
    private string _nodeId;

    [SerializeField] private string _characterName;
    [SerializeField, TextArea(3, 6)] private string _dialogText;
    [SerializeField] private bool _isPlayerSpeaking = false;
    [SerializeField] private float _autoAdvanceDelay = 0f;

    // Tree Structure - SerializeReference for polymorphic serialization
    [SerializeReference] private List<DialogNode> _parentNodes;
    [SerializeReference] private DialogNode _childNode; // only used if no choices are present
    [SerializeReference] private List<DialogChoice> _choices; // special nodes

    // Dialog Events - SerializeReference for polymorphic event handling
    [SerializeReference] private List<DialogEvent> _startEvents;
    [SerializeReference] private List<DialogEvent> _endEvents;

    // Legacy Unity Events for backwards compatibility (optional)
    [SerializeField] private UnityEvent _onDialogStart;
    [SerializeField] private UnityEvent _onDialogEnd;

    #endregion

    #region Properties with Get/Set Methods

    /// <summary> Unique identifier for this node, used for referencing </summary>
    public string NodeName
    {
        get => _nodeId ?? string.Empty;
        set => _nodeId = value;
    }

    /// <summary> Name of the character speaking </summary>
    public string CharacterName
    {
        get => _characterName ?? string.Empty;
        set => _characterName = value;
    }

    /// <summary> The dialog text content </summary>
    public string DialogText
    {
        get => _dialogText ?? string.Empty;
        set => _dialogText = value;
    }

    /// <summary> Whether the player is speaking this dialog </summary>
    public bool IsPlayerSpeaking
    {
        get => _isPlayerSpeaking;
        set => _isPlayerSpeaking = value;
    }

    /// <summary> Auto-advance delay in seconds (0 = wait for input) </summary>
    public float AutoAdvanceDelay
    {
        get => _autoAdvanceDelay;
        set => _autoAdvanceDelay = Mathf.Max(0f, value);
    }

    /// <summary> Parent nodes in the dialog tree </summary>
    public List<DialogNode> ParentNodes
    {
        get
        {
            if (_parentNodes == null)
                _parentNodes = new List<DialogNode>();
            return _parentNodes;
        }
        set => _parentNodes = value;
    }

    /// <summary> Child node for auto-advance (null if choices are present) </summary>
    public DialogNode ChildNode
    {
        get => _childNode;
        set => _childNode = value;
    }

    /// <summary> Available choices at this node </summary>
    public List<DialogChoice> Choices
    {
        get
        {
            if (_choices == null)
                _choices = new List<DialogChoice>();
            return _choices;
        }
        set => _choices = value;
    }

    /// <summary> Events triggered when this dialog starts </summary>
    public List<DialogEvent> StartEvents
    {
        get
        {
            if (_startEvents == null)
                _startEvents = new List<DialogEvent>();
            return _startEvents;
        }
        set => _startEvents = value;
    }

    /// <summary> Events triggered when this dialog ends </summary>
    public List<DialogEvent> EndEvents
    {
        get
        {
            if (_endEvents == null)
                _endEvents = new List<DialogEvent>();
            return _endEvents;
        }
        set => _endEvents = value;
    }

    /// <summary> Legacy Unity Event for dialog start (for backwards compatibility) </summary>
    public UnityEvent OnDialogStart
    {
        get
        {
            if (_onDialogStart == null)
                _onDialogStart = new UnityEvent();
            return _onDialogStart;
        }
        set => _onDialogStart = value;
    }

    /// <summary> Legacy Unity Event for dialog end (for backwards compatibility) </summary>
    public UnityEvent OnDialogEnd
    {
        get
        {
            if (_onDialogEnd == null)
                _onDialogEnd = new UnityEvent();
            return _onDialogEnd;
        }
        set => _onDialogEnd = value;
    }

    #endregion

    #region Computed Properties

    /// <summary> True if this node has no parent nodes </summary>
    public bool IsRootNode => _parentNodes == null || _parentNodes.Count == 0;

    /// <summary> True if this node has available choices </summary>
    public bool HasChoices => _choices != null && _choices.Count > 0;

    /// <summary> True if this node auto-advances to another node </summary>
    public bool HasAutoAdvance => _childNode != null && _autoAdvanceDelay >= 0f;

    #endregion

    #region Constructors

    /// <summary> Default constructor for serialization </summary>
    public DialogNode()
    {
        Initialize();
    }

    /// <summary> Constructor with parameters </summary>
    public DialogNode(string speaker, string text, bool playerSpeaking = false, string nodeId = null)
    {
        _characterName = speaker;
        _dialogText = text;
        _isPlayerSpeaking = playerSpeaking;
        _nodeId = nodeId;
        Initialize();
    }

    /// <summary> Initialize all collections </summary>
    private void Initialize()
    {
        _parentNodes = new List<DialogNode>();
        _choices = new List<DialogChoice>();
        _startEvents = new List<DialogEvent>();
        _endEvents = new List<DialogEvent>();
        _onDialogStart = new UnityEvent();
        _onDialogEnd = new UnityEvent();
    }

    #endregion

    #region Node Hierarchy Management

    /// <summary> Set the next node for auto-advance </summary>
    public void SetChildNode(DialogNode next)
    {
        // Remove old parent reference if changing next node
        if (_childNode != null)
        {
            _childNode.RemoveParentNode(this);
        }

        _childNode = next;

        // Set up parent relationship
        if (next != null)
        {
            next.AddParentNode(this);
        }

        // Clear choices when setting auto-advance
        if (next != null && HasChoices)
        {
            _choices?.Clear();
        }
    }

    /// <summary> Create and link a new node that this one will auto-advance to </summary>
    public DialogNode CreateChildNode(string speaker, string text, bool playerSpeaking = false, string nodeId = null)
    {
        // DialogNode is now a regular class, just use new
        var newNode = new DialogNode(speaker, text, playerSpeaking, nodeId);
        
        SetChildNode(newNode);
        return newNode;
    }

    /// <summary> Add a parent node (for tree structure) </summary>
    public void AddParentNode(DialogNode sourceNode)
    {
        if (sourceNode != null && !ParentNodes.Contains(sourceNode))
        {
            ParentNodes.Add(sourceNode);
        }
    }

    /// <summary> Remove a parent node </summary>
    public void RemoveParentNode(DialogNode sourceNode)
    {
        if (_parentNodes != null)
        {
            _parentNodes.Remove(sourceNode);
        }
    }

    #endregion

    #region Choice Management

    /// <summary> Add a choice to this node and create/link to target node, set ChildNode to null </summary>
    public DialogChoice AddChoice(string choiceText, DialogNode targetNode, String choiceId)
    {
        // DialogChoice is now a regular class, just use new
        var newChoice = new DialogChoice(choiceText, this, targetNode, choiceId);

        Choices.Add(newChoice);
        _childNode = null; // Clear auto-advance when choices are added

        return newChoice;
    }

    /// <summary> Remove a choice by index </summary>
    public void RemoveChoice(int index)
    {
        if (_choices == null || index < 0 || index >= _choices.Count) return;

        // Remove parent reference if the choice had a target
        if (_choices[index]?.TargetNode != null)
        {
            _choices[index].TargetNode.RemoveParentNode(this);
        }

        _choices.RemoveAt(index);
    }

    #endregion

    #region Dialog Event Management

    /// <summary> Add a dialog event to be triggered when this node starts </summary>
    public void AddStartEvent(DialogEvent dialogEvent)
    {
        if (dialogEvent != null)
        {
            StartEvents.Add(dialogEvent);
        }
    }

    /// <summary> Add a dialog event to be triggered when this node ends </summary>
    public void AddEndEvent(DialogEvent dialogEvent)
    {
        if (dialogEvent != null)
        {
            EndEvents.Add(dialogEvent);
        }
    }

    /// <summary> Remove a start event by index </summary>
    public void RemoveStartEvent(int index)
    {
        if (_startEvents != null && index >= 0 && index < _startEvents.Count)
        {
            _startEvents.RemoveAt(index);
        }
    }

    /// <summary> Remove an end event by index </summary>
    public void RemoveEndEvent(int index)
    {
        if (_endEvents != null && index >= 0 && index < _endEvents.Count)
        {
            _endEvents.RemoveAt(index);
        }
    }

    /// <summary> Execute all start events for this node </summary>
    public void ExecuteStartEvents()
    {
        if (_startEvents != null)
        {
            foreach (var dialogEvent in _startEvents)
            {
                dialogEvent?.Execute();
            }
        }

        // Execute legacy UnityEvent for backwards compatibility
        _onDialogStart?.Invoke();
    }

    /// <summary> Execute all end events for this node </summary>
    public void ExecuteEndEvents()
    {
        if (_endEvents != null)
        {
            foreach (var dialogEvent in _endEvents)
            {
                dialogEvent?.Execute();
            }
        }

        // Execute legacy UnityEvent for backwards compatibility
        _onDialogEnd?.Invoke();
    }

    #endregion

    #region Validation and Utility

    /// <summary> Validate this node's configuration </summary>
    public bool IsValid()
    {
        // Must have either dialog text or be a connector node
        if (string.IsNullOrWhiteSpace(_dialogText) && !IsConnectorNode())
            return false;

        // Cannot have both choices and auto-advance
        if (HasChoices && HasAutoAdvance)
            return false;

        // Validate all events
        if (_startEvents != null)
        {
            foreach (var evt in _startEvents)
            {
                if (evt != null && !evt.IsValid())
                    return false;
            }
        }

        if (_endEvents != null)
        {
            foreach (var evt in _endEvents)
            {
                if (evt != null && !evt.IsValid())
                    return false;
            }
        }

        return true;
    }

    /// <summary> Check if this is a connector node (no dialog, just connects nodes) </summary>
    private bool IsConnectorNode()
    {
        return string.IsNullOrWhiteSpace(_dialogText) &&
               string.IsNullOrWhiteSpace(_characterName) &&
               (HasChoices || HasAutoAdvance);
    }

    /// <summary> Get display name for this node </summary>
    public string GetDisplayName()
    {
        if (!string.IsNullOrEmpty(_nodeId))
            return _nodeId;

        if (!string.IsNullOrEmpty(_dialogText))
            return _dialogText.Length > 30 ? _dialogText.Substring(0, 30) + "..." : _dialogText;

        return "Empty Node";
    }

    #endregion
}