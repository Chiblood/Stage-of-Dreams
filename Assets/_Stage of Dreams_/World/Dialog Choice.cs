/* Dialog Choice.cs
 * Defines a single choice as a part of a dialog node. 
 * Choices link to other nodes (DialogNode or any DataNode subclass).
 * Includes the choice text, target node, and any associated events to fire once the player selects the choice.
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary> Represents a choice the player can make in dialog </summary>
[System.Serializable]
public class DialogChoice
{
    // Choice Display - no headers, PropertyDrawer handles organization
    #region Backing Fields - SerializeField for Unity serialization

    [SerializeField] private string _choiceText;
    [SerializeField] private string _choiceId;
    [SerializeField] private List<bool> _conditions; // Conditions to show this choice (not implemented yet)
    [SerializeField] private DataNode _parentNode; // The node this choice belongs to
    [SerializeReference] private DataNode _targetNode; // Where this choice leads

    [SerializeField, Tooltip("Name of target node (for convergent paths) - will be resolved at runtime")]
    private string _targetNodeName; // For referencing nodes by name

    // Choice Events - SerializeReference for polymorphic event handling
    [SerializeReference] private List<DialogEvent> _choiceEvents;

    // Legacy Unity Events for backwards compatibility (optional)
    [SerializeField] private UnityEvent _onChoiceSelected; // Single UnityEvent for simple cases

    #endregion

    #region Properties with Get/Set Methods

    /// <summary> The display text for this choice </summary>
    public string ChoiceText
    {
        get => _choiceText ?? string.Empty;
        set => _choiceText = value;
    }

    /// <summary> Unique identifier for this choice </summary>
    public string ChoiceId
    {
        get => _choiceId ?? string.Empty;
        set => _choiceId = value;
    }

    /// <summary> The node this choice belongs to </summary>
    public DataNode ParentNode
    {
        get => _parentNode;
        set => _parentNode = value;
    }

    /// <summary> The target node this choice leads to </summary>
    public DataNode TargetNode
    {
        get => _targetNode;
        set => _targetNode = value;
    }

    /// <summary> Name of target node for convergent paths (resolved at runtime) </summary>
    public string TargetNodeName
    {
        get => _targetNodeName ?? string.Empty;
        set => _targetNodeName = value;
    }

    /// <summary> Events triggered when this choice is selected (DialogEvent system) </summary>
    public List<DialogEvent> ChoiceEvents
    {
        get
        {
            if (_choiceEvents == null)
                _choiceEvents = new List<DialogEvent>();
            return _choiceEvents;
        }
        set => _choiceEvents = value;
    }

    /// <summary> Legacy Unity Events for choice selection (for backwards compatibility) </summary>
    public UnityEvent OnChoiceSelected
    {
        get
        {
            if (_onChoiceSelected == null)
                _onChoiceSelected = new UnityEvent();
            return _onChoiceSelected;
        }
        set => _onChoiceSelected = value;
    }

    #endregion

    #region Computed Properties

    /// <summary> Check if this choice references a node by name </summary>
    public bool HasNamedTarget => !string.IsNullOrEmpty(_targetNodeName);

    /// <summary> Check if this choice has a valid target (either direct reference or named) </summary>
    public bool HasValidTarget => _targetNode != null || HasNamedTarget;

    /// <summary> Check if named target has been resolved to actual node </summary>
    public bool IsTargetResolved()
    {
        // If has named target, check if it's been resolved to actual node
        if (HasNamedTarget)
            return _targetNode != null;

        // Otherwise just check if direct target exists
        return _targetNode != null;
    }

    /// <summary> Check if this choice has events to execute </summary>
    public bool HasEvents => (_choiceEvents != null && _choiceEvents.Count > 0) ||
                            (_onChoiceSelected != null && _onChoiceSelected.GetPersistentEventCount() > 0);

    #endregion

    #region Constructors

    /// <summary> Default constructor for serialization </summary>
    public DialogChoice()
    {
        Initialize();
    }

    /// <summary> Constructor with parameters - accepts DataNode for compatibility </summary>
    public DialogChoice(string choiceText, DataNode parentNode = null, DataNode targetNode = null, string choiceId = null)
    {
        _choiceText = choiceText;
        _choiceId = choiceId;
        _parentNode = parentNode;
        _targetNode = targetNode;
        Initialize();
    }

    /// <summary> Initialize all collections </summary>
    private void Initialize()
    {
        _choiceEvents = new List<DialogEvent>();
        _onChoiceSelected = new UnityEvent();
    }

    #endregion

    #region Choice Event Management

    /// <summary> Add a choice event to this choice </summary>
    public void AddChoiceEvent(DialogEvent dialogEvent)
    {
        if (dialogEvent != null)
        {
            ChoiceEvents.Add(dialogEvent);
        }
    }

    /// <summary> Remove a choice event by index </summary>
    public void RemoveChoiceEvent(int index)
    {
        if (_choiceEvents != null && index >= 0 && index < _choiceEvents.Count)
        {
            _choiceEvents.RemoveAt(index);
        }
    }

    /// <summary> Execute all choice events </summary>
    public void ExecuteChoiceEvents()
    {
        // Execute new DialogEvent system
        if (_choiceEvents != null)
        {
            foreach (var dialogEvent in _choiceEvents)
            {
                dialogEvent?.Execute();
            }
        }

        // Execute legacy UnityEvents for backwards compatibility
        _onChoiceSelected?.Invoke();
    }

    #endregion

    #region Target Management

    /// <summary> Create and set a target node for this choice (creates DialogNode by default) </summary>
    public DataNode CreateTargetNode(string speaker, string text, bool playerSpeaking = false, string nodeId = null)
    {
        _targetNode = new DialogNode(speaker, text, playerSpeaking, nodeId);
        if (_targetNode != null && _parentNode != null)
        {
            _targetNode.AddParentNode(_parentNode);
        }
        return _targetNode;
    }

    /// <summary> Set the target node for this choice </summary>
    public void SetTarget(DataNode target)
    {
        // Remove old parent reference if changing target
        if (_targetNode != null && _parentNode != null)
        {
            _targetNode.RemoveParentNode(_parentNode);
        }

        _targetNode = target;

        // Set up parent relationship
        if (target != null && _parentNode != null)
        {
            target.AddParentNode(_parentNode);
        }
    }

    /// <summary> Set target by node name (for convergent paths) </summary>
    public void SetTargetByName(string nodeName)
    {
        _targetNodeName = nodeName;
        // Clear direct reference when using named reference
        _targetNode = null;
    }

    /// <summary> Resolve named target to actual node reference </summary>
    public bool ResolveNamedTarget(DialogTree tree)
    {
        if (!HasNamedTarget) return _targetNode != null;

        var namedNode = tree.FindNodeByName(_targetNodeName);
        if (namedNode != null)
        {
            SetTarget(namedNode);
            return true;
        }

        Debug.LogWarning($"Could not resolve target node name '{_targetNodeName}' in tree '{tree.treeName}'");
        return false;
    }

    #endregion

    #region Validation and Utility

    /// <summary> Validate this choice's configuration </summary>
    public bool IsValid()
    {
        // Must have choice text
        if (string.IsNullOrWhiteSpace(_choiceText))
            return false;

        // Must have either a target node or named target
        if (!HasValidTarget)
            return false;

        // Validate all choice events
        if (_choiceEvents != null)
        {
            foreach (var evt in _choiceEvents)
            {
                if (evt != null && !evt.IsValid())
                    return false;
            }
        }

        return true;
    }

    /// <summary> Get display name for this choice </summary>
    public string GetDisplayName()
    {
        if (!string.IsNullOrEmpty(_choiceText))
            return _choiceText.Length > 30 ? _choiceText.Substring(0, 30) + "..." : _choiceText;

        return "Empty Choice";
    }

    /// <summary> Get target information for debugging </summary>
    public string GetTargetInfo()
    {
        if (_targetNode != null)
            return $"Direct -> {_targetNode.GetDisplayName()}";

        if (HasNamedTarget)
            return $"Named -> [{_targetNodeName}]";

        return "No Target";
    }

    #endregion
}
