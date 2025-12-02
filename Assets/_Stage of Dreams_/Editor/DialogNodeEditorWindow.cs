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

using UnityEditor;
using UnityEngine;

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
    private SerializedProperty nodeTypeProp;
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
        // Check if this is a specialized minigame node type
        if (IsRememberTheScriptNode(nodeProperty))
        {
            RememberTheScriptNodeEditor.OpenWindow(nodeProperty, tree);
            return;
        }

        // Otherwise, open standard dialog editor
        DialogNodeEditorWindow window = CreateInstance<DialogNodeEditorWindow>();
        window.minSize = new Vector2(500, 650);
        window.Initialize(nodeProperty, tree);
        window.Show();
    }

    /// <summary>
    /// Check if a node is a RememberTheScript minigame node
    /// </summary>
    private static bool IsRememberTheScriptNode(SerializedProperty nodeProperty)
    {
        SerializedProperty nodeTypeProp = nodeProperty.FindPropertyRelative("_nodeType");
        if (nodeTypeProp != null)
        {
            return (DialogNodeType)nodeTypeProp.enumValueIndex == DialogNodeType.RememberTheScript;
        }

        // Fallback to old method for backwards compatibility
        SerializedProperty isRememberScriptProp = nodeProperty.FindPropertyRelative("_isRememberScriptNode");
        return isRememberScriptProp != null && isRememberScriptProp.boolValue;
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

    private void OnDisable()
    {
        // Apply any pending changes before closing
        if (serializedObject != null && serializedObject.targetObject != null)
        {
            try
            {
                serializedObject.ApplyModifiedProperties();
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[DialogNodeEditorWindow] Error applying properties on disable: {ex.Message}");
            }
        }
    }

    private void RefreshPropertyReferences()
    {
        if (nodeProperty == null) return;

        nodeTypeProp = nodeProperty.FindPropertyRelative("_nodeType");
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

    /// <summary>
    /// Create a new auto-advance child node
    /// </summary>
    private void CreateChildNode()
    {
        if (childNodeProp == null)
        {
            EditorUtility.DisplayDialog("Error", "Cannot create child node - property reference is null.", "OK");
            return;
        }

        if (serializedObject == null || serializedObject.targetObject == null)
        {
            EditorUtility.DisplayDialog("Error", "Cannot create child node - serialized object is invalid.", "OK");
            return;
        }

        try
        {
            // Create new DialogNode
            var newNode = new DialogNode("Speaker", "Enter dialog text here", false);

            childNodeProp.managedReferenceValue = newNode;

            serializedObject.ApplyModifiedProperties();

            if (serializedObject.targetObject != null)
            {
                EditorUtility.SetDirty(serializedObject.targetObject);
            }

            // Refresh tree
            if (parentTree != null)
            {
                parentTree.RefreshNodeList();
                EditorUtility.SetDirty(parentTree);
            }

            EditorUtility.DisplayDialog("Child Node Created",
                "Auto-advance child node created successfully!\nClick 'Edit Child' to configure it.", "OK");

            // Force repaint
            Repaint();
        }
        catch (System.Exception ex)
        {
            EditorUtility.DisplayDialog("Error Creating Node",
                $"Failed to create child node:\n{ex.Message}", "OK");
            Debug.LogError($"[DialogNodeEditorWindow] Error creating child node: {ex}");
        }
    }

    private void OnGUI()
    {
        // Check if references are still valid
        // Use more defensive checks to prevent NullReferenceException
        if (serializedObject == null)
        {
            EditorGUILayout.HelpBox("SerializedObject is null. This window can be closed.", MessageType.Warning);

            if (GUILayout.Button("Close Window"))
            {
                Close();
            }
            return;
        }

        // Check targetObject separately with try-catch to handle Unity's internal null checking
        bool targetObjectValid = false;
        try
        {
            targetObjectValid = serializedObject.targetObject != null;
        }
        catch (System.NullReferenceException)
        {
            // Unity's internal null check threw - object is definitely invalid
            targetObjectValid = false;
        }

        if (!targetObjectValid || nodeProperty == null)
        {
            EditorGUILayout.HelpBox("Node reference lost. This window can be closed.", MessageType.Warning);

            if (GUILayout.Button("Close Window"))
            {
                Close();
            }
            return;
        }

        // Refresh property references if needed
        if (nodeIdProp == null)
        {
            RefreshPropertyReferences();
        }

        // Update serialized object
        try
        {
            serializedObject.Update();
        }
        catch (System.Exception ex)
        {
            EditorGUILayout.HelpBox($"Error updating serialized object: {ex.Message}", MessageType.Error);
            if (GUILayout.Button("Close Window"))
            {
                Close();
            }
            return;
        }

        // Sync current node reference
        if (currentNode == null && nodeProperty.propertyType == SerializedPropertyType.ManagedReference)
        {
            currentNode = nodeProperty.managedReferenceValue as DialogNode;
        }

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        // Draw node properties
        DrawNodeProperties();

        EditorGUILayout.Space(10);

        // Draw tree navigation
        DrawTreeNavigation();

        EditorGUILayout.Space(10);

        // Draw choices section
        DrawChoicesSection();

        EditorGUILayout.Space(10);

        // Draw events section (optional)
        DrawEventsSection();

        EditorGUILayout.EndScrollView();

        // Apply changes
        try
        {
            if (serializedObject.ApplyModifiedProperties())
            {
                // Double-check targetObject is still valid before setting dirty
                try
                {
                    if (serializedObject.targetObject != null)
                    {
                        EditorUtility.SetDirty(serializedObject.targetObject);
                    }
                }
                catch (System.NullReferenceException)
                {
                    // Object became null during ApplyModifiedProperties - just skip SetDirty
                    Debug.LogWarning("[DialogNodeEditorWindow] Target object became null during property application");
                }

                UpdateWindowTitle();
                Repaint();
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"[DialogNodeEditorWindow] Error applying properties: {ex.Message}");
        }
    }

    #region Drawing Sections

    private void DrawNodeProperties()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Node Properties", EditorStyles.boldLabel);

        EditorGUILayout.Space(5);

        // Node Type
        if (nodeTypeProp != null)
        {
            EditorGUILayout.PropertyField(nodeTypeProp, new GUIContent("Node Type"));
        }

        // Node ID
        if (nodeIdProp != null)
        {
            EditorGUILayout.PropertyField(nodeIdProp, new GUIContent("Node ID/Name"));
        }

        // Character Name
        if (characterNameProp != null)
        {
            EditorGUILayout.PropertyField(characterNameProp, new GUIContent("Speaker Name"));
        }

        // Dialog Text
        if (dialogTextProp != null)
        {
            EditorGUILayout.PropertyField(dialogTextProp, new GUIContent("Dialog Text"), GUILayout.Height(80));
        }

        // Is Player Speaking
        if (isPlayerSpeakingProp != null)
        {
            EditorGUILayout.PropertyField(isPlayerSpeakingProp, new GUIContent("Is Player Speaking"));
        }

        // Auto Advance Delay
        if (autoAdvanceDelayProp != null)
        {
            EditorGUILayout.PropertyField(autoAdvanceDelayProp, new GUIContent("Auto Advance Delay (s)"));
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

            // Parent Nodes
            if (parentNodesProp != null && parentNodesProp.arraySize > 0)
            {
                EditorGUILayout.LabelField("Parent Nodes:", EditorStyles.miniBoldLabel);

                for (int i = 0; i < parentNodesProp.arraySize; i++)
                {
                    SerializedProperty parentProp = parentNodesProp.GetArrayElementAtIndex(i);

                    if (parentProp.managedReferenceValue != null)
                    {
                        DialogNode parentNode = parentProp.managedReferenceValue as DialogNode;
                        string buttonLabel = GetNodeButtonLabel(parentNode, $"Parent {i + 1}");

                        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                        EditorGUILayout.LabelField($"⬆ {buttonLabel}", EditorStyles.wordWrappedLabel);

                        // Edit button
                        GUI.backgroundColor = new Color(0.7f, 0.9f, 1f); // Light blue
                        if (GUILayout.Button("Edit", EditorStyles.miniButton, GUILayout.Width(50)))
                        {
                            OpenParentNodeForEditing(parentNode, i);
                        }
                        GUI.backgroundColor = Color.white;

                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
            else
            {
                EditorGUILayout.LabelField("No parent nodes (this is a root node)", EditorStyles.miniLabel);
            }

            EditorGUILayout.Space(5);

            // Child Node (Auto-advance)
            EditorGUILayout.LabelField("Child Node (Auto-advance):", EditorStyles.miniBoldLabel);

            if (childNodeProp != null && childNodeProp.managedReferenceValue != null)
            {
                DialogNode childNode = childNodeProp.managedReferenceValue as DialogNode;
                string childLabel = GetNodeButtonLabel(childNode, "Child Node");

                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                EditorGUILayout.LabelField($"⬇ {childLabel}", EditorStyles.wordWrappedLabel);

                // Edit button
                GUI.backgroundColor = new Color(0.7f, 0.9f, 1f); // Light blue
                if (GUILayout.Button("Edit", EditorStyles.miniButton, GUILayout.Width(50)))
                {
                    OpenChildNodeForEditing(childNode);
                }

                // Clear button
                GUI.backgroundColor = Color.yellow;
                if (GUILayout.Button("Clear", EditorStyles.miniButton, GUILayout.Width(50)))
                {
                    if (EditorUtility.DisplayDialog("Clear Child Node?",
                        "This will remove the auto-advance child node. Continue?", "Clear", "Cancel"))
                    {
                        childNodeProp.managedReferenceValue = null;
                    }
                }
                GUI.backgroundColor = Color.white;

                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.HelpBox("No child node. Create one for auto-advance.", MessageType.Info);

                GUI.backgroundColor = new Color(0.7f, 1f, 0.7f);
                if (GUILayout.Button("+ Create Child Node", GUILayout.Height(25)))
                {
                    CreateChildNode();
                }
                GUI.backgroundColor = Color.white;
            }
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawChoicesSection()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        showChoices = EditorGUILayout.Foldout(showChoices, "Choices", true, EditorStyles.foldoutHeader);

        if (showChoices)
        {
            if (choicesProp != null)
            {
                EditorGUILayout.HelpBox("Choices are best edited through the DialogTree inspector for full functionality.", MessageType.Info);
                EditorGUILayout.PropertyField(choicesProp, true);
            }
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawEventsSection()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        showEvents = EditorGUILayout.Foldout(showEvents, "Events (Advanced)", true, EditorStyles.foldoutHeader);

        if (showEvents)
        {
            EditorGUILayout.HelpBox("Events are best edited through specialized editors.", MessageType.Info);

            if (onDialogStartProp != null)
            {
                EditorGUILayout.PropertyField(onDialogStartProp, new GUIContent("On Dialog Start"));
            }

            if (onDialogEndProp != null)
            {
                EditorGUILayout.PropertyField(onDialogEndProp, new GUIContent("On Dialog End"));
            }
        }

        EditorGUILayout.EndVertical();
    }

    #endregion

    #region Helper Methods

    private string GetNodeButtonLabel(DialogNode node, string fallback)
    {
        if (node == null) return fallback;

        string nodeId = !string.IsNullOrEmpty(node.NodeName) ? $"[{node.NodeName}]" : "[No ID]";

        // Check node type
        if (node.NodeType == DialogNodeType.RememberTheScript)
        {
            return $"{nodeId} 🎭 RememberTheScript";
        }

        string preview = !string.IsNullOrEmpty(node.DialogText)
            ? (node.DialogText.Length > 30 ? node.DialogText.Substring(0, 30) + "..." : node.DialogText)
            : "<No text>";

        return $"{nodeId} \"{preview}\"";
    }

    /// <summary>
    /// Open child node for editing in appropriate editor window
    /// </summary>
    private void OpenChildNodeForEditing(DialogNode childNode)
    {
        if (childNode == null || parentTree == null)
        {
            EditorUtility.DisplayDialog("Error", "Cannot open child node - reference is missing.", "OK");
            return;
        }

        // Find the child node in the tree's allNodes list
        SerializedObject treeObject = new SerializedObject(parentTree);
        SerializedProperty allNodesProp = treeObject.FindProperty("allNodes");

        if (allNodesProp != null)
        {
            for (int i = 0; i < allNodesProp.arraySize; i++)
            {
                SerializedProperty nodeProp = allNodesProp.GetArrayElementAtIndex(i);
                DialogNode node = nodeProp.managedReferenceValue as DialogNode;

                if (node == childNode)
                {
                    // Open appropriate editor based on node type
                    OpenWindow(nodeProp, parentTree);
                    return;
                }
            }
        }

        EditorUtility.DisplayDialog("Error",
            "Could not find child node in tree.\nTry refreshing the tree from DialogTree inspector.", "OK");
    }

    /// <summary>
    /// Open parent node for editing in appropriate editor window
    /// </summary>
    private void OpenParentNodeForEditing(DialogNode parentNode, int parentIndex)
    {
        if (parentNode == null || parentTree == null)
        {
            EditorUtility.DisplayDialog("Error", "Cannot open parent node - reference is missing.", "OK");
            return;
        }

        // Find the parent node in the tree's allNodes list
        SerializedObject treeObject = new SerializedObject(parentTree);
        SerializedProperty allNodesProp = treeObject.FindProperty("allNodes");

        if (allNodesProp != null)
        {
            for (int i = 0; i < allNodesProp.arraySize; i++)
            {
                SerializedProperty nodeProp = allNodesProp.GetArrayElementAtIndex(i);
                DialogNode node = nodeProp.managedReferenceValue as DialogNode;

                if (node == parentNode)
                {
                    // Open appropriate editor based on node type
                    OpenWindow(nodeProp, parentTree);
                    return;
                }
            }
        }

        EditorUtility.DisplayDialog("Error",
            "Could not find parent node in tree.\nTry refreshing the tree from DialogTree inspector.", "OK");
    }

    #endregion
}
