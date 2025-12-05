/* DialogChoiceEditorWindow.cs
 * Dedicated editor window for editing DialogChoice properties with a clean, spacious interface.
 * Provides better editing experience than inline PropertyDrawer.
 * 
 * How to use:
 * 1. In DialogNode inspector, right-click on a choice
 * 2. Select "Edit in Window" (or use the button in the PropertyDrawer)
 * 3. Edit choice properties in the dedicated window
 * 4. Changes are automatically saved
 */

using UnityEditor;
using UnityEngine;

/// <summary>
/// Dedicated editor window for editing DialogChoice with improved UX
/// </summary>
public class DialogChoiceEditorWindow : EditorWindow
{
    private SerializedObject serializedObject;
    private SerializedProperty choiceProperty;
    private DialogChoice currentChoice;
    private DialogTree parentTree;
    private Vector2 scrollPosition;

    // Cached properties
    private SerializedProperty choiceTextProp;
    private SerializedProperty choiceIdProp;
    private SerializedProperty targetNodeProp;
    private SerializedProperty targetNodeNameProp;
    private SerializedProperty onChoiceSelectedProp;

    // UI State
    private bool showTargetPreview = true;
    private bool showEvents = true;

    /// <summary>
    /// Open the choice editor window for a specific choice
    /// </summary>
    public static void OpenWindow(SerializedProperty choiceProperty, DialogTree tree)
    {
        DialogChoiceEditorWindow window = GetWindow<DialogChoiceEditorWindow>("Edit Dialog Choice");
        window.minSize = new Vector2(400, 500);
        window.Initialize(choiceProperty, tree);
        window.Show();
    }

    private void Initialize(SerializedProperty property, DialogTree tree)
    {
        choiceProperty = property;
        parentTree = tree;
        serializedObject = property.serializedObject;

        // IMPORTANT: For array elements, we don't use managedReferenceValue directly
        // The property IS the choice, not a reference to it
        RefreshPropertyReferences();

        // Try to get the actual choice object
        // For SerializeReference in a list, the property itself represents the object
        if (property.propertyType == SerializedPropertyType.ManagedReference)
        {
            currentChoice = property.managedReferenceValue as DialogChoice;
        }

        if (currentChoice == null)
        {
            Debug.LogWarning("DialogChoiceEditorWindow: Could not get DialogChoice directly from property. This is expected for array elements.");
            // Don't treat this as an error - we can still edit via SerializedProperty
            // The currentChoice is only used for validation display
        }
    }

    private void RefreshPropertyReferences()
    {
        if (choiceProperty == null) return;

        // Cache property references - these work regardless of how the choice is stored
        choiceTextProp = choiceProperty.FindPropertyRelative("_choiceText");
        choiceIdProp = choiceProperty.FindPropertyRelative("_choiceId");
        targetNodeProp = choiceProperty.FindPropertyRelative("_targetNode");
        targetNodeNameProp = choiceProperty.FindPropertyRelative("_targetNodeName");
        onChoiceSelectedProp = choiceProperty.FindPropertyRelative("_onChoiceSelected");

        // Debug: Log which properties were found (only if they're null - this is now expected during normal operation)
        if (choiceTextProp == null)
            Debug.LogWarning("DialogChoiceEditorWindow: Could not find _choiceText property");
        if (choiceIdProp == null)
            Debug.LogWarning("DialogChoiceEditorWindow: Could not find _choiceId property");
        if (targetNodeProp == null)
            Debug.LogWarning("DialogChoiceEditorWindow: Could not find _targetNode property");
        if (targetNodeNameProp == null)
            Debug.LogWarning("DialogChoiceEditorWindow: Could not find _targetNodeName property");
    }

    private void OnGUI()
    {
        if (serializedObject == null || choiceProperty == null)
        {
            EditorGUILayout.HelpBox("No choice selected. This window can be closed.", MessageType.Info);
            return;
        }

        // Refresh property references if any are null (can happen after recompile)
        if (choiceTextProp == null || choiceIdProp == null || targetNodeProp == null || targetNodeNameProp == null)
        {
            RefreshPropertyReferences();
        }

        // CRITICAL: Update at the start of every frame
        serializedObject.Update();

        // Note: We don't need to sync currentChoice since we work directly with properties

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        DrawHeader();
        EditorGUILayout.Space(10);

        DrawChoiceDisplay();
        EditorGUILayout.Space(10);

        DrawTargetConfiguration();
        EditorGUILayout.Space(10);

        DrawActionsSection();
        EditorGUILayout.Space(10);

        DrawEventsSection();
        EditorGUILayout.Space(10);

        DrawQuickActions();

        EditorGUILayout.EndScrollView();

        // CRITICAL: Apply changes at the end
        if (serializedObject.ApplyModifiedProperties())
        {
            EditorUtility.SetDirty(serializedObject.targetObject);

            // Force repaint to show changes immediately
            Repaint();
        }

        // Debug info (can be removed later)
        if (Event.current.type == EventType.Repaint)
        {
            // Check if properties are being found correctly
            if (choiceTextProp == null)
                Debug.LogWarning("DialogChoiceEditorWindow: choiceTextProp is null!");
        }
    }

    private void DrawHeader()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 14,
            alignment = TextAnchor.MiddleCenter
        };

        EditorGUILayout.LabelField("Dialog Choice Editor", headerStyle);

        // Get preview text from the property instead of the object
        string preview = "<No text set>";
        if (choiceTextProp != null && !string.IsNullOrEmpty(choiceTextProp.stringValue))
        {
            preview = choiceTextProp.stringValue;
        }

        GUIStyle previewStyle = new GUIStyle(EditorStyles.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Italic
        };

        EditorGUILayout.LabelField($"\"{preview}\"", previewStyle);

        EditorGUILayout.EndVertical();
    }

    private void DrawChoiceDisplay()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Choice Display", EditorStyles.boldLabel);

        EditorGUILayout.Space(5);

        // Choice Text with larger text area
        EditorGUILayout.LabelField("Choice Text", EditorStyles.miniBoldLabel);
        EditorGUILayout.HelpBox("This is the text shown to the player as a choice option.", MessageType.Info);

        if (choiceTextProp != null)
        {
            EditorGUI.BeginChangeCheck();

            string newText = EditorGUILayout.TextArea(
                choiceTextProp.stringValue ?? "",
                GUILayout.Height(60)
            );

            if (EditorGUI.EndChangeCheck())
            {
                choiceTextProp.stringValue = newText;
                // Mark as modified
                GUI.changed = true;
            }
        }
        else
        {
            EditorGUILayout.HelpBox("?? Could not find choice text property. Check Console for errors.", MessageType.Error);
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawTargetConfiguration()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Target Configuration", EditorStyles.boldLabel);

        EditorGUILayout.Space(5);

        // Target Node Name (for convergent paths)
        EditorGUILayout.LabelField("Named Target (Convergent Paths)", EditorStyles.miniBoldLabel);
        EditorGUILayout.HelpBox("Reference a node by name for convergent dialog paths (multiple choices leading to same node).", MessageType.Info);

        if (targetNodeNameProp != null)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Target Node Name");

            EditorGUI.BeginChangeCheck();
            string newTargetName = EditorGUILayout.TextField(targetNodeNameProp.stringValue ?? "");
            if (EditorGUI.EndChangeCheck())
            {
                targetNodeNameProp.stringValue = newTargetName;
                GUI.changed = true;
            }

            EditorGUILayout.EndHorizontal();

            // Show available named nodes
            if (parentTree != null && GUILayout.Button("Select from Named Nodes", GUILayout.Height(25)))
            {
                ShowNamedNodesMenu();
            }
        }
        else
        {
            EditorGUILayout.HelpBox("?? Could not find target node name property. Check Console for errors.", MessageType.Error);
        }

        EditorGUILayout.Space(10);

        // Warning if both targets are set
        if (!string.IsNullOrEmpty(targetNodeNameProp?.stringValue) && targetNodeProp?.managedReferenceValue != null)
        {
            EditorGUILayout.HelpBox("?? Both direct target and named target are set. Named target takes precedence.", MessageType.Warning);
        }

        // Direct Target Node
        EditorGUILayout.LabelField("Direct Target Node", EditorStyles.miniBoldLabel);
        EditorGUILayout.HelpBox("Inline target node (creates new branching path).", MessageType.Info);

        if (targetNodeProp != null)
        {
            // Show target preview
            showTargetPreview = EditorGUILayout.Foldout(showTargetPreview, "Target Node Preview", true);
            if (showTargetPreview)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(targetNodeProp, new GUIContent("Target Node"), true);
                EditorGUI.indentLevel--;
            }
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawActionsSection()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Actions & Identification", EditorStyles.boldLabel);

        EditorGUILayout.Space(5);

        // Choice ID
        EditorGUILayout.LabelField("Choice ID", EditorStyles.miniBoldLabel);
        EditorGUILayout.HelpBox("Unique identifier for this choice (used for save games, analytics, etc).", MessageType.Info);

        if (choiceIdProp != null)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Choice ID");

            EditorGUI.BeginChangeCheck();
            string newId = EditorGUILayout.TextField(choiceIdProp.stringValue ?? "");
            if (EditorGUI.EndChangeCheck())
            {
                choiceIdProp.stringValue = newId;
                GUI.changed = true;
            }

            EditorGUILayout.EndHorizontal();

            if (string.IsNullOrEmpty(choiceIdProp.stringValue))
            {
                if (GUILayout.Button("Generate Unique ID", GUILayout.Height(25)))
                {
                    choiceIdProp.stringValue = $"choice_{System.Guid.NewGuid().ToString().Substring(0, 8)}";
                    GUI.changed = true;
                }
            }
        }
        else
        {
            EditorGUILayout.HelpBox("?? Could not find choice ID property. Check Console for errors.", MessageType.Error);
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
            EditorGUILayout.HelpBox("Events triggered when this choice is selected by the player.", MessageType.Info);

            if (onChoiceSelectedProp != null)
            {
                EditorGUILayout.PropertyField(onChoiceSelectedProp, new GUIContent("On Choice Selected"), true);
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

        // Clear targets button
        GUI.backgroundColor = Color.yellow;
        if (GUILayout.Button("Clear All Targets", GUILayout.Height(30)))
        {
            if (EditorUtility.DisplayDialog("Clear Targets?",
                "This will remove both direct and named targets. Continue?", "Clear", "Cancel"))
            {
                if (targetNodeProp != null)
                    targetNodeProp.managedReferenceValue = null;
                if (targetNodeNameProp != null)
                    targetNodeNameProp.stringValue = "";
                GUI.changed = true;
            }
        }
        GUI.backgroundColor = Color.white;

        // Validate button
        if (GUILayout.Button("Validate Choice", GUILayout.Height(30)))
        {
            ValidateChoice();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(5);

        // Status display - use properties instead of object
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Choice Status", EditorStyles.miniBoldLabel);

        bool hasText = choiceTextProp != null && !string.IsNullOrEmpty(choiceTextProp.stringValue);
        bool hasTarget = (targetNodeProp != null && targetNodeProp.managedReferenceValue != null) ||
                        (targetNodeNameProp != null && !string.IsNullOrEmpty(targetNodeNameProp.stringValue));
        bool hasId = choiceIdProp != null && !string.IsNullOrEmpty(choiceIdProp.stringValue);
        bool isValid = hasText && hasTarget;

        DrawStatusLine("Has Text", hasText);
        DrawStatusLine("Has Valid Target", hasTarget);
        DrawStatusLine("Has ID", hasId);
        DrawStatusLine("Is Valid", isValid);

        EditorGUILayout.EndVertical();

        EditorGUILayout.EndVertical();
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
            EditorGUILayout.LabelField("?", statusStyle);
        }
        else
        {
            statusStyle.normal.textColor = Color.red;
            EditorGUILayout.LabelField("?", statusStyle);
        }

        EditorGUILayout.EndHorizontal();
    }

    private void ShowNamedNodesMenu()
    {
        if (parentTree == null) return;

        var namedNodes = parentTree.GetNamedNodes();

        if (namedNodes.Count == 0)
        {
            EditorUtility.DisplayDialog("No Named Nodes",
                "There are no named nodes in this dialog tree. Add a node name to nodes you want to reference.", "OK");
            return;
        }

        GenericMenu menu = new GenericMenu();

        foreach (var node in namedNodes)
        {
            string nodeName = node.NodeName;
            string preview = string.IsNullOrEmpty(node.DialogText)
                ? "<No text>"
                : (node.DialogText.Length > 40 ? node.DialogText.Substring(0, 40) + "..." : node.DialogText);

            menu.AddItem(
                new GUIContent($"{nodeName} - {preview}"),
                targetNodeNameProp.stringValue == nodeName,
                () =>
                {
                    targetNodeNameProp.stringValue = nodeName;
                    serializedObject.ApplyModifiedProperties();
                }
            );
        }

        menu.ShowAsContext();
    }

    private void ValidateChoice()
    {
        // Validate using properties instead of object
        bool hasText = choiceTextProp != null && !string.IsNullOrEmpty(choiceTextProp.stringValue);
        bool hasTarget = (targetNodeProp != null && targetNodeProp.managedReferenceValue != null) ||
                        (targetNodeNameProp != null && !string.IsNullOrEmpty(targetNodeNameProp.stringValue));

        bool isValid = hasText && hasTarget;

        if (isValid)
        {
            EditorUtility.DisplayDialog("Validation Success",
                "? This choice is valid and ready to use!", "OK");
        }
        else
        {
            string issues = "Issues found:\n";

            if (!hasText)
                issues += "• Missing choice text\n";

            if (!hasTarget)
                issues += "• No valid target (need either direct target or named target)\n";

            EditorUtility.DisplayDialog("Validation Failed", issues, "OK");
        }
    }
}
