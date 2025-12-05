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

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Node type enum for specialized dialog behaviors
/// </summary>
public enum DialogNodeType
{
    StandardDialog,      // Regular dialog with choices or auto-advance
    RememberTheScript,   // Typing minigame
    // Future minigame types can be added here:
    // CalmDialog,
    // ImproveSkill,
    // etc.
}

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

    [SerializeField, Tooltip("Node type - determines which editor and behavior to use")]
    private DialogNodeType _nodeType = DialogNodeType.StandardDialog;

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

    // RememberTheScript Minigame Settings
    [SerializeField, Tooltip("Enable RememberTheScript typing minigame for this node")]
    private bool _isRememberScriptNode = false;

    [SerializeField, Tooltip("The exact phrase the player must type")]
    private string _targetPhrase = "";

    [SerializeField, Tooltip("Maximum number of mistakes allowed before failure")]
    private int _maxMistakes = 3;

    [SerializeField, Tooltip("Time limit in seconds (0 = no limit)")]
    private float _timeLimit = 30f;

    [SerializeField, Tooltip("Audience score penalty per mistake")]
    private float _scorePerMistake = -5f;

    [SerializeField, Tooltip("Audience score reward on successful completion")]
    private float _scoreOnSuccess = 20f;

    [SerializeField, Tooltip("Should typing be case-sensitive?")]
    private bool _caseSensitive = false;

    // NEW: Direct object reference for failure node (same as success node pattern)
    [SerializeReference, Tooltip("Node to jump to on failure (null = retry)")]
    private DialogNode _failureNode = null;

    // LEGACY: Keep string reference for backwards compatibility
    [SerializeField, Tooltip("LEGACY: Node name to jump to on failure (use _failureNode instead)")]
    private string _failureNodeName = "";

    #endregion

    #region Properties with Get/Set Methods

    /// <summary> Type of this dialog node (determines editor and behavior) </summary>
    public DialogNodeType NodeType
    {
        get => _nodeType;
        set => _nodeType = value;
    }

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

    // RememberTheScript Minigame Properties
    /// <summary> Whether this node triggers the RememberTheScript typing minigame </summary>
    public bool IsRememberScriptNode
    {
        get => _isRememberScriptNode;
        set => _isRememberScriptNode = value;
    }

    /// <summary> The exact phrase the player must type </summary>
    public string TargetPhrase
    {
        get => _targetPhrase ?? string.Empty;
        set => _targetPhrase = value;
    }

    /// <summary> Maximum number of mistakes allowed before failure </summary>
    public int MaxMistakes
    {
        get => _maxMistakes;
        set => _maxMistakes = Mathf.Max(0, value);
    }

    /// <summary> Time limit in seconds (0 = no limit) </summary>
    public float TimeLimit
    {
        get => _timeLimit;
        set => _timeLimit = Mathf.Max(0f, value);
    }

    /// <summary> Audience score penalty per mistake </summary>
    public float ScorePerMistake
    {
        get => _scorePerMistake;
        set => _scorePerMistake = value;
    }

    /// <summary> Audience score reward on successful completion </summary>
    public float ScoreOnSuccess
    {
        get => _scoreOnSuccess;
        set => _scoreOnSuccess = value;
    }

    /// <summary> Should typing be case-sensitive? </summary>
    public bool CaseSensitive
    {
        get => _caseSensitive;
        set => _caseSensitive = value;
    }

    /// <summary> Direct object reference to failure node (null = retry) </summary>
    public DialogNode FailureNode
    {
        get => _failureNode;
        set
        {
            _failureNode = value;
            // Keep legacy name in sync
            if (value != null)
            {
                _failureNodeName = value.NodeName;
            }
            else
            {
                _failureNodeName = "";
            }
        }
    }

    /// <summary> Node name to jump to on failure (LEGACY - use FailureNode instead) </summary>
    public string FailureNodeName
    {
        get
        {
            // Prefer the object reference name if it exists
            if (_failureNode != null && !string.IsNullOrEmpty(_failureNode.NodeName))
            {
                return _failureNode.NodeName;
            }
            // Fall back to legacy string field
            return _failureNodeName ?? string.Empty;
        }
        set => _failureNodeName = value;
    }

    #endregion

    #region Computed Properties

    /// <summary> True if this node has no parent nodes </summary>
    public bool IsRootNode => _parentNodes == null || _parentNodes.Count == 0;

    /// <summary> True if this node has available choices </summary>
    public bool HasChoices => _choices != null && _choices.Count > 0;

    /// <summary> True if this node auto-advances to another node </summary>
    public bool HasAutoAdvance => _childNode != null && _autoAdvanceDelay >= 0f;

    /// <summary> True if this node has RememberTheScript minigame configured </summary>
    public bool HasRememberScriptMinigame => _nodeType == DialogNodeType.RememberTheScript &&
                                               _isRememberScriptNode &&
                                               !string.IsNullOrEmpty(_targetPhrase);

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
    public DialogChoice AddChoice(String choiceText, DialogNode targetNode, String choiceId)
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

        // Validate RememberTheScript minigame settings
        if (_isRememberScriptNode)
        {
            // Must have a target phrase
            if (string.IsNullOrEmpty(_targetPhrase))
                return false;

            // Max mistakes must be positive
            if (_maxMistakes <= 0)
                return false;

            // Time limit must be non-negative
            if (_timeLimit < 0f)
                return false;
        }

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

    /// <summary>
    /// Configure this node as a RememberTheScript minigame node
    /// </summary>
    public void ConfigureRememberScript(
        string targetPhrase,
        int maxMistakes = 3,
        float timeLimit = 30f,
        float scoreOnSuccess = 20f,
        float scorePerMistake = -5f,
        bool caseSensitive = false,
        string failureNodeName = "")
    {
        _isRememberScriptNode = true;
        _targetPhrase = targetPhrase;
        _maxMistakes = maxMistakes;
        _timeLimit = timeLimit;
        _scoreOnSuccess = scoreOnSuccess;
        _scorePerMistake = scorePerMistake;
        _caseSensitive = caseSensitive;
        _failureNodeName = failureNodeName;
    }

    #endregion

    #region Node Type Conversion

    /// <summary>
    /// Convert this node to a different type while preserving connections and core data
    /// </summary>
    public void ConvertToType(DialogNodeType newType)
    {
        if (_nodeType == newType)
            return; // Already this type

        DialogNodeType oldType = _nodeType;
        _nodeType = newType;

        // Handle type-specific conversions
        switch (newType)
        {
            case DialogNodeType.StandardDialog:
                ConvertToStandardDialog(oldType);
                break;

            case DialogNodeType.RememberTheScript:
                ConvertToRememberTheScript(oldType);
                break;
        }

        Debug.Log($"[DialogNode] Converted node '{NodeName}' from {oldType} to {newType}");
    }

    private void ConvertToStandardDialog(DialogNodeType fromType)
    {
        // Clear minigame-specific data
        _isRememberScriptNode = false;
        _targetPhrase = "";
        _maxMistakes = 3;
        _timeLimit = 0f;
        _scorePerMistake = -5f;
        _scoreOnSuccess = 20f;
        _caseSensitive = false;
        _failureNodeName = "";

        // Sync NodeType
        _nodeType = DialogNodeType.StandardDialog;

        // Preserve: NodeName, CharacterName, DialogText, IsPlayerSpeaking, AutoAdvanceDelay
        // Preserve: ParentNodes, ChildNode, Choices, Events

        Debug.Log($"[DialogNode] Cleared {fromType} minigame data");
    }

    private void ConvertToRememberTheScript(DialogNodeType fromType)
    {
        // Enable RememberTheScript with default values
        _isRememberScriptNode = true;
        _nodeType = DialogNodeType.RememberTheScript;

        // Set defaults if not already configured
        if (string.IsNullOrEmpty(_targetPhrase))
        {
            _targetPhrase = "Enter phrase here";
        }

        if (_maxMistakes == 0)
            _maxMistakes = 3;

        if (_timeLimit == 0f)
            _timeLimit = 30f;

        if (_scoreOnSuccess == 0f)
            _scoreOnSuccess = 20f;

        if (_scorePerMistake == 0f)
            _scorePerMistake = -5f;

        // Preserve: NodeName, CharacterName, DialogText, IsPlayerSpeaking
        // Preserve: ParentNodes, ChildNode, Choices, Events
        // Note: AutoAdvanceDelay ignored during minigame

        Debug.Log($"[DialogNode] Initialized RememberTheScript minigame settings");
    }

    /// <summary>
    /// Get a human-readable description of what this node type does
    /// </summary>
    public static string GetNodeTypeDescription(DialogNodeType type)
    {
        switch (type)
        {
            case DialogNodeType.StandardDialog:
                return "Standard dialog with text, choices, and events";

            case DialogNodeType.RememberTheScript:
                return "Typing minigame - player must type phrase correctly";

            default:
                return "Unknown node type";
        }
    }

    /// <summary>
    /// Check if conversion will lose data (warn user before converting)
    /// </summary>
    public bool WillConversionLoseData(DialogNodeType newType)
    {
        if (_nodeType == newType)
            return false;

        // Converting from RememberTheScript to StandardDialog loses minigame data
        if (_nodeType == DialogNodeType.RememberTheScript && newType == DialogNodeType.StandardDialog)
        {
            return !string.IsNullOrEmpty(_targetPhrase) ||
                   _maxMistakes != 3 ||
                   _timeLimit != 30f ||
                   _scoreOnSuccess != 20f ||
                   _scorePerMistake != -5f ||
                   _caseSensitive ||
                   !string.IsNullOrEmpty(_failureNodeName);
        }

        // Add checks for future minigame types here

        return false;
    }

    #endregion

    #region Minigame Outcome Node Management

    /// <summary>
    /// Create default success and failure nodes for a minigame
    /// </summary>
    public void CreateMinigameOutcomeNodes()
    {
        if (_nodeType != DialogNodeType.RememberTheScript)
        {
            Debug.LogWarning("[DialogNode] CreateMinigameOutcomeNodes called on non-minigame node");
            return;
        }

        // Create success node if it doesn't exist
        if (_childNode == null)
        {
            var successNode = new DialogNode(
                "Director",
                "Excellent work! Your performance was flawless!",
                false,
                $"{_nodeId}_success"
            );
            SetChildNode(successNode);
            Debug.Log($"[DialogNode] Created success node: {successNode.NodeName}");
        }

        // Set failure node reference if not already set
        // NOTE: The actual failure node object needs to be created by the DialogTree or editor
        // because we don't have direct access to the tree's node list from here
        if (string.IsNullOrEmpty(_failureNodeName))
        {
            string failureNodeId = $"{_nodeId}_failure";
            _failureNodeName = failureNodeId;

            Debug.Log($"[DialogNode] Set failure node reference: {failureNodeId}");
            Debug.Log($"[DialogNode] Note: Failure node object must be created separately in the tree");
        }
    }

    /// <summary>
    /// Link to an existing node in the tree as the success node
    /// </summary>
    public void LinkToSuccessNode(DialogNode existingNode)
    {
        if (existingNode == null)
        {
            Debug.LogWarning("[DialogNode] Cannot link to null success node");
            return;
        }

        SetChildNode(existingNode);
        Debug.Log($"[DialogNode] Linked to existing success node: {existingNode.NodeName}");
    }

    /// <summary>
    /// Link to an existing node in the tree as the failure node (by name)
    /// </summary>
    public void LinkToFailureNode(string nodeName)
    {
        if (string.IsNullOrEmpty(nodeName))
        {
            Debug.LogWarning("[DialogNode] Cannot link to failure node with empty name");
            return;
        }

        _failureNodeName = nodeName;
        Debug.Log($"[DialogNode] Set failure node reference: {nodeName}");
    }

    /// <summary>
    /// Clear the failure node reference (allows retry)
    /// </summary>
    public void ClearFailureNode()
    {
        _failureNodeName = "";
        Debug.Log("[DialogNode] Cleared failure node - player can retry");
    }

    /// <summary>
    /// Check if this minigame node has outcome nodes configured
    /// </summary>
    public bool HasMinigameOutcomes()
    {
        return _childNode != null; // Success node exists
        // Failure node is optional (empty = retry)
    }

    #endregion
}
