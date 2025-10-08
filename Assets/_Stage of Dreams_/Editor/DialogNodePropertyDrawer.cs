using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(DialogNode))]
public class DialogNodePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        
        // Calculate rects
        var foldoutRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        var contentRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, position.height - EditorGUIUtility.singleLineHeight);
        
        // Foldout
        property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label, true);
        
        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;
            
            // Draw properties
            var speakerNameProp = property.FindPropertyRelative("speakerName");
            var dialogTextProp = property.FindPropertyRelative("dialogText");
            var isPlayerSpeakingProp = property.FindPropertyRelative("isPlayerSpeaking");
            var autoAdvanceDelayProp = property.FindPropertyRelative("autoAdvanceDelay");
            var choicesProp = property.FindPropertyRelative("choices");
            var nextNodeProp = property.FindPropertyRelative("nextNode");
            var onDialogStartProp = property.FindPropertyRelative("onDialogStart");
            var onDialogEndProp = property.FindPropertyRelative("onDialogEnd");
            
            float yPos = contentRect.y;
            float lineHeight = EditorGUIUtility.singleLineHeight + 2;
            
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
            
            // Auto Advance Delay
            if (autoAdvanceDelayProp != null)
            {
                EditorGUI.PropertyField(new Rect(contentRect.x, yPos, contentRect.width, EditorGUIUtility.singleLineHeight), autoAdvanceDelayProp);
                yPos += lineHeight;
            }
            
            // Choices
            if (choicesProp != null)
            {
                float choicesHeight = EditorGUI.GetPropertyHeight(choicesProp, true);
                EditorGUI.PropertyField(new Rect(contentRect.x, yPos, contentRect.width, choicesHeight), choicesProp, true);
                yPos += choicesHeight + 2;
            }
            
            // Next Node
            if (nextNodeProp != null)
            {
                float nextNodeHeight = EditorGUI.GetPropertyHeight(nextNodeProp, true);
                EditorGUI.PropertyField(new Rect(contentRect.x, yPos, contentRect.width, nextNodeHeight), nextNodeProp, true);
                yPos += nextNodeHeight + 2;
            }
            
            // Events
            if (onDialogStartProp != null)
            {
                float eventHeight = EditorGUI.GetPropertyHeight(onDialogStartProp, true);
                EditorGUI.PropertyField(new Rect(contentRect.x, yPos, contentRect.width, eventHeight), onDialogStartProp, true);
                yPos += eventHeight + 2;
            }
            
            if (onDialogEndProp != null)
            {
                float eventHeight = EditorGUI.GetPropertyHeight(onDialogEndProp, true);
                EditorGUI.PropertyField(new Rect(contentRect.x, yPos, contentRect.width, eventHeight), onDialogEndProp, true);
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
        var speakerNameProp = property.FindPropertyRelative("speakerName");
        var dialogTextProp = property.FindPropertyRelative("dialogText");
        var isPlayerSpeakingProp = property.FindPropertyRelative("isPlayerSpeaking");
        var autoAdvanceDelayProp = property.FindPropertyRelative("autoAdvanceDelay");
        var choicesProp = property.FindPropertyRelative("choices");
        var nextNodeProp = property.FindPropertyRelative("nextNode");
        var onDialogStartProp = property.FindPropertyRelative("onDialogStart");
        var onDialogEndProp = property.FindPropertyRelative("onDialogEnd");
        
        if (speakerNameProp != null)
            height += EditorGUIUtility.singleLineHeight + 2;
        
        if (dialogTextProp != null)
            height += EditorGUI.GetPropertyHeight(dialogTextProp) + 2;
        
        if (isPlayerSpeakingProp != null)
            height += EditorGUIUtility.singleLineHeight + 2;
        
        if (autoAdvanceDelayProp != null)
            height += EditorGUIUtility.singleLineHeight + 2;
        
        if (choicesProp != null)
            height += EditorGUI.GetPropertyHeight(choicesProp, true) + 2;
        
        if (nextNodeProp != null)
            height += EditorGUI.GetPropertyHeight(nextNodeProp, true) + 2;
        
        if (onDialogStartProp != null)
            height += EditorGUI.GetPropertyHeight(onDialogStartProp, true) + 2;
        
        if (onDialogEndProp != null)
            height += EditorGUI.GetPropertyHeight(onDialogEndProp, true) + 2;
        
        return height;
    }
}