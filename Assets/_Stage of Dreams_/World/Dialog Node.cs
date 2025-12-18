/* Dialog Node.cs
 * Defines a single node in a dialog tree. The node will be part of a larger DialogTree asset.
 * Now inherits from DataNode base class for shared functionality.
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
using UnityEngine;

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
/// Inherits from DataNode for shared tree structure and event functionality.
/// </summary>
[System.Serializable]
public class DialogNode : DataNode
{
    #region DialogNode-Specific Backing Fields

    [SerializeField, Tooltip("Node type - determines which editor and behavior to use")]
    private DialogNodeType _nodeType = DialogNodeType.StandardDialog;

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

    // Direct object reference for failure node (same as success node pattern)
    [SerializeReference, Tooltip("Node to jump to on failure (null = retry)")]
    private DataNode _failureNode = null;

    // LEGACY: Keep string reference for backwards compatibility
    [SerializeField, Tooltip("LEGACY: Node name to jump to on failure (use _failureNode instead)")]
    private string _failureNodeName = "";

    #endregion

    #region DialogNode-Specific Properties

    /// <summary> Type of this dialog node (determines editor and behavior) </summary>
    public DialogNodeType NodeType
    {
        get => _nodeType;
        set => _nodeType = value;
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
    public DataNode FailureNode
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

    /// <summary> True if this node has RememberTheScript minigame configured </summary>
    public bool HasRememberScriptMinigame => _nodeType == DialogNodeType.RememberTheScript &&
                                               _isRememberScriptNode &&
                                               !string.IsNullOrEmpty(_targetPhrase);

    #endregion

    #region Constructors

    /// <summary> Default constructor for serialization </summary>
    public DialogNode() : base()
    {
    }

    /// <summary> Constructor with parameters </summary>
    public DialogNode(string speaker, string text, bool playerSpeaking = false, string nodeId = null) 
        : base(speaker, text, playerSpeaking, nodeId)
    {
    }

    #endregion

    #region Helper Methods - DialogNode Specific

    /// <summary> Create and link a new node that this one will auto-advance to </summary>
    public DialogNode CreateChildNode(string speaker, string text, bool playerSpeaking = false, string nodeId = null)
    {
        var newNode = new DialogNode(speaker, text, playerSpeaking, nodeId);
        SetChildNode(newNode);
        return newNode;
    }

    #endregion

    #region Validation - Override from DataNode

    /// <summary> Validate this node's configuration </summary>
    public override bool IsValid()
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

    #endregion

    #region Minigame Configuration

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
