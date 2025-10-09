/* NPC Content.cs
 * The virtual/abstract class for NPC Content to be inherited by whatever NPC. 
 * Includes the dialog tree with and a list of action choices to be fed to the DialogManager class.
 * 
 * How to use in Unity:
 * 1. Create a new script that inherits from NPCContent.
 * 2. Define the dialog trees and custom actions within the new script.
 * 3. Attach the script to an NPC GameObject.
 * 
 */

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Base class for NPC content that contains dialog trees and handles custom actions.
/// Inherit from this class to create specific NPCs with their own dialog and behaviors.
/// </summary>
public class NPCContent : MonoBehaviour
{
    [Header("NPC Info")]
    public string npcName;
    [TextArea(2, 3)]
    public string npcDescription = ""; // Optional description for editor
    
    [Header("Dialog Content")]
    public DialogTree mainDialogTree;
    public DialogTree[] additionalDialogTrees; // For different conversation contexts
    
    [Header("Dialog Settings")]
    [SerializeField] private bool validateOnStart = true;
    [SerializeField] private bool enableDebugLogs = false;
    
    // Dialog state tracking
    private bool isInDialog = false;
    private int dialogCount = 0;
    private DialogTree lastUsedTree = null;

    // Events for external systems to hook into
    public System.Action<NPCContent> OnDialogStartedEvent;
    public System.Action<NPCContent> OnDialogEndedEvent;
    public System.Action<string> OnCustomActionEvent;

    private void Start()
    {
        if (validateOnStart)
        {
            ValidateDialogContent();
        }
    }
    
    /// <summary>
    /// Validate all dialog content attached to this NPC
    /// </summary>
    public bool ValidateDialogContent()
    {
        bool isValid = true;
        List<string> errors = new List<string>();
        
        // Check if NPC has a name
        if (string.IsNullOrEmpty(npcName))
        {
            errors.Add("NPC name is not set");
            isValid = false;
        }
        
        // Check main dialog tree
        if (mainDialogTree == null)
        {
            errors.Add("Main dialog tree is not assigned");
            isValid = false;
        }
        else if (!mainDialogTree.IsValid())
        {
            errors.Add($"Main dialog tree '{mainDialogTree.treeName}' is invalid (no starting node)");
            isValid = false;
        }
        
        // Check additional dialog trees
        if (additionalDialogTrees != null)
        {
            for (int i = 0; i < additionalDialogTrees.Length; i++)
            {
                var tree = additionalDialogTrees[i];
                if (tree == null)
                {
                    errors.Add($"Additional dialog tree at index {i} is null");
                    isValid = false;
                }
                else if (!tree.IsValid())
                {
                    errors.Add($"Additional dialog tree '{tree.treeName}' at index {i} is invalid");
                    isValid = false;
                }
                else if (string.IsNullOrEmpty(tree.treeName))
                {
                    errors.Add($"Additional dialog tree at index {i} has no name");
                    isValid = false;
                }
            }
            
            // Check for duplicate tree names
            HashSet<string> treeNames = new HashSet<string>();
            if (mainDialogTree != null && !string.IsNullOrEmpty(mainDialogTree.treeName))
            {
                treeNames.Add(mainDialogTree.treeName);
            }
            
            foreach (var tree in additionalDialogTrees)
            {
                if (tree != null && !string.IsNullOrEmpty(tree.treeName))
                {
                    if (treeNames.Contains(tree.treeName))
                    {
                        errors.Add($"Duplicate tree name found: '{tree.treeName}'");
                        isValid = false;
                    }
                    else
                    {
                        treeNames.Add(tree.treeName);
                    }
                }
            }
        }
        
        // Log results
        if (isValid)
        {
            LogDebug($"NPC '{npcName}' dialog content validation passed");
        }
        else
        {
            LogError($"NPC '{npcName}' dialog content validation failed:");
            foreach (var error in errors)
            {
                LogError($"  - {error}");
            }
        }
        
        return isValid;
    }
    
    /// <summary>
    /// Get the main dialog tree for this NPC
    /// </summary>
    public DialogTree GetMainDialogTree()
    {
        return mainDialogTree;
    }
    
    /// <summary>
    /// Get a specific dialog tree that is attached to additionalDialogTrees array by name
    /// </summary>
    public DialogTree GetDialogTree(string treeName)
    {
        if (string.IsNullOrEmpty(treeName))
        {
            LogWarning($"Attempted to get dialog tree with null/empty name from NPC '{npcName}'");
            return null;
        }
        
        // Check main tree first
        if (mainDialogTree != null && mainDialogTree.treeName == treeName)
        {
            LogDebug($"Found dialog tree '{treeName}' in main tree for NPC '{npcName}'");
            return mainDialogTree;
        }
        
        // Check additional trees
        if (additionalDialogTrees != null)
        {
            foreach (var tree in additionalDialogTrees)
            {
                if (tree != null && tree.treeName == treeName)
                {
                    LogDebug($"Found dialog tree '{treeName}' in additional trees for NPC '{npcName}'");
                    return tree;
                }
            }
        }
        
        LogWarning($"Dialog tree '{treeName}' not found for NPC '{npcName}'");
        return null;
    }
    
    /// <summary>
    /// Get all available dialog tree names
    /// </summary>
    public string[] GetAvailableTreeNames()
    {
        List<string> treeNames = new List<string>();
        
        if (mainDialogTree != null && !string.IsNullOrEmpty(mainDialogTree.treeName))
        {
            treeNames.Add(mainDialogTree.treeName);
        }
        
        if (additionalDialogTrees != null)
        {
            foreach (var tree in additionalDialogTrees)
            {
                if (tree != null && !string.IsNullOrEmpty(tree.treeName))
                {
                    treeNames.Add(tree.treeName);
                }
            }
        }
        
        return treeNames.ToArray();
    }
    
    /// <summary>
    /// Get the number of available dialog trees
    /// </summary>
    public int GetDialogTreeCount()
    {
        int count = 0;
        
        if (mainDialogTree != null) count++;
        
        if (additionalDialogTrees != null)
        {
            foreach (var tree in additionalDialogTrees)
            {
                if (tree != null) count++;
            }
        }
        
        return count;
    }
    
    /// <summary>
    /// Check if this NPC has any valid dialog content
    /// </summary>
    public bool HasValidDialogContent()
    {
        return (mainDialogTree != null && mainDialogTree.IsValid()) ||
               (additionalDialogTrees != null && System.Array.Exists(additionalDialogTrees, tree => tree != null && tree.IsValid()));
    }
    
    /// <summary>
    /// Get the best dialog tree to use (main tree if valid, otherwise first valid additional tree)
    /// </summary>
    public DialogTree GetBestAvailableTree()
    {
        // Prefer main tree
        if (mainDialogTree != null && mainDialogTree.IsValid())
        {
            return mainDialogTree;
        }
        
        // Fall back to first valid additional tree
        if (additionalDialogTrees != null)
        {
            foreach (var tree in additionalDialogTrees)
            {
                if (tree != null && tree.IsValid())
                {
                    return tree;
                }
            }
        }
        
        return null;
    }
    
    /// <summary>
    /// Override this method to handle custom actions triggered by dialog choices
    /// </summary>
    public virtual void HandleCustomAction(string actionId)
    {
        if (string.IsNullOrEmpty(actionId))
        {
            LogWarning($"NPC '{npcName}' received null/empty custom action");
            return;
        }
        
        LogDebug($"NPC '{npcName}' received custom action: {actionId}");
        
        // Fire event for external systems
        OnCustomActionEvent?.Invoke(actionId);
        
        // Override in derived classes to implement specific NPC behaviors
    }
    
    /// <summary>
    /// Called when dialog with this NPC starts
    /// </summary>
    public virtual void OnDialogStarted()
    {
        isInDialog = true;
        dialogCount++;
        
        LogDebug($"Dialog started with NPC '{npcName}' (dialog #{dialogCount})");
        
        // Fire event for external systems
        OnDialogStartedEvent?.Invoke(this);
        
        // Override in derived classes for custom behavior
    }
    
    /// <summary>
    /// Called when dialog with this NPC ends
    /// </summary>
    public virtual void OnDialogEnded()
    {
        isInDialog = false;
        lastUsedTree = null; // Could track which tree was used if needed
        
        LogDebug($"Dialog ended with NPC '{npcName}'");
        
        // Fire event for external systems
        OnDialogEndedEvent?.Invoke(this);
        
        // Override in derived classes for custom behavior
    }
    
    /// <summary>
    /// Check if this NPC is currently in dialog
    /// </summary>
    public bool IsInDialog()
    {
        return isInDialog;
    }
    
    /// <summary>
    /// Get the total number of dialogs this NPC has had
    /// </summary>
    public int GetDialogCount()
    {
        return dialogCount;
    }
    
    /// <summary>
    /// Reset dialog state (useful for testing or game state resets)
    /// </summary>
    public void ResetDialogState()
    {
        isInDialog = false;
        dialogCount = 0;
        lastUsedTree = null;
        LogDebug($"Dialog state reset for NPC '{npcName}'");
    }
    
    /// <summary>
    /// Get debug information about this NPC's dialog setup
    /// </summary>
    public string GetDebugInfo()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine($"NPC: {npcName}");
        sb.AppendLine($"Description: {npcDescription}");
        sb.AppendLine($"In Dialog: {isInDialog}");
        sb.AppendLine($"Dialog Count: {dialogCount}");
        sb.AppendLine($"Main Tree: {(mainDialogTree != null ? mainDialogTree.treeName : "None")}");
        sb.AppendLine($"Additional Trees: {(additionalDialogTrees?.Length ?? 0)}");
        
        if (additionalDialogTrees != null && additionalDialogTrees.Length > 0)
        {
            for (int i = 0; i < additionalDialogTrees.Length; i++)
            {
                var tree = additionalDialogTrees[i];
                sb.AppendLine($"  [{i}]: {(tree != null ? tree.treeName : "Null")}");
            }
        }
        
        return sb.ToString();
    }
    
    #region Logging Methods
    private void LogDebug(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"[NPCContent] {message}");
        }
    }
    
    private void LogWarning(string message)
    {
        Debug.LogWarning($"[NPCContent] {message}");
    }
    
    private void LogError(string message)
    {
        Debug.LogError($"[NPCContent] {message}");
    }
    #endregion
    
    #region Editor Support
    private void OnValidate()
    {
        // Ensure NPC name is set from GameObject name if empty
        if (string.IsNullOrEmpty(npcName) && !string.IsNullOrEmpty(gameObject.name))
        {
            npcName = gameObject.name;
        }
        
        // Validate during edit time in play mode
        if (Application.isPlaying && validateOnStart)
        {
            ValidateDialogContent();
        }
    }
    
    [ContextMenu("Validate Dialog Content")]
    private void EditorValidateDialogContent()
    {
        ValidateDialogContent();
    }
    
    [ContextMenu("Print Debug Info")]
    private void EditorPrintDebugInfo()
    {
        Debug.Log(GetDebugInfo());
    }
    #endregion
}