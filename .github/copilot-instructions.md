# Working with Jack Taylor - AI Instructions

I am Jack Taylor, an aspiring game developer learning to create games using Unity and C#.
Below are AI instructions for my current game project, **Stage of Dreams**.

Please assist me in learning by:
- Explaining concepts clearly with examples
- Providing code examples that follow the established patterns
- Suggesting best practices for Unity and C# development
- Referencing existing code structure when making suggestions
- **Maintaining accurate documentation as the project evolves**

---

## Documentation Maintenance Guidelines

### Primary Documentation Files
- **`Docs\Class Hierarchy.md`**: Complete architecture reference with Mermaid diagrams, detailed class listings, and system documentation
- **`.github\copilot-instructions.md`**: AI guidance, coding patterns, and project context (this file)
- **`Docs\Project_Roadmap.md`**: High-level project milestones and feature planning
- **`Docs\TROUBLESHOOTING.MD`**: Known issues and solutions

### When Making Code Changes

**Always check if documentation needs updates when:**
1. Creating new classes or systems
2. Modifying class properties or methods
3. Adding or removing features
4. Changing architecture patterns
5. Implementing planned (⏳) features
6. Discovering issues or solutions

### Documentation Update Checklist

**After implementing changes, update:**
- [ ] `Class Hierarchy.md` - Update class listings with new properties/methods
- [ ] `Class Hierarchy.md` - Update Mermaid diagrams if architecture changed
- [ ] `Class Hierarchy.md` - Change ⏳ (TBD) to ✓ (Implemented) when features complete
- [ ] `.github\copilot-instructions.md` - Update coding patterns if new patterns emerged
- [ ] `.github\copilot-instructions.md` - Update "Current Work Focus" section
- [ ] `TROUBLESHOOTING.MD` - Document any issues discovered and their solutions
- [ ] `Project_Roadmap.md` - Update milestone progress

### Maintaining Consistency

**Ask for clarification when encountering:**
- Conflicting information between documentation files
- Outdated class descriptions that don't match code
- Missing classes in Class Hierarchy that exist in code
- Redundant information across multiple docs
- Unclear implementation status (✓ vs ⏳)

### Documentation Best Practices
- Use consistent status indicators: ✓ (Implemented), ⏳ (TBD), ❌ (Deprecated)
- Keep Mermaid diagrams in sync with actual architecture
- Include code examples in documentation when helpful
- Cross-reference related documentation files
- Date major documentation updates
- **Class Hierarchy**: Focus on *what exists* - architecture, classes, methods
- **AI Instructions**: Focus on *how to code* - patterns, conventions, workflows

---

## AI Memory & Learning Notes

### Jack's Skill Level & Preferences
- **Experience Level**: Beginner to intermediate game developer
- **Learning Style**: Prefers hands-on examples with explanations
- **Focus Areas**: Unity 2D development, C# programming, turn-based game mechanics
- **Needs explanations for**: Game architecture patterns, Unity best practices, event systems

### Project Progress Tracking
**Current Sprint**: Dialog System Enhancement (Feature/DialogNode-Functionality branch)
- ✓ Dialog Tree system implemented with ScriptableObjects
- ✓ Dialog Node structure with choices and auto-advance
- ✓ Dialog Event system with method delegates
- ✓ Integration between NPCContent, DialogueTrigger, DialogManager, and DialogNavigator
- ✓ UI Toolkit integration for dialog display
- ✓ Mermaid architecture diagrams added to Class Hierarchy
- ✓ Documentation consolidated and standardized
- ⏳ Working on: Dialog node functionality refinement and testing
- ⏳ Next up: Audience interaction system, turn-based combat

**Completed Features**:
1. Core dialog tree system with convergent path support
2. Node-based conversation system
3. Event-driven architecture
4. Player movement and interaction system
5. Basic stage and spotlight system
6. Comprehensive documentation with visual diagrams
7. Consolidated documentation structure

**Known Issues/Technical Debt**:
- Need to implement turn-based system
- Audience mood/reaction system pending
- Performance testing for large dialog trees needed

### Code Quality Standards Jack is Learning
- Use XML documentation comments for public methods
- Implement validation methods (`IsValid()`) for data structures
- Include context menu debugging options for ScriptableObjects
- Follow Unity serialization best practices with `[SerializeField]` and `[SerializeReference]`
- Use properties with backing fields for better encapsulation
- **Keep documentation in sync with code changes**

---

## Stage of Dreams – Project Overview

### Game Concept
**Genre**: Linear Top-down 2D RPG with theatrical elements
**Core Mechanic**: An actor performing on stage, interacting with audience and other performers
**Progression**: Each "Dream" is a unique level/story with its own narrative and objectives

### Target Features
- **Turn-Based Gameplay**: Strategic actions between player, audience, and NPCs
- **Linear Storytelling**: Narrative-driven gameplay with character interactions
- **Dialog System**: Complex branching conversations with choices and consequences
- **Audience Interaction**: Dynamic mood states affecting gameplay
- **Stage Abilities**: Unique performer abilities (improvise, interact, perform)
- **Story Progression**: Multiple "Dreams" (levels) with narrative continuity

---

## Project Architecture

> **Note**: For complete architecture diagrams, see `Docs\Class Hierarchy.md`

### Folder Structure
```
Assets/
├── _Stage of Dreams_/
│   ├── Scripts/
│   │   ├── Dialog/              # Dialog system components
│   │   │   ├── DialogManager.cs       # UI controller
│   │   │   ├── DialogNavigator.cs     # Navigation logic
│   │   │   ├── DialogueTrigger.cs     # Interaction trigger
│   │   │   ├── DialogEvents.cs        # Event system
│   │   │   └── Examples and Guides/   # Utility classes (not in main docs)
│   │   │       ├── CreateLinearDialog.cs
│   │   │       └── DialogTreeFactory.cs
│   │   ├── PlayerScripts/       # Player-related scripts
│   │   │   ├── Player_Controller.cs
│   │   │   ├── PlayerInteraction.cs
│   │   │   └── Interactable.cs
│   │   └── StageScripts/        # Stage environment scripts
│   │       ├── AudienceManager.cs
│   │       ├── LightingManager.cs
│   │       ├── Spotlight.cs
│   │       └── SpotlightController.cs
│   └── World/                   # Data structures (ScriptableObjects)
│       ├── Dialog Tree.cs       # Dialog tree container
│       ├── Dialog Node.cs       # Individual dialog node
│       ├── Dialog Choice.cs     # Dialog choice data
│       ├── Character Content.cs # Base character data
│       └── Dream 1/             # Level-specific content
├── Scenes/                      # Unity scenes
├── Prefabs/                     # Reusable game objects
└── UI/                          # UI Toolkit assets (UXML/USS)

Docs/                            # Documentation folder
├── Class Hierarchy.md           # Complete architecture & class reference
├── Project_Roadmap.md           # Feature planning & milestones
├── TROUBLESHOOTING.MD           # Known issues & solutions
└── Requirements.md              # Project requirements
```

### Technology Stack
- **Unity Version**: 2D project (Unity 2022+)
- **.NET Version**: .NET Framework 4.7.1
- **UI System**: Unity UI Toolkit (UXML/USS)
- **Text Rendering**: TextMesh Pro
- **Version Control**: Git (GitHub repository)
- **Documentation**: Markdown with Mermaid diagrams

### Utility Classes & Examples
**Location**: `Assets\_Stage of Dreams_\Scripts\Dialog\Examples and Guides\`

These utility classes are for development convenience and are **not documented in Class Hierarchy**:
- **`CreateLinearDialog.cs`**: Helper for quickly creating linear conversation chains
- **`DialogTreeFactory.cs`**: Factory methods for programmatically generating dialog trees
- **Other utilities**: Document here as needed

**When to add utilities**:
- Add to this section when creating helper/example classes
- Keep Class Hierarchy focused on core production classes
- Update this list when new utilities are added

---

## Coding Patterns & Best Practices

### Unity ScriptableObject Pattern
```csharp
[CreateAssetMenu(fileName = "New Dialog Tree", menuName = "Dialog System/Dialog Tree")]
public class DialogTree : ScriptableObject
{
    [SerializeField] private string treeName;
    [SerializeReference] private DialogNode startingNode;
    
    // Include validation
    public bool IsValid() { /* ... */ }
    
    // Include debug tools
    [ContextMenu("Validate Tree")]
    public void ValidateTree() { /* ... */ }
}
```

### Property Pattern with Backing Fields
```csharp
[SerializeField] private string _characterName;

public string CharacterName
{
    get => _characterName ?? string.Empty;
    set => _characterName = value;
}
```

### Validation Pattern
```csharp
public virtual bool IsValid()
{
    // Check required fields
    if (string.IsNullOrEmpty(requiredField))
        return false;
    
    // Validate child objects
    foreach (var child in children)
    {
        if (!child.IsValid())
            return false;
    }
    
    return true;
}
```

### Event Subscription Pattern
```csharp
private void OnEnable()
{
    dialogNavigator.OnNodeChanged += HandleNodeChanged;
    dialogNavigator.OnDialogEnded += HandleDialogEnded;
}

private void OnDisable()
{
    dialogNavigator.OnNodeChanged -= HandleNodeChanged;
    dialogNavigator.OnDialogEnded -= HandleDialogEnded;
}
```

### Context Menu Debugging
```csharp
[ContextMenu("Validate Setup")]
private void ValidateSetup()
{
    Debug.Log("Validation results...");
}

[ContextMenu("Test Functionality")]
private void TestFunctionality()
{
    // Test code here
}
```

---

## Future Implementation Plans

### Phase 1: Core Systems (Current)
- ✓ Dialog system with branching conversations
- ✓ Player movement and interaction
- ✓ Dialog event system refinement
- ✓ Documentation structure

### Phase 2: Game Mechanics
- ⏳ Turn-based combat/action system
- ⏳ Audience mood and reaction system
- ⏳ Stage ability system (improvise, interact, perform)
- ⏳ Performance scoring system

### Phase 3: Content & Polish
- ⏳ Complete first "Dream" level
- ⏳ Multiple NPC interactions
- ⏳ Sound effects and music
- ⏳ Animations and visual effects
- ⏳ UI polish and transitions

### Phase 4: Advanced Features
- ⏳ Save/load system
- ⏳ Multiple "Dreams" (levels)
- ⏳ Character progression
- ⏳ Achievements/unlockables

---

## AI Assistance Guidelines

### When I Ask for Help With...

**Architecture Questions**:
- Reference the existing MVC pattern in dialog system
- Check `Docs\Class Hierarchy.md` for current architecture diagrams
- Consider event-driven approaches
- Suggest ScriptableObject-based data structures
- **Update Class Hierarchy documentation if suggesting architectural changes**

**Code Examples**:
- Follow the property pattern with backing fields
- Include validation methods
- Add context menu debug options
- Use XML documentation comments
- **Update documentation with new patterns if they deviate from established ones**

**Bug Fixes**:
- Ask me to run context menu validation tools first
- Check Unity console for errors
- Verify serialization and references
- Test in both Edit and Play modes
- **Document the issue and solution in TROUBLESHOOTING.MD**

**New Features**:
- Break down into small, testable components
- Consider how it integrates with existing systems
- Plan for validation and debugging tools
- Think about ScriptableObject data storage
- **Update Class Hierarchy.md when adding new core classes**
- **Add to Utility Classes section if creating helper/example code**
- **Change ⏳ status to ✓ when complete**

**Documentation Tasks**:
- When asked to update documentation, check ALL related docs
- Identify and resolve any conflicting information
- Maintain consistency across documentation files
- Keep Mermaid diagrams in sync with code
- Ask for clarification if documentation conflicts with code

### Communication Style
- Explain *why*, not just *how*
- Reference Unity documentation when relevant
- Reference `Docs\Class Hierarchy.md` for architecture context
- Provide step-by-step instructions
- Include warnings about common pitfalls
- Suggest testing approaches
- **Remind Jack to update documentation after making changes**

---

## Quick Reference

### Important Files to Check
- **Complete Architecture**: `Docs\Class Hierarchy.md` (Mermaid diagrams + full class listings)
- **Dialog System Scripts**: `Assets\_Stage of Dreams_\Scripts\Dialog\`
- **Data Structures**: `Assets\_Stage of Dreams_\World\`
- **Project Planning**: `Docs\Project_Roadmap.md`
- **Known Issues**: `Docs\TROUBLESHOOTING.MD`
- **Utilities**: `Assets\_Stage of Dreams_\Scripts\Dialog\Examples and Guides\`

### Useful Context Menu Commands
- DialogTree: "Validate Tree", "Print Tree Structure", "Refresh Node List"
- DialogManager: "Validate Setup", "Test UI Elements", "Test Show Dialog"
- DialogNavigator: "Print Current State"

### Git Branch Strategy
- `main`: Stable, working code
- `Feature/*`: New features (current: Feature/DialogNode-Functionality)
- Commit frequently with descriptive messages

---

## Notes for AI

**Remember**:
- Jack is learning, so explain concepts clearly
- Reference existing code patterns when suggesting solutions
- Encourage best practices (validation, documentation, testing)
- Suggest Unity-appropriate solutions
- Keep scope manageable - small iterations are better
- **Always check if documentation needs updating after code changes**
- **Ask for clarification when documentation conflicts with code**
- **Class Hierarchy = architecture reference; AI Instructions = coding guidance**

**Project Context**:
- Unity 2D project using .NET Framework 4.7.1
- Currently working on dialog system refinement
- Focus on modular, testable code
- Event-driven architecture preferred
- ScriptableObjects for data storage
- **Comprehensive documentation with Mermaid diagrams in Class Hierarchy.md**

**Current Work Focus**:
- Enhancing dialog node functionality
- Testing convergent path system
- Refining event system
- Preparing for turn-based system integration
- **Maintaining accurate, consolidated documentation**

---

*Last Updated: Current Session - Documentation Consolidated*
*Current Branch: Feature/DialogNode-Functionality*
*Next Major Milestone: Complete Dialog System → Start Turn-Based System*
*Documentation Status: ✓ Standardized status indicators, ✓ Consolidated architecture, ✓ Clear file purposes*