# DialogTree Editing Workflow Guide

**Last Updated**: January 2025  
**Feature Branch**: Feature/DialogNode-Functionality  
**Status**: ✅ Implemented

---

## Overview

The DialogTree editing system uses **dedicated windows** for node editing with a **read-only inspector** for tree navigation and overview. This provides a cleaner, more focused editing experience for complex dialog trees.

---

## Quick Start

### Creating a New Dialog Tree

1. **Create Asset**
   - Right-click in Project window
   - Navigate to: `Create > Dialog System > Dialog Tree`
   - Name your tree (e.g., "Act1_Director_Intro")

2. **Initial Setup**
   - Select the new tree asset
   - Inspector shows empty tree message
   - Go to **Quick Tree Builder** section (foldout)
   - Fill in speaker name and dialog text
   - Click **"Create Starting Node"**

3. **View Your Tree**
   - Starting Node preview appears in **Dialog Flow** section
   - Shows speaker name and first line of text
   - Click **"✏ Edit Starting Node"** to open editor window

---

## Main Workflow

### 1. Inspector Overview (Read-Only)

When you select a DialogTree asset, the inspector shows:

#### Tree Information (Editable)
- **Tree Name**: Display name for the tree
- **Description**: Notes about this dialog tree
- **Auto Update Node List**: Automatically refresh node list

#### Dialog Flow (Read-Only)
- **Starting Node Preview**
  - Speaker name
  - First 50 characters of dialog text
  - **"✏ Edit Starting Node"** button → Opens editor window

- **All Nodes (Foldout)**
  - List of all nodes in tree
  - Format: `1. [node_id] Speaker: "Preview text..."`
  - Each node has **"Edit"** button → Opens editor window

#### Quick Tree Actions
- **Validate Tree**: Check for issues
- **Print Structure**: Console output of tree hierarchy  
- **Refresh Nodes**: Update node list

#### Quick Tree Builder (Foldout)
- **Create Starting Node**: Quick create with speaker/text
- **Add Sequential Node**: Append to end of tree
- Useful for rapid prototyping

#### Advanced Tools (Foldout)
- **Save As New Tree**: Duplicate tree as new asset
- **Clear All Nodes**: Remove all nodes (⚠ Cannot be undone!)

---

### 2. Editing Nodes in Dedicated Windows

#### Opening a Node Editor

**Method 1: From Inspector**
- Click **"✏ Edit Starting Node"** button
- OR click **"Edit"** next to any node in All Nodes list

**Method 2: From Another Node Window**
- Use **Tree Navigation** buttons (see below)

#### Node Editor Window Layout

```
┌──────────────────────────────────────────┐
│ Edit Dialog Node: [node_id]             │
├──────────────────────────────────────────┤
│ [Header with Node Preview]              │
├──────────────────────────────────────────┤
│ Node Identification                      │
│  ├─ Node Name/ID: [TextField]           │
│  └─ Position in Tree: "2 of 5 nodes"    │
├──────────────────────────────────────────┤
│ Dialog Content                           │
│  ├─ Speaker Name: [TextField]           │
│  ├─ Dialog Text: [Large TextArea]       │
│  └─ Is Player Speaking: [Toggle]        │
├──────────────────────────────────────────┤
│ Flow Control                             │
│  ├─ Auto Advance Delay: [Float]         │
│  └─ Has Choices: Yes/No                  │
├──────────────────────────────────────────┤
│ Tree Navigation (FOLDOUT)                │
│  ├─ Parent Nodes:                        │
│  │   └─ [⬆ Edit Parent: node_xxx]       │
│  ├─ Child Node:                          │
│  │   └─ [⬇ Edit Child: node_yyy]        │
│  └─ Choice Targets:                      │
│      ├─ [➜ Edit Choice 1 Target]        │
│      └─ [➜ Edit Choice 2 Target]        │
├──────────────────────────────────────────┤
│ Choices Management (FOLDOUT)             │
│  ├─ 1. "Choice text..."                 │
│  ├─ 2. "Another choice..."              │
│  └─ [+ Add New Choice]                   │
├──────────────────────────────────────────┤
│ Unity Events (FOLDOUT)                   │
│  ├─ On Dialog Start                      │
│  └─ On Dialog End                        │
├──────────────────────────────────────────┤
│ Quick Actions                            │
│  ├─ [Validate Node] [Clear Child]       │
│  └─ Status: ✅ Valid                     │
└──────────────────────────────────────────┘
```

#### Editing Node Properties

**Node Identification**
- **Node Name/ID**: Unique identifier
  - Useful for convergent paths (multiple paths leading to same node)
  - Tip: Use `Tools > Dialog System > Generate Node ID` for standardized IDs
- **Position in Tree**: Shows index (e.g., "3 of 10 nodes")

**Dialog Content**
- **Speaker Name**: Character speaking (e.g., "Director", "Player")
- **Dialog Text**: The actual dialog (use large text area)
- **Is Player Speaking**: Toggle for player dialog

**Flow Control**
- **Auto Advance Delay**: 
  - `0` = Wait for player input
  - `> 0` = Auto-advance after X seconds
- **Status Info**: Shows if node has choices or child

---

### 3. Navigating the Tree

The **Tree Navigation** section is the key feature of the new workflow!

#### Parent Nodes
- Lists ALL parent nodes (important for convergent dialog)
- Format: `[⬆ Edit Parent: node_id] "Preview..."`
- Click any parent button → Opens that parent in NEW window

**Example: Convergent Node**
```
Parent Nodes: (3)
  ⬆ Edit Parent 1: [act1_choice1] "I'm ready!"
  ⬆ Edit Parent 2: [act1_choice2] "I'm nervous"
  ⬆ Edit Parent 3: [act1_choice3] "Not sure yet"
```

#### Child Node (Auto-Advance)
- Shows if node has auto-advance child
- Format: `[⬇ Edit Child: node_id] "Preview..."`
- Click button → Opens child in NEW window

**Example:**
```
Child Node: (1)
  ⬇ Edit Child: [act1_director_intro_02] "Welcome to..."
```

#### Choice Targets
- Lists all nodes reached via player choices
- Format: `[➜ Edit Choice X Target: node_id]`
- Shows choice text for context
- Click button → Opens target node in NEW window

**Example:**
```
Choice Targets: (2)
  ➜ Edit Choice 1 Target: [act1_ready_path] 
     Choice: "I'm ready to perform!"
  ➜ Edit Choice 2 Target: [act1_nervous_path]
     Choice: "I'm too nervous..."
```

---

### 4. Managing Choices

#### Adding Choices

1. In node editor window, go to **Choices Management** section
2. Click **"+ Add New Choice"** button
3. New choice added with default text
4. Choice appears in list: `1. "Choice X"`

#### Viewing Choices

- Choices listed with numbers
- Shows choice text preview
- Currently: Edit via inspector (inline)
- Future: "Edit in Window" button (pending assembly fix)

#### Editing Choice Targets

**Method 1: Via Tree Navigation**
- In **Tree Navigation** > **Choice Targets**
- Click **"➜ Edit Choice X Target"** button
- Opens target node in new window

**Method 2: Set Named Target**
- In node editor, scroll to choice
- Set **Target Node Name** to reference existing node
- Useful for convergent paths

---

### 5. Working with Multiple Windows

#### Opening Multiple Windows

The new workflow supports **multiple node editor windows** simultaneously!

**Example Workflow:**
1. Open starting node (1 window)
2. Click "Edit Child" (2 windows)
3. Click "Edit Choice 1 Target" from child (3 windows)
4. Click "Edit Parent" from target (4 windows)
5. Continue as needed (5+ windows is fine!)

#### Benefits
- ✅ Compare nodes side-by-side
- ✅ Edit branching paths simultaneously
- ✅ No constant clicking back and forth
- ✅ Easier to maintain narrative consistency

#### Window Management Tips
- **Docking**: Drag windows to dock together as tabs
- **Floating**: Keep windows floating for multi-monitor setups
- **Closing**: Close windows when done (changes auto-save)

---

### 6. Validation and Quality Checks

#### Validate Individual Node

In node editor window:
1. Go to **Quick Actions** section
2. Click **"Validate Node"** button
3. Dialog shows validation results:
   - ✅ Has Speaker
   - ✅ Has Dialog Text
   - ✅ Has Valid Flow (not both choices AND child)
   - ⚠ No node ID (recommended but optional)

#### Validate Entire Tree

In DialogTree inspector:
1. Go to **Quick Tree Actions**
2. Click **"Validate Tree"** button
3. Console shows:
   - Total nodes count
   - Total choices count
   - End nodes count
   - Named nodes count
   - Convergent nodes count
   - Max tree depth
   - Warnings/errors

#### Print Tree Structure

In DialogTree inspector:
1. Go to **Quick Tree Actions**
2. Click **"Print Structure"** button
3. Console shows hierarchical tree:
   ```
   Dialog Tree: Act1_Director_Intro
     Director: Welcome to the stage!
       Choice: I'm ready!
         Player: Let's do this!
       Choice: I'm nervous
         Director: That's okay...
   ```

---

## Common Tasks

### Creating a Linear Dialog Sequence

1. **Create Starting Node**
   - Use Quick Tree Builder
   - Fill in speaker and text
   - Click "Create Starting Node"

2. **Add Sequential Nodes**
   - Use Quick Tree Builder
   - Fill in next speaker and text
   - Click "Add Sequential Node"
   - Repeat for each line

3. **Edit Nodes**
   - Click "Edit" next to any node
   - Use "Edit Child" to move through sequence

### Creating a Branching Dialog

1. **Create Starting Node**
   - As above

2. **Open Node Editor**
   - Click "Edit Starting Node"

3. **Add Choices**
   - Go to Choices Management
   - Click "+ Add New Choice" (twice or more)
   - Edit choice text in inspector

4. **Create Target Nodes for Each Choice**
   - Use Quick Tree Builder to create nodes
   - OR create inline via choice properties

5. **Edit Choice Paths**
   - Use Tree Navigation > Choice Targets
   - Click to edit each choice's target node

### Creating Convergent Paths

**Scenario**: Multiple choices lead to same outcome node

1. **Create Branch Point**
   - Node with multiple choices

2. **Create Outcome Node**
   - Give it a unique Node Name/ID (e.g., "act1_convergence_01")

3. **Set Named Targets**
   - In each choice, set "Target Node Name" to "act1_convergence_01"
   - OR use DialogTree context menu "Resolve Named References"

4. **Verify**
   - Open outcome node in editor
   - Check "Parent Nodes" section
   - Should see all incoming paths listed

---

## Tips & Best Practices

### Node Naming
- ✅ **DO**: Use standardized IDs: `act1_director_intro_01`
- ✅ **DO**: Use Node ID Generator tool (`Tools > Dialog System`)
- ❌ **DON'T**: Leave nodes without IDs (hard to reference later)

### Tree Organization
- ✅ **DO**: Use descriptive tree names: "Act1_DirectorIntro"
- ✅ **DO**: Add descriptions to trees
- ✅ **DO**: Validate frequently (catch issues early)

### Window Management
- ✅ **DO**: Close windows when done editing a section
- ✅ **DO**: Use docking for organized workspace
- ✅ **DO**: Open multiple windows to compare branches

### Dialog Content
- ✅ **DO**: Keep dialog concise (players read quickly)
- ✅ **DO**: Use auto-advance for rapid exchanges
- ✅ **DO**: Test dialog flow in Play Mode

---

## Troubleshooting

### "Can't Find Node ID Generator Button"
**Cause**: Temporarily disabled due to assembly references  
**Solution**: Use menu instead: `Tools > Dialog System > Generate Node ID`

### "Changes Not Saving"
**Cause**: Multiple windows editing same node  
**Solution**: 
- Only edit a node in ONE window at a time
- Close duplicate windows
- Check Console for errors

### "Tree Navigation Shows Wrong Nodes"
**Cause**: Node list not refreshed  
**Solution**:
- In DialogTree inspector: Quick Tree Actions > "Refresh Nodes"
- OR toggle "Auto Update Node List"

### "Can't Edit Choices in Window"
**Cause**: DialogChoiceEditor integration temporarily disabled  
**Solution**: Edit choices via inspector for now (pending assembly fix)

### "Too Many Windows Open"
**Not a Problem!** Unity can handle 5-10+ windows  
**If Cluttered**: Close windows or dock them as tabs

---

## Keyboard Shortcuts

| Action | Shortcut |
|--------|----------|
| Save All | `Ctrl+S` |
| Refresh Inspector | `F5` (with inspector focused) |
| Open Node ID Generator | `Tools > Dialog System > Generate Node ID` |

---

## Advanced Features

### Save As New Tree

Want to create variations of a dialog tree?

1. Select original DialogTree
2. Go to **Advanced Tools** section
3. Click **"Save As New Tree"**
4. Choose location and name
5. New tree created with all nodes duplicated

**Use Cases:**
- Create alternate dialog versions
- Backup before major changes
- Template for similar conversations

### Auto Update Node List

Toggle in **Tree Information** section

- **Enabled (default)**: Automatically refresh node list when tree changes
- **Disabled**: Manual refresh via "Refresh Nodes" button
- **Performance**: Disable for very large trees (100+ nodes)

---

## Migration from Old Workflow

### Old Workflow
1. Select DialogTree
2. Expand Starting Node in inspector
3. Edit all properties inline (lots of scrolling)
4. Expand child nodes recursively (nested and cramped)

### New Workflow
1. Select DialogTree
2. View read-only overview
3. Click "Edit" to open dedicated window
4. Navigate via Tree Navigation buttons

### Migration Steps
1. **No code changes needed** - existing trees work automatically
2. **New editing habit**: Click "Edit" instead of expanding in inspector
3. **Navigation**: Use navigation buttons instead of scrolling

---

## Known Issues & Limitations

### Temporary Limitations (Pending Fix)

1. **Node ID Generator Button**
   - Status: Disabled in node editor
   - Workaround: Use menu `Tools > Dialog System > Generate Node ID`
   - Reason: Circular assembly reference
   - ETA: Next update

2. **Dialog Choice Editor Button**
   - Status: Disabled in choices list
   - Workaround: Edit choices via inspector
   - Reason: Circular assembly reference
   - ETA: Next update

### By Design

1. **Inspector Shows Read-Only**
   - This is intentional! Use "Edit" buttons
   - Cleaner inspector layout

2. **PropertyDrawer Shows Minimal Info**
   - Only shows preview when Unity draws the property
   - Full editing in dedicated windows

---

## Getting Help

If you encounter issues:

1. **Check Console**: Look for error messages
2. **Validate Tree**: Use Quick Tree Actions > Validate Tree
3. **Check TROUBLESHOOTING.MD**: See `Docs/TROUBLESHOOTING.MD`
4. **GitHub Issues**: Report bugs on project repository

---

## Feedback & Suggestions

This workflow is new! If you have suggestions for improvements:

- Open an issue on GitHub
- Document your workflow pain points
- Suggest UI improvements

---

**End of Dialog Tree Editing Workflow Guide**

**Quick Links:**
- [Troubleshooting Guide](./TROUBLESHOOTING.MD)
- [Dialog Node ID Naming Convention](./DialogNodeID-NamingConvention.md)
- [Class Hierarchy](./Class Hierarchy.md)
