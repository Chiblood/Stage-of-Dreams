/* DialogTree.cs
 * The dialog tree structure for managing NPC dialogues and player choices.
 * Supports branching conversations based on player selections.
 * Includes a list of action choices to be fed to the DialogManager class. Within this list should be the capability to call functions in other scripts.
 * 
 * How to use in Unity:
 * 1. Create a new script that inherits from DialogTree.
 * 2. Define the dialog nodes and choices within the new script.
 * 3. Attach the script to an NPC GameObject.
 * 
 */

using UnityEngine;

/// <summary>
/// Contains a complete dialog tree structure starting from one node.
/// Simple container that holds the conversation flow for an NPC.
/// </summary>
[CreateAssetMenu(fileName = "New Dialog Tree", menuName = "Dialog System/Dialog Tree")]
public class DialogTree : ScriptableObject
{
    [Header("Tree Info")]
    public string treeName;
    [TextArea(2, 4)]
    public string description;
    
    [Header("Dialog Flow")]
    public DialogNode startingNode;
    
    /// <summary>
    /// Get the first node to start the conversation
    /// </summary>
    public DialogNode GetStartingNode()
    {
        return startingNode;
    }
    
    /// <summary>
    /// Check if this tree has a valid starting point
    /// </summary>
    public bool IsValid()
    {
        return startingNode != null;
    }
}