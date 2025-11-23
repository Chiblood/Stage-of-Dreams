/* DialogTreeUtilities.cs
 * Utility functions for maintaining and fixing Dialog Trees in the project.
 * Provides menu items for batch operations on Dialog Trees.
 */

using UnityEngine;
using UnityEditor;
using System.Linq;

/// <summary>
/// Editor utilities for managing Dialog Trees across the project
/// </summary>
public static class DialogTreeUtilities
{
    [MenuItem("Tools/Dialog System/Fix All Dialog Trees")]
    public static void FixAllDialogTrees()
    {
        // Find all DialogTree assets in the project
        string[] guids = AssetDatabase.FindAssets("t:DialogTree");
        int fixedCount = 0;
        int totalCount = guids.Length;
        
        Debug.Log($"Found {totalCount} Dialog Tree assets. Checking for issues...");
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            DialogTree tree = AssetDatabase.LoadAssetAtPath<DialogTree>(path);
            
            if (tree != null)
            {
                bool wasFixed = FixDialogTree(tree, path);
                if (wasFixed)
                {
                    fixedCount++;
                    EditorUtility.SetDirty(tree);
                }
            }
        }
        
        if (fixedCount > 0)
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"[FIXED] Fixed {fixedCount} out of {totalCount} Dialog Trees");
        }
        else
        {
            Debug.Log($"[OK] All {totalCount} Dialog Trees are valid!");
        }
    }
    
    [MenuItem("Tools/Dialog System/Validate All Dialog Trees")]
    public static void ValidateAllDialogTrees()
    {
        string[] guids = AssetDatabase.FindAssets("t:DialogTree");
        int validCount = 0;
        int invalidCount = 0;
        int totalCount = guids.Length;
        
        Debug.Log($"=== Validating {totalCount} Dialog Trees ===");
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            DialogTree tree = AssetDatabase.LoadAssetAtPath<DialogTree>(path);
            
            if (tree != null)
            {
                if (tree.IsValid())
                {
                    validCount++;
                    Debug.Log($"[OK] VALID: {tree.treeName} ({path})");
                }
                else
                {
                    invalidCount++;
                    Debug.LogWarning($"[!] INVALID: {tree.treeName} ({path}) - Missing or invalid starting node");
                }
            }
        }
        
        Debug.Log($"=== Validation Complete ===");
        Debug.Log($"[OK] Valid: {validCount}");
        Debug.Log($"[!] Invalid: {invalidCount}");
        Debug.Log($"[#] Total: {totalCount}");
    }
    
    [MenuItem("Tools/Dialog System/List All Dialog Trees")]
    public static void ListAllDialogTrees()
    {
        string[] guids = AssetDatabase.FindAssets("t:DialogTree");
        
        Debug.Log($"=== Found {guids.Length} Dialog Trees ===");
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            DialogTree tree = AssetDatabase.LoadAssetAtPath<DialogTree>(path);
            
            if (tree != null)
            {
                string status = tree.IsValid() ? "[OK] Valid" : "[!] Invalid";
                int nodeCount = tree.GetAllNodes().Count;
                Debug.Log($"{status} | {tree.treeName} | Nodes: {nodeCount} | Path: {path}");
            }
        }
    }
    
    private static bool FixDialogTree(DialogTree tree, string path)
    {
        bool wasFixed = false;
        
        // Check if tree has no starting node
        if (tree.startingNode == null)
        {
            Debug.LogWarning($"[WARNING] Dialog Tree '{tree.treeName}' ({path}) has no starting node. Creating default starting node...");
            
            // Create a default starting node
            tree.CreateStartingNode(
                "Speaker", 
                $"[Default dialog for {tree.treeName}]\nPlease edit this node to add your actual dialog content.",
                false,
                "StartNode"
            );
            
            wasFixed = true;
            Debug.Log($"[FIXED] Created default starting node for '{tree.treeName}'");
        }
        else
        {
            // Refresh node list to ensure it's up to date
            tree.RefreshNodeList();
        }
        
        return wasFixed;
    }
    
    [MenuItem("Tools/Dialog System/Create Example Dialog Tree")]
    public static void CreateExampleDialogTree()
    {
        // Create a new Dialog Tree asset
        DialogTree newTree = ScriptableObject.CreateInstance<DialogTree>();
        newTree.treeName = "Example Dialog Tree";
        newTree.description = "An example dialog tree created by the utility";
        
        // Create a simple dialog structure
        DialogNode start = newTree.CreateStartingNode(
            "Guide", 
            "Welcome! This is an example dialog tree. Would you like to learn more?",
            false,
            "Start"
        );
        
        // Add choice branches
        DialogNode yesNode = newTree.AddChoiceNode(
            start, 
            "Yes, tell me more!", 
            "Guide", 
            "Great! Dialog trees let you create branching conversations with choices and events.",
            false,
            null,
            "YesBranch"
        );
        
        DialogNode noNode = newTree.AddChoiceNode(
            start, 
            "No, I understand", 
            "Guide", 
            "Alright! Feel free to edit this tree or create your own.",
            false,
            null,
            "NoBranch"
        );
        
        // Add sequential nodes
        newTree.AddSequentialNode(
            yesNode,
            "Guide",
            "You can add choices, events, and create complex conversations. Good luck!",
            false,
            0f,
            "EndYes"
        );
        
        // Save the asset
        string path = "Assets/_Stage of Dreams_/World/Dream 1/Example Dialog Tree.asset";
        AssetDatabase.CreateAsset(newTree, path);
        AssetDatabase.SaveAssets();
        
        // Select the new asset in the project
        EditorGUIUtility.PingObject(newTree);
        Selection.activeObject = newTree;
        
        Debug.Log($"[CREATED] Created example Dialog Tree at: {path}");
    }
}
