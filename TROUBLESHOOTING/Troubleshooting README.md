# Stage of Dreams - Troubleshooting Index

**Last Updated**: January 2025  
**Unity Version**: 6000.2.6f2  
**.NET Framework**: 4.7.1  
**Branch**: Feature/Minigames

Welcome to the troubleshooting documentation! This guide is organized by system for easy navigation.

---

## 📚 Troubleshooting Guides by System

### 🎭 [Dialog System](./Dialog-System.md)
Issues with:
- Dialog UI not appearing
- Dialogs not advancing
- Choices not showing
- Minigame input not working
- Custom actions not triggering
- Event system issues

**Quick diagnostic tools**: DialogManager context menus

---

### 🎨 [UI Toolkit](./UI-Toolkit.md)
Issues with:
- UI elements not found
- Elements not visible
- Layout problems
- Button clicks not working
- CSS/USS styling
- Performance / lag
- **⭐ Minigame UI not showing** (SOLVED!)

**Related components**: UIDocument, UXML, USS, VisualTreeAsset

---

### 🔧 [Git & Version Control](./Git-VersionControl.md)
Issues with:
- Submodule errors
- Merge conflicts (scenes/prefabs)
- Git LFS / large files
- Branch management
- Sync problems
- Best practices

**Tools**: Git commands, LFS, Unity Smart Merge

---

### 🎮 [Unity Editor](./Unity-Editor.md)
Issues with:
- Project won't open
- Compilation errors won't clear
- Inspector problems
- Performance / lag
- Asset import issues
- Play Mode problems
- UI Builder issues

**Tools**: Console, Profiler, Frame Debugger

---

## 🚀 Quick Start

**Most Common Issues**:

1. **Dialog not showing?** → [Dialog System: UI Not Appearing](./Dialog-System.md#dialog-ui-not-appearing)
2. **Typing not working in minigame?** → [Dialog System: Minigame Freezes](./Dialog-System.md#dialog-ui-freezes-when-minigame-starts)
3. **Minigame UI not visible?** → [UI Toolkit: Parent Container Hidden](./UI-Toolkit.md#1-parent-container-dialogbox-hidden-while-child-added--most-common-) ✅ SOLVED
4. **Git errors?** → [Git: Submodule Issues](./Git-VersionControl.md#submodule-issues)
5. **Unity won't open?** → [Unity: Won't Open Project](./Unity-Editor.md#unity-wont-open-project)
6. **UI elements not found?** → [UI Toolkit: Elements Not Found](./UI-Toolkit.md#ui-elements-not-found)

---

## 🔍 Diagnostic Workflow

### When Something Doesn't Work:

1. **Check Console First** (Ctrl+Shift+C)
   - Red errors = must fix
   - Yellow warnings = review
   - Filter by context: Dialog, UI, Input

2. **Use Context Menus**
   - Right-click DialogManager component
   - Run diagnostic commands
   - Check output in Console

3. **Verify Setup**
   - DialogManager → "Validate Setup"
   - Check all components assigned
   - Verify references not broken

4. **Search This Guide**
   - Use browser Find (Ctrl+F)
   - Search for error message
   - Search for symptom/behavior

5. **Check Related Files**
   - Scene file not corrupted
   - Prefabs have all components
   - ScriptableObjects have data

---

## 📝 Error Message Reference

### Common Errors & Where to Look:

| Error Message | Guide | Section |
|---------------|-------|---------|
| "DialogBox not found" | UI Toolkit | Elements Not Found |
| "NullReferenceException: DialogNavigator" | Dialog System | Navigation Issues |
| "No submodule mapping found" | Git | Submodule Issues |
| "Assembly has reference to non-existent" | Unity Editor | Compilation Errors |
| "Can't add script component" | Unity Editor | Inspector Issues |
| "UI Toolkit: VisualTreeAsset is null" | UI Toolkit | UI Not Visible |
| "Merge conflict in .unity file" | Git | Merge Conflicts |
| "Interact action not found" | Unity Editor | Input System Issues |
| "You are trying to read Input using UnityEngine.Input" | Dialog System | Input System Errors |
| "Minigame UI shown but nothing visible" ✅ | UI Toolkit | Parent Container Hidden (SOLVED) |

---

## 🛠️ Useful Tools & Commands

### Unity Context Menus

**DialogManager Component** (Right-click in Inspector):
- `Validate Setup` - Check configuration
- `Print Current State` - Debug current dialog
- `Test UI Elements` - Verify UI structure
- `Force Show Dialog UI` - Test visibility
- `Debug Dialog Layout` - Check positioning

**MinigameUIManager Component** (Right-click in Inspector):
- `Force Show Minigame UI (Test)` - Test minigame visibility
- `Print Current State` - Check minigame state

### Git Commands

```bash
# Check status
git status

# View history
git log --oneline --graph

# Discard changes
git checkout -- file.cs

# Undo commit
git reset --soft HEAD~1
```

### Unity Shortcuts

- `Ctrl+Shift+C` - Open Console
- `Ctrl+R` - Refresh Assets
- `Ctrl+S` - Save Project
- `F2` - Rename
- `Ctrl+D` - Duplicate

---

## 📞 Still Stuck?

If you've checked the relevant guide and still have issues:

1. **Check Documentation**:
   - `Docs/Class Hierarchy.md` - System architecture
   - `.github/copilot-instructions.md` - Project patterns

2. **Use Diagnostic Tools**:
   - Unity Console with Error Pause enabled
   - Unity Profiler for performance issues
   - Frame Debugger for rendering issues

3. **Common Fixes**:
   - Restart Unity
   - Reimport All Assets
   - Delete Library folder
   - Check for Unity updates

4. **Prevention**:
   - Save often (Ctrl+S)
   - Commit working changes to Git
   - Test after each feature
   - Use context menu diagnostics

---

## 🔄 Recently Solved Issues

**January 2025**:
- ✅ **Minigame UI invisible - Parent container hidden** → Keep DialogBox visible, only hide child elements (text/buttons) ⭐ NEW SOLUTION ⭐
- ✅ **Input.inputString error in minigames** → Switched to new Input System (Keyboard API) for character input
- ✅ **Minigame UI not visible** → Added USS styling requirements documentation
- ✅ Dialog UI freezing during minigames → Input routing fixed
- ✅ Partial classes not recognized → Files moved to same directory
- ✅ Minigame typing not working → Input handler added to DialogManager

**Solution Details**:
The minigame UI was invisible because `TransitionToMinigame()` was hiding the DialogBox (`display: none`) before adding the minigame container as a child. Since the parent was hidden, the child was also invisible. The fix: hide only the dialog elements (text, buttons) while keeping DialogBox visible to hold the minigame UI.

**Key Learning**: In UI Toolkit, if a parent has `display: none`, all children are invisible regardless of their own display settings. Always check parent visibility when debugging invisible UI elements.

---

**Navigation**: [Dialog](./Dialog-System.md) | [UI](./UI-Toolkit.md) | [Git](./Git-VersionControl.md) | [Unity](./Unity-Editor.md)
