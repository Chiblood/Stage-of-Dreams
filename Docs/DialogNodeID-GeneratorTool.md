# Node ID Generator Tool - User Guide

**Tool**: Node ID Generator Window  
**Access**: `Tools → Dialog System → Generate Node ID`  
**Version**: 1.0  
**Created**: January 2025

---

## Quick Start

### 1. Open the Tool
**Unity Menu Bar**: `Tools → Dialog System → Generate Node ID`

### 2. Fill in Components
- **Context**: Where/when (e.g., `act1`, `dream1`)
- **Speaker**: Who's speaking (e.g., `director`, `player`)
- **Action**: What's happening (e.g., `intro`, `branch`)
- **Sequence**: Number (01-99)

### 3. Copy the ID
Click **📋 Copy to Clipboard** button

### 4. Paste into Node
Paste the ID into the "Node Name" field in your DialogNode

---

## Features

### Auto-Complete Dropdowns
Each field has a dropdown showing:
- **Standard presets** (from naming convention)
- **Your existing patterns** (learned from your trees)

Simply click the dropdown or start typing!

### Real-Time Validation
The tool checks:
- ✅ All fields filled
- ✅ No duplicate IDs
- ✅ Proper format
- ✅ Length under 40 characters

**Color coding**:
- ✅ Green = Valid
- ❌ Red = Invalid (see warning message)

### Quick Actions

#### Reset
Clear all fields and start fresh

#### Load from Selection
1. Select a DialogTree asset in Project window
2. Click "Load from Selection"
3. Tool finds highest sequence number in that tree
4. Suggests next available number

#### Suggest Next
Searches ALL trees for matching pattern and suggests next sequence number

---

## Batch Mode

### Generate Multiple Sequential IDs

1. Click "Batch Mode" to expand
2. Set how many IDs you want (2-20)
3. Click "Generate X Sequential IDs"
4. All IDs copied to clipboard!

**Example Output**:
```
act1_director_intro_01
act1_director_intro_02
act1_director_intro_03
act1_director_intro_04
act1_director_intro_05
```

---

## Integration with Inspector

### Generate from DialogNode

When editing a DialogNode:
1. Find the "Node Name" field
2. Click **"Generate ID"** button next to it
3. Generator window opens
4. Generate and copy ID
5. Return to Inspector and paste

---

## Common Workflows

### Creating a Linear Sequence
```
1. Set: context=act1, speaker=director, action=intro, sequence=01
2. Copy: act1_director_intro_01
3. Increment sequence: 02
4. Copy: act1_director_intro_02
5. Continue...
```

**Pro Tip**: Use Batch Mode to generate 5-10 at once!

### Creating a Branch
```
Branch Point:
→ act1_branch_emotion_01

Choice Outcomes:
→ act1_choice_nervous_01
→ act1_choice_excited_01
→ act1_choice_ready_01

Convergence:
→ act1_convergence_prep_01
```

### NPC Conversations
```
First time:
→ npc_director_greeting_01

Repeatable:
→ npc_director_idle_01
→ npc_director_idle_02
→ npc_director_idle_03
```

---

## Tips & Tricks

### Use Dropdowns for Consistency
Instead of typing, use the dropdowns to ensure spelling consistency across your team.

### Increment Sequence with Buttons
Use the ⬆ and ⬇ buttons next to sequence instead of typing.

### Load from Selection for Context
When adding nodes to an existing conversation:
1. Select the DialogTree
2. Click "Load from Selection"
3. Tool auto-fills context and suggests next number

### Keep Tool Open While Working
The window stays open so you can generate multiple IDs without reopening.

### Copy Entire Batch
Use Batch Mode when planning out a whole conversation tree - paste into a text file for reference!

---

## Validation Messages

### ✅ Success Messages
- **"✅ Valid ID (X characters)"**  
  Everything looks good!

### ⚠️ Warning Messages
- **"All fields must be filled"**  
  Fill in all four components

- **"ID too long (X/40 characters)"**  
  Use shorter abbreviations

- **"Duplicate! ID already exists in tree: X"**  
  This ID is already used - increment sequence number

---

## Keyboard Shortcuts

While in the tool window:
- `Tab` - Move between fields
- `⬆` / `⬇` - Adjust sequence number
- `Ctrl+C` - Copy generated ID (when field selected)

---

## Examples

### Story Act Dialog
```
Context: act1
Speaker: director  
Action: intro
Sequence: 01
→ Result: act1_director_intro_01
```

### Performance Minigame
```
Context: perf
Speaker: narrator
Action: calm
Sequence: 01
→ Result: perf_narrator_calm_01
```

### Branching Choice
```
Context: act2
Speaker: choice
Action: nervous
Sequence: 01
→ Result: act2_choice_nervous_01
```

### Convergent Node
```
Context: act2
Speaker: convergence
Action: stage
Sequence: 01
→ Result: act2_convergence_stage_01
```

---

## Troubleshooting

### Duplicate ID Warning
**Problem**: Generated ID already exists  
**Solution**: Increment sequence number or click "Suggest Next"

### Dropdown Empty
**Problem**: No suggestions in dropdown  
**Solution**: Normal on first use - type manually or use presets

### Tool Won't Open
**Problem**: Menu item doesn't work  
**Solution**: 
1. Check `NodeIdGeneratorWindow.cs` is in `Editor` folder
2. Reimport script: Right-click → Reimport
3. Restart Unity

### IDs Not Validating
**Problem**: Tool always shows invalid  
**Solution**:
1. Check Console for errors
2. Verify DialogTree assets load correctly
3. Try "Reset" button

---

## Advanced Usage

### Custom Patterns
You can use custom context/speaker/action values:
- Type directly in fields
- Tool will clean input (lowercase, no special chars)
- New patterns will appear in dropdowns after use

### Learning from Existing Trees
The tool automatically learns patterns from your DialogTree assets:
1. Scans all DialogTree assets in project
2. Extracts unique patterns
3. Adds to dropdown suggestions
4. Updates each time you open the window

---

## Best Practices

### Planning Phase
1. Open tool
2. Generate IDs for entire conversation
3. Use Batch Mode
4. Paste into planning document
5. Create nodes and assign IDs

### Implementation Phase
1. Open tool alongside Inspector
2. Generate one ID at a time
3. Copy and paste immediately
4. Use "Suggest Next" between nodes

### Team Collaboration
1. Share naming convention document
2. Use tool to ensure consistency
3. Review generated IDs in code reviews
4. Keep tool updated with team patterns

---

## Related Documentation

- **Naming Convention Guide**: `Docs/DialogNodeID-NamingConvention.md`
- **Troubleshooting**: `Docs/TROUBLESHOOTING.MD`
- **Project Roadmap**: `Docs/Project_Roadmap.md`

---

## Quick Reference Card

```
🛠️ Access:        Tools → Dialog System → Generate Node ID
📋 Format:        [context]_[speaker]_[action]_[sequence]
📋 Copy:          Click "📋 Copy to Clipboard" button
📦 Batch Mode:    Generate 2-20 IDs at once
✅ Validation:    Real-time with color coding
❓ Help:          Click "❓ Open Full Documentation"
```

---

**End of User Guide**

For naming conventions, see: `Docs/DialogNodeID-NamingConvention.md`  
For troubleshooting, see: `Docs/TROUBLESHOOTING.MD`
