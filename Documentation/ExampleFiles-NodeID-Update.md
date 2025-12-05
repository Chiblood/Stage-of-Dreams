# Example Files Updated - Node ID Naming Convention

**Updated**: January 2025  
**Purpose**: All example and factory files now demonstrate proper Node ID naming convention

---

## Files Updated

### 1. DialogTreeFactory.cs
**Location**: `Assets/_Stage of Dreams_/Scripts/Dialog/Examples and Guides/`

#### Updated Templates:

**Create Test Dialog Tree**
- ? Uses `npc_` context for NPC conversations
- ? Sequential numbering: `_01`, `_02`
- ? Choice branches properly named
- Example IDs:
  - `npc_stagemgr_greeting_01` - Opening greeting
  - `npc_stagemgr_nervous_01` - Nervous choice path
  - `npc_stagemgr_confident_01` - Confident choice path

**Create Director Audition Template**
- ? Demonstrates **branch ? choice ? convergence** pattern
- ? Named convergence points
- ? Uses named node references for convergent paths
- Example IDs:
  - `npc_branch_role_01` - Branch point
  - `npc_choice_hero_01` - Hero choice outcome
  - `npc_choice_villain_01` - Villain choice outcome
  - `npc_convergence_accepted_01` - Convergence point

**Create Rehearsal Feedback Template**
- ? Multiple convergence points demonstrated
- ? Proper use of named node references
- Example IDs:
  - `npc_director_feedback_01` - Initial feedback
  - `npc_choice_confident_01` - Confident self-assessment
  - `npc_convergence_notes_01` - Where all paths merge

**Create Stage Crew Banter Template**
- ? Shows multiple convergence points in single conversation
- ? Complex branching with proper IDs
- Example IDs:
  - `npc_lighttech_greeting_01` - Opening
  - `npc_convergence_story_01` - First convergence
  - `npc_convergence_solidarity_01` - Final convergence

---

### 2. DialogTreeExample.cs
**Location**: `Assets/_Stage of Dreams_/Scripts/Dialog/Examples and Guides/`

#### Updated Context Menu Methods:

**Create Simple Performance Dialog**
- ? Uses `act1_` context for story acts
- ? Multiple choice branches with proper IDs
- ? Sub-choices for practice options
- Example IDs:
  - `act1_director_ready_01` - Ready check
  - `act1_choice_ready_01` - Ready branch
  - `act1_choice_practice_01` - Practice branch
  - `act1_choice_scared_01` - Scared branch

**Create Linear Story Dialog**
- ? Sequential node array with proper IDs
- ? Ghost theater storyline
- ? Demonstrates linear progression
- Example IDs:
  - `act1_narrator_intro_01` - Opening narration
  - `act1_player_question_01` - First question
  - `act1_ghost_identity_01` - Ghost reveals self
  - `act1_ghost_request_01` - Final request

**Create Complex Branching Dialog**
- ? Nested branching structure
- ? Lead role sub-branches
- ? Custom action IDs for role granting
- Example IDs:
  - `npc_casting_greeting_01` - Casting director opening
  - `npc_choice_lead_01` - Lead role choice
  - `npc_choice_hero_01` - Hero sub-choice
  - `npc_casting_grantHero_01` - Role granted

---

### 3. CreateLinearDialog.cs
**Location**: `Assets/_Stage of Dreams_/Scripts/Dialog/Examples and Guides/`

#### Updated Menu Command:

**Create Linear Conversation**
- ? Assistant Director conversation
- ? Sequential node ID array
- ? Linear progression pattern
- Example IDs:
  - `npc_asst_challenge_01` - Initial challenge
  - `npc_player_identify_01` - Player identifies
  - `npc_asst_apologize_01` - Apology
  - `npc_asst_welcome_01` - Welcome message

---

## Naming Convention Patterns Demonstrated

### 1. Linear Sequences
```
context_speaker_action_01
context_speaker_action_02
context_speaker_action_03
```

**Examples from code**:
- `act1_narrator_intro_01`, `_02`, `_03`
- `npc_director_feedback_01`, `_02`

### 2. Branch ? Choice Pattern
```
context_branch_topic_01        ? Branch point
?? context_choice_optiona_01  ? Choice outcome A
?? context_choice_optionb_01  ? Choice outcome B
?? context_choice_optionc_01  ? Choice outcome C
```

**Examples from code**:
- `act1_director_ready_01` (branch)
  - `act1_choice_ready_01`
  - `act1_choice_practice_01`
  - `act1_choice_scared_01`

### 3. Convergent Paths
```
context_choice_path1_01 ????
context_choice_path2_01 ?????? context_convergence_result_01
context_choice_path3_01 ????
```

**Examples from code**:
- `npc_choice_hero_01` ???
- `npc_choice_villain_01` ??? `npc_convergence_accepted_01`
- `npc_choice_support_01` ?

### 4. NPC Conversations
```
npc_speaker_greeting_01  ? Initial greeting
npc_speaker_idle_01      ? Repeatable content
npc_speaker_advice_01    ? Special interactions
```

**Examples from code**:
- `npc_stagemgr_greeting_01`
- `npc_lighttech_greeting_01`
- `npc_casting_greeting_01`

### 5. Story Acts
```
act1_speaker_action_01
act2_speaker_action_01
dream1_speaker_action_01
```

**Examples from code**:
- `act1_director_ready_01`
- `act1_player_question_01`
- `act1_narrator_intro_01`

---

## What Each Example Teaches

### DialogTreeFactory

| Template | Teaches | Key Pattern |
|----------|---------|-------------|
| Test Dialog Tree | Basic NPC conversation | `npc_speaker_action_seq` |
| Director Audition | Convergent paths | `branch ? choice ? convergence` |
| Rehearsal Feedback | Multiple convergences | Named node references |
| Stage Crew Banter | Complex structure | Multiple convergence points |

### DialogTreeExample

| Method | Teaches | Key Pattern |
|--------|---------|-------------|
| Simple Performance | Story act dialog | `act1_speaker_action_seq` |
| Linear Story | Sequential dialog | Linear progression |
| Complex Branching | Nested choices | Nested branch structure |

### CreateLinearDialog

| Feature | Teaches | Key Pattern |
|---------|---------|-------------|
| Linear Conversation | Array-based creation | Sequential IDs in array |

---

## Usage in Unity

### Access Menu Items:
```
Dialog System ? Create Test Dialog Tree
Dialog System ? Create Director Audition Template
Dialog System ? Create Rehearsal Feedback Template
Dialog System ? Create Stage Crew Banter Template
Dialog System ? Create Linear Conversation
```

### Context Menu (on GameObject with DialogTreeExample):
```
Right-click ? Create Simple Performance Dialog
Right-click ? Create Linear Story Dialog
Right-click ? Create Complex Branching Dialog
```

---

## Learning Path

### 1. Start with Linear (Easiest)
- Use `CreateLinearDialog.cs`
- Learn sequential Node IDs
- Understand basic structure

### 2. Move to Simple Branching
- Use `DialogTreeFactory.CreateTestDialogTree()`
- Learn choice branches
- Practice `branch ? choice` pattern

### 3. Try Convergent Paths
- Use `DialogTreeFactory.CreateDirectorAuditionTemplate()`
- Learn named node references
- Understand convergence points

### 4. Master Complex Trees
- Use `DialogTreeExample.CreateComplexBranchingDialog()`
- Learn nested branching
- Practice multiple patterns together

---

## Best Practices Shown

### ? DO (as shown in examples):
- Use descriptive action keywords
- Keep sequences sequential (01, 02, 03)
- Name convergence points clearly
- Use context prefixes consistently
- Add comments explaining patterns

### ? DON'T (not shown in examples):
- Skip Node IDs (all examples have IDs)
- Use inconsistent naming
- Mix contexts inappropriately
- Forget to name convergence points
- Reuse sequence numbers

---

## Testing the Examples

### Quick Test:
1. **Open Unity**
2. **Go to**: `Dialog System` menu
3. **Click**: Any Create menu item
4. **Check**: Project window for new DialogTree asset
5. **Inspect**: Asset to see Node IDs in action

### Validation Test:
1. **Create an example tree**
2. **Select the DialogTree asset**
3. **Run**: Context Menu ? "Print Tree Structure"
4. **Check Console**: See Node IDs in structure output

### Learning Test:
1. **Create Test Dialog Tree**
2. **Open Node ID Generator**: `Tools ? Dialog System ? Generate Node ID`
3. **Try creating**: A similar ID pattern
4. **Compare**: Your ID with example IDs

---

## Code Snippets from Examples

### Creating with Node IDs:
```csharp
// From DialogTreeFactory.cs
DialogNode startNode = tree.CreateStartingNode(
    speakerName: "Stage Manager",
    dialogText: "Ah, you must be our new performer!",
    isPlayerSpeaking: false,
    nodeName: "npc_stagemgr_greeting_01"  // ? Proper Node ID
);
```

### Choice Branches with IDs:
```csharp
// From DialogTreeExample.cs
var readyNode = exampleTree.AddChoiceNode(
    startNode, 
    "I'm ready!", 
    "Director", 
    "Excellent! Remember...",
    false,
    "choice_ready_01",          // ? Choice ID
    "act1_choice_ready_01"      // ? Node ID
);
```

### Linear Array with Node IDs:
```csharp
// From CreateLinearDialog.cs
string[] nodeNames = new string[]
{
    "npc_asst_challenge_01",
    "npc_player_identify_01",
    "npc_asst_apologize_01"
};

tree.CreateLinearConversation(speakers, dialogTexts, isPlayerSpeaking, nodeNames);
```

### Convergent Paths:
```csharp
// From DialogTreeFactory.cs
DialogNode congratulations = tree.AddSequentialNode(
    parentNode: heroPath,
    speakerName: "Director",
    dialogText: "Wonderful! You clearly have talent.",
    nodeName: "npc_convergence_accepted_01"  // ? Named for convergence
);

// Other paths reference this node by name
tree.AddChoiceToNamedNode(villainPath, "Deliver monologue", "npc_convergence_accepted_01");
```

---

## Next Steps

### For Learners:
1. ? Run each example
2. ? Study the generated Node IDs
3. ? Open Node ID Generator tool
4. ? Try creating similar IDs
5. ? Read the naming convention guide

### For Developers:
1. ? Use examples as templates
2. ? Copy Node ID patterns
3. ? Modify for your stories
4. ? Maintain consistent naming
5. ? Validate with generator tool

---

## Related Documentation

- **Naming Convention**: `Docs/DialogNodeID-NamingConvention.md`
- **Generator Tool**: `Docs/DialogNodeID-GeneratorTool.md`
- **Class Hierarchy**: `Docs/Class Hierarchy.md`
- **Troubleshooting**: `Docs/TROUBLESHOOTING.MD`

---

**End of Example Files Update Summary**

All example files now demonstrate proper Node ID naming convention!
