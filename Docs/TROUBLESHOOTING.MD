# Stage of Dreams - Troubleshooting Guide

**Last Updated**: January 2025  
**Project Version**: Feature/DialogNode-Functionality branch  
**Unity Version**: 6000.2.6f2  
**.NET Framework**: 4.7.1

---

## Table of Contents
1. [Dialog System Issues](#dialog-system-issues)
2. [UI Toolkit / Dialog Display Issues](#ui-toolkit--dialog-display-issues)
3. [Git & Version Control Issues](#git--version-control-issues)
4. [Unity Editor Issues](#unity-editor-issues)
5. [Build & Compilation Issues](#build--compilation-issues)
6. [Input System Issues](#input-system-issues)
7. [Common Error Messages](#common-error-messages)
8. [Quick Diagnostics](#quick-diagnostics)

---

## Dialog System Issues

### Issue: Dialog UI Not Appearing

**Symptoms**:
- Dialog is triggered but nothing appears on screen
- Console shows "DialogBox found!" but UI remains invisible
- Dialog navigation works (can hear/log events) but UI doesn't show

**Possible Causes & Solutions**:

1. **UI Elements Not Properly Connected**
   ```csharp
   // Check in Unity Inspector on DialogManager GameObject:
   // ? UIDocument component assigned
   // ? Visual Tree Asset assigned
   // ? Panel Settings assigned (in UIDocument)
   ```

2. **DialogBox CSS Display Issue**
   - **Check**: Open DialogManager in scene, select UIDocument
   - **Run**: DialogManager ? Context Menu ? "Force Show Dialog UI" (in Play Mode)
   - **Verify**: Console shows world bounds are non-zero

3. **Missing UI Toolkit Setup**
   - Verify `GivenDialogLabel` and `DialogBox` exist in UXML
   - Check button names: `DialogOption1Btn`, `DialogOption2Btn`, etc.
   - Run: DialogManager ? Context Menu ? "Test UI Elements"

4. **Z-Order / Sort Order Issue**
   - Check UIDocument ? Sort Order (try values: 0, 100, 1000)
   - Verify Panel Settings ? Scale Mode is set correctly

**Quick Fix**:
```csharp
// In Play Mode, select DialogManager GameObject
// Context Menu ? "Force Show Dialog UI"
// Check Console for diagnostic output
```

---

### Issue: Dialog Starts But Immediately Ends

**Symptoms**:
- `StartDialog()` returns true
- Console shows "Dialog started"
- Immediately followed by "Dialog ended"

**Possible Causes**:

1. **Empty Dialog Tree**
   - Check: NPC's DialogTree has a valid StartingNode
   - Check: StartingNode is not null in Inspector

2. **Auto-Advance with Zero Delay**
   - Check: Dialog nodes don't have auto-advance enabled with 0 delay
   - Inspect nodes in dialog tree asset

3. **Invalid Node References**
   - Check: All choice nodes have valid target nodes
   - Run: DialogTree ? Context Menu ? "Validate Tree"

**Debug Steps**:
```csharp
// Add to DialogNavigator to debug:
LogDebug($"Starting node: {currentNode?.DialogText ?? "NULL"}");
LogDebug($"Node valid: {currentNode?.IsValid() ?? false}");
LogDebug($"Has choices: {currentNode?.HasChoices ?? false}");
```

---

### Issue: Choices Not Appearing

**Symptoms**:
- Dialog displays correctly
- Node has choices defined
- No choice buttons appear

**Possible Causes**:

1. **Button Elements Not Found**
   - Check: Button names match UXML exactly
   - Expected: `DialogOption1Btn`, `DialogOption2Btn`, etc.
   - Run: DialogManager ? Context Menu ? "Test UI Elements"

2. **Choices Not Properly Added to Node**
   - Verify: `node.Choices.Count > 0`
   - Check: Choices are marked `[SerializeReference]` in DialogNode

3. **CSS Display None**
   - Buttons may be hidden by CSS
   - Check USS file for `.choice-visible` class

**Quick Test**:
```csharp
// Run in DialogManager Context Menu:
// "Test Show Dialog" - Should show 2 test choices
```

---

### Issue: Events Not Firing

**Symptoms**:
- Dialog events (OnDialogStarted, OnNodeDisplayed, etc.) not triggering
- Custom actions on choices don't execute

**Possible Causes**:

1. **No Event Subscribers**
   - Check: Something is subscribed to DialogManager events
   - Example: `DialogManager.Instance.OnDialogStarted += HandleDialogStarted;`

2. **Events Not Properly Configured**
   - Check: DialogNode has events added in Inspector
   - Verify: Events have ExecuteDelegate set

3. **Navigator Events Not Connected**
   - Check: DialogManager subscribed to navigator events in `InitializeNavigator()`

**Debug**:
```csharp
// Add to DialogManager.Start():
LogDebug($"Event subscribers: OnDialogStarted={OnDialogStarted?.GetInvocationList().Length ?? 0}");
```

---

### Issue: Player Can't Advance Dialog

**Symptoms**:
- Dialog appears but pressing interact key doesn't advance
- No choices showing, but manual advance doesn't work

**Possible Causes**:

1. **Input Action Not Configured**
   - Check: `interactAction` is not null in DialogManager
   - Verify: PlayerInput component assigned in Inspector

2. **Input Action Name Mismatch**
   - Check: `interactActionName` matches Input Action asset
   - Default: "Interact"

3. **InitializeInput() Failed**
   - Check Console for: "Failed to initialize input"
   - Verify: Input System package is installed

**Quick Fix**:
```csharp
// Check in DialogManager.InitializeInput():
// if (playerInput == null) playerInput = FindFirstObjectByType<PlayerInput>();
```

---

## UI Toolkit / Dialog Display Issues

### Issue: UI Elements Not Found

**Symptoms**:
- Console error: "DialogBox GroupBox not found in UI"
- Console error: "GivenDialogLabel Label not found in UI"

**Solution**:
1. Open UI Builder and verify element names:
   - GroupBox with name: `DialogBox`
   - Label with name: `GivenDialogLabel`
   - Buttons with names: `DialogOption1Btn` through `DialogOption5Btn`

2. Check element hierarchy in UXML matches expected structure

3. Run: DialogManager ? Context Menu ? "Test UI Elements"

**Expected UXML Structure**:
```xml
<ui:UXML>
  <GroupBox name="DialogBox" class="dialog-box">
    <Label name="GivenDialogLabel" />
    <Button name="DialogOption1Btn" />
    <Button name="DialogOption2Btn" />
    <!-- etc -->
  </GroupBox>
</ui:UXML>
```

---

### Issue: Dialog Box Off-Screen or Invisible

**Symptoms**:
- DialogBox.worldBound shows (0, 0, 0, 0)
- Elements exist but have no size/position

**Solution**:
1. Check Panel Settings:
   - Reference Resolution: Match your game resolution
   - Scale Mode: Try "Constant Pixel Size" or "Scale With Screen Size"

2. Check USS Positioning:
   ```css
   .dialog-box {
     position: absolute;
     bottom: 20px;
     left: 10%;
     right: 10%;
     width: 80%;
   }
   ```

3. Force Layout Update:
   - DialogManager ? Context Menu ? "Debug Dialog Layout"

---

### Issue: Text Not Displaying / Shows as "�" or garbled characters

**Symptoms**:
- Dialog text appears as placeholder or garbled

**Solution**:
1. Check TextMesh Pro is installed
2. Verify Label uses correct font asset
3. Check text encoding (should be UTF-8)

---

## Git & Version Control Issues

### Issue: Merge Conflicts in UserSettings

**Symptoms**:
- Git shows conflicts in `UserSettings/Layouts/` files
- Merge conflicts in `.dwlt` files

**Solution**:
```bash
# These files should NEVER be committed
# Discard them and use .gitignore:
git restore --theirs UserSettings/
git rm -r --cached UserSettings/
```

**Prevention**:
- Verify `UserSettings/` is in `.gitignore`
- Never commit user-specific settings

---

### Issue: Unity Project Files Keep Changing

**Symptoms**:
- `*.csproj` and `*.sln` files always show as modified
- `ProjectSettings/ProjectVersion.txt` changes frequently

**Solution**:
```bash
# These are auto-generated - ignore them:
git restore *.csproj *.sln
git restore ProjectSettings/ProjectVersion.txt

# Make sure they're in .gitignore
git check-ignore *.csproj  # Should show: .gitignore:39:*.csproj
```

---

### Issue: Large Repository Size

**Symptoms**:
- Git clone/push takes forever
- Repository over 500MB

**Cause**: `Library/` folder was committed

**Solution**:
```bash
# Remove from tracking (if already committed):
git rm -r --cached Library/
git commit -m "chore: Remove Library folder from tracking"

# Verify it's ignored:
git check-ignore Library/  # Should show: .gitignore:9:/[Ll]ibrary/
```

---

### Issue: .gitignore Not Working

**Symptoms**:
- Files still tracked even though in `.gitignore`
- Unity auto-generated files showing in `git status`

**Solution**:
```bash
# Files must be removed from tracking first:
git rm -r --cached <file-or-folder>
git commit -m "Remove ignored files from tracking"

# Then .gitignore will work going forward
```

**Note**: `.gitignore` only affects **untracked** files. Already-tracked files must be explicitly removed.

---

## Unity Editor Issues

### Issue: Unity Freezes on Dialog Trigger

**Symptoms**:
- Unity locks up when starting dialog
- Infinite loop in Console

**Possible Causes**:

1. **Circular Dialog Reference**
   - Check: Dialog tree doesn't reference itself
   - Check: No infinite auto-advance loops

2. **Missing Node References**
   - Causes recursive validation calls
   - Check: All choice targets are valid

**Debug**:
```csharp
// Add to DialogNavigator.StartDialog():
if (visitedNodes.Contains(node)) {
    LogError("Circular reference detected!");
    return false;
}
```

---

### Issue: ScriptableObject Changes Not Saving

**Symptoms**:
- Changes to DialogTree, DialogNode lost on play mode exit
- Assets revert to old state

**Solution**:
1. Always edit ScriptableObjects in **Edit Mode**, not Play Mode
2. Force save: `Ctrl+S` or `Assets ? Save All`
3. Check: Asset is not set to `DontSave` flag

---

### Issue: NullReferenceException on Dialog Start

**Symptoms**:
- `NullReferenceException` in DialogManager or DialogNavigator
- Dialog system crashes

**Debug Steps**:
```csharp
// Check initialization:
// DialogManager ? Context Menu ? "Print Current State"
// Look for:
// - Initialized: true
// - Navigator Active: false (before starting)
// - UIDocument/rootElement: not null
```

**Common Causes**:
1. DialogManager not initialized (Start() not called yet)
2. UIDocument component missing
3. Visual Tree Asset not assigned

---

## Build & Compilation Issues

### Issue: Build Fails - Missing References

**Symptoms**:
- "The type or namespace name 'X' could not be found"
- References work in Editor but fail in build

**Solution**:
1. Check Assembly Definitions if using them
2. Verify all scripts in correct folders
3. Check Player Settings ? Other Settings ? API Compatibility Level

---

### Issue: Dialog System Works in Editor But Not in Build

**Symptoms**:
- Dialog works in Unity Editor
- Fails in built game

**Possible Causes**:
1. **Code Stripping**
   - Check: Player Settings ? Managed Stripping Level
   - Try: Set to "Minimal" or "Low"

2. **Missing Assets in Build**
   - Check: All DialogTree assets in Resources or marked for build

---

### Issue: Cannot Edit DialogNodes or DialogChoices in Inspector

**Symptoms**:
- Dialog Tree shows nodes but fields are not editable
- PropertyDrawer shows empty/blank fields
- Console error: "managedReferenceValue is only available on fields with the [SerializeReference] attribute"
- Fields not being populated with data

**Root Cause**: 
DialogNode and DialogChoice were incorrectly inheriting from `ScriptableObject` while using `[SerializeReference]`. This is a fundamental architectural incompatibility:
- `[SerializeReference]` ? For inline serialization of **regular C# classes**
- `ScriptableObject` ? For separate asset files referenced via `[SerializeField]`

**Solution - ALREADY FIXED** (January 2025):

The issue has been resolved. DialogNode and DialogChoice are now regular `[System.Serializable]` classes, not ScriptableObjects.

**If you still experience issues:**

1. **Verify the class declarations:**
   ```csharp
   // Correct - regular serializable class
   [System.Serializable]
   public class DialogNode { ... }
   
   [System.Serializable]
   public class DialogChoice { ... }
   
   // WRONG - don't use ScriptableObject with [SerializeReference]
   public class DialogNode : ScriptableObject { ... } // ? BAD
   ```

2. **Check PropertyDrawer field names match backing fields:**
   - PropertyDrawers must use underscore-prefixed field names
   - Example: `_characterName`, `_dialogText`, `_choices`, etc.
   - Use `FindPropertyRelative("_fieldName")` not `FindPropertyRelative("fieldName")`

3. **Verify [SerializeReference] is used correctly:**
   ```csharp
   // In DialogTree.cs
   [SerializeReference] public DialogNode startingNode;  // ? Correct
   [SerializeReference] private List<DialogNode> allNodes;  // ? Correct
   
   // In DialogNode.cs  
   [SerializeReference] private DialogNode _childNode;  // ? Correct
   [SerializeReference] private List<DialogChoice> _choices;  // ? Correct
   ```

4. **Force Unity to refresh:**
   - Close Unity
   - Delete `Library/` folder
   - Reopen Unity (will reimport everything)

**Verification Steps:**
1. Select a DialogTree asset
2. Expand "Starting Node"
3. You should see editable fields:
   - Node Name
   - Speaker Name (Character Name)
   - Dialog Text (text area)
   - Is Player Speaking
   - Auto Advance Delay
   - Choices (expandable list)

**Prevention:**
- Never use `ScriptableObject` with `[SerializeReference]`
- Use regular classes with `[System.Serializable]` for inline data
- Use `new ClassName()` for instantiation, not `ScriptableObject.CreateInstance<T>()`

---

### Issue: Difficult to Edit DialogChoices in Nested Lists

**Symptoms**:
- Choices nested in DialogNode are hard to edit inline
- PropertyDrawer fields cramped and difficult to navigate
- Need better UX for editing individual choices

**Solution - DialogChoice Editor Window** (January 2025):

A dedicated **DialogChoiceEditorWindow** has been created for better editing experience.

**Status**: ✅ **WORKING** - Window opens and fields are editable via SerializedProperty

**How to Access**:

1. **Method 1: Single Choice**
   - Expand a DialogNode in the Inspector
   - Expand the Choices list
   - Click **"Edit in Window"** button next to any choice
   - Choice opens in dedicated editor window

2. **Method 2: All Choices at Once**
   - Expand a DialogNode
   - Click **"Edit in Windows"** button in Choices header
   - All choices open in separate windows
   - Edit multiple choices side-by-side

**Window Features:**
- Large text area for choice text (60px height)
- Named node selector with dropdown
- Target node preview (collapsible)
- Choice ID with auto-generate button
- Validation tools with status indicators (✅/❌)
- Quick actions (Clear Targets, Validate)
- Unity Events section

**Recent Fix (January 2025)**:
Window now properly handles DialogChoice objects in lists by working directly with SerializedProperty instead of trying to access managedReferenceValue on array elements.

---

### NEW WORKFLOW: DialogTree Editing with Window-Based System (January 2025)

**Status**: ✅ **IMPLEMENTED**

#### Overview
The DialogTree editing system has been refactored to use dedicated windows for node editing with a read-only inspector for tree navigation.

#### New Workflow

**Step 1: Select DialogTree Asset**
1. In Project window, select your DialogTree asset
2. Inspector shows read-only tree overview

**Step 2: View Tree Overview (Read-Only)**
- **Tree Information**: Edit tree name and description
- **Starting Node Preview**: Shows speaker + first line of text
- **All Nodes List**: Shows all nodes with [NodeID] and preview
- **Quick Tree Actions**: Validate, Print Structure, Refresh

**Step 3: Edit Nodes in Dedicated Windows**
1. Click **"Edit Starting Node"** button (or "Edit" next to any node)
2. DialogNodeEditorWindow opens
3. Edit all properties:
   - Node ID (with link to Node ID Generator)
   - Speaker Name
   - Dialog Text (large text area)
   - Is Player Speaking
   - Auto Advance Delay
   - Unity Events

**Step 4: Navigate Between Nodes**
- Use **Tree Navigation** section in node editor:
  - **Parent Nodes**: Click to edit any parent (convergent nodes show all parents)
  - **Child Node**: Click to edit auto-advance child
  - **Choice Targets**: Click to edit nodes reached via choices
- Each navigation button opens a NEW window
- Can have 5+ windows open simultaneously

**Step 5: Manage Choices**
- Add new choices with "+ Add New Choice" button
- View all choices in Choices Management section
- Click choice targets in Tree Navigation to edit target nodes

#### Benefits of New Workflow
✅ Cleaner inspector with less scrolling  
✅ Focused editing environment  
✅ Easier navigation of complex trees  
✅ Multiple nodes can be edited simultaneously  
✅ Better for large dialog trees  

#### Known Limitations
⚠ **Temporary**: NodeIdGenerator and DialogChoiceEditor integration buttons disabled  
  - **Workaround**: Use menu `Tools > Dialog System > Generate Node ID`  
  - **Workaround**: Edit choices via inspector for now  
  - **Fix**: Will be re-enabled after resolving circular assembly references

---

### Issue: Emojis Displaying as `?` in Unity Console

**Symptoms**:
- Debug logs show `?` instead of emojis
- Console output: `? Invalid | TreeName | Nodes: 0`
- Unicode characters not displaying correctly

**Cause**:
Unity's Console window uses a basic font that doesn't support Unicode emoji glyphs or extended characters.

**Solution**: Use ASCII-safe alternatives

#### Emoji Replacement Guide

Replace emojis with ASCII text equivalents:

| Emoji | ASCII | Example Usage |
|-------|-------|---------------|
| ✅ | `[OK]` | `Debug.Log("[OK] Valid");` |
| ❌ | `[!]` or `[ERROR]` | `Debug.LogError("[ERROR] Failed");` |
| ⚠️ | `[WARNING]` | `Debug.LogWarning("[WARNING] Issue");` |
| 🔧 | `[FIXED]` | `Debug.Log("[FIXED] Repaired");` |
| 📊 | `[#]` or `[STATS]` | `Debug.Log("[#] Count: 5");` |
| ✨ | `[CREATED]` | `Debug.Log("[CREATED] New item");` |
| 🎭 | `[DIALOG]` | `Debug.Log("[DIALOG] Started");` |
| 💡 | `[INFO]` | `Debug.Log("[INFO] Tip");` |
| 🚀 | `[START]` | `Debug.Log("[START] Loading");` |
| 🎉 | `[COMPLETE]` | `Debug.Log("[COMPLETE] Done");` |

#### Before & After Example

**Before (with emojis)**:
```csharp
Debug.Log($"✅ DialogTree validated: {nodeCount} nodes");
Debug.LogWarning($"⚠️ Missing speaker name");
Debug.LogError($"❌ Failed to load tree");
```

**After (ASCII-safe)**:
```csharp
Debug.Log($"[OK] DialogTree validated: {nodeCount} nodes");
Debug.LogWarning($"[WARNING] Missing speaker name");
Debug.LogError($"[ERROR] Failed to load tree");
```

#### Files to Check

Common files that may contain emojis:
- `Assets\_Stage of Dreams_\Editor\DialogTreeUtilities.cs` ✅ Fixed
- `Assets\_Stage of Dreams_\Editor\DialogNodeEditorWindow.cs`
- `Assets\_Stage of Dreams_\Scripts\Dialog\DialogNavigator.cs`
- Any custom editor scripts

#### Prevention

Add to `.github\copilot-instructions.md`:
```markdown
**Use ASCII-safe characters in Debug.Log statements (no emojis)**
```

---

