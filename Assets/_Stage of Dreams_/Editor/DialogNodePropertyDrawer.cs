/* DialogNodePropertyDrawer.cs
 * DEPRECATED: Simplified Property Drawer for DialogNode
 * 
 * This PropertyDrawer is now simplified since detailed editing is done in DialogNodeEditorWindow.
 * It only shows minimal preview information when Unity's serialization system draws the property.
 * 
 * Users should use DialogTreeEditor inspector with "Edit" buttons to open nodes in dedicated windows.
 */

using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(DialogNode))]
public class DialogNodePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Get node info for better label display
        var nodeNameProp = property.FindPropertyRelative("_nodeId");
        var speakerNameProp = property.FindPropertyRelative("_characterName");
        var dialogTextProp = property.FindPropertyRelative("_dialogText");
        
        string displayLabel = label.text;
        
        // Enhanced label with node name if available
        if (nodeNameProp != null && !string.IsNullOrEmpty(nodeNameProp.stringValue))
        {
            displayLabel = $"[{nodeNameProp.stringValue}]";
        }
        else if (speakerNameProp != null && !string.IsNullOrEmpty(speakerNameProp.stringValue))
        {
            string previewText = "";
            if (dialogTextProp != null && !string.IsNullOrEmpty(dialogTextProp.stringValue))
            {
                previewText = dialogTextProp.stringValue.Length > 20 
                    ? dialogTextProp.stringValue.Substring(0, 20) + "..." 
                    : dialogTextProp.stringValue;
            }
            displayLabel = $"{speakerNameProp.stringValue}: {previewText}";
        }

        // Simple foldout with minimal info
        var foldoutRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, displayLabel, true);
        
        if (property.isExpanded)
        {
            float yPos = position.y + EditorGUIUtility.singleLineHeight + 2;
            float lineHeight = EditorGUIUtility.singleLineHeight + 2;
            
            EditorGUI.indentLevel++;
            
            // Show minimal read-only info
            if (nodeNameProp != null)
            {
                EditorGUI.LabelField(new Rect(position.x, yPos, position.width, EditorGUIUtility.singleLineHeight), 
                    "Node ID:", nodeNameProp.stringValue);
                yPos += lineHeight;
            }
            
            if (speakerNameProp != null)
            {
                EditorGUI.LabelField(new Rect(position.x, yPos, position.width, EditorGUIUtility.singleLineHeight), 
                    "Speaker:", speakerNameProp.stringValue);
                yPos += lineHeight;
            }
            
            if (dialogTextProp != null)
            {
                string preview = string.IsNullOrEmpty(dialogTextProp.stringValue) ? "<No text>" :
                    (dialogTextProp.stringValue.Length > 100 
                        ? dialogTextProp.stringValue.Substring(0, 100) + "..." 
                        : dialogTextProp.stringValue);
                
                EditorGUI.LabelField(new Rect(position.x, yPos, position.width, EditorGUIUtility.singleLineHeight * 2), 
                    $"Preview: \"{preview}\"", EditorStyles.wordWrappedLabel);
                yPos += lineHeight * 2;
            }
            
            // Help message
            var helpRect = new Rect(position.x, yPos, position.width, EditorGUIUtility.singleLineHeight * 2);
            EditorGUI.HelpBox(helpRect, "Use 'Edit' button in DialogTree inspector to edit this node in a dedicated window.", MessageType.Info);
            
            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!property.isExpanded)
            return EditorGUIUtility.singleLineHeight;
        
        // Height when expanded: foldout + ID + Speaker + Preview (2 lines) + Help (2 lines) + spacing
        return EditorGUIUtility.singleLineHeight * 7 + 10;
    }
}
