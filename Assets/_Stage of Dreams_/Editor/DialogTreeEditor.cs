using UnityEngine;
using UnityEditor;

/// <summary>
/// Enhanced custom editor for DialogTree that works with SerializeReference fields.
/// </summary>
[CustomEditor(typeof(DialogTree))]
public class DialogTreeEditor : Editor
{
    private DialogTree dialogTree;
    
    private void OnEnable()
    {
        dialogTree = (DialogTree)target;
    }
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        EditorGUILayout.HelpBox("? Enhanced DialogTree Editor with SerializeReference support!", MessageType.Info);
        
        // Draw the default inspector with property drawers
        DrawDefaultInspector();
        
        EditorGUILayout.Space(10);
        
        // Quick buttons for validation
        EditorGUILayout.LabelField("Quick Actions", EditorStyles.boldLabel);
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
            EditorUtility.SetDirty(dialogTree);
        }
        
        EditorGUILayout.EndHorizontal();
        
        serializedObject.ApplyModifiedProperties();
    }
}