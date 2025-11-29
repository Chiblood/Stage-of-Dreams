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
5. Implementing planned (â³) features
6. Discovering issues or solutions

### Documentation Update Checklist

**After implementing changes, update:**
- [ ] `Class Hierarchy.md` - Update class listings with new properties/methods
- [ ] `Class Hierarchy.md` - Update Mermaid diagrams if architecture changed
- [ ] `Class Hierarchy.md` - Change â³ (TBD) to âœ“ (Implemented) when features complete
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
- Unclear implementation status (âœ“ vs â³)

### Documentation Best Practices
- Use consistent status indicators: âœ“ (Implemented), â³ (TBD), âŒ (Deprecated)
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
**Current Sprint**: GameState Management & Minigame Foundation (Feature/Minigames branch)
- Dialog Tree system implemented with ScriptableObjects
- Dialog Node structure with choices and auto-advance
- Dialog Event system with method delegates
- Integration between NPCContent, DialogueTrigger, DialogManager, and DialogNavigator
- UI Toolkit integration for dialog display
- Mermaid architecture diagrams added to Class Hierarchy
- Documentation consolidated and standardized
- **GameStateManager singleton implemented** - centralized state tracking
- **GameStateData ScriptableObject** - save/load infrastructure
- Working on: Minigame implementation (CalmDialog, RememberTheScript)
- Next up: Minigame UI, audience interaction system, turn-based combat

**Completed Features**:
1. Core dialog tree system with convergent path support
2. Node-based conversation system
3. Event-driven architecture
4. Player movement and interaction system
5. Basic stage and spotlight system
6. Comprehensive documentation with visual diagrams
7. Consolidated documentation structure
8. **GameState management system** (audience metrics, scores, progression, session tracking)
9. **Event-driven state updates** for UI integration

**Known Issues/Technical Debt**:
- Need to implement minigame UI components
- Need to implement turn-based system
- Audience mood/reaction system partially implemented (tracking in GameState, needs audio/visual feedback)
- Performance testing for large dialog trees needed

### Code Quality Standards Jack is Learning
- Use XML documentation comments for public methods
- Implement validation methods (`IsValid()`) for data structures
- Include context menu debugging options for ScriptableObjects
- Follow Unity serialization best practices with `[SerializeField]` and `[SerializeReference]`
- Use properties with backing fields for better encapsulation
- **Keep documentation in sync with code changes**

---

## Stage of Dreams â€“ Project Overview

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
|-- _Stage of Dreams_/
|   |-- Scripts/
|   |   |-- Dialog/              # Dialog system components
|   |   |   |-- DialogManager.cs       # UI controller
|   |   |   |-- DialogNavigator.cs     # Navigation logic
|   |   |   |-- DialogueTrigger.cs     # Interaction trigger
|   |   |   |-- DialogEvents.cs        # Event system
|   |   |   |-- Examples and Guides/   # Utility classes (not in main docs)
|   |   |       |-- CreateLinearDialog.cs
|   |   |       |-- DialogTreeFactory.cs
|   |   |-- PlayerScripts/       # Player-related scripts
|   |   |   |-- Player_Controller.cs
|   |   |   |-- PlayerInteraction.cs
|   |   |   |-- Interactable.cs
|   |   |-- StageScripts/        # Stage environment scripts
|   |       |-- AudienceManager.cs
|   |       |-- LightingManager.cs
|   |       |-- Spotlight.cs
|   |       |-- SpotlightController.cs
|   |-- World/                   # Data structures (ScriptableObjects)
|       |-- Dialog Tree.cs       # Dialog tree container
|       |-- Dialog Node.cs       # Individual dialog node
|       |-- Dialog Choice.cs     # Dialog choice data
|       |-- Character Content.cs # Base character data
|       |-- Dream 1/             # Level-specific content
|-- Scenes/                      # Unity scenes
|-- Prefabs/                     # Reusable game objects
|-- UI/                          # UI Toolkit assets (UXML/USS)

Docs/                            # Documentation folder
|-- Class Hierarchy.md           # Complete architecture & class reference
|-- Project_Roadmap.md           # Feature planning & milestones
|-- TROUBLESHOOTING.MD           # Known issues & solutions
|-- Requirements.md              # Project requirements
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

### DialogEvent Pattern (Custom Event System)
```csharp
// Creating custom dialog events
[System.Serializable]
public class MyCustomEvent : DialogEvent
{
    [SerializeField] private string myParameter;
    
    protected override void OnExecute()
    {
        // Your custom logic here
        MyGameSystem.Instance?.DoSomething(myParameter);
    }
    
    public override bool IsValid()
    {
        return base.IsValid() && !string.IsNullOrEmpty(myParameter);
    }
}

// Adding events to dialog nodes
DialogNode node = tree.GetStartingNode();
node.AddStartEvent(new MyCustomEvent { myParameter = "value" });

// Events execute automatically when:
// - Node starts (StartEvents)
// - Node ends (EndEvents)  
// - Choice selected (ChoiceEvents)

// Events support method delegates
var evt = new MethodCallEvent();
evt.SetMethod(() => Debug.Log("Custom action!"));
node.AddStartEvent(evt);
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

## Dialog System Documentation

### Primary Resources
- **User Guide**: `Docs/Dialog-System-Complete-Guide.md` - For designers and content creators
- **Technical Reference**: `Docs/Dialog-System-Technical-Reference.md` - For programmers and system integrators
- **Class Hierarchy**: `Docs/Class Hierarchy.md` - Complete architecture overview

### When to Use Each
- Learning the system → User Guide
- Creating dialog content → User Guide
- Troubleshooting → User Guide > Troubleshooting section
- Integrating with code → Technical Reference
- Extending the system → Technical Reference > Extension Guide
- Understanding architecture → Technical Reference > System Architecture
- **Creating custom events** → Technical Reference > Event System

### Dialog Event System Quick Reference
The dialog system uses a polymorphic event system with method delegate support:

**Base Event Types:**
- `DialogEvent` - Abstract base class with Execute() and IsValid()
- `MethodCallEvent` - Execute any Action delegate
- `ParameterizedMethodEvent<T>` - Execute methods with parameters
- `StaticMethodCallEvent` - Call static methods via reflection

**Usage Pattern:**
```csharp
// Add event to node
var evt = new MethodCallEvent();
evt.SetMethod(() => GameStateManager.Instance.AdjustApplause(10f));
node.AddStartEvent(evt);

// Add event to choice
var choiceEvt = new ParameterizedMethodEvent<string>();
choiceEvt.SetMethod(MinigameManager.Instance.StartMinigame, "calm_dialog");
choice.AddChoiceEvent(choiceEvt);
```

**When to Use:**
- Node start/end events for setup/cleanup
- Choice events for game system integration
- Custom events for game-specific behaviors
- Prefer DialogEvents over UnityEvents for type safety
