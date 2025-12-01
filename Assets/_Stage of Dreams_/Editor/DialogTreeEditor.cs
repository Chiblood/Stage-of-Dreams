/* DialogTreeEditor.cs
 * Enhanced custom editor for the DialogTree asset that works with SerializeReference fields.
 * Provides read-only tree overview with navigation to dedicated node editor windows.
 * 
 * How to use in Unity:
 * 1. Place this script in an "Editor" folder within your Assets directory.
 * 2. Select a DialogTree asset to see the enhanced custom inspector.
 * 3. Click "Edit" buttons to open nodes in dedicated editor windows.
 * 4. Use Quick Tree Builder for rapid prototyping.
 */

using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary> 
/// Enhanced custom editor for DialogTree with read-only overview and window-based editing.
/// Provides tree navigation and management tools without inline node editing.
/// </summary>
[CustomEditor(typeof(DialogTree))]
public class DialogTreeEditor : Editor
{
    private DialogTree dialogTree;
    private string newSpeakerName = "Speaker";
    private string newDialogText = "Enter dialog text here";
    private bool newIsPlayerSpeaking = false;
    private DialogNodeType newNodeType = DialogNodeType.StandardDialog;

    // Enable foldout sections for better organization
    private bool showQuickActions = true;
    private bool showQuickBuilder = false;
    private bool showAdvancedTools = false;
    private bool showAllNodes = false; // NEW: For all nodes list
    
    private void OnEnable()
    {
        dialogTree = (DialogTree)target;
    }
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        // Header
        EditorGUILayout.HelpBox("DialogTree Editor - Read-Only Overview\n" +
            "DialogTreeEditor.cs\n" +
            "Click 'Edit' buttons to open nodes in dedicated windows for detailed editing.", 
            MessageType.Info);
                
        // Draw properties manually
        DrawTreeInfoSection();
        DrawDialogFlowSection(); // Now read-only with edit buttons

        // Quick Actions Section
        showQuickActions = EditorGUILayout.Foldout(showQuickActions, "Quick Tree Actions", true);
        if (showQuickActions)
        {
            EditorGUILayout.BeginVertical("box");
            DrawQuickActionsSection();
            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.Space(5);

        // Quick Builder Section (for rapid prototyping)
        showQuickBuilder = EditorGUILayout.Foldout(showQuickBuilder, "Quick Tree Builder", true);
        if (showQuickBuilder)
        {
            EditorGUILayout.BeginVertical("box");
            DrawQuickBuilderSection();
            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.Space(5);

        // Advanced Tools Section
        showAdvancedTools = EditorGUILayout.Foldout(showAdvancedTools, "Advanced Tools", true);
        if (showAdvancedTools)
        {
            EditorGUILayout.BeginVertical("box");
            DrawAdvancedToolsSection();
            EditorGUILayout.EndVertical();
        }
        
        serializedObject.ApplyModifiedProperties();
        
        if (GUI.changed)
        {
            EditorUtility.SetDirty(dialogTree);
        }
    }
    
    #region Drawing the sections for the Inspector Editor
    
    private void DrawTreeInfoSection()
    {
        EditorGUILayout.LabelField("Tree Information", EditorStyles.boldLabel);
        
        SerializedProperty treeNameProp = serializedObject.FindProperty("treeName");
        SerializedProperty descriptionProp = serializedObject.FindProperty("description");
        SerializedProperty autoUpdateProp = serializedObject.FindProperty("autoUpdateNodeList");
        
        if (treeNameProp != null)
            EditorGUILayout.PropertyField(treeNameProp);
        
        if (descriptionProp != null)
            EditorGUILayout.PropertyField(descriptionProp);
        
        if (autoUpdateProp != null)
            EditorGUILayout.PropertyField(autoUpdateProp);
    }
    
    private void DrawDialogFlowSection()
    {
        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("Dialog Flow (Read-Only)", EditorStyles.boldLabel);
        
        // Starting Node Preview
        if (dialogTree.GetStartingNode() == null)
        {
            EditorGUILayout.HelpBox("No starting node. Use Quick Tree Builder to create one.", MessageType.Info);
            return;
        }
        
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        
        // Starting node info
        var startNode = dialogTree.GetStartingNode();
        EditorGUILayout.LabelField("Starting Node:", EditorStyles.miniBoldLabel);
        
        EditorGUI.indentLevel++;
        EditorGUILayout.LabelField($"Speaker: {startNode.CharacterName}");
        
        string preview = string.IsNullOrEmpty(startNode.DialogText) ? "<No text>" :
            (startNode.DialogText.Length > 50 
                ? startNode.DialogText.Substring(0, 50) + "..." 
                : startNode.DialogText);
        EditorGUILayout.LabelField($"Text: \"{preview}\"");
        EditorGUI.indentLevel--;
        
        EditorGUILayout.Space(5);
        
        // Edit Starting Node button
        GUI.backgroundColor = new Color(0.7f, 1f, 0.7f); // Light green
        if (GUILayout.Button("✏ Edit Starting Node", GUILayout.Height(30)))
        {
            SerializedProperty startingNodeProp = serializedObject.FindProperty("startingNode");
            if (startingNodeProp != null)
            {
                DialogNodeEditorWindow.OpenWindow(startingNodeProp, dialogTree);
            }
        }
        GUI.backgroundColor = Color.white;
        
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.Space(10);
        
        // All Nodes List (Foldout)
        DrawAllNodesList();
    }
    
    /// <summary>
    /// NEW: Draw read-only list of all nodes with edit buttons
    /// </summary>
    private void DrawAllNodesList()
    {
        var allNodes = dialogTree.GetAllNodes();
        
        if (allNodes.Count == 0) return;
        
        showAllNodes = EditorGUILayout.Foldout(showAllNodes, $"All Nodes ({allNodes.Count})", true, EditorStyles.foldoutHeader);
        
        if (showAllNodes)
        {
            EditorGUI.indentLevel++;
            
            EditorGUILayout.HelpBox("Read-only node list. Click 'Edit' to open in editor window.", MessageType.Info);
            
            for (int i = 0; i < allNodes.Count; i++)
            {
                DialogNode node = allNodes[i];
                
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                
                // Node type icon
                string typeIcon = "";
                switch (node.NodeType)
                {
                    case DialogNodeType.RememberTheScript:
                        typeIcon = "🎭";
                        break;
                    case DialogNodeType.StandardDialog:
                        typeIcon = "💬";
                        break;
                }
                
                // Node info
                string nodeId = !string.IsNullOrEmpty(node.NodeName) ? node.NodeName : "<No ID>";
                string speaker = !string.IsNullOrEmpty(node.CharacterName) ? node.CharacterName : "<No Speaker>";
                string preview = !string.IsNullOrEmpty(node.DialogText)
                    ? (node.DialogText.Length > 30 ? node.DialogText.Substring(0, 30) + "..." : node.DialogText)
                    : "<No text>";
                
                // Display format: "🎭 1. [nodeId] Speaker: "Preview text...""
                EditorGUILayout.LabelField($"{typeIcon} {i + 1}. [{nodeId}] {speaker}: \"{preview}\"");
                
                // Edit button
                GUI.backgroundColor = new Color(0.8f, 0.9f, 1f); // Light blue
                if (GUILayout.Button("Edit", EditorStyles.miniButton, GUILayout.Width(50)))
                {
                    // Find the property for this node in allNodes list
                    SerializedProperty allNodesProp = serializedObject.FindProperty("allNodes");
                    if (allNodesProp != null && i < allNodesProp.arraySize)
                    {
                        SerializedProperty nodeProp = allNodesProp.GetArrayElementAtIndex(i);
                        DialogNodeEditorWindow.OpenWindow(nodeProp, dialogTree);
                    }
                }
                GUI.backgroundColor = Color.white;
                
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUI.indentLevel--;
        }
    }
    
    private void DrawQuickActionsSection()
    {
        EditorGUILayout.LabelField("Tree Validation & Management", EditorStyles.miniBoldLabel);
        
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Validate Tree"))
        {
            dialogTree.ValidateTree();
        }
        
        if (GUILayout.Button("Print Structure"))
        {
            dialogTree.PrintTreeStructure();
        }
        
        if (GUILayout.Button("Refresh Nodes"))
        {
            dialogTree.RefreshNodeList();
            serializedObject.Update();
            EditorUtility.SetDirty(dialogTree);
        }
        
        EditorGUILayout.EndHorizontal();
        
        // Tree Statistics
        if (dialogTree.GetStartingNode() != null)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Tree Statistics", EditorStyles.miniBoldLabel);
            
            var allNodes = dialogTree.GetAllNodes();
            int choiceCount = 0;
            int endNodes = 0;
            
            foreach (var node in allNodes)
            {
                if (node.HasChoices) choiceCount += node.Choices.Count;
                if (!node.HasChoices && node.ChildNode == null) endNodes++;
            }
            
            EditorGUILayout.LabelField($"Total Nodes: {allNodes.Count}");
            EditorGUILayout.LabelField($"Total Choices: {choiceCount}");
            EditorGUILayout.LabelField($"End Nodes: {endNodes}");
            
            if (endNodes == 0)
            {
                EditorGUILayout.HelpBox("No end nodes found - conversation may loop indefinitely!", MessageType.Warning);
            }
        }
        else
        {
            EditorGUILayout.HelpBox("No dialog tree created yet. Use Quick Tree Builder to get started.", MessageType.Info);
        }
    }
    
    private void DrawQuickBuilderSection()
    {
        EditorGUILayout.LabelField("Rapid Node Creation", EditorStyles.miniBoldLabel);
        EditorGUILayout.HelpBox("Quick prototyping tools. For detailed editing, use the node editor windows.", MessageType.Info);
        
        // Node Type Selection
        newNodeType = (DialogNodeType)EditorGUILayout.EnumPopup("Node Type", newNodeType);
        
        // Input fields for quick creation
        newSpeakerName = EditorGUILayout.TextField("Speaker Name", newSpeakerName);
        newDialogText = EditorGUILayout.TextArea(newDialogText, GUILayout.Height(40));
        newIsPlayerSpeaking = EditorGUILayout.Toggle("Is Player Speaking", newIsPlayerSpeaking);
        
        EditorGUILayout.Space(5);
        
        // Quick creation buttons
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Create Starting Node"))
        {
            if (dialogTree.GetStartingNode() != null)
            {
                if (!EditorUtility.DisplayDialog("Replace Starting Node?", 
                    "A starting node already exists. Replace it?", "Replace", "Cancel"))
                    return;
            }
            
            Undo.RecordObject(dialogTree, "Create Starting Node");
            var newNode = dialogTree.CreateStartingNode(newSpeakerName, newDialogText, newIsPlayerSpeaking);
            
            // Set node type
            if (newNode != null)
            {
                newNode.NodeType = newNodeType;
                if (newNodeType == DialogNodeType.RememberTheScript)
                {
                    newNode.ConfigureRememberScript("Enter phrase here", 3, 30f, 20f, -5f, false, "");
                    
                    // Create success node
                    var successNode = new DialogNode(
                        "Director",
                        "Excellent work! Your performance was flawless!",
                        false,
                        $"{newNode.NodeName}_success"
                    );
                    newNode.SetChildNode(successNode);
                    
                    // Create failure node
                    string failureNodeId = $"{newNode.NodeName}_failure";
                    var failureNode = new DialogNode(
                        "Director",
                        "Let's try that again. Remember your lines!",
                        false,
                        failureNodeId
                    );
                    
                    // Set failure reference
                    newNode.LinkToFailureNode(failureNodeId);
                    
                    // Manually add both outcome nodes to tree since they're not connected through normal flow
                    dialogTree.RefreshNodeList(); // This should pick up the success node through child relationship
                    
                    // The failure node needs manual addition since it's only referenced by name
                    var allNodes = dialogTree.GetAllNodes();
                    if (!allNodes.Contains(failureNode))
                    {
                        // Add to the tree's internal list (this is a workaround)
                        // The tree should be able to manage this better
                        Debug.LogWarning("[DialogTreeEditor] Failure node created but may not appear in tree until refresh");
                    }
                    
                    EditorUtility.DisplayDialog("Minigame Node Created", 
                        "RememberTheScript node created with outcome nodes!\n\n" +
                        "✓ Success node: " + successNode.NodeName + "\n" +
                        "✗ Failure node: " + failureNodeId + "\n\n" +
                        "Open the RememberTheScriptNodeEditor to properly set up the failure node link.", "OK");
                }
            }
            
            serializedObject.Update();
            EditorUtility.SetDirty(dialogTree);
        }
        
        if (GUILayout.Button("Add Sequential Node"))
        {
            if (dialogTree.GetStartingNode() == null)
            {
                EditorUtility.DisplayDialog("Error", "Create a starting node first!", "OK");
            }
            else
            {
                Undo.RecordObject(dialogTree, "Add Sequential Node");
                var lastNode = FindLastNode();
                var newNode = dialogTree.AddSequentialNode(lastNode, newSpeakerName, newDialogText, newIsPlayerSpeaking);
                
                // Set node type
                if (newNode != null)
                {
                    newNode.NodeType = newNodeType;
                    if (newNodeType == DialogNodeType.RememberTheScript)
                    {
                        newNode.ConfigureRememberScript("Enter phrase here", 3, 30f, 20f, -5f, false, "");
                        
                        // Create success node
                        var successNode = new DialogNode(
                            "Director",
                            "Excellent work! Your performance was flawless!",
                            false,
                            $"{newNode.NodeName}_success"
                        );
                        newNode.SetChildNode(successNode);
                        
                        // Create failure node
                        string failureNodeId = $"{newNode.NodeName}_failure";
                        var failureNode = new DialogNode(
                            "Director",
                            "Let's try that again. Remember your lines!",
                            false,
                            failureNodeId
                        );
                        
                        // Set failure reference
                        newNode.LinkToFailureNode(failureNodeId);
                        
                        // Refresh node list
                        dialogTree.RefreshNodeList();
                        
                        EditorUtility.DisplayDialog("Minigame Node Created", 
                            "RememberTheScript node created with outcome nodes!\n\n" +
                            "✓ Success node: " + successNode.NodeName + "\n" +
                            "✗ Failure node: " + failureNodeId + "\n\n" +
                            "Open the RememberTheScriptNodeEditor to properly set up the failure node link.", "OK");
                    }
                }
                
                serializedObject.Update();
                EditorUtility.SetDirty(dialogTree);
            }
        }
        
        EditorGUILayout.EndHorizontal();
    }
    
    private void DrawAdvancedToolsSection()
    {
        EditorGUILayout.LabelField("Advanced Operations", EditorStyles.miniBoldLabel);
        
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        
        // Save As New Tree
        EditorGUILayout.LabelField("Duplication", EditorStyles.miniBoldLabel);
        EditorGUILayout.HelpBox("Create a copy of this dialog tree as a new asset.", MessageType.Info);
        
        GUI.backgroundColor = new Color(0.7f, 0.9f, 1f); // Light blue
        if (GUILayout.Button("Save As New Tree", GUILayout.Height(30)))
        {
            SaveAsNewTree();
        }
        GUI.backgroundColor = Color.white;
        
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.Space(5);
        
        // Dangerous operations
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Dangerous Operations", EditorStyles.miniBoldLabel);
        EditorGUILayout.HelpBox("⚠ These operations cannot be undone!", MessageType.Warning);
        
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Clear All Nodes", GUILayout.Height(30)))
        {
            if (EditorUtility.DisplayDialog("Clear Dialog Tree", 
                "Are you sure you want to clear all nodes? This cannot be undone.", "Clear", "Cancel"))
            {
                Undo.RecordObject(dialogTree, "Clear All Nodes");
                ClearAllNodes();
                serializedObject.Update();
                EditorUtility.SetDirty(dialogTree);
            }
        }
        GUI.backgroundColor = Color.white;
        
        EditorGUILayout.EndVertical();
    }

    #endregion

    #region Helper Methods
    
    /// <summary>
    /// NEW: Save the current tree as a new asset
    /// </summary>
    private void SaveAsNewTree()
    {
        string currentPath = AssetDatabase.GetAssetPath(dialogTree);
        string directory = Path.GetDirectoryName(currentPath);
        string fileName = Path.GetFileNameWithoutExtension(currentPath);
        
        // Prompt for new name
        string newName = EditorUtility.SaveFilePanel(
            "Save Dialog Tree As",
            directory,
            fileName + "_Copy",
            "asset"
        );
        
        if (string.IsNullOrEmpty(newName)) return; // User cancelled
        
        // Make path relative to project
        if (newName.StartsWith(Application.dataPath))
        {
            newName = "Assets" + newName.Substring(Application.dataPath.Length);
        }
        
        // Create copy
        if (AssetDatabase.CopyAsset(currentPath, newName))
        {
            AssetDatabase.Refresh();
            
            // Load and select the new asset
            DialogTree newTree = AssetDatabase.LoadAssetAtPath<DialogTree>(newName);
            if (newTree != null)
            {
                // Update the name in the new tree
                newTree.treeName = Path.GetFileNameWithoutExtension(newName);
                EditorUtility.SetDirty(newTree);
                AssetDatabase.SaveAssets();
                
                // Select the new tree
                Selection.activeObject = newTree;
                EditorGUIUtility.PingObject(newTree);
                
                Debug.Log($"DialogTree copied to: {newName}");
            }
        }
        else
        {
            EditorUtility.DisplayDialog("Error", "Failed to copy dialog tree asset.", "OK");
        }
    }
    
    private DialogNode FindLastNode()
    {
        var allNodes = dialogTree.GetAllNodes();
        foreach (var node in allNodes)
        {
            if (!node.HasChoices && node.ChildNode == null)
            {
                return node;
            }
        }
        return dialogTree.GetStartingNode();
    }
    
    private void ClearAllNodes()
    {
        Undo.RecordObject(dialogTree, "Clear All Nodes");
        dialogTree.startingNode = null;
        dialogTree.RefreshNodeList();
        EditorUtility.SetDirty(dialogTree);
        
        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();
    }
    
    #endregion
}
