# Working with Jack Taylor - AI Instructions

I am Jack Taylor, an aspiring game developer learning to create games using Unity and C#.
Below are AI instructions for my current game project, **Stage of Dreams**.

Please assist me in learning by:
- Explaining concepts clearly with examples
- Providing code examples that follow the established patterns
- Suggesting best practices for Unity and C# development
- Referencing existing code structure when making suggestions

---

## AI Memory & Learning Notes

### Jack's Skill Level & Preferences
- **Experience Level**: Beginner to intermediate game developer
- **Learning Style**: Prefers hands-on examples with explanations
- **Focus Areas**: Unity 2D development, C# programming, turn-based game mechanics
- **Needs explanations for**: Game architecture patterns, Unity best practices, event systems

### Project Progress Tracking
**Current Sprint**: Dialog System Enhancement (Feature/DialogNode-Functionality branch)
- ? Dialog Tree system implemented with ScriptableObjects
- ? Dialog Node structure with choices and auto-advance
- ? Dialog Event system with method delegates
- ? Integration between NPCContent, DialogueTrigger, DialogManager, and DialogNavigator
- ? UI Toolkit integration for dialog display
- ?? Working on: Dialog node functionality refinement and testing
- ?? Next up: Audience interaction system, turn-based combat

**Completed Features**:
1. Core dialog tree system with convergent path support
2. Node-based conversation system
3. Event-driven architecture
4. Player movement and interaction system
5. Basic stage and spotlight system

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

---

## Stage of Dreams – Project Overview

### Game Concept
**Genre**: Top-down 2D RPG with theatrical elements
**Core Mechanic**: An actor performing on stage, interacting with audience and other performers
**Progression**: Each "Dream" is a unique level/story with its own narrative and objectives

### Target Features
- **Turn-Based Gameplay**: Strategic actions between player, audience, and NPCs
- **Dialog System**: Complex branching conversations with choices and consequences
- **Audience Interaction**: Dynamic mood states affecting gameplay
- **Stage Abilities**: Unique performer abilities (improvise, interact, perform)
- **Story Progression**: Multiple "Dreams" (levels) with narrative continuity

---

## Project Architecture

### Folder Structure
```
Assets/
??? _Stage of Dreams_/
?   ??? Scripts/
?   ?   ??? Dialog/              # Dialog system components
?   ?   ?   ??? DialogManager.cs       # UI controller
?   ?   ?   ??? DialogNavigator.cs     # Navigation logic
?   ?   ?   ??? DialogueTrigger.cs     # Interaction trigger
?   ?   ?   ??? DialogEvents.cs        # Event system
?   ?   ??? PlayerScripts/       # Player-related scripts
?   ?   ?   ??? Player_Controller.cs
?   ?   ?   ??? PlayerInteraction.cs
?   ?   ?   ??? Interactable.cs
?   ?   ??? StageScripts/        # Stage environment scripts
?   ?   ?   ??? AudienceManager.cs
?   ?   ?   ??? LightingManager.cs
?   ?   ?   ??? Spotlight.cs
?   ?   ?   ??? SpotlightController.cs
?   ?   ??? Main Menu Events.cs
?   ??? World/                   # Data structures (ScriptableObjects)
?       ??? Dialog Tree.cs       # Dialog tree container
?       ??? Dialog Node.cs       # Individual dialog node
?       ??? Dialog Choice.cs     # Dialog choice data
?       ??? Character Content.cs # Base character data
?       ??? Dream 1/             # Level-specific content
??? Scenes/                      # Unity scenes
??? Prefabs/                     # Reusable game objects
??? UI/                          # UI Toolkit assets (UXML/USS)
```

### Technology Stack
- **Unity Version**: 2D project (Unity 2022+)
- **.NET Version**: .NET Framework 4.7.1
- **UI System**: Unity UI Toolkit (UXML/USS)
- **Text Rendering**: TextMesh Pro
- **Version Control**: Git (GitHub repository)

---

## Core Systems Documentation

### 1. Dialog System Architecture

**Pattern**: Model-View-Controller (MVC) with event-driven communication

#### Components:

**A. Data Layer (Model)**
- `DialogTree` (ScriptableObject): Container for entire conversation tree
  - Stores starting node and manages node collection
  - Provides tree traversal and validation methods
  - Supports named nodes for convergent dialog paths
  
- `DialogNode` (ScriptableObject): Individual conversation node
  - Properties: speaker name, dialog text, player speaking flag
  - Supports: auto-advance, choices, parent/child relationships
  - Events: start events, end events
  
- `DialogChoice` (ScriptableObject): Choice option in dialog
  - Properties: choice text, target node, choice events
  - Supports: named node references, convergent paths

**B. Logic Layer (Controller)**
- `DialogNavigator`: Navigation logic engine
  - Manages current node and navigation state
  - Handles choice selection and auto-advance
  - Fires navigation events (node changed, dialog ended, custom actions)
  
- `DialogueTrigger`: Interaction trigger component
  - Bridges player interaction with dialog system
  - Validates NPC content and starts dialog sessions
  - Manages dialog lifecycle events

**C. View Layer**
- `DialogManager`: UI controller
  - Manages UI Toolkit elements (UXML/USS)
  - Displays dialog text and choices
  - Handles user input for choices
  - Coordinates with DialogNavigator

#### Integration Flow:
```
Player Interaction ? DialogueTrigger ? DialogManager ? DialogNavigator
                                            ?
                                       NPC Content
                                            ?
                                       Dialog Tree
```

#### Key Features:
- **Convergent Paths**: Multiple choices can lead to same node via named references
- **Event System**: Custom events can be triggered at node start/end or choice selection
- **Validation**: Comprehensive validation at multiple levels (tree, node, choice)
- **Auto-Advance**: Nodes can automatically progress after delay
- **Method Delegates**: Events support both UnityEvents and direct method calls

### 2. Event System

**Pattern**: Observer pattern with polymorphic event types

#### Base Event Class:
```csharp
public abstract class DialogEvent
{
    - Execute(): Triggers the event
    - IsValid(): Validates event configuration
    - SetExecuteDelegate(Action): Supports method delegates
}
```

#### Implemented Event Types:
- `MethodCallEvent`: Execute custom methods
- `ParameterizedMethodEvent<T>`: Execute methods with parameters
- `StaticMethodCallEvent`: Call static methods via reflection

#### Usage Pattern:
```csharp
// At node start/end
node.AddStartEvent(new MethodCallEvent());
node.AddEndEvent(new MethodCallEvent());

// At choice selection
choice.AddChoiceEvent(new ParameterizedMethodEvent<string>());
```

### 3. Player Interaction System

**Components**:
- `Player_Controller`: Handles player movement and input
- `PlayerInteraction`: Manages interaction with world objects
- `Interactable`: Interface/base class for interactable objects

**Pattern**: Uses Unity's collision/trigger system with interface-based callbacks

### 4. Stage Systems (In Progress)

**Components**:
- `AudienceManager`: Manages audience NPCs and mood states
- `LightingManager`: Controls stage lighting
- `SpotlightController`: Individual spotlight behavior

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

## Common Development Tasks

### Creating a New Dialog Tree
1. Right-click in Project ? Create ? Dialog System ? Dialog Tree
2. Use `CreateStartingNode()` to set first node
3. Use `AddChoiceNode()` or `AddSequentialNode()` to build tree
4. Use `ValidateTree()` context menu to check structure
5. Assign to NPC's character content

### Adding Dialog Events
```csharp
// Create event
var myEvent = new MethodCallEvent();
myEvent.SetMethod(() => Debug.Log("Event fired!"), "Test Event");
myEvent.EventName = "My Custom Event";

// Add to node
node.AddStartEvent(myEvent);  // or AddEndEvent()

// Add to choice
choice.AddChoiceEvent(myEvent);
```

### Creating Convergent Dialog Paths
```csharp
// Name important nodes
DialogNode convergencePoint = tree.AddSequentialNode(
    parentNode, "Speaker", "This is where paths converge", 
    false, 0f, "ConvergencePoint"
);

// Reference from multiple choices
choice1.SetTargetByName("ConvergencePoint");
choice2.SetTargetByName("ConvergencePoint");

// Resolve references
tree.ResolveNamedReferences();
```

---

## Testing & Debugging Guidelines

### Dialog System Testing
1. Use context menu "Validate Tree" on DialogTree assets
2. Use context menu "Print Tree Structure" to visualize tree
3. Enable debug logs in DialogManager inspector
4. Use "Test Show Dialog" context menu in Play mode

### Common Issues & Solutions

**Issue**: Dialog doesn't start
- Check: NPC has valid DialogTree assigned
- Check: DialogTree has valid starting node
- Check: DialogManager and DialogNavigator are initialized

**Issue**: Choices not appearing
- Check: Node has choices added
- Check: Choice has valid target node
- Check: UI elements exist and are named correctly

**Issue**: Events not firing
- Check: Event is enabled
- Check: Event has valid configuration
- Check: Delegates are properly assigned

---

## Future Implementation Plans

### Phase 1: Core Systems (Current)
- ? Dialog system with branching conversations
- ? Player movement and interaction
- ?? Dialog event system refinement

### Phase 2: Game Mechanics
- ?? Turn-based combat/action system
- ?? Audience mood and reaction system
- ?? Stage ability system (improvise, interact, perform)
- ?? Performance scoring system

### Phase 3: Content & Polish
- ?? Complete first "Dream" level
- ?? Multiple NPC interactions
- ?? Sound effects and music
- ?? Animations and visual effects
- ?? UI polish and transitions

### Phase 4: Advanced Features
- ?? Save/load system
- ?? Multiple "Dreams" (levels)
- ?? Character progression
- ?? Achievements/unlockables

---

## AI Assistance Guidelines

### When I Ask for Help With...

**Architecture Questions**:
- Reference the existing MVC pattern in dialog system
- Consider event-driven approaches
- Suggest ScriptableObject-based data structures

**Code Examples**:
- Follow the property pattern with backing fields
- Include validation methods
- Add context menu debug options
- Use XML documentation comments

**Bug Fixes**:
- Ask me to run context menu validation tools first
- Check Unity console for errors
- Verify serialization and references
- Test in both Edit and Play modes

**New Features**:
- Break down into small, testable components
- Consider how it integrates with existing systems
- Plan for validation and debugging tools
- Think about ScriptableObject data storage

### Communication Style
- Explain *why*, not just *how*
- Reference Unity documentation when relevant
- Provide step-by-step instructions
- Include warning about common pitfalls
- Suggest testing approaches

---

## Quick Reference

### Important Files to Check
- Dialog System: `Assets\_Stage of Dreams_\Scripts\Dialog\`
- Data Structures: `Assets\_Stage of Dreams_\World\`
- Documentation: `Docs\` folder

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

**Project Context**:
- Unity 2D project using .NET Framework 4.7.1
- Currently working on dialog system refinement
- Focus on modular, testable code
- Event-driven architecture preferred
- ScriptableObjects for data storage

**Current Work Focus**:
- Enhancing dialog node functionality
- Testing convergent path system
- Refining event system
- Preparing for turn-based system integration

---

*Last Updated: [Current Session]*
*Current Branch: Feature/DialogNode-Functionality*
*Next Major Milestone: Complete Dialog System ? Start Turn-Based System*