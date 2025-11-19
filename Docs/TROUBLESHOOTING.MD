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

### Issue: Text Not Displaying / Shows as "???"

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

**Status**: ? **WORKING** - Window opens and fields are editable via SerializedProperty

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
- Validation tools with status indicators (?/?)
- Quick actions (Clear Targets, Validate)
- Unity Events section

**Recent Fix (January 2025)**:
Window now properly handles DialogChoice objects in lists by working directly with SerializedProperty instead of trying to access managedReferenceValue on array elements.

**If you experience issues:**

1. **Window opens but shows warning about "Failed to get DialogChoice":**
   - This is expected and harmless
   - The window works via SerializedProperty, not direct object access
   - If fields are editable, it's working correctly

2. **Verify property references are valid:**
   - Check that `choiceProperty` is not null
   - Ensure all `FindPropertyRelative()` calls use correct field names
   - Field names should be underscore-prefixed: `_choiceText`, `_choiceId`, etc.

3. **Force window refresh:**
   - Close all choice editor windows
   - Reopen from DialogNode inspector
   - Check Console for any errors

**Debug Steps**:
```csharp
// In DialogChoiceEditorWindow.OnGUI():
Debug.Log($"SerializedObject null? {serializedObject == null}");
Debug.Log($"ChoiceProperty null? {choiceProperty == null}");
Debug.Log($"ChoiceTextProp null? {choiceTextProp == null}");
```

**Related Files**:
- `DialogChoiceEditorWindow.cs` - Dedicated editor window
- `DialogChoicePropertyDrawer.cs` - Inline drawer with "Edit in Window" button
- `DialogNodePropertyDrawer.cs` - Has "Edit in Windows" button for all choices

**Known Limitations**:
- Window must remain open to save changes (changes saved on window close)
- Multiple windows can be resource-intensive
- Named node dropdown only shows nodes with `NodeName` set

---

### Issue: Dialog Trees Missing Starting Nodes

**Symptoms**:
- Console error: "Main dialog tree 'X' is invalid (no starting node)"
- Can't edit Dialog Trees in Inspector

**Solution**: Run `Tools ? Dialog System ? Fix All Dialog Trees` to automatically create missing starting nodes

**Related Tools**:
- `Tools ? Dialog System ? Validate All Dialog Trees` - Check all trees
- `Tools ? Dialog System ? List All Dialog Trees` - Show tree status

---

### Issue: SpotlightController Input Errors

**Symptoms**:
- Error: "You are trying to read Input using UnityEngine.Input class..."

**Solution**: Already fixed - test input is commented out. Use Context Menu commands instead.

---

## Input System Issues

### Issue: Input Actions Not Found

**Symptoms**:
- Console warning: "Interact action is null"
- Input doesn't work even though PlayerInput assigned

**Solution**:
```csharp
// In DialogManager.InitializeInput():
// Make sure Input Action Asset has action named "Interact"
// Check Input System package version (should be 1.7.0+)
```

---

### Issue: Multiple Input Actions Triggering

**Symptoms**:
- Dialog advances twice per key press
- Choice selected multiple times

**Solution**:
- Use `WasPressedThisFrame()` not `IsPressed()`
- Check for duplicate PlayerInput components in scene

---

## Common Error Messages

### "DialogNavigator not available - cannot start dialog"

**Cause**: DialogManager.navigator is null

**Fix**:
```csharp
// Check: DialogManager.InitializeNavigator() succeeded
// Verify: No exceptions in Console during initialization
```

---

### "NPC has no valid dialog content"

**Cause**: NPCContent.HasValidDialogContent() returns false

**Fix**:
1. Check NPC has at least one DialogTree assigned
2. Verify tree has valid StartingNode
3. Run: NPCContent ? Context Menu ? "Validate Content"

---

### "Dialog tree 'X' not found on NPC 'Y'"

**Cause**: Requesting specific tree that doesn't exist

**Fix**:
```csharp
// Check NPC has tree with exact name match
// Tree names are case-sensitive
```

---

### "Failed to get root visual element"

**Cause**: UIDocument not properly initialized

**Fix**:
1. Check UIDocument enabled in hierarchy
2. Verify GameObject with DialogManager is active
3. Check Visual Tree Asset is assigned

---

## Quick Diagnostics

### Dialog System Health Check
```csharp
// Run in Play Mode:
// DialogManager ? Context Menu ? "Validate Setup"
// Should show all ? checks passing
```

### UI Connection Test
```csharp
// Run in Play Mode:
// DialogManager ? Context Menu ? "Test Show Dialog"
// Should display test dialog with 2 choices
```

### Git Repository Health Check
```bash
# Check .gitignore is working:
git status --ignored | grep Library  # Should show "ignored"

# Check file count (should be < 1000 tracked files):
git ls-files | wc -l

# Check for large files (should be none > 10MB):
git ls-files | xargs du -h | sort -rh | head -20
```

### Unity Project Health
```
Check:
? Unity version matches team (6000.2.6f2)
? No errors in Console (before entering Play Mode)
? All scripts compile without warnings
? PlayerInput component in scene with correct Input Actions
```

---

## Getting Help

### Before Asking for Help, Provide:

1. **Unity Version**: Help ? About Unity
2. **Console Errors**: Copy full error with stack trace
3. **Steps to Reproduce**: Exact steps that cause the issue
4. **Expected vs Actual**: What should happen vs what does happen
5. **Relevant Code**: DialogManager, Navigator, or node setup

### Debug Logs to Include:
```csharp
// Run these Context Menu commands:
// DialogManager ? "Print Current State"
// DialogManager ? "Validate Setup"
// DialogNavigator ? "Print Current State" (if available)
```

### Screenshots to Include:
- Unity Inspector showing DialogManager setup
- Console window with errors
- UI Builder hierarchy (if UI issue)

---

## Known Issues & Workarounds

### Issue: TextMesh Pro Import Required

**Symptom**: First time opening project, TMP import window appears

**Solution**: 
1. Click "Import TMP Essentials"
2. Restart Unity
3. Not a bug - this is normal

---

### Issue: Line Ending Warnings (LF vs CRLF)

**Symptom**: Git warnings about line endings

**Solution**:
```bash
# Create/update .gitattributes in root:
* text=auto
*.cs text eol=lf
*.asset text eol=lf
*.unity text eol=lf
```

---

### Issue: Unity Cloud Build Not Supported

**Status**: Not tested/configured yet

**Workaround**: Use local builds only for now

---

## Change Log

| Date | Issue | Solution | Added By |
|------|-------|----------|----------|
| Jan 2025 | Dialog UI not appearing | Added force show debug tools | Jack |
| Jan 2025 | .gitignore not working | Moved to root, added cleanup | Jack |
| Jan 2025 | UserSettings causing conflicts | Added to .gitignore, documented | Jack |
| Jan 2025 | Dialog Trees Missing Starting Nodes | Added fix tool and validation tools | Jack |
| Jan 2025 | SpotlightController Input Errors | Commented out test input, added note | Jack |
| Jan 2025 | Cannot Edit DialogNodes/Choices | Fixed ScriptableObject vs [SerializeReference] conflict | Jack |
| Jan 2025 | Difficult to Edit Choices in Lists | Created DialogChoiceEditorWindow - RESOLVED | Jack |

---

## Contributing to This Guide

**Found a new issue?** Add it using this template:

```markdown
### Issue: [Brief Description]

**Symptoms**:
- What the user sees/experiences

**Possible Causes**:
1. First possible cause
2. Second possible cause

**Solution**:
[Step-by-step fix]

**Prevention**:
[How to avoid in future]
```

---

**End of Troubleshooting Guide**

For general setup instructions, see: `.github/copilot-instructions.md`  
For Git cleanup commands, see: `Docs/.gitignore-cleanup-commands.txt`  
For change review process, see: `Docs/.git-change-review-report.md`
