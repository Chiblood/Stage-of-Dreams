/* DialogNodeEditorWindow.cs
 * Dedicated editor window for editing DialogNode properties with tree navigation.
 * Provides focused editing environment with parent/child/choice navigation buttons.
 * 
 * How to use:
 * 1. Select a DialogTree asset in the inspector
 * 2. Click "Edit Starting Node" or any "Edit" button next to a node
 * 3. Edit node properties in the dedicated window
 * 4. Use Tree Navigation buttons to traverse the dialog tree
 * 5. Multiple windows can be open simultaneously
 */

using UnityEngine;
using UnityEditor;

/// <summary>
/// Dedicated editor window for editing DialogNode with tree navigation capabilities
/// </summary>
public class DialogNodeEditorWindow : EditorWindow
{
    // Core references
    private SerializedObject serializedObject;
    private SerializedProperty nodeProperty;
    private DialogNode currentNode;
    private DialogTree parentTree;
    
    // UI State
    private Vector2 scrollPosition;
    private bool showNavigation = true;
    private bool showChoices = true;
    private bool showEvents = false;
    
    // Cached properties (for performance)
    private SerializedProperty nodeIdProp;
    private SerializedProperty characterNameProp;
    private SerializedProperty dialogTextProp;
    private SerializedProperty isPlayerSpeakingProp;
    private SerializedProperty autoAdvanceDelayProp;
    private SerializedProperty parentNodesProp;
    private SerializedProperty childNodeProp;
    private SerializedProperty choicesProp;
    private SerializedProperty onDialogStartProp;
    private SerializedProperty onDialogEndProp;
    
    /// <summary>
    /// Open window for a specific node
    /// </summary>
    public static void OpenWindow(SerializedProperty nodeProperty, DialogTree tree)
    {
        DialogNodeEditorWindow window = CreateInstance<DialogNodeEditorWindow>();
        window.minSize = new Vector2(500, 650);
        window.Initialize(nodeProperty, tree);
        window.Show();
    }
    
    private void Initialize(SerializedProperty property, DialogTree tree)
    {
        nodeProperty = property;
        parentTree = tree;
        serializedObject = property.serializedObject;
        
        // Try to get the actual node object
        if (property.propertyType == SerializedPropertyType.ManagedReference)
        {
            currentNode = property.managedReferenceValue as DialogNode;
        }
        
        RefreshPropertyReferences();
        
        // Update window title with node info
        UpdateWindowTitle();
    }
    
    private void RefreshPropertyReferences()
    {
        if (nodeProperty == null) return;
        
        nodeIdProp = nodeProperty.FindPropertyRelative("_nodeId");
        characterNameProp = nodeProperty.FindPropertyRelative("_characterName");
        dialogTextProp = nodeProperty.FindPropertyRelative("_dialogText");
        isPlayerSpeakingProp = nodeProperty.FindPropertyRelative("_isPlayerSpeaking");
        autoAdvanceDelayProp = nodeProperty.FindPropertyRelative("_autoAdvanceDelay");
        parentNodesProp = nodeProperty.FindPropertyRelative("_parentNodes");
        childNodeProp = nodeProperty.FindPropertyRelative("_childNode");
        choicesProp = nodeProperty.FindPropertyRelative("_choices");
        onDialogStartProp = nodeProperty.FindPropertyRelative("_onDialogStart");
        onDialogEndProp = nodeProperty.FindPropertyRelative("_onDialogEnd");
    }
    
    private void UpdateWindowTitle()
    {
        string nodeId = nodeIdProp?.stringValue ?? "";
        
        if (!string.IsNullOrEmpty(nodeId))
        {
            titleContent = new GUIContent($"Edit DialogNode: {nodeId}");
        }
        else if (characterNameProp != null && !string.IsNullOrEmpty(characterNameProp.stringValue))
        {
            titleContent = new GUIContent($"Edit DialogNode: {characterNameProp.stringValue}");
        }
        else
        {
            titleContent = new GUIContent("Edit DialogNode: <No ID>");
        }
    }
    
    private void OnGUI()
    {
        if (serializedObject == null || nodeProperty == null)
        {
            EditorGUILayout.HelpBox("No node selected. This window can be closed.", MessageType.Info);
            return;
        }
        
        // Refresh property references if any are null (can happen after recompile)
        if (nodeIdProp == null || characterNameProp == null || dialogTextProp == null)
        {
            RefreshPropertyReferences();
        }
        
        // CRITICAL: Update at the start of every frame
        serializedObject.Update();
        
        // Sync currentNode reference
        if (currentNode == null && nodeProperty.propertyType == SerializedPropertyType.ManagedReference)
        {
            currentNode = nodeProperty.managedReferenceValue as DialogNode;
        }
        
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        DrawHeader();
        EditorGUILayout.Space(10);
        
        DrawNodeIdentification();
        EditorGUILayout.Space(10);
        
        DrawDialogContent();
        EditorGUILayout.Space(10);
        
        DrawFlowControl();
        EditorGUILayout.Space(10);
        
        DrawTreeNavigation();
        EditorGUILayout.Space(10);
        
        DrawChoicesManagement();
        EditorGUILayout.Space(10);
        
        DrawEventsSection();
        EditorGUILayout.Space(10);
        
        DrawQuickActions();
        
        EditorGUILayout.EndScrollView();
        
        // CRITICAL: Apply changes at the end
        if (serializedObject.ApplyModifiedProperties())
        {
            EditorUtility.SetDirty(serializedObject.targetObject);
            UpdateWindowTitle(); // Update title if node ID changed
            Repaint();
        }
    }
    
    #region Drawing Sections
    
    private void DrawHeader()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        
        GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 16,
            alignment = TextAnchor.MiddleCenter
        };
        
        EditorGUILayout.LabelField("Dialog Node Editor", headerStyle);
        
        // Get preview info
        string nodeId = nodeIdProp?.stringValue ?? "<No ID>";
        string speaker = characterNameProp?.stringValue ?? "<No Speaker>";
        string preview = "<No text>";
        
        if (dialogTextProp != null && !string.IsNullOrEmpty(dialogTextProp.stringValue))
        {
            preview = dialogTextProp.stringValue.Length > 50 
                ? dialogTextProp.stringValue.Substring(0, 50) + "..." 
                : dialogTextProp.stringValue;
        }
        
        GUIStyle previewStyle = new GUIStyle(EditorStyles.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Italic,
            wordWrap = true
        };
        
        EditorGUILayout.LabelField($"[{nodeId}] {speaker}", previewStyle);
        EditorGUILayout.LabelField($"\"{preview}\"", previewStyle);
        
        EditorGUILayout.EndVertical();
    }
    
    private void DrawNodeIdentification()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Node Identification", EditorStyles.boldLabel);
        
        EditorGUILayout.Space(5);
        
        // Node Name/ID
        EditorGUILayout.BeginHorizontal();
        
        if (nodeIdProp != null)
        {
            EditorGUI.BeginChangeCheck();
            string newId = EditorGUILayout.TextField(new GUIContent("Node Name/ID", "Unique identifier for this node"), 
                nodeIdProp.stringValue ?? "");
            if (EditorGUI.EndChangeCheck())
            {
                nodeIdProp.stringValue = newId;
                GUI.changed = true;
            }
        }
        
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.HelpBox("Tip: Use Tools > Dialog System > Generate Node ID to create standardized IDs", MessageType.Info);
        
        // Position in tree info
        if (parentTree != null)
        {
            var allNodes = parentTree.GetAllNodes();
            int nodeIndex = -1;
            
            for (int i = 0; i < allNodes.Count; i++)
            {
                if (allNodes[i] == currentNode)
                {
                    nodeIndex = i;
                    break;
                }
            }
            
            if (nodeIndex >= 0)
            {
                EditorGUILayout.LabelField($"Position in Tree: {nodeIndex + 1} of {allNodes.Count} nodes", EditorStyles.miniLabel);
            }
        }
        
        EditorGUILayout.EndVertical();
    }
    
    private void DrawDialogContent()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Dialog Content", EditorStyles.boldLabel);
        
        EditorGUILayout.Space(5);
        
        // Speaker Name
        if (characterNameProp != null)
        {
            EditorGUI.BeginChangeCheck();
            string newSpeaker = EditorGUILayout.TextField(new GUIContent("Speaker Name", "Character speaking this dialog"), 
                characterNameProp.stringValue ?? "");
            if (EditorGUI.EndChangeCheck())
            {
                characterNameProp.stringValue = newSpeaker;
                GUI.changed = true;
            }
        }
        
        // Dialog Text (Large TextArea)
        if (dialogTextProp != null)
        {
            EditorGUILayout.LabelField("Dialog Text", EditorStyles.miniBoldLabel);
            EditorGUI.BeginChangeCheck();
            string newText = EditorGUILayout.TextArea(
                dialogTextProp.stringValue ?? "", 
                GUILayout.Height(80)
            );
            if (EditorGUI.EndChangeCheck())
            {
                dialogTextProp.stringValue = newText;
                GUI.changed = true;
            }
        }
        
        // Is Player Speaking
        if (isPlayerSpeakingProp != null)
        {
            EditorGUI.BeginChangeCheck();
            bool newValue = EditorGUILayout.Toggle(new GUIContent("Is Player Speaking", "True if this is the player's dialog"), 
                isPlayerSpeakingProp.boolValue);
            if (EditorGUI.EndChangeCheck())
            {
                isPlayerSpeakingProp.boolValue = newValue;
                GUI.changed = true;
            }
        }
        
        EditorGUILayout.EndVertical();
    }
    
    private void DrawFlowControl()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Flow Control", EditorStyles.boldLabel);
        
        EditorGUILayout.Space(5);
        
        // Auto Advance Delay
        if (autoAdvanceDelayProp != null)
        {
            EditorGUI.BeginChangeCheck();
            float newDelay = EditorGUILayout.FloatField(
                new GUIContent("Auto Advance Delay", "Seconds to wait before auto-advancing (0 = wait for input)"), 
                autoAdvanceDelayProp.floatValue);
            if (EditorGUI.EndChangeCheck())
            {
                autoAdvanceDelayProp.floatValue = Mathf.Max(0f, newDelay);
                GUI.changed = true;
            }
        }
        
        // Read-only info
        bool hasChoices = choicesProp != null && choicesProp.arraySize > 0;
        bool hasChild = childNodeProp != null && childNodeProp.managedReferenceValue != null;
        
        EditorGUILayout.LabelField($"Has Choices: {(hasChoices ? "Yes" : "No")}", EditorStyles.miniLabel);
        EditorGUILayout.LabelField($"Has Child (Auto-Advance): {(hasChild ? "Yes" : "No")}", EditorStyles.miniLabel);
        
        if (hasChoices && hasChild)
        {
            EditorGUILayout.HelpBox("⚠ Node has both choices and child node. This is invalid - choices will take precedence.", MessageType.Warning);
        }
        
        EditorGUILayout.EndVertical();
    }
    
    private void DrawTreeNavigation()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        
        showNavigation = EditorGUILayout.Foldout(showNavigation, "Tree Navigation", true, EditorStyles.foldoutHeader);
        
        if (showNavigation)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.HelpBox("Navigate to related nodes. Each button opens a new editor window.", MessageType.Info);
            
            // Parent Nodes
            DrawParentNavigationButtons();
            
            EditorGUILayout.Space(5);
            
            // Child Nodes (including both auto-advance child AND choice targets)
            DrawChildNavigationButtons();
        }
        
        EditorGUILayout.EndVertical();
    }
    
    private void DrawParentNavigationButtons()
    {
        EditorGUILayout.LabelField("Parent Nodes", EditorStyles.miniBoldLabel);
        
        if (parentNodesProp == null || parentNodesProp.arraySize == 0)
        {
            EditorGUILayout.HelpBox("No parent nodes (this may be the starting node)", MessageType.Info);
            return;
        }
        
        for (int i = 0; i < parentNodesProp.arraySize; i++)
        {
            SerializedProperty parentProp = parentNodesProp.GetArrayElementAtIndex(i);
            
            if (parentProp.managedReferenceValue != null)
            {
                DialogNode parentNode = parentProp.managedReferenceValue as DialogNode;
                
                string buttonLabel = GetNodeButtonLabel(parentNode, $"Parent {i + 1}");
                
                GUI.backgroundColor = new Color(1f, 0.9f, 0.7f); // Light orange
                if ( GUILayout.Button($"⬆ Edit Parent {i + 1}: {buttonLabel}", GUILayout.Height(25)))
                {
                    OpenWindow(parentProp, parentTree);
                }
                GUI.backgroundColor = Color.white;
            }
        }
    }
    
    private void DrawChildNavigationButtons()
    {
        EditorGUILayout.LabelField("Child Nodes", EditorStyles.miniBoldLabel);
        
        bool hasChild = childNodeProp != null && childNodeProp.managedReferenceValue != null;
        bool hasChoices = choicesProp != null && choicesProp.arraySize > 0;
        
        if (!hasChild && !hasChoices)
        {
            EditorGUILayout.HelpBox("No child nodes. Use buttons below to create one.", MessageType.Info);
            
            // Create Child Node buttons
            EditorGUILayout.BeginHorizontal();
            
            GUI.backgroundColor = new Color(0.7f, 1f, 0.7f); // Light green
            if (GUILayout.Button("+ Create Auto-Advance Child", GUILayout.Height(30)))
            {
                CreateChildNode();
            }
            
            if (GUILayout.Button("+ Create Choice Branch", GUILayout.Height(30)))
            {
                CreateChoiceBranch();
            }
            GUI.backgroundColor = Color.white;
            
            EditorGUILayout.EndHorizontal();
            return;
        }
        
        // Show auto-advance child if exists
        if (hasChild)
        {
            DialogNode childNode = childNodeProp.managedReferenceValue as DialogNode;
            string buttonLabel = GetNodeButtonLabel(childNode, "Child");
            
            GUI.backgroundColor = new Color(0.7f, 1f, 0.7f); // Light green
            if (GUILayout.Button($"⬇ Edit Child (Auto-Advance): {buttonLabel}", GUILayout.Height(25)))
            {
                OpenWindow(childNodeProp, parentTree);
            }
            GUI.backgroundColor = Color.white;
        }
        
        // Show choice targets if exist
        if (hasChoices)
        {
            for (int i = 0; i < choicesProp.arraySize; i++)
            {
                SerializedProperty choiceProp = choicesProp.GetArrayElementAtIndex(i);
                SerializedProperty targetNodeProp = choiceProp.FindPropertyRelative("_targetNode");
                SerializedProperty choiceTextProp = choiceProp.FindPropertyRelative("_choiceText");
                
                string choiceText = choiceTextProp?.stringValue ?? $"Choice {i + 1}";
                
                if (targetNodeProp != null && targetNodeProp.managedReferenceValue != null)
                {
                    DialogNode targetNode = targetNodeProp.managedReferenceValue as DialogNode;
                    string nodeLabel = GetNodeButtonLabel(targetNode, $"Target {i + 1}");
                    
                    GUI.backgroundColor = new Color(0.7f, 0.9f, 1f); // Light blue
                    if (GUILayout.Button($"⬇ Edit Child (Choice {i + 1}): {nodeLabel}\n    Choice: \"{choiceText}\"", GUILayout.Height(35)))
                    {
                        OpenWindow(targetNodeProp, parentTree);
                    }
                    GUI.backgroundColor = Color.white;
                }
                else
                {
                    EditorGUILayout.LabelField($"Choice {i + 1}: \"{choiceText}\" - No target set", EditorStyles.helpBox);
                }
            }
        }
        
        // Create additional child options
        EditorGUILayout.Space(5);
        EditorGUILayout.BeginHorizontal();
        
        if (!hasChild && hasChoices)
        {
            GUI.backgroundColor = new Color(0.7f, 1f, 0.7f);
            if (GUILayout.Button("+ Create Auto-Advance Child", GUILayout.Height(25)))
            {
                if (EditorUtility.DisplayDialog("Warning", 
                    "This node already has choices. Adding an auto-advance child may cause issues.\nContinue?", 
                    "Create", "Cancel"))
                {
                    CreateChildNode();
                }
            }
            GUI.backgroundColor = Color.white;
        }
        
        EditorGUILayout.EndHorizontal();
    }
    
    private void DrawChoicesManagement()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        
        showChoices = EditorGUILayout.Foldout(showChoices, "Choices Management", true, EditorStyles.foldoutHeader);
        
        if (showChoices)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.HelpBox("Manage player choices. Edit choices inline below.", MessageType.Info);
            
            if (choicesProp != null)
            {
                EditorGUILayout.LabelField($"Total Choices: {choicesProp.arraySize}", EditorStyles.miniBoldLabel);
                
                // Draw choices inline with PropertyField (uses DialogChoicePropertyDrawer)
                for (int i = 0; i < choicesProp.arraySize; i++)
                {
                    SerializedProperty choiceProp = choicesProp.GetArrayElementAtIndex(i);
                    EditorGUILayout.PropertyField(choiceProp, new GUIContent($"Choice {i + 1}"), true);
                    EditorGUILayout.Space(2);
                }
                
                EditorGUILayout.Space(5);
                
                // Add New Choice button
                GUI.backgroundColor = new Color(0.7f, 1f, 0.7f);
                if (GUILayout.Button("+ Add New Choice", GUILayout.Height(25)))
                {
                    AddNewChoice();
                }
                GUI.backgroundColor = Color.white;
            }
        }
        
        EditorGUILayout.EndVertical();
    }
    
    private void DrawEventsSection()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        
        showEvents = EditorGUILayout.Foldout(showEvents, "Unity Events", true, EditorStyles.foldoutHeader);
        
        if (showEvents)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.HelpBox("Events triggered when this node starts/ends.", MessageType.Info);
            
            if (onDialogStartProp != null)
            {
                EditorGUILayout.PropertyField(onDialogStartProp, new GUIContent("On Dialog Start"), true);
            }
            
            EditorGUILayout.Space(5);
            
            if (onDialogEndProp != null)
            {
                EditorGUILayout.PropertyField(onDialogEndProp, new GUIContent("On Dialog End"), true);
            }
        }
        
        EditorGUILayout.EndVertical();
    }
    
    private void DrawQuickActions()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Quick Actions", EditorStyles.boldLabel);
        
        EditorGUILayout.Space(5);
        
        EditorGUILayout.BeginHorizontal();
        
        // Validate Node
        if (GUILayout.Button("Validate Node", GUILayout.Height(30)))
        {
            ValidateNode();
        }
        
        // Clear Child
        GUI.backgroundColor = Color.yellow;
        if (GUILayout.Button("Clear Child", GUILayout.Height(30)))
        {
            if (EditorUtility.DisplayDialog("Clear Child Node?", 
                "This will remove the auto-advance child node. Continue?", "Clear", "Cancel"))
            {
                if (childNodeProp != null)
                {
                    childNodeProp.managedReferenceValue = null;
                    GUI.changed = true;
                }
            }
        }
        GUI.backgroundColor = Color.white;
        
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space(5);
        
        // Status display
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Node Status", EditorStyles.miniBoldLabel);
        
        bool hasId = nodeIdProp != null && !string.IsNullOrEmpty(nodeIdProp.stringValue);
        bool hasSpeaker = characterNameProp != null && !string.IsNullOrEmpty(characterNameProp.stringValue);
        bool hasText = dialogTextProp != null && !string.IsNullOrEmpty(dialogTextProp.stringValue);
        bool hasChoices = choicesProp != null && choicesProp.arraySize > 0;
        bool hasChild = childNodeProp != null && childNodeProp.managedReferenceValue != null;
        bool isValid = hasSpeaker && hasText && !(hasChoices && hasChild);
        
        DrawStatusLine("Has ID/Name", hasId);
        DrawStatusLine("Has Speaker", hasSpeaker);
        DrawStatusLine("Has Dialog Text", hasText);
        DrawStatusLine("Has Valid Flow", !(hasChoices && hasChild));
        DrawStatusLine("Is Valid", isValid);
        
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.EndVertical();
    }
    
    #endregion
    
    #region Helper Methods
    
    /// <summary>
    /// Get formatted button label for a node
    /// </summary>
    private string GetNodeButtonLabel(DialogNode node, string fallback)
    {
        if (node == null) return fallback;
        
        string nodeId = !string.IsNullOrEmpty(node.NodeName) ? $"[{node.NodeName}]" : "[No ID]";
        string preview = !string.IsNullOrEmpty(node.DialogText) 
            ? (node.DialogText.Length > 30 ? node.DialogText.Substring(0, 30) + "..." : node.DialogText)
            : "<No text>";
        
        return $"{nodeId} \"{preview}\"";
    }
    
    private void DrawStatusLine(string label, bool status)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(label, GUILayout.Width(150));
        
        GUIStyle statusStyle = new GUIStyle(EditorStyles.label)
        {
            fontStyle = FontStyle.Bold
        };
        
        if (status)
        {
            statusStyle.normal.textColor = Color.green;
            EditorGUILayout.LabelField("✅", statusStyle);
        }
        else
        {
            statusStyle.normal.textColor = Color.red;
            EditorGUILayout.LabelField("❌", statusStyle);
        }
        
        EditorGUILayout.EndHorizontal();
    }
    
    /// <summary>
    /// Create a new auto-advance child node
    /// </summary>
    private void CreateChildNode()
    {
        if (childNodeProp == null) return;
        
        // Create new DialogNode
        var newNode = new DialogNode("Speaker", "Enter dialog text here", false);
        
        childNodeProp.managedReferenceValue = newNode;
        
        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(serializedObject.targetObject);
        
        // Refresh tree
        if (parentTree != null)
        {
            parentTree.RefreshNodeList();
        }
        
        EditorUtility.DisplayDialog("Child Node Created", 
            "Auto-advance child node created successfully!\nClick 'Edit Child' to configure it.", "OK");
    }
    
    /// <summary>
    /// Create a new choice branch (adds a choice without target)
    /// </summary>
    private void CreateChoiceBranch()
    {
        if (choicesProp == null) return;
        
        // Add new choice to array
        int newIndex = choicesProp.arraySize;
        choicesProp.arraySize++;
        SerializedProperty newChoiceProp = choicesProp.GetArrayElementAtIndex(newIndex);
        
        // CRITICAL: For SerializeReference, we need to create the actual object
        if (newChoiceProp != null)
        {
            // Create a new DialogChoice instance
            var newChoice = new DialogChoice($"Choice {newIndex + 1}", null);
            
            // Set the managed reference value
            newChoiceProp.managedReferenceValue = newChoice;
            
            // Now find and set properties
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
            
            // Re-get the property after applying
            newChoiceProp = choicesProp.GetArrayElementAtIndex(newIndex);
            
            SerializedProperty choiceTextProp = newChoiceProp.FindPropertyRelative("_choiceText");
            SerializedProperty choiceIdProp = newChoiceProp.FindPropertyRelative("_choiceId");
            
            if (choiceTextProp != null)
                choiceTextProp.stringValue = $"Choice {newIndex + 1}";
            
            if (choiceIdProp != null)
                choiceIdProp.stringValue = $"choice_{System.Guid.NewGuid().ToString().Substring(0, 8)}";
        }
        
        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(serializedObject.targetObject);
        
        EditorUtility.DisplayDialog("Choice Created", 
            "New choice created! Edit it in the Choices Management section below.", "OK");
    }
    
    private void AddNewChoice()
    {
        if (choicesProp == null) return;
        
        // Add new choice to array
        int newIndex = choicesProp.arraySize;
        choicesProp.arraySize++;
        SerializedProperty newChoiceProp = choicesProp.GetArrayElementAtIndex(newIndex);
        
        // CRITICAL: For SerializeReference, we need to create the actual object
        if (newChoiceProp != null)
        {
            // Create a new DialogChoice instance
            var newChoice = new DialogChoice($"Choice {newIndex + 1}", null);
            
            // Set the managed reference value
            newChoiceProp.managedReferenceValue = newChoice;
            
            // Now find and set properties
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
            
            // Re-get the property after applying
            newChoiceProp = choicesProp.GetArrayElementAtIndex(newIndex);
            
            SerializedProperty choiceTextProp = newChoiceProp.FindPropertyRelative("_choiceText");
            SerializedProperty choiceIdProp = newChoiceProp.FindPropertyRelative("_choiceId");
            
            if (choiceTextProp != null)
                choiceTextProp.stringValue = $"Choice {newIndex + 1}";
            
            if (choiceIdProp != null)
                choiceIdProp.stringValue = $"choice_{System.Guid.NewGuid().ToString().Substring(0, 8)}";
        }
        
        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(serializedObject.targetObject);
    }
    
    private void ValidateNode()
    {
        bool hasId = nodeIdProp != null && !string.IsNullOrEmpty(nodeIdProp.stringValue);
        bool hasSpeaker = characterNameProp != null && !string.IsNullOrEmpty(characterNameProp.stringValue);
        bool hasText = dialogTextProp != null && !string.IsNullOrEmpty(dialogTextProp.stringValue);
        bool hasChoices = choicesProp != null && choicesProp.arraySize > 0;
        bool hasChild = childNodeProp != null && childNodeProp.managedReferenceValue != null;
        bool isValid = hasSpeaker && hasText && !(hasChoices && hasChild);
        
        if (isValid)
        {
            EditorUtility.DisplayDialog("Validation Success", 
                "✅ This node is valid and ready to use!", "OK");
        }
        else
        {
            string issues = "Issues found:\n\n";
            
            if (!hasSpeaker)
                issues += "❌ Missing speaker name\n";
            
            if (!hasText)
                issues += "❌ Missing dialog text\n";
            
            if (hasChoices && hasChild)
                issues += "❌ Node has both choices and child node (invalid - remove one)\n";
            
            if (!hasId)
                issues += "⚠ No node ID/name (recommended for referencing)\n";
            
            EditorUtility.DisplayDialog("Validation Failed", issues, "OK");
        }
    }
    
    #endregion
}
