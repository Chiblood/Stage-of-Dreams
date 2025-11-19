# Dialog Choice Editor Window - Testing Guide

**Created**: January 2025  
**Status**: Ready for Testing  
**Feature**: DialogChoiceEditorWindow with enhanced editing capabilities

---

## Overview

The DialogChoiceEditorWindow provides a dedicated, spacious interface for editing individual DialogChoice properties that were previously difficult to edit in nested lists.

---

## How to Test

### Test 1: Open Single Choice Window

**Steps**:
1. Open Unity Editor
2. In Project window, select any DialogTree asset
3. In Inspector, expand "Starting Node"
4. Expand the "Choices" list (if node has choices)
5. Click **"Edit in Window"** button next to any choice

**Expected Result**:
- ? Window opens titled "Edit Dialog Choice"
- ? Window shows choice text in header
- ? All sections are visible

**If Window Doesn't Open**:
- Check Console for errors
- Verify choice property is not null
- Try clicking button again

---

### Test 2: Edit Choice Text

**Steps**:
1. Open choice editor window
2. Find "Choice Display" section
3. Click in the large text area (60px height)
4. Type or modify text
5. Click outside the text area

**Expected Result**:
- ? Text is editable
- ? Changes are visible immediately
- ? Header updates with new text
- ? No Console errors

**If Not Editable**:
- Check Console for: "Could not find _choiceText property"
- Verify DialogChoice.cs has `[SerializeField] private string _choiceText`
- Try closing and reopening window

---

### Test 3: Generate Choice ID

**Steps**:
1. Open choice editor window
2. Find "Actions & Identification" section
3. Clear the Choice ID field if it has content
4. Click **"Generate Unique ID"** button

**Expected Result**:
- ? New ID appears in format: `choice_XXXXXXXX` (8 characters)
- ? ID is unique (GUID-based)
- ? Button disappears when ID exists

**If Button Doesn't Work**:
- Check Console for errors
- Verify choiceIdProp is not null
- Try typing ID manually first

---

### Test 4: Select Named Target Node

**Steps**:
1. Ensure your DialogTree has at least one node with a "Node Name" set
2. Open choice editor window
3. Find "Target Configuration" section
4. Click **"Select from Named Nodes"** button

**Expected Result**:
- ? Dropdown menu appears
- ? Shows all named nodes in format: `NodeName - Dialog preview...`
- ? Clicking a node sets it as target
- ? Target Node Name field updates

**If No Nodes Appear**:
- Dialog displays: "There are no named nodes in this dialog tree"
- **Fix**: Add a "Node Name" to at least one node in the tree
- Then try again

---

### Test 5: Validate Choice

**Steps**:
1. Open choice editor window
2. Scroll to "Quick Actions" section
3. Click **"Validate Choice"** button

**Expected Result**:
- ? Dialog appears with validation status
- If valid: Shows "? This choice is valid and ready to use!"
- If invalid: Lists specific issues:
  - Missing choice text
  - No valid target
  - etc.

**Status Indicators Should Show**:
- ? (green) = Has Text
- ? (green) = Has Valid Target
- ? (green) = Has ID
- ? (green) = Is Valid

---

### Test 6: Edit Multiple Choices

**Steps**:
1. Select a DialogTree with a node that has multiple choices
2. Expand the node
3. Click **"Edit in Windows"** button in Choices header

**Expected Result**:
- ? Multiple windows open (one per choice)
- ? Each window shows different choice
- ? Windows are independent
- ? Can edit all simultaneously

**Performance Note**:
- Opening 5+ windows may slow Unity
- This is expected behavior

---

### Test 7: Clear Targets

**Steps**:
1. Open choice editor window with a choice that has a target
2. Click **"Clear All Targets"** button (yellow)
3. Confirm in dialog

**Expected Result**:
- ? Both Direct Target and Named Target are cleared
- ? Target Node Name field becomes empty
- ? Target Node Preview shows "None"
- ? Validation shows "? Has Valid Target"

---

### Test 8: Edit Events

**Steps**:
1. Open choice editor window
2. Find "Unity Events" section (collapsible)
3. Expand the section
4. Click "+" to add an event
5. Assign a method

**Expected Result**:
- ? Can add/remove events
- ? Can assign methods
- ? Events persist after closing window

---

### Test 9: Window Persistence

**Steps**:
1. Open choice editor window
2. Make changes to several fields
3. Close the window
4. Reopen the same choice

**Expected Result**:
- ? All changes were saved
- ? Fields show updated values
- ? No data loss

**If Changes Lost**:
- Check if `ApplyModifiedProperties()` is being called
- Verify `EditorUtility.SetDirty()` is called
- Check Console for serialization errors

---

### Test 10: Property Null Handling

**Steps**:
1. Open choice editor window
2. Check Console for warnings

**Expected Messages** (during initialization):
- "Could not find _choiceText property" = ? BAD
- "Could not find _choiceId property" = ? BAD
- No warnings = ? GOOD

**If Warnings Appear**:
1. Verify DialogChoice.cs has correct field names:
   ```csharp
   [SerializeField] private string _choiceText;
   [SerializeField] private string _choiceId;
   [SerializeField] private string _targetNodeName;
   [SerializeReference] private DialogNode _targetNode;
   ```

2. Check DialogChoiceEditorWindow.cs is using correct names in `FindPropertyRelative()`

---

## Known Issues & Workarounds

### Issue: "Failed to get DialogChoice from property!" Error

**Symptoms**:
- Console error: "DialogChoiceEditorWindow: Failed to get DialogChoice from property!"
- Window may still open and work correctly

**Status**: **FIXED** (January 2025)

**Explanation**:
This error occurred because DialogChoice objects in lists can't be accessed via `managedReferenceValue` on array elements. The window now works directly with `SerializedProperty` which handles list items correctly.

**If You Still See This Error**:
- It's now just a warning, not a fatal error
- The window should still function normally
- All editing is done via SerializedProperty, not the direct object reference
- If fields are editable, the window is working correctly

---

### Issue: Window Opens But Fields Are Empty

**Symptoms**:
- Window opens successfully
- All sections visible
- But text fields show nothing
- Can't type in fields

**Debug Steps**:
1. Check Console for property warnings
2. Close window
3. Delete `Library/` folder
4. Reopen Unity (forces reimport)
5. Try again

**If Still Not Working**:
```csharp
// Add to DialogChoiceEditorWindow.OnGUI():
Debug.Log($"ChoiceTextProp: {choiceTextProp?.stringValue ?? "NULL"}");
Debug.Log($"ChoiceIdProp: {choiceIdProp?.stringValue ?? "NULL"}");
```

---

### Issue: Changes Not Saving

**Symptoms**:
- Can edit fields
- Changes visible while window open
- But revert when window closes

**Solution**:
1. Check `serializedObject.ApplyModifiedProperties()` returns true
2. Verify `EditorUtility.SetDirty()` is called
3. Try manual save: `Ctrl+S`

---

### Issue: "Could Not Find Property" Warnings

**Cause**: Field names in DialogChoice don't match PropertyDrawer expectations

**Fix**:
1. Verify all DialogChoice fields use underscore prefix:
   - `_choiceText` ? not `choiceText` ?
   - `_choiceId` ? not `choiceId` ?
   - `_targetNode` ? not `targetNode` ?

2. If using auto-properties, they won't work with SerializedProperty
3. Must use backing fields with `[SerializeField]`

---

## Success Criteria

The feature is working correctly if:

? Window opens without errors  
? All text fields are editable  
? Changes save and persist  
? Validation works correctly  
? Named node dropdown populates  
? ID generation works  
? Multiple windows can open  
? No Console errors during normal use  

---

## Reporting Issues

If you encounter problems, provide:

1. **Console Errors**: Copy full error with stack trace
2. **Steps to Reproduce**: Exact steps that cause the issue
3. **Window State**: Screenshot of window when issue occurs
4. **DialogChoice Configuration**: How the choice is set up

**Debug Command**:
```csharp
// Run in Immediate Window (or add to code):
Debug.Log($"SerializedObject: {serializedObject != null}");
Debug.Log($"ChoiceProperty: {choiceProperty != null}");
Debug.Log($"Properties found: Text={choiceTextProp != null}, ID={choiceIdProp != null}");
```

---

## Next Steps After Testing

Once testing confirms the window works:

1. ? Mark issue as "RESOLVED" in TROUBLESHOOTING.MD
2. ? Update changelog with completion date
3. ? Remove debug logging from code
4. ? Create user documentation
5. ? Consider additional features:
   - Preview of target node inline
   - Copy/paste choice functionality
   - Batch operations on multiple choices

---

**End of Testing Guide**

For general troubleshooting, see: `Docs/TROUBLESHOOTING.MD`  
For project instructions, see: `.github/copilot-instructions.md`
