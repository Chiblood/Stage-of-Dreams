/*
 * Custom Property Drawer for DialogChoice
 * Enhanced with support for named node targeting and convergent dialog paths.
 * 
 * How to use in Unity:
 * 1. Place this script in an "Editor" folder within your Unity project.
 * 2. This drawer will automatically be used for any DialogChoice fields in the inspector.
 * 3. It provides a foldout view for each choice, displaying relevant properties.
 * 4. Expand the foldout to edit the choice text, target node, custom action ID, and events.
 * 5. Use the target node name field to create convergent dialog paths.
 */

using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(DialogChoice))]
public class DialogChoicePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        
        // Calculate rects
        var foldoutRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        var contentRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, position.height - EditorGUIUtility.singleLineHeight);
        
        // Get choice text for label
        var choiceTextProp = property.FindPropertyRelative("_choiceText");
        var targetNodeNameProp = property.FindPropertyRelative("_targetNodeName");
        
        string displayLabel = "Choice";
        if (choiceTextProp != null && !string.IsNullOrEmpty(choiceTextProp.stringValue))
        {
            displayLabel = choiceTextProp.stringValue;
            
            // Add target info to label for clarity
            if (targetNodeNameProp != null && !string.IsNullOrEmpty(targetNodeNameProp.stringValue))
            {
                displayLabel += $" → [{targetNodeNameProp.stringValue}]";
            }
        }
        
        // Foldout (full width - no button)
        property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, displayLabel, true);
        
        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;
            
            // Draw properties
            var targetNodeProp = property.FindPropertyRelative("_targetNode");
            var customActionIdProp = property.FindPropertyRelative("_choiceId");
            var onChoiceSelectedProp = property.FindPropertyRelative("_onChoiceSelected");
            
            float yPos = contentRect.y;
            float lineHeight = EditorGUIUtility.singleLineHeight + 2;
            
            // Choice Display Section
            EditorGUI.LabelField(new Rect(contentRect.x, yPos, contentRect.width, EditorGUIUtility.singleLineHeight), 
                "Choice Display", EditorStyles.boldLabel);
            yPos += lineHeight;
            
            // Choice Text
            if (choiceTextProp != null)
            {
                EditorGUI.PropertyField(new Rect(contentRect.x, yPos, contentRect.width, EditorGUIUtility.singleLineHeight), 
                    choiceTextProp, new GUIContent("Choice Text", "Text displayed to the player for this choice"));
                yPos += lineHeight;
            }
            
            // Target Section
            EditorGUI.LabelField(new Rect(contentRect.x, yPos, contentRect.width, EditorGUIUtility.singleLineHeight), 
                "Target Configuration", EditorStyles.boldLabel);
            yPos += lineHeight;
            
            // Target Node Name (for convergent paths)
            if (targetNodeNameProp != null)
            {
                EditorGUI.PropertyField(new Rect(contentRect.x, yPos, contentRect.width, EditorGUIUtility.singleLineHeight), 
                    targetNodeNameProp, new GUIContent("Target Node Name", "Name of the node to jump to (for convergent paths)"));
                yPos += lineHeight;
                
                // Show warning if both target node and target node name are set
                if (!string.IsNullOrEmpty(targetNodeNameProp.stringValue) && targetNodeProp?.managedReferenceValue != null)
                {
                    EditorGUI.HelpBox(new Rect(contentRect.x, yPos, contentRect.width, EditorGUIUtility.singleLineHeight * 2), 
                        "Both direct target and named target are set. Named target takes precedence.", MessageType.Warning);
                    yPos += EditorGUIUtility.singleLineHeight * 2 + 2;
                }
            }
            
            // Target Node (direct reference)
            if (targetNodeProp != null)
            {
                float targetNodeHeight = EditorGUI.GetPropertyHeight(targetNodeProp, true);
                EditorGUI.PropertyField(new Rect(contentRect.x, yPos, contentRect.width, targetNodeHeight), 
                    targetNodeProp, new GUIContent("Direct Target Node", "Direct reference to the target node"), true);
                yPos += targetNodeHeight + 2;
            }
            
            // Actions Section
            EditorGUI.LabelField(new Rect(contentRect.x, yPos, contentRect.width, EditorGUIUtility.singleLineHeight), 
                "Actions", EditorStyles.boldLabel);
            yPos += lineHeight;
            
            // Custom Action ID
            if (customActionIdProp != null)
            {
                EditorGUI.PropertyField(new Rect(contentRect.x, yPos, contentRect.width, EditorGUIUtility.singleLineHeight), 
                    customActionIdProp, new GUIContent("Choice ID", "Unique identifier for this choice"));
                yPos += lineHeight;
            }
            
            // Dialog Events Section (for future game systems)
            var choiceEventsProp = property.FindPropertyRelative("_choiceEvents");
            if (choiceEventsProp != null)
            {
                EditorGUI.LabelField(new Rect(contentRect.x, yPos, contentRect.width, EditorGUIUtility.singleLineHeight), 
                    "Dialog Events (Future: Crowd reactions, applause, etc.)", EditorStyles.miniBoldLabel);
                yPos += lineHeight;
                
                float dialogEventsHeight = EditorGUI.GetPropertyHeight(choiceEventsProp, true);
                EditorGUI.PropertyField(new Rect(contentRect.x, yPos, contentRect.width, dialogEventsHeight), 
                    choiceEventsProp, new GUIContent("Choice Events", "DialogEvents for game systems (crowd, applause, etc.)"), true);
                yPos += dialogEventsHeight + 2;
            }
            
            // Unity Events Section (legacy/simple cases)
            if (onChoiceSelectedProp != null)
            {
                EditorGUI.LabelField(new Rect(contentRect.x, yPos, contentRect.width, EditorGUIUtility.singleLineHeight), 
                    "Unity Events (Legacy)", EditorStyles.miniBoldLabel);
                yPos += lineHeight;
                
                // Fixed: Don't check objectReferenceValue for UnityEvent - it's not an object reference
                float eventHeight = EditorGUIUtility.singleLineHeight * 3; // Safe default height
                
                try
                {
                    eventHeight = EditorGUI.GetPropertyHeight(onChoiceSelectedProp, true);
                    EditorGUI.PropertyField(new Rect(contentRect.x, yPos, contentRect.width, eventHeight), 
                        onChoiceSelectedProp, new GUIContent("On Choice Selected", "Unity event triggered when this choice is selected"), true);
                }
                catch
                {
                    // If drawing fails, show a helpful message
                    EditorGUI.HelpBox(new Rect(contentRect.x, yPos, contentRect.width, EditorGUIUtility.singleLineHeight * 2), 
                        "UnityEvent not initialized. This is normal for new choices.\nSave the asset to initialize.", MessageType.Info);
                    eventHeight = EditorGUIUtility.singleLineHeight * 2;
                }
                
                yPos += eventHeight + 2;
            }
            
            // Add helpful buttons for target management
            EditorGUI.LabelField(new Rect(contentRect.x, yPos, contentRect.width, EditorGUIUtility.singleLineHeight), 
                "Target Management", EditorStyles.boldLabel);
            yPos += lineHeight;
            
            var clearButtonRect = new Rect(contentRect.x, yPos, contentRect.width, EditorGUIUtility.singleLineHeight);
            
            if (GUI.Button(clearButtonRect, "Clear All Targets"))
            {
                if (targetNodeProp != null)
                    targetNodeProp.managedReferenceValue = null;
                if (targetNodeNameProp != null)
                    targetNodeNameProp.stringValue = "";
                
                property.serializedObject.ApplyModifiedProperties();
            }
            
            EditorGUI.indentLevel--;
        }
        
        EditorGUI.EndProperty();
    }
    
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!property.isExpanded)
            return EditorGUIUtility.singleLineHeight;
        
        float height = EditorGUIUtility.singleLineHeight; // Foldout line
        
        // Add heights for all properties and section headers
        var choiceTextProp = property.FindPropertyRelative("_choiceText");
        var targetNodeProp = property.FindPropertyRelative("_targetNode");
        var targetNodeNameProp = property.FindPropertyRelative("_targetNodeName");
        var customActionIdProp = property.FindPropertyRelative("_choiceId");
        var onChoiceSelectedProp = property.FindPropertyRelative("_onChoiceSelected");
        
        // Choice Display section
        height += EditorGUIUtility.singleLineHeight + 2; // Header
        
        if (choiceTextProp != null)
            height += EditorGUIUtility.singleLineHeight + 2;
        
        // Target Configuration section
        height += EditorGUIUtility.singleLineHeight + 2; // Header
        
        if (targetNodeNameProp != null)
        {
            height += EditorGUIUtility.singleLineHeight + 2;
            
            // Warning box height if both targets are set
            if (!string.IsNullOrEmpty(targetNodeNameProp.stringValue) && targetNodeProp?.managedReferenceValue != null)
            {
                height += EditorGUIUtility.singleLineHeight * 2 + 2;
            }
        }
        
        if (targetNodeProp != null)
            height += EditorGUI.GetPropertyHeight(targetNodeProp, true) + 2;
        
        // Actions section
        height += EditorGUIUtility.singleLineHeight + 2; // Header
        
        if (customActionIdProp != null)
            height += EditorGUIUtility.singleLineHeight + 2;
        
        // Dialog Events section
        var choiceEventsProp = property.FindPropertyRelative("_choiceEvents");
        if (choiceEventsProp != null)
        {
            height += EditorGUIUtility.singleLineHeight + 2; // Header
            height += EditorGUI.GetPropertyHeight(choiceEventsProp, true) + 2;
        }
        
        // Unity Events section
        if (onChoiceSelectedProp != null)
        {
            height += EditorGUIUtility.singleLineHeight + 2; // Header
            
            try
            {
                height += EditorGUI.GetPropertyHeight(onChoiceSelectedProp, true) + 2;
            }
            catch
            {
                // If getting height fails (uninitialized UnityEvent), use safe default
                height += EditorGUIUtility.singleLineHeight * 3 + 2;
            }
        }
        
        // Target Management section
        height += EditorGUIUtility.singleLineHeight + 2; // Header
        height += EditorGUIUtility.singleLineHeight + 2; // Button
        
        return height;
    }
}
