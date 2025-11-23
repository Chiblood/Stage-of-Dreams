# Dialog Node ID - Naming Convention Guide

**Project**: Stage of Dreams  
**Created**: January 2025  
**Version**: 1.0  
**Status**: Official Standard

---

## Overview

This document defines the official naming convention for Dialog Node IDs in the Stage of Dreams project. Consistent Node IDs improve organization, debugging, and team collaboration.

---

## Standard Format

```
[context]_[speaker]_[action]_[sequence]
```

### Components

| Component | Required | Description | Examples |
|-----------|----------|-------------|----------|
| **Context** | ✅ Yes | Where/when node appears | `act1`, `dream1`, `tutorial` |
| **Speaker** | ✅ Yes | Who's speaking | `player`, `director`, `stagemgr` |
| **Action** | ✅ Yes | What's happening | `intro`, `choice`, `advice` |
| **Sequence** | ✅ Yes | Two-digit number | `01`, `02`, `15` |

---

## Context Prefixes

Use these standardized context prefixes:

### Story Progression
- `act1`, `act2`, `act3` - Main story acts
- `dream1`, `dream2`, `dream3` - Specific dream/level
- `climax` - Climax box events

### Interaction Types
- `npc` - NPC conversations (can be revisited)
- `side` - Side conversations
- `tutorial` - Tutorial sequences

### Gameplay
- `perf` - Performance/minigame dialog
- `cond` - Conditional content

---

## Speaker Abbreviations

Keep speaker identifiers short (3-8 characters):

### Main Characters
- `player` - Player character
- `director` - Director NPC
- `stagemgr` - Stage Manager
- `narrator` - Narrator/system voice

### Audience
- `audience` - Audience reactions
- `crowd` - Crowd responses

### Use First Name or Abbreviation
- `john` - Character named John
- `emmadirector` - Emma the Director (if multiple characters named Emma)

---

## Action Keywords

Common action keywords (keep to 3-8 characters):

### Dialog Flow
- `intro` - Introduction/opening
- `outro` - Conclusion/closing
- `branch` - Branch point (choices begin)
- `choice` - Result of a choice
- `response` - Response to player action
- `convergence` - Where paths merge

### Conversation Types
- `advice` - Giving advice
- `pep` - Pep talk/encouragement
- `challenge` - Challenge/confrontation
- `question` - Asking a question
- `explain` - Explaining something

### Performance
- `prep` - Preparation
- `perform` - During performance
- `react` - Reaction to performance
- `success` - Successful outcome
- `fail` - Failed outcome

---

## Sequence Numbers

### Rules
- Always use **two digits** with leading zero: `01`, `02`, `09`, `10`
- Increment by 1 for linear sequences
- Use gaps (10, 20, 30) if you plan to insert nodes later

### Examples
```
Linear sequence:
act1_director_intro_01
act1_director_intro_02
act1_director_intro_03

With gaps for future expansion:
act1_director_intro_10
act1_director_intro_20  ← Can add _11, _12 between if needed
act1_director_intro_30
```

---

## Complete Examples

### Linear Story Sequence
```
act1_director_welcome_01  → Director welcomes player
act1_director_welcome_02  → Director continues greeting
act1_player_response_01   → Player responds
act1_director_final_01    → Director concludes
```

### Branching Conversation
```
act1_branch_emotion_01          → Branch point: How do you feel?
  ├─ act1_choice_nervous_01     → If player chose "nervous"
  ├─ act1_choice_excited_01     → If player chose "excited"
  └─ act1_choice_ready_01       → If player chose "ready"
act1_convergence_stage_01       → All paths merge here
```

### NPC Conversation (Repeatable)
```
npc_director_idle_01      → First idle conversation
npc_director_idle_02      → Second idle conversation
npc_director_advice_01    → Advice dialog (can ask again)
```

### Performance Minigame
```
perf_calm_intro_01        → Introduce "Calm Dialogue" minigame
perf_calm_attempt_01      → Player attempts minigame
perf_calm_success_01      → Success outcome
perf_calm_fail_01         → Failure outcome
perf_calm_retry_01        → Retry option
```

### Tutorial Sequence
```
tutorial_movement_01      → Teach movement
tutorial_movement_02      → Practice movement
tutorial_interact_01      → Teach interaction
tutorial_interact_02      → Practice interaction
```

### Convergent Path Example
```
act1_branch_emotion_01
  ├─ act1_choice_nervous_01
  │   └─ act1_choice_nervous_02
  ├─ act1_choice_excited_01
  │   └─ act1_choice_excited_02
  └─ act1_choice_ready_01
      └─ act1_choice_ready_02

act1_convergence_prep_01   → All three paths reference this node
act1_convergence_prep_02
```

---

## Special Node Types

### Branch Points
Nodes where player makes a choice:
```
act1_branch_emotion_01
dream2_branch_difficulty_01
```

### Convergence Points
Nodes where multiple paths merge:
```
act1_convergence_backstage_01
dream1_convergence_stage_01
```

### Conditional Nodes
Only appear based on game state:
```
act1_cond_nervous_01      → Only if player chose nervous earlier
act2_cond_highscore_01    → Only if performance score is high
```

### Repeatable Nodes
Can be revisited multiple times:
```
npc_director_idle_01
npc_director_idle_02
side_help_repeat_01
```

---

## Validation Rules

Node IDs must follow these rules:

### Format Rules
✅ **DO:**
- Use lowercase letters only
- Separate components with underscores `_`
- End with 2-digit number
- Keep total length under 40 characters
- Use consistent abbreviations

❌ **DON'T:**
- Use spaces or special characters
- Use camelCase or PascalCase
- Skip the sequence number
- Mix naming conventions
- Use ambiguous abbreviations

### Validation Checks
The system validates:
1. Has 4 parts separated by underscores
2. Last part is a 2-digit number
3. No invalid characters
4. Unique across the tree
5. Follows project conventions

---

## Node ID Generator Tool

### Accessing the Tool
**Unity Menu**: `Tools → Dialog System → Generate Node ID`

### Features
- **Auto-Complete**: Suggests context/speaker based on history
- **Validation**: Real-time validation as you type
- **Copy to Clipboard**: One-click copy of generated ID
- **Batch Mode**: Generate multiple sequential IDs
- **Auto-Increment**: Automatically suggests next sequence number

### Quick Generate
In the DialogNode Inspector, use the **"Generate Node ID"** button to open a quick generator pre-filled with context from the current tree.

---

## Best Practices

### Planning Your Tree
1. **Map out branches first** - Identify branch and convergence points
2. **Name convergence points early** - So choices can reference them
3. **Use gaps in sequences** - Leave room for expansion
4. **Document special nodes** - Add comments for complex structures

### Working with Team
1. **Consistent abbreviations** - Agree on speaker/action keywords
2. **Document custom contexts** - If adding new contexts, update this guide
3. **Review IDs in PRs** - Check naming convention compliance
4. **Use the generator** - Don't manually type IDs

### Refactoring
1. **Don't reuse deleted IDs** - Could break save games
2. **Keep history** - Comment out old IDs, don't delete
3. **Update references** - Check for named target references
4. **Run validation** - Use `Tools → Dialog System → Validate All Trees`

---

## Common Patterns

### Linear Story Flow
```
act1_director_intro_01 → _02 → _03
act1_player_response_01
act1_director_final_01
```

### Two-Choice Branch
```
[parent]_branch_topic_01
├─ [parent]_choice_optiona_01
└─ [parent]_choice_optionb_01
[parent]_convergence_result_01
```

### Three-Choice Branch (Your Common Pattern)
```
act1_branch_emotion_01
├─ act1_choice_nervous_01
├─ act1_choice_excited_01
└─ act1_choice_ready_01
act1_convergence_prep_01
```

### NPC Conversation Loop
```
npc_director_greeting_01  → Initial greeting
npc_director_idle_01      → Repeatable idle talk
npc_director_idle_02      → Alternative idle talk
npc_director_advice_01    → Can ask for advice
npc_director_goodbye_01   → Exit conversation
```

---

## Migration Guide

### For Existing Nodes Without IDs
1. Run: `Tools → Dialog System → Auto-Generate Missing IDs`
2. Review generated IDs
3. Manually adjust if needed
4. Run: `Tools → Dialog System → Validate All Trees`

### For Nodes With Non-Standard IDs
1. Run: `Tools → Dialog System → Validate All Trees`
2. Note warnings for non-compliant IDs
3. Use: `Tools → Dialog System → Generate Node ID` to create proper IDs
4. Update references manually
5. Validate again

---

## Troubleshooting

### "Duplicate Node ID" Warning
**Cause**: Two nodes have the same ID  
**Fix**: Change sequence number on one node

### "Invalid Node ID Format" Warning
**Cause**: ID doesn't follow convention  
**Fix**: Use the Node ID Generator tool to create a proper ID

### "Node ID Too Long" Warning
**Cause**: ID exceeds 40 characters  
**Fix**: Use shorter abbreviations for speaker/action

---

## Quick Reference Card

```
Format:     [context]_[speaker]_[action]_[sequence]
Example:    act1_director_welcome_01

Context:    act1, dream1, npc, tutorial, perf
Speaker:    player, director, stagemgr, narrator
Action:     intro, branch, choice, response, convergence
Sequence:   01, 02, 03, ... (always 2 digits)

Generate:   Tools → Dialog System → Generate Node ID
Validate:   Tools → Dialog System → Validate All Trees
```

---

## Version History

| Version | Date | Changes |
|---------|------|---------|
| 1.0 | Jan 2025 | Initial naming convention established |

---

**End of Naming Convention Guide**

For implementation details, see: `Docs\DialogNodeID-GeneratorTool.md`  
For troubleshooting, see: `Docs\TROUBLESHOOTING.MD`
