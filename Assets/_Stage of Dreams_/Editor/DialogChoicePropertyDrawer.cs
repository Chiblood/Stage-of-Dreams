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
        var choiceTextProp = property.FindPropertyRelative("choiceText");
        string displayLabel = string.IsNullOrEmpty(choiceTextProp?.stringValue) ? "Choice" : choiceTextProp.stringValue;
        
        // Foldout
        property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, displayLabel, true);
        
        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;
            
            // Draw properties
            var targetNodeProp = property.FindPropertyRelative("targetNode");
            var customActionIdProp = property.FindPropertyRelative("customActionId");
            var onChoiceSelectedProp = property.FindPropertyRelative("onChoiceSelected");
            
            float yPos = contentRect.y;
            float lineHeight = EditorGUIUtility.singleLineHeight + 2;
            
            // Choice Text
            if (choiceTextProp != null)
            {
                EditorGUI.PropertyField(new Rect(contentRect.x, yPos, contentRect.width, EditorGUIUtility.singleLineHeight), choiceTextProp);
                yPos += lineHeight;
            }
            
            // Custom Action ID
            if (customActionIdProp != null)
            {
                EditorGUI.PropertyField(new Rect(contentRect.x, yPos, contentRect.width, EditorGUIUtility.singleLineHeight), customActionIdProp);
                yPos += lineHeight;
            }
            
            // Target Node
            if (targetNodeProp != null)
            {
                float targetNodeHeight = EditorGUI.GetPropertyHeight(targetNodeProp, true);
                EditorGUI.PropertyField(new Rect(contentRect.x, yPos, contentRect.width, targetNodeHeight), targetNodeProp, true);
                yPos += targetNodeHeight + 2;
            }
            
            // Events
            if (onChoiceSelectedProp != null)
            {
                float eventHeight = EditorGUI.GetPropertyHeight(onChoiceSelectedProp, true);
                EditorGUI.PropertyField(new Rect(contentRect.x, yPos, contentRect.width, eventHeight), onChoiceSelectedProp, true);
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
        
        // Add heights for all properties
        var choiceTextProp = property.FindPropertyRelative("choiceText");
        var targetNodeProp = property.FindPropertyRelative("targetNode");
        var customActionIdProp = property.FindPropertyRelative("customActionId");
        var onChoiceSelectedProp = property.FindPropertyRelative("onChoiceSelected");
        
        if (choiceTextProp != null)
            height += EditorGUIUtility.singleLineHeight + 2;
        
        if (customActionIdProp != null)
            height += EditorGUIUtility.singleLineHeight + 2;
        
        if (targetNodeProp != null)
            height += EditorGUI.GetPropertyHeight(targetNodeProp, true) + 2;
        
        if (onChoiceSelectedProp != null)
            height += EditorGUI.GetPropertyHeight(onChoiceSelectedProp, true) + 2;
        
        return height;
    }
}