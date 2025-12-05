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
- **Always verify current date** - Use `Get-Date` command before adding dates to documentation
- Date major documentation updates with accurate month/year
- **Class Hierarchy**: Focus on *what exists* - architecture, classes, methods
- **AI Instructions**: Focus on *how to code* - patterns, conventions, workflows

---

## AI Memory & Learning Notes

### Jack's Skill Level & Preferences
- **Experience Level**: Beginner to intermediate game developer
- **Learning Style**: Prefers hands-on examples with explanations
- **Focus Areas**: Unity development, C# programming, UI systems
- **Needs explanations for**: Game architecture patterns, Unity best practices, event systems

### Project Progress Tracking
**Current Sprint**: Minigame UI Implementation (Feature/Minigames branch)
- Dialog Tree system implemented with ScriptableObjects
- Dialog Node structure with choices and auto-advance
- Dialog Event system with method delegates
- Integration between NPCContent, DialogueTrigger, DialogManager, and DialogNavigator
- UI Toolkit integration for dialog display
- Mermaid architecture diagrams added to Class Hierarchy
- Documentation consolidated and standardized
- **GameStateManager singleton implemented** - centralized state tracking
- **GameStateData ScriptableObject** - save/load infrastructure
- **RememberTheScript minigame COMPLETE** ✅
	- Full typing validation system
	- Mistake tracking and reset logic
	- Timer system
	- GameStateManager integration
	- **UI integration WORKING** ✅ NEW!
	- MinigameUIManager event subscription
	- RememberTheScriptUI component
	- USS styling applied
	- Visibility issue resolved
- **SolutionFileModifier implemented** - Automatic documentation inclusion in Visual Studio
- Working on: Testing minigame flow, CalmDialog implementation
- Next up: Multiple minigame nodes, demo scene creation, deployment

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
10. **RememberTheScript minigame COMPLETE** ✅
11. **Node type system with conversion** - Switch between dialog and minigame nodes
12. **Specialized editor windows** - Dedicated editors for each node type
13. **Auto-creation of minigame outcome nodes** - Success/failure nodes created automatically
14. **Node linking system** - Link minigame outcomes to existing nodes
15. **Clean Architecture** - DialogNode (Data) → DialogNavigator (Logic with Events) → DialogManager (UI subscribes to events)
16. **SolutionFileModifier** - Automatic documentation file inclusion in Visual Studio Solution Explorer
17. **MinigameUIManager** - Event-driven UI coordinator for minigames ✅ NEW!
18. **RememberTheScriptUI** - Typing minigame visual component ✅ NEW!
19. **USS styling** - MinigameUI.uss with complete minigame styling ✅ NEW!

**Recent Fixes/Lessons Learned**:
- ✅ **UI Toolkit Parent/Child Visibility** - If parent has `display: none`, all children are invisible regardless of their own display settings
- ✅ **Minigame Transition Fix** - Hide dialog elements (text/buttons) but keep DialogBox container visible for minigame UI
- ✅ **Input System Migration** - Use `Keyboard.current` from new Input System instead of `Input.inputString`
- ✅ **Event-Driven Minigames** - DialogNavigator fires events → MinigameUIManager subscribes → RememberTheScriptUI updates display

**Known Issues/Technical Debt**:
- Need to test complete minigame flow (start → type → success/failure → return to dialog)
- Need to implement timer display updates
- Need to implement CalmDialog minigame
- Need to create demo scene with multiple minigame nodes
- Need to implement turn-based system
- SolutionFileModifier requires legacy .sln format (Unity 2022+ defaults to .slnx)

**UI Toolkit Patterns Learned**:
1. **Parent Visibility Rule**: Always check parent `display` property when debugging invisible children
2. **Container Pattern**: Use a visible container (DialogBox) to hold both dialog and minigame UI, swap children rather than hiding container
3. **USS Attachment**: Attach USS to UIDocument component, not to script fields
4. **Debug Logging**: Log both parent and child display/opacity values when troubleshooting visibility
5. **Transition Handling**: Hide/show specific elements rather than entire containers during transitions

---

## Common Patterns & Best Practices

### UI Toolkit Visibility Management

**❌ AVOID - Hiding Parent Container**:
```csharp
// This makes ALL children invisible!
dialogBox.style.display = DisplayStyle.None;
minigameContainer.Add(myUI);  // Won't be visible!
```

**✅ CORRECT - Hide Children, Keep Parent Visible**:
```csharp
// Hide dialog elements
dialogLabel.style.display = DisplayStyle.None;
choiceButtons.style.display = DisplayStyle.None;

// Keep container visible
dialogBox.style.display = DisplayStyle.Flex;

// Add minigame UI to visible container
minigameContainer.Add(myUI);
minigameContainer.style.display = DisplayStyle.Flex;
```

### Event-Driven Minigame Architecture

**Pattern**:
```
DialogNavigator (Logic + Events)
    ↓ fires events
MinigameUIManager (Coordinator)
    ↓ updates
RememberTheScriptUI (Visual Component)
```

**Benefits**:
- Clean separation of concerns
- Easy to add new minigames
- UI and logic decoupled
- Testable components

### Debugging UI Toolkit Issues

**Always Log**:
```csharp
Debug.Log($"Parent - Display: {parent.style.display.value}, Opacity: {parent.style.opacity.value}");
Debug.Log($"Child - Display: {child.style.display.value}, Opacity: {child.style.opacity.value}");
Debug.Log($"WorldBound: {element.worldBound}");
Debug.Log($"Parent: {element.parent?.name}");
```

**Check Order**:
1. Parent display property (must be Flex)
2. Child display property
3. USS file attached
4. CSS classes applied
5. WorldBound (non-zero = visible area)
6. Element hierarchy (proper parent/child)

---

## Coding Conventions

### UI Component Initialization
```csharp
public void Initialize(params...)
{
    // 1. Validate parameters
    if (param == null) { LogError(); return; }
    
    // 2. Store references
    this.field = param;
    
    // 3. Subscribe to events
    SubscribeToEvents();
    
    // 4. Build UI structure
    BuildUI();
    
    // 5. Log success
    LogDebug("Initialized successfully");
}
```

### UI Visibility Transitions
```csharp
private void TransitionToNewUI()
{
    // 1. Hide OLD content (children)
    HideOldElements();
    
    // 2. Keep CONTAINER visible
    container.style.display = DisplayStyle.Flex;
    
    // 3. Add NEW content
    ShowNewElements();
    
    // 4. Make NEW content visible
    newContent.style.display = DisplayStyle.Flex;
    newContent.style.opacity = 1f;
    
    // 5. Log state
    LogDebug($"Transitioned - Container: {container.style.display.value}");
}
```

### Diagnostic Logging
```csharp
private void LogDebug(string message)
{
    if (enableDebugLogs)
    {
        Debug.Log($"[{GetType().Name}] {message}");
    }
}
```

---

**Last Updated**: January 2025  
**Status**: Minigame UI Phase 1 COMPLETE ✅  
**Next**: Testing & CalmDialog implementation
