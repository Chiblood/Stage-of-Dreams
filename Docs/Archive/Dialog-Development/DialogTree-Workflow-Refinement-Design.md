# DialogTree Workflow Refinement - Design Document

**Created**: January 2025  
**Status**: Design Phase  
**Branch**: Feature/DialogNode-Functionality

---

## Overview

This document outlines the design for refining the DialogTree editing workflow to separate **tree overview/navigation** (in Inspector) from **detailed node editing** (in dedicated windows).

### Goals
1. **Cleaner Inspector**: Read-only tree overview with navigation capabilities
2. **Focused Editing**: Dedicated windows for editing individual nodes
3. **Tree Navigation**: Easy traversal between parent/child/choice nodes
4. **Multiple Windows**: Support opening multiple node editors simultaneously

---

## Architecture Overview

### Component Roles

| Component | Role | Editing Capability |
|-----------|------|-------------------|
| **DialogTreeEditor** | Tree overview & navigation hub | Tree metadata only |
| **DialogNodeEditorWindow** | Individual node editing | Full node editing |
| **DialogChoiceEditorWindow** | Individual choice editing | Full choice editing |
| **DialogNodePropertyDrawer** | Minimal (deprecated for editing) | None - shows "Edit in Window" button |
| **DialogChoicePropertyDrawer** | Minimal (existing) | None - shows "Edit in Window" button |

---

## DialogTree Inspector Layout

### Section 1: Tree Information (Editable)
```
Tree Information
├─ Tree Name: [TextField]
├─ Description: [TextArea]
└─ Auto Update Node List: [Toggle]
```

### Section 2: Dialog Flow (Read-Only Preview)
```
Dialog Flow (Read-Only)
├─ Starting Node Preview:
│   ├─ Speaker: "Director"
│   ├─ First Line: "Welcome to the stage! Are you..."
│   └─ [Edit Starting Node] button
│
└─ All Nodes (Foldout):
    ├─ Node 1: [node_001] "Director: Welcome to..." [Edit]
    ├─ Node 2: [node_002] "Player: I'm ready!" [Edit]
    └─ Node 3: [node_003] "Director: Break a leg!" [Edit]
```

### Section 3: Quick Tree Actions
```
Quick Tree Actions
├─ [Validate Tree]
├─ [Print Structure]
└─ [Refresh Node List]
```

### Section 4: Quick Tree Builder
```
Quick Tree Builder (Foldout)
├─ Speaker Name: [TextField]
├─ Dialog Text: [TextArea]
├─ Is Player Speaking: [Toggle]
├─ [Create Starting Node]
└─ [Add Sequential Node]
```

### Section 5: Advanced Tools
```
Advanced Tools (Foldout)
├─ [Save As New Tree]
└─ [Clear All Nodes] (red/warning)
```

---

## DialogNodeEditorWindow Design

### Window Layout

```
┌─────────────────────────────────────────────────┐
│ Edit Dialog Node: [node_001]                   │
├─────────────────────────────────────────────────┤
│                                                 │
│ [Header with Node Preview]                     │
│                                                 │
├─────────────────────────────────────────────────┤
│ Node Identification                             │
│ ├─ Node Name/ID: [TextField] [Generate ID]     │
│ └─ Position in Tree: "2nd of 5 nodes"          │
├─────────────────────────────────────────────────┤
│ Dialog Content                                  │
│ ├─ Speaker Name: [TextField]                   │
│ ├─ Dialog Text: [TextArea - Large]             │
│ └─ Is Player Speaking: [Toggle]                │
├─────────────────────────────────────────────────┤
│ Flow Control                                    │
│ ├─ Auto Advance Delay: [FloatField]            │
│ └─ Has Choices: [ReadOnly]                     │
├─────────────────────────────────────────────────┤
│ Tree Navigation ◄─── NEW SECTION               │
│ ├─ Parent Nodes: (2)                           │
│ │   ├─ [Edit Parent: node_000] "Director: ..." │
│ │   └─ [Edit Parent: node_005] "Player: ..."   │
│ │                                               │
│ ├─ Child Node: (1)                              │
│ │   └─ [Edit Child: node_003] "Player: I'm..." │
│ │                                               │
│ └─ Choice Targets: (3)                          │
│     ├─ [Edit Choice 1: node_010] "I'm ready"   │
│     ├─ [Edit Choice 2: node_011] "I'm nervous" │
│     └─ [Edit Choice 3: node_012] "Not yet"     │
├─────────────────────────────────────────────────┤
│ Choices Management                              │
│ ├─ Choices: [List - Foldout]                   │
│ │   ├─ Choice 1: "I'm ready" [Edit in Window]  │
│ │   └─ Choice 2: "Not yet" [Edit in Window]    │
│ └─ [Add New Choice]                             │
├─────────────────────────────────────────────────┤
│ Events (Foldout)                                │
│ ├─ On Dialog Start: [UnityEvent]               │
│ └─ On Dialog End: [UnityEvent]                 │
├─────────────────────────────────────────────────┤
│ Quick Actions                                   │
│ ├─ [Validate Node] [Clear Child] [Clear All]   │
│ └─ Status: ✅ Valid                             │
└─────────────────────────────────────────────────┘
```

### Key Features

#### 1. Tree Navigation Section
- **Parent Nodes**: List of all parent nodes with "Edit Parent" buttons
  - If multiple parents (convergent node), show all
  - Button format: `[Edit Parent: {nodeId}] "{preview}"`
  
- **Child Node**: If auto-advance node exists
  - Single button: `[Edit Child: {nodeId}] "{preview}"`
  
- **Choice Targets**: List all choices with edit buttons
  - Button format: `[Edit Choice Target: {nodeId}] "{choiceText}"`
  - Opens the target node, not the choice editor

#### 2. Choice Management
- Collapsed list view of choices
- Each choice has "Edit in Window" button (opens DialogChoiceEditorWindow)
- "Add New Choice" button

#### 3. Window Behavior
- Each "Edit Parent/Child/Choice" button **opens a NEW window**
- Windows remain independent (can have 5+ open simultaneously)
- Each window tracks its own SerializedProperty reference
- Changes save immediately to the DialogTree asset

---

## Technical Implementation

### DialogNodeEditorWindow.cs

```csharp
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
        window.titleContent = new GUIContent("Edit Dialog Node");
        window.minSize = new Vector2(450, 600);
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
    }
    
    private void RefreshPropertyReferences()
    {
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
    
    private void OnGUI()
    {
        serializedObject.Update();
        
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        DrawHeader();
        DrawNodeIdentification();
        DrawDialogContent();
        DrawFlowControl();
        DrawTreeNavigation();      // ◄─── NEW
        DrawChoicesManagement();
        DrawEventsSection();
        DrawQuickActions();
        
        EditorGUILayout.EndScrollView();
        
        if (serializedObject.ApplyModifiedProperties())
        {
            EditorUtility.SetDirty(serializedObject.targetObject);
        }
    }
    
    /// <summary>
    /// NEW: Draw tree navigation buttons
    /// </summary>
    private void DrawTreeNavigation()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        
        showNavigation = EditorGUILayout.Foldout(showNavigation, "Tree Navigation", true, EditorStyles.foldoutHeader);
        
        if (showNavigation)
        {
            EditorGUILayout.Space(5);
            
            // Parent Nodes
            DrawParentNavigationButtons();
            
            EditorGUILayout.Space(5);
            
            // Child Node
            DrawChildNavigationButton();
            
            EditorGUILayout.Space(5);
            
            // Choice Targets
            DrawChoiceTargetNavigationButtons();
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
                
                if (GUILayout.Button($"Edit Parent: {buttonLabel}", GUILayout.Height(25)))
                {
                    OpenWindow(parentProp, parentTree);
                }
            }
        }
    }
    
    private void DrawChildNavigationButton()
    {
        EditorGUILayout.LabelField("Child Node (Auto-Advance)", EditorStyles.miniBoldLabel);
        
        if (childNodeProp == null || childNodeProp.managedReferenceValue == null)
        {
            EditorGUILayout.HelpBox("No child node (auto-advance not set)", MessageType.Info);
            return;
        }
        
        DialogNode childNode = childNodeProp.managedReferenceValue as DialogNode;
        string buttonLabel = GetNodeButtonLabel(childNode, "Child");
        
        if (GUILayout.Button($"Edit Child: {buttonLabel}", GUILayout.Height(25)))
        {
            OpenWindow(childNodeProp, parentTree);
        }
    }
    
    private void DrawChoiceTargetNavigationButtons()
    {
        EditorGUILayout.LabelField("Choice Targets", EditorStyles.miniBoldLabel);
        
        if (choicesProp == null || choicesProp.arraySize == 0)
        {
            EditorGUILayout.HelpBox("No choices available", MessageType.Info);
            return;
        }
        
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
                
                if (GUILayout.Button($"Edit Choice {i + 1} Target: {nodeLabel} ('{choiceText}')", GUILayout.Height(25)))
                {
                    OpenWindow(targetNodeProp, parentTree);
                }
            }
            else
            {
                EditorGUILayout.LabelField($"Choice {i + 1}: '{choiceText}' - No target set", EditorStyles.helpBox);
            }
        }
    }
    
    /// <summary>
    /// Helper: Get formatted button label for a node
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
}
```

---

## DialogTreeEditor Refactoring

### Changes Required

1. **Remove Inline Node Editing**: Remove `DrawDialogFlowSection()` property field drawing
2. **Add Read-Only Node List**: Show node IDs + previews with "Edit" buttons
3. **Remove Quick Builder Examples**: Keep only SaveAs and Clear functions
4. **Add Navigation Helpers**: Buttons to open starting node editor

### New Inspector Layout Code

```csharp
private void DrawDialogFlowSection()
{
    EditorGUILayout.LabelField("Dialog Flow (Read-Only)", EditorStyles.boldLabel);
    
    // Starting Node Preview
    if (dialogTree.GetStartingNode() == null)
    {
        EditorGUILayout.HelpBox("No starting node. Use Quick Tree Builder to create one.", MessageType.Info);
        return;
    }
    
    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
    
    // Starting node info
    var startNode = dialogTree.GetStartingNode();
    EditorGUILayout.LabelField("Starting Node:", EditorStyles.miniBoldLabel);
    EditorGUILayout.LabelField($"  Speaker: {startNode.CharacterName}");
    
    string preview = startNode.DialogText.Length > 50 
        ? startNode.DialogText.Substring(0, 50) + "..." 
        : startNode.DialogText;
    EditorGUILayout.LabelField($"  Text: \"{preview}\"");
    
    if (GUILayout.Button("Edit Starting Node", GUILayout.Height(30)))
    {
        SerializedProperty startingNodeProp = serializedObject.FindProperty("startingNode");
        DialogNodeEditorWindow.OpenWindow(startingNodeProp, dialogTree);
    }
    
    EditorGUILayout.EndVertical();
    
    EditorGUILayout.Space(10);
    
    // All Nodes List (Foldout)
    DrawAllNodesList();
}

private void DrawAllNodesList()
{
    var allNodes = dialogTree.GetAllNodes();
    
    if (allNodes.Count == 0) return;
    
    showAllNodes = EditorGUILayout.Foldout(showAllNodes, $"All Nodes ({allNodes.Count})", true);
    
    if (showAllNodes)
    {
        EditorGUI.indentLevel++;
        
        for (int i = 0; i < allNodes.Count; i++)
        {
            DialogNode node = allNodes[i];
            
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            
            // Node info
            string nodeId = !string.IsNullOrEmpty(node.NodeName) ? node.NodeName : $"<No ID>";
            string preview = !string.IsNullOrEmpty(node.DialogText)
                ? (node.DialogText.Length > 30 ? node.DialogText.Substring(0, 30) + "..." : node.DialogText)
                : "<No text>";
            
            EditorGUILayout.LabelField($"{i + 1}. [{nodeId}] \"{preview}\"", GUILayout.Width(300));
            
            // Edit button
            if (GUILayout.Button("Edit", EditorStyles.miniButton, GUILayout.Width(50)))
            {
                // Find the property for this node in allNodes list
                SerializedProperty allNodesProp = serializedObject.FindProperty("allNodes");
                SerializedProperty nodeProp = allNodesProp.GetArrayElementAtIndex(i);
                DialogNodeEditorWindow.OpenWindow(nodeProp, dialogTree);
            }
            
            EditorGUILayout.EndHorizontal();
        }
        
        EditorGUI.indentLevel--;
    }
}
```

---

## Multiple Windows Support

### Unity Editor Considerations

**Question**: Can Unity handle 5+ EditorWindows simultaneously?

**Answer**: ✅ **Yes, Unity can handle multiple EditorWindows without issue**

- Unity's EditorWindow system is designed for multiple concurrent windows
- Each window is an independent object with its own lifecycle
- Performance impact is minimal for 5-10 windows
- Users can dock/undock windows as needed

### Best Practices
1. **Window Management**: Each window is independent (no singleton pattern)
2. **Memory**: Windows clean up when closed (no memory leaks)
3. **Updates**: Each window tracks its own SerializedProperty
4. **Performance**: Only OnGUI() is called per frame (minimal overhead)

### Potential Issues & Solutions

| Issue | Solution |
|-------|----------|
| Too many windows cluttering workspace | User can dock windows together or use tabs |
| Confusion about which node is being edited | Window title shows node ID |
| Windows referencing deleted nodes | Check for null references in OnGUI() |
| Changes not syncing across windows | All windows use same SerializedObject reference |

---

## DialogNodePropertyDrawer Changes

### Current Behavior
- Fully inline editing with nested child nodes
- Complex layout with collapsible sections
- Height calculations for nested content

### New Behavior
- Minimal display: Node ID + preview text
- Single "Edit in Window" button
- No nested content (much simpler)

### New Layout
```
[Foldout] [node_001] "Director: Welcome to the stage!..."
  ├─ Node ID: node_001
  ├─ Speaker: Director
  ├─ Preview: "Welcome to the stage!..."
  └─ [Edit in Window] (button)
```

---

## Implementation Checklist

### Phase 1: DialogNodeEditorWindow Creation
- [ ] Create `DialogNodeEditorWindow.cs` based on DialogChoiceEditorWindow pattern
- [ ] Implement core sections: Header, Node ID, Dialog Content, Flow Control
- [ ] Add Tree Navigation section with Parent/Child/Choice buttons
- [ ] Implement navigation button click handlers (open new windows)
- [ ] Add Choices Management section
- [ ] Add Events section
- [ ] Add Quick Actions and validation

### Phase 2: DialogTreeEditor Refactoring
- [ ] Refactor `DrawDialogFlowSection()` to read-only preview
- [ ] Add `DrawAllNodesList()` with edit buttons
- [ ] Remove inline node editing (property fields)
- [ ] Update Quick Tree Builder (keep minimal functionality)
- [ ] Refactor Advanced Tools (remove examples, keep SaveAs and Clear)
- [ ] Add "Edit Starting Node" button

### Phase 3: DialogNodePropertyDrawer Simplification
- [ ] Simplify OnGUI() to show minimal preview
- [ ] Add "Edit in Window" button
- [ ] Remove complex nested layout code
- [ ] Simplify GetPropertyHeight() calculation
- [ ] Remove Create/Delete child node buttons (moved to window)

### Phase 4: Testing & Polish
- [ ] Test opening single node window
- [ ] Test navigation between parent/child nodes
- [ ] Test opening multiple windows (5+)
- [ ] Test editing and saving changes
- [ ] Test with complex trees (convergent paths)
- [ ] Update TROUBLESHOOTING.MD with new workflow
- [ ] Create user guide for new workflow

---

## Testing Scenarios

### Scenario 1: Basic Navigation
1. Create DialogTree with 5 sequential nodes
2. Open starting node in editor window
3. Click "Edit Child" to open next node
4. Verify all 5 nodes can be opened via navigation
5. Verify changes save correctly

### Scenario 2: Convergent Paths
1. Create tree with 3 choices leading to 1 convergent node
2. Open starting node
3. Navigate to Choice 1 target → should show 3 parents
4. Click each parent button → verify all 3 open correctly

### Scenario 3: Multiple Windows
1. Open starting node
2. Click "Edit Child" (2 windows open)
3. Click "Edit Choice 1 Target" from child (3 windows open)
4. Continue opening nodes until 5+ windows are open
5. Verify all windows remain functional
6. Make edits in different windows
7. Verify all changes save correctly

### Scenario 4: Read-Only Inspector
1. Select DialogTree asset
2. Verify Starting Node shows preview only (no inline editing)
3. Click "Edit Starting Node" → window opens
4. Verify "All Nodes" list shows all nodes
5. Click "Edit" on any node → window opens

---

## Migration Guide for Users

### Old Workflow
1. Select DialogTree asset
2. Expand Starting Node in inspector
3. Edit all properties inline
4. Create child nodes inline
5. Manage choices inline

### New Workflow
1. Select DialogTree asset
2. View read-only tree overview
3. Click "Edit Starting Node" to open editor window
4. Edit all properties in dedicated window
5. Navigate to child/parent nodes via buttons
6. Manage choices in window (or open choice editor)

### Benefits
- ✅ Cleaner inspector
- ✅ Focused editing environment
- ✅ Easier navigation of complex trees
- ✅ Multiple nodes can be edited simultaneously
- ✅ Less scrolling and collapsing sections

---

## Documentation Updates Required

### Files to Update
1. **TROUBLESHOOTING.MD**: Add new workflow section
2. **.github/copilot-instructions.md**: Update workflow description
3. **Docs/Class Hierarchy.md**: Update editor component descriptions
4. **Create New File**: `DialogTree-Editing-Workflow-Guide.md`

### New Documentation File: DialogTree-Editing-Workflow-Guide.md
```markdown
# DialogTree Editing Workflow Guide

## Overview
The DialogTree editing system uses dedicated windows for node editing with a read-only inspector for tree navigation.

## Creating a New Dialog Tree
1. Right-click in Project window
2. Create > Dialog System > Dialog Tree
3. Select the new tree asset
4. Click "Create Starting Node" in Quick Tree Builder

## Editing Nodes
1. Select DialogTree asset
2. Click "Edit Starting Node" button
3. Edit properties in dedicated window
4. Navigate to other nodes using Tree Navigation buttons

## Tree Navigation
- **Parent Nodes**: Click to edit any parent node
- **Child Node**: Click to edit auto-advance node
- **Choice Targets**: Click to edit nodes reached via choices

## Managing Choices
- Add choices in the Choices Management section
- Click "Edit in Window" to edit choice properties
- Click "Edit Choice Target" to edit the target node

## Tips
- You can have multiple node editor windows open
- Changes save automatically
- Use "Validate Tree" to check for issues
- Use "Print Structure" to see tree hierarchy
```

---

## Future Enhancements (Post-Implementation)

### Potential Additions
1. **Visual Tree Graph**: Node graph editor (Unity GraphView)
2. **Search Functionality**: Find nodes by ID or text
3. **Bookmarks**: Save frequently edited nodes
4. **History**: Recently edited nodes list
5. **Batch Operations**: Edit multiple nodes at once
6. **Undo/Redo**: Improve undo support for node editing

---

**End of Design Document**

**Next Steps**: 
1. Review design with Jack
2. Proceed to implementation (Phase 1)
3. Test and iterate
4. Update documentation
