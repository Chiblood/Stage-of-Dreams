/* DialogTreeEditor.cs
 * Enhanced custom editor for the DialogTree asset that works with SerializeReference fields.
 * Combines custom inspector functionality with property drawer support for optimal editing experience.
 * 
 * How to use in Unity:
 * 1. Place this script in an "Editor" folder within your Assets directory.
 * 2. Select a DialogTree asset to see the enhanced custom inspector.
 * 3. Use the property drawers for detailed node/choice editing and quick actions for tree management.
 * 4. Ensure your DialogTree and DialogNode classes use [SerializeReference] attributes.
 */

using UnityEngine;
using UnityEditor;

/// <summary> Enhanced custom editor for DialogTree that works with SerializeReference fields and property drawers.
/// Provides both detailed editing through property drawers and quick tree management tools.
/// </summary>
[CustomEditor(typeof(DialogTree))] // Specify the data type this editor is for
public class DialogTreeEditor : Editor
{
    private DialogTree dialogTree;
    private string newSpeakerName = "Speaker";
    private string newDialogText = "Enter dialog text here";
    private bool newIsPlayerSpeaking = false;
    private string newChoiceText = "Continue";
    private string newCustomActionId = "";

    // Enable foldout sections for better organization
    private bool showQuickActions = true;
    private bool showQuickBuilder = false;
    private bool showAdvancedTools = false;
    
    private void OnEnable()
    {
        dialogTree = (DialogTree)target;
    }
    // Override the default OnInspectorGUI, is immediately called when selecting a 
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        // Header with SerializeReference confirmation
        EditorGUILayout.HelpBox("Enhanced DialogTree Editor with SerializeReference support!\n" +
            "DialogTreeEditor.cs\n" +
            "Uses the property drawers from DialogNodePropertyDrawer.cs and DialogChoicePropertyDrawer.cs for detailed editing, or the quick tools for rapid tree building.", 
            MessageType.Info);
                
        // Draw properties manually to avoid conflicts
        DrawTreeInfoSection();
        DrawDialogFlowSection();

        // Quick Actions Section
        showQuickActions = EditorGUILayout.Foldout(showQuickActions, "Quick Actions", true);
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
        EditorGUILayout.LabelField("Tree Information from TreeEditor.cs", EditorStyles.boldLabel);
        
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
        EditorGUILayout.LabelField("Dialog Flow", EditorStyles.boldLabel); // Section header

        // Draw starting node with property drawer support from DialogNodePropertyDrawer.cs
        SerializedProperty startingNodeProp = serializedObject.FindProperty("startingNode");

        // Check if starting node exists and allow creation if not
        if (startingNodeProp != null) 
        {
            if (startingNodeProp.managedReferenceValue == null)
            {
                EditorGUILayout.HelpBox("No starting node found. Use 'Create Starting Node' in Quick Tree Builder below to create one.", MessageType.Info);
                
                // Simple create button right here for convenience
                if (GUILayout.Button("Create Starting Node"))
                {
                    Undo.RecordObject(dialogTree, "Create Starting Node");
                    dialogTree.CreateStartingNode("Speaker", "Enter dialog text here", false);
                    serializedObject.Update();

                    // Mark as dirty to ensure changes are saved, especially for new assets
                    EditorUtility.SetDirty(dialogTree);
                    // Force Unity to recognize the change
                    serializedObject.ApplyModifiedProperties();
                }
            }
            else
            {
                EditorGUILayout.PropertyField(startingNodeProp, new GUIContent("Starting Node"), true); // Draw with property drawer support
            }
        }
        
        // Draw all nodes list (read-only) - only if it has content
        SerializedProperty allNodesProp = serializedObject.FindProperty("allNodes");
        if (allNodesProp != null && allNodesProp.arraySize > 0)
        {
            GUI.enabled = false;
            EditorGUILayout.PropertyField(allNodesProp, new GUIContent("All Nodes (Auto-Updated)"), true);
            GUI.enabled = true;
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
                if (node.HasChoices) choiceCount += node.choices.Length;
                if (node.IsEndNode) endNodes++;
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
        EditorGUILayout.HelpBox("Use this for quick prototyping. For detailed editing, use the Starting Node property drawer above.", MessageType.Info);
        
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
            dialogTree.CreateStartingNode(newSpeakerName, newDialogText, newIsPlayerSpeaking);
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
                dialogTree.AddSequentialNode(lastNode, newSpeakerName, newDialogText, newIsPlayerSpeaking);
                serializedObject.Update();
                EditorUtility.SetDirty(dialogTree);
            }
        }
        
        EditorGUILayout.EndHorizontal();
        
        // Choice creation
        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("Add Choice", EditorStyles.miniBoldLabel);
        newChoiceText = EditorGUILayout.TextField("Choice Text", newChoiceText);
        newCustomActionId = EditorGUILayout.TextField("Custom Action ID (Optional)", newCustomActionId);
        
        if (GUILayout.Button("Add Choice Node"))
        {
            if (dialogTree.GetStartingNode() == null)
            {
                EditorUtility.DisplayDialog("Error", "Create a starting node first!", "OK");
            }
            else
            {
                Undo.RecordObject(dialogTree, "Add Choice Node");
                var lastNode = FindLastNode();
                string actionId = string.IsNullOrEmpty(newCustomActionId) ? null : newCustomActionId;
                dialogTree.AddChoiceNode(lastNode, newChoiceText, newSpeakerName, newDialogText, newIsPlayerSpeaking, actionId);
                serializedObject.Update();
                EditorUtility.SetDirty(dialogTree);
            }
        }
    }
    
    private void DrawAdvancedToolsSection()
    {
        EditorGUILayout.LabelField("Example & Template Creation", EditorStyles.miniBoldLabel);
        
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Create Linear Example"))
        {
            if (ShouldProceedWithCreation())
            {
                Undo.RecordObject(dialogTree, "Create Linear Example");
                CreateLinearExample();
                serializedObject.Update();
                EditorUtility.SetDirty(dialogTree);
            }
        }
        
        if (GUILayout.Button("Create Branching Example"))
        {
            if (ShouldProceedWithCreation())
            {
                Undo.RecordObject(dialogTree, "Create Branching Example");
                CreateBranchingExample();
                serializedObject.Update();
                EditorUtility.SetDirty(dialogTree);
            }
        }
        
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space(5);
        
        // Dangerous operations
        EditorGUILayout.LabelField("Dangerous Operations", EditorStyles.miniBoldLabel);
        
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Clear All Nodes"))
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
    }

    #endregion

    private bool ShouldProceedWithCreation()
    {
        if (dialogTree.GetStartingNode() != null)
        {
            return EditorUtility.DisplayDialog("Replace Existing Tree?", 
                "This will replace the existing dialog tree. Continue?", "Replace", "Cancel");
        }
        return true;
    }
    
    private DialogNode FindLastNode() // Finds the last node in the current dialog tree for appending new nodes, only works for linear trees
    {
        var allNodes = dialogTree.GetAllNodes();
        foreach (var node in allNodes)
        {
            if (node.IsEndNode)
            {
                return node;
            }
        }
        return dialogTree.GetStartingNode();
    }
    private void ClearAllNodes()
    {
        SerializedProperty startingNodeProp = serializedObject.FindProperty("startingNode");
        if (startingNodeProp != null)
        {
            startingNodeProp.managedReferenceValue = null;
        }
        
        serializedObject.ApplyModifiedProperties();
        dialogTree.RefreshNodeList();
    }
    #region Example Creations
    private void CreateLinearExample()
    {
        string[] speakers = { "Director", "Player", "Director", "Player" };
        string[] texts = {
            "Welcome to the stage! Are you ready for your performance?",
            "I'm a bit nervous, but I think I'm ready.",
            "Nervous is good! It means you care. Break a leg out there!",
            "Thank you! I'll do my best!"
        };
        bool[] playerSpeaking = { false, true, false, true };
        
        dialogTree.CreateLinearConversation(speakers, texts, playerSpeaking);
    }
    
    private void CreateBranchingExample()
    {
        DialogNode start = dialogTree.CreateStartingNode("Stage Manager", "How are you feeling about tonight's performance?");

        DialogNode nervousNode = dialogTree.AddChoiceNode(start, "I'm really nervous...", "Stage Manager", 
            "That's completely normal! Even the best actors get nervous. Let's do some breathing exercises.");

        DialogNode excitedNode = dialogTree.AddChoiceNode(start, "I'm excited!", "Stage Manager", 
            "Wonderful! That energy will really show on stage. Channel that excitement into your performance.");

        DialogNode readyNode = dialogTree.AddChoiceNode(start, "I'm ready!", "Stage Manager", 
            "Perfect! I can see the confidence in your eyes. The audience is going to love you.");
        
        dialogTree.AddSequentialNode(nervousNode, "Player", "Thanks, that really helps!", true);
        dialogTree.AddSequentialNode(excitedNode, "Player", "I can't wait to get on stage!", true);
        dialogTree.AddChoiceNode(readyNode, "Start the show!", "Stage Manager", "Break a leg!", false, "start_performance");
    }
    #endregion  
}