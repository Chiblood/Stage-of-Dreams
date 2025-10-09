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
        var nodeNameProp = property.FindPropertyRelative("nodeName");
        var speakerNameProp = property.FindPropertyRelative("speakerName");
        var dialogTextProp = property.FindPropertyRelative("dialogText");
        
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
            var isPlayerSpeakingProp = property.FindPropertyRelative("isPlayerSpeaking");
            var autoAdvanceDelayProp = property.FindPropertyRelative("autoAdvanceDelay");
            var choicesProp = property.FindPropertyRelative("choices");
            var nextNodeProp = property.FindPropertyRelative("nextNode");
            var parentDialogProp = property.FindPropertyRelative("parentDialog");
            var onDialogStartProp = property.FindPropertyRelative("onDialogStart");
            var onDialogEndProp = property.FindPropertyRelative("onDialogEnd");
            

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
            if (parentDialogProp != null)
            {
                GUI.enabled = false;
                float parentHeight = EditorGUI.GetPropertyHeight(parentDialogProp, true);
                EditorGUI.PropertyField(new Rect(contentRect.x, yPos, contentRect.width, parentHeight), 
                    parentDialogProp, new GUIContent("Parent Node", "Parent node in the dialog tree (automatically set)"), true);
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
                EditorGUI.PropertyField(new Rect(contentRect.x, yPos, contentRect.width, choicesHeight), 
                    choicesProp, new GUIContent("Choices", "Player choice options from this node"), true);
                yPos += choicesHeight + 2;
            }

            // Next Node with create/delete buttons
            if (nextNodeProp != null)
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

                // If nextNode exists, draw it here recursively
                if (nextNodeProp.managedReferenceValue != null)
                {
                    float nestedNodeHeight = EditorGUI.GetPropertyHeight(nextNodeProp, true);
                    EditorGUI.PropertyField(new Rect(contentRect.x, yPos, contentRect.width, nestedNodeHeight), 
                        nextNodeProp, new GUIContent(""), true);
                    yPos += nestedNodeHeight + 2;
                }
                // Next Node field
                //EditorGUI.PropertyField(nextNodeRect, nextNodeProp, new GUIContent("Next Node", "Node to auto-advance to"));
                
                // Create button
                if (GUI.Button(createButtonRect, "Create", EditorStyles.miniButtonLeft))
                {
                    CreateNextNode(property);
                    yPos += nextNodeHeight + 2; // Move down to avoid overlap
                }
                
                // Delete button (only if next node exists)
                GUI.enabled = nextNodeProp.managedReferenceValue != null;
                if (GUI.Button(deleteButtonRect, "Delete", EditorStyles.miniButtonRight))
                {
                    DeleteNextNode(property);
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
    private void CreateNextNode(SerializedProperty nodeProperty)
    {
        var nextNodeProp = nodeProperty.FindPropertyRelative("nextNode");
        if (nextNodeProp != null)
        {
            // Create a new DialogNode instance
            var newNode = new DialogNode("Speaker", "Enter dialog text here", false);
            nextNodeProp.managedReferenceValue = newNode;
            
            // Set parent reference
            var newNodeSerializedProp = nextNodeProp;
            if (newNodeSerializedProp != null)
            {
                var parentDialogProp = newNodeSerializedProp.FindPropertyRelative("parentDialog");
                if (parentDialogProp != null)
                {
                    parentDialogProp.managedReferenceValue = nodeProperty.managedReferenceValue;
                }
            }
            
            // Apply changes and force refresh
            nodeProperty.serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(nodeProperty.serializedObject.targetObject);
            
            // Force Unity to recalculate the PropertyDrawer height
            // This is crucial for recursive PropertyDrawer layouts
            var targetObject = nodeProperty.serializedObject.targetObject;
            if (targetObject != null)
            {
                // Force repaint of the inspector
                EditorUtility.SetDirty(targetObject);
                // Trigger a layout refresh
                nodeProperty.serializedObject.UpdateIfRequiredOrScript();
                // Request inspector repaint
                UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
            }
        }
    }

    private void DeleteNextNode(SerializedProperty nodeProperty)
    {
        var nextNodeProp = nodeProperty.FindPropertyRelative("nextNode");
        if (nextNodeProp != null)
        {
            nextNodeProp.managedReferenceValue = null;
            
            // Apply changes and force refresh
            nodeProperty.serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(nodeProperty.serializedObject.targetObject);
            
            // Force Unity to recalculate the PropertyDrawer height
            var targetObject = nodeProperty.serializedObject.targetObject;
            if (targetObject != null)
            {
                // Force repaint of the inspector
                EditorUtility.SetDirty(targetObject);
                // Trigger a layout refresh
                nodeProperty.serializedObject.UpdateIfRequiredOrScript();
                // Request inspector repaint
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
        var nodeNameProp = property.FindPropertyRelative("nodeName");
        var speakerNameProp = property.FindPropertyRelative("speakerName");
        var dialogTextProp = property.FindPropertyRelative("dialogText");
        var isPlayerSpeakingProp = property.FindPropertyRelative("isPlayerSpeaking");
        var autoAdvanceDelayProp = property.FindPropertyRelative("autoAdvanceDelay");
        var choicesProp = property.FindPropertyRelative("choices");
        var nextNodeProp = property.FindPropertyRelative("nextNode");
        var parentDialogProp = property.FindPropertyRelative("parentDialog");
        var onDialogStartProp = property.FindPropertyRelative("onDialogStart");
        var onDialogEndProp = property.FindPropertyRelative("onDialogEnd");
        
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

        if (parentDialogProp != null)
            height += EditorGUI.GetPropertyHeight(parentDialogProp, true) + 2;
        
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

        // Choices section  
        if (choicesProp != null)
            height += EditorGUI.GetPropertyHeight(choicesProp, true) + 2;

        // Next node section
        if (nextNodeProp != null)
        {
            height += EditorGUIUtility.singleLineHeight + 2; // The Next Node field itself
            
            // CRITICAL: Add height for the nested DialogNode if it exists
            if (nextNodeProp.managedReferenceValue != null)
            {
                float nestedNodeHeight = EditorGUI.GetPropertyHeight(nextNodeProp, true);
                height += nestedNodeHeight + 2; // Height of the nested node
            }
        }
        
        return height;
    }
}