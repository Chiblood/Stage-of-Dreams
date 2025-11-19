/* DialogNodePropertyDrawer.cs
 * Custom Property Drawer for DialogNode which will be called by the DialogTreeEditor.cs. 
 * Defines the drawer to be displayed within the Unity Editor's Inspector when a Dialog Tree is selected.
 */

using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(DialogNode))] // Specify the data type this drawer is for
public class DialogNodePropertyDrawer : PropertyDrawer
{
    private bool showEvents = false; // Allows for a Collapsible Events section
    
    // Override the OnGUI method
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        #region Drawing Setup
        EditorGUI.BeginProperty(position, label, property);

        // Calculate rects (rectangles) which are used for layout
        var foldoutRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        var contentRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, position.height - EditorGUIUtility.singleLineHeight);
        
        // Get node info for better label display
        var nodeNameProp = property.FindPropertyRelative("_nodeId");
        var speakerNameProp = property.FindPropertyRelative("_characterName");
        var dialogTextProp = property.FindPropertyRelative("_dialogText");
        
        string displayLabel = label.text;
        
        // Enhanced label with node name if available
        if (nodeNameProp != null && !string.IsNullOrEmpty(nodeNameProp.stringValue))
        {
            displayLabel = $"[{nodeNameProp.stringValue}] {displayLabel}";
        }
        else if (speakerNameProp != null && !string.IsNullOrEmpty(speakerNameProp.stringValue))
        {
            string previewText = "";
            if (dialogTextProp != null && !string.IsNullOrEmpty(dialogTextProp.stringValue))
            {
                previewText = dialogTextProp.stringValue.Length > 30 
                    ? dialogTextProp.stringValue.Substring(0, 30) + "..." 
                    : dialogTextProp.stringValue;
            }
            displayLabel = $"{speakerNameProp.stringValue}: {previewText}";
        }

        // Foldout for collapsing/expanding the Dialog Node details
        property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, displayLabel, true);
        
        if (property.isExpanded)
        {
            // Draw properties
            var isPlayerSpeakingProp = property.FindPropertyRelative("_isPlayerSpeaking");
            var autoAdvanceDelayProp = property.FindPropertyRelative("_autoAdvanceDelay");
            var choicesProp = property.FindPropertyRelative("_choices");
            var childNodeProp = property.FindPropertyRelative("_childNode");
            var parentNodesProp = property.FindPropertyRelative("_parentNodes");
            var onDialogStartProp = property.FindPropertyRelative("_onDialogStart");
            var onDialogEndProp = property.FindPropertyRelative("_onDialogEnd");
            

            float yPos = contentRect.y; // Start drawing below the foldout, this is where y position starts within contentRect
            float lineHeight = EditorGUIUtility.singleLineHeight + 2; // Line height with spacing
            #endregion

            #region Main Content
            // Dialog Content Header
            EditorGUI.LabelField(new Rect(contentRect.x, yPos, contentRect.width, EditorGUIUtility.singleLineHeight), 
                "Dialog Content", EditorStyles.boldLabel);
            yPos += lineHeight;

            EditorGUI.indentLevel++;

            // Node Name (for identification and convergent nodes)
            if (nodeNameProp != null)
            {
                EditorGUI.PropertyField(new Rect(contentRect.x, yPos, contentRect.width, EditorGUIUtility.singleLineHeight), 
                    nodeNameProp, new GUIContent("Node Name", "Unique identifier for this node - useful for convergent dialog paths"));
                yPos += lineHeight;
            }

            // Speaker Name
            if (speakerNameProp != null)
            {
                EditorGUI.PropertyField(new Rect(contentRect.x, yPos, contentRect.width, EditorGUIUtility.singleLineHeight), speakerNameProp);
                yPos += lineHeight;
            }
            
            // Dialog Text (TextArea)
            if (dialogTextProp != null)
            {
                float textAreaHeight = EditorGUI.GetPropertyHeight(dialogTextProp);
                EditorGUI.PropertyField(new Rect(contentRect.x, yPos, contentRect.width, textAreaHeight), dialogTextProp);
                yPos += textAreaHeight + 2;
            }
            
            // Is Player Speaking
            if (isPlayerSpeakingProp != null)
            {
                EditorGUI.PropertyField(new Rect(contentRect.x, yPos, contentRect.width, EditorGUIUtility.singleLineHeight), isPlayerSpeakingProp);
                yPos += lineHeight;
            }

            // Tree Structure Header
            EditorGUI.LabelField(new Rect(contentRect.x, yPos, contentRect.width, EditorGUIUtility.singleLineHeight),
                "Tree Structure", EditorStyles.boldLabel);
            yPos += lineHeight;

            // Parent Dialog (read-only, shows tree hierarchy)
            if (parentNodesProp != null)
            {
                GUI.enabled = false;
                float parentHeight = EditorGUI.GetPropertyHeight(parentNodesProp, true);
                EditorGUI.PropertyField(new Rect(contentRect.x, yPos, contentRect.width, parentHeight), 
                    parentNodesProp, new GUIContent("Parent Nodes", "Parent nodes in the dialog tree (automatically set)"), true);
                GUI.enabled = true;
                yPos += parentHeight + 2; // Add consistent spacing
            }

            // Events Header (Collapsible)
            EditorGUI.indentLevel++;
            var eventsRect = new Rect(contentRect.x, yPos, contentRect.width, EditorGUIUtility.singleLineHeight);
            showEvents = EditorGUI.Foldout(eventsRect, showEvents, "Events", true, EditorStyles.boldLabel);
            yPos += lineHeight;

            if (showEvents)
            {
                EditorGUI.indentLevel++;

                // Events
                if (onDialogStartProp != null)
                {
                    float eventHeight = EditorGUI.GetPropertyHeight(onDialogStartProp, true);
                    EditorGUI.PropertyField(new Rect(contentRect.x, yPos, contentRect.width, eventHeight), 
                        onDialogStartProp, new GUIContent("On Dialog Start", "Called when this dialog node begins"), true);
                    yPos += eventHeight + 2; // Make spacing consistent
                }

                if (onDialogEndProp != null)
                {
                    float eventHeight = EditorGUI.GetPropertyHeight(onDialogEndProp, true);
                    EditorGUI.PropertyField(new Rect(contentRect.x, yPos, contentRect.width, eventHeight), 
                        onDialogEndProp, new GUIContent("On Dialog End", "Called when this dialog node ends"), true);
                    yPos += eventHeight + 2; // Make spacing consistent
                }
                
                EditorGUI.indentLevel--;
            }

            EditorGUI.indentLevel--;

            // Flow Control Header
            EditorGUI.LabelField(new Rect(contentRect.x, yPos, contentRect.width, EditorGUIUtility.singleLineHeight), 
                "Flow Control", EditorStyles.boldLabel);
            yPos += lineHeight;
            
            // Auto Advance Delay
            if (autoAdvanceDelayProp != null)
            {
                EditorGUI.PropertyField(new Rect(contentRect.x, yPos, contentRect.width, EditorGUIUtility.singleLineHeight), 
                    autoAdvanceDelayProp, new GUIContent("Auto Advance Delay", "Seconds to wait before auto-advancing (0 = wait for input)"));
                yPos += lineHeight;
            }
            #endregion

            #region Tree Creation
            // Choices
            if (choicesProp != null)
            {
                float choicesHeight = EditorGUI.GetPropertyHeight(choicesProp, true);
                
                // Add header with button to manage choices
                EditorGUI.LabelField(new Rect(contentRect.x, yPos, contentRect.width * 0.7f, EditorGUIUtility.singleLineHeight), 
                    "Choices", EditorStyles.boldLabel);
                
                // Add "Edit All Choices" button
                var editAllButtonRect = new Rect(contentRect.x + contentRect.width * 0.7f, yPos, contentRect.width * 0.3f, EditorGUIUtility.singleLineHeight);
                if (GUI.Button(editAllButtonRect, "Edit in Windows", EditorStyles.miniButton))
                {
                    OpenAllChoicesInWindows(property);
                }
                yPos += EditorGUIUtility.singleLineHeight + 2;
                
                // Draw the choices list
                EditorGUI.PropertyField(new Rect(contentRect.x, yPos, contentRect.width, choicesHeight), 
                    choicesProp, GUIContent.none, true);
                yPos += choicesHeight + 2;
            }

            // Next Node with create/delete buttons
            if (childNodeProp != null)
            {
                float nextNodeHeight = EditorGUIUtility.singleLineHeight;
                var nextNodeRect = new Rect(
                    contentRect.x, 
                    yPos, 
                    contentRect.width * 0.13f, 
                    nextNodeHeight);
                var createButtonRect = new Rect(
                    contentRect.width * 0.50f, 
                    yPos, 
                    contentRect.width * 0.13f, 
                    nextNodeHeight);
                var deleteButtonRect = new Rect(
                    contentRect.width * 0.75f, 
                    yPos, 
                    contentRect.width * 0.13f, 
                    nextNodeHeight);

                // If childNode exists, draw it here recursively
                if (childNodeProp.managedReferenceValue != null)
                {
                    float nestedNodeHeight = EditorGUI.GetPropertyHeight(childNodeProp, true);
                    EditorGUI.PropertyField(new Rect(contentRect.x, yPos, contentRect.width, nestedNodeHeight), 
                        childNodeProp, new GUIContent("Child Node (Auto-Advance)"), true);
                    yPos += nestedNodeHeight + 2;
                }
                
                // Create button
                if (GUI.Button(createButtonRect, "Create Child", EditorStyles.miniButtonLeft))
                {
                    CreateChildNode(property);
                    yPos += nextNodeHeight + 2; // Move down to avoid overlap
                }
                
                // Delete button (only if child node exists)
                GUI.enabled = childNodeProp.managedReferenceValue != null;
                if (GUI.Button(deleteButtonRect, "Delete Child", EditorStyles.miniButtonRight))
                {
                    DeleteChildNode(property);
                    yPos -= nextNodeHeight + 2; // Move back up to avoid overlap
                }
                GUI.enabled = true;
                
                // Move yPos down AFTER drawing all elements on this line
                yPos += nextNodeHeight + 2;
            }
            #endregion

            yPos += 5; // Space between sections

            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }
    #region Node Creation and Deletion
    private void OpenAllChoicesInWindows(SerializedProperty nodeProperty)
    {
        var choicesProp = nodeProperty.FindPropertyRelative("_choices");
        if (choicesProp == null || choicesProp.arraySize == 0)
        {
            EditorUtility.DisplayDialog("No Choices", "This node has no choices to edit.", "OK");
            return;
        }
        
        // Find the parent DialogTree
        DialogTree parentTree = nodeProperty.serializedObject.targetObject as DialogTree;
        
        // Open a window for each choice
        for (int i = 0; i < choicesProp.arraySize; i++)
        {
            var choiceProp = choicesProp.GetArrayElementAtIndex(i);
            if (choiceProp != null)
            {
                DialogChoiceEditorWindow.OpenWindow(choiceProp, parentTree);
            }
        }
    }
    
    private void CreateChildNode(SerializedProperty nodeProperty)
    {
        var childNodeProp = nodeProperty.FindPropertyRelative("_childNode");
        if (childNodeProp != null)
        {
            // DialogNode is now a regular class, just use new
            var newNode = new DialogNode("Speaker", "Enter dialog text here", false);
            
            childNodeProp.managedReferenceValue = newNode;
            
            // Apply changes and force refresh
            nodeProperty.serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(nodeProperty.serializedObject.targetObject);
            
            // Force Unity to recalculate the PropertyDrawer height
            var targetObject = nodeProperty.serializedObject.targetObject;
            if (targetObject != null)
            {
                EditorUtility.SetDirty(targetObject);
                nodeProperty.serializedObject.UpdateIfRequiredOrScript();
                UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
            }
        }
    }

    private void DeleteChildNode(SerializedProperty nodeProperty)
    {
        var childNodeProp = nodeProperty.FindPropertyRelative("_childNode");
        if (childNodeProp != null)
        {
            childNodeProp.managedReferenceValue = null;
            
            // Apply changes and force refresh
            nodeProperty.serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(nodeProperty.serializedObject.targetObject);
            
            // Force Unity to recalculate the PropertyDrawer height
            var targetObject = nodeProperty.serializedObject.targetObject;
            if (targetObject != null)
            {
                EditorUtility.SetDirty(targetObject);
                nodeProperty.serializedObject.UpdateIfRequiredOrScript();
                UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
            }
        }
    }
    #endregion

    // Override GetPropertyHeight to calculate the height of the whole drawer so it can integrate with DialogTreeEditor
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!property.isExpanded)
            return EditorGUIUtility.singleLineHeight;
        
        float height = EditorGUIUtility.singleLineHeight; // Foldout line height

        // Add heights for all properties plus section headers
        var nodeNameProp = property.FindPropertyRelative("_nodeId");
        var speakerNameProp = property.FindPropertyRelative("_characterName");
        var dialogTextProp = property.FindPropertyRelative("_dialogText");
        var isPlayerSpeakingProp = property.FindPropertyRelative("_isPlayerSpeaking");
        var autoAdvanceDelayProp = property.FindPropertyRelative("_autoAdvanceDelay");
        var choicesProp = property.FindPropertyRelative("_choices");
        var childNodeProp = property.FindPropertyRelative("_childNode");
        var parentNodesProp = property.FindPropertyRelative("_parentNodes");
        var onDialogStartProp = property.FindPropertyRelative("_onDialogStart");
        var onDialogEndProp = property.FindPropertyRelative("_onDialogEnd");
        
        // Dialog Content section
        height += EditorGUIUtility.singleLineHeight + 2; // Header

        if (nodeNameProp != null)
            height += EditorGUIUtility.singleLineHeight + 2;

        if (speakerNameProp != null)
            height += EditorGUIUtility.singleLineHeight + 2;
        
        if (dialogTextProp != null)
            height += EditorGUI.GetPropertyHeight(dialogTextProp) + 2;
        
        if (isPlayerSpeakingProp != null)
            height += EditorGUIUtility.singleLineHeight + 2;

        // Tree Structure section
        height += EditorGUIUtility.singleLineHeight + 2; // Header

        if (parentNodesProp != null)
            height += EditorGUI.GetPropertyHeight(parentNodesProp, true) + 2;
        
        // Events section (collapsible)
        height += EditorGUIUtility.singleLineHeight + 2; // Header

        if (showEvents)
        {
            if (onDialogStartProp != null)
                height += EditorGUI.GetPropertyHeight(onDialogStartProp, true) + 2;

            if (onDialogEndProp != null)
                height += EditorGUI.GetPropertyHeight(onDialogEndProp, true) + 2;
        }
        
        height += 5; // Space between sections
        
        // Flow Control section
        height += EditorGUIUtility.singleLineHeight + 2; // Header
        
        if (autoAdvanceDelayProp != null)
            height += EditorGUIUtility.singleLineHeight + 2;

        // Choices section - account for header and button
        if (choicesProp != null)
        {
            height += EditorGUIUtility.singleLineHeight + 2; // Header with button
            height += EditorGUI.GetPropertyHeight(choicesProp, true) + 2;
        }

        // Child node section
        if (childNodeProp != null)
        {
            height += EditorGUIUtility.singleLineHeight + 2; // The Child Node field itself
            
            // CRITICAL: Add height for the nested DialogNode if it exists
            if (childNodeProp.managedReferenceValue != null)
            {
                float nestedNodeHeight = EditorGUI.GetPropertyHeight(childNodeProp, true);
                height += nestedNodeHeight + 2; // Height of the nested node
            }
        }
        
        return height;
    }
}