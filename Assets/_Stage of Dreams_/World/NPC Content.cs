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

/// <summary>
/// Base class for NPC content that contains dialog trees and handles custom actions.
/// Inherit from this class to create specific NPCs with their own dialog and behaviors.
/// </summary>
public class NPCContent : MonoBehaviour
{
    [Header("NPC Info")]
    public string npcName;
    
    [Header("Dialog Content")]
    public DialogTree mainDialogTree;
    public DialogTree[] additionalDialogTrees; // For different conversation contexts
    
    /// <summary>
    /// Get the main dialog tree for this NPC
    /// </summary>
    public DialogTree GetMainDialogTree()
    {
        return mainDialogTree;
    }
    
    /// <summary>
    /// Get a specific dialog tree by name
    /// </summary>
    public DialogTree GetDialogTree(string treeName)
    {
        if (mainDialogTree != null && mainDialogTree.treeName == treeName)
            return mainDialogTree;
            
        if (additionalDialogTrees != null)
        {
            foreach (var tree in additionalDialogTrees)
            {
                if (tree != null && tree.treeName == treeName)
                    return tree;
            }
        }
        
        return null;
    }
    
    /// <summary>
    /// Override this method to handle custom actions triggered by dialog choices
    /// </summary>
    public virtual void HandleCustomAction(string actionId)
    {
        Debug.Log($"{npcName} received custom action: {actionId}");
        // Override in derived classes to implement specific NPC behaviors
    }
    
    /// <summary>
    /// Called when dialog with this NPC starts
    /// </summary>
    public virtual void OnDialogStarted()
    {
        // Override in derived classes for custom behavior
    }
    
    /// <summary>
    /// Called when dialog with this NPC ends
    /// </summary>
    public virtual void OnDialogEnded()
    {
        // Override in derived classes for custom behavior
    }
}