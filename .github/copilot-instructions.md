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
â”œâ”€â”€ _Stage of Dreams_/
â”‚   â”œâ”€â”€ Scripts/
â”‚   â”‚   â”œâ”€â”€ Dialog/              # Dialog system components
â”‚   â”‚   â”‚   â”œâ”€â”€ DialogManager.cs       # UI controller
â”‚   â”‚   â”‚   â”œâ”€â”€ DialogNavigator.cs     # Navigation logic
â”‚   â”‚   â”‚   â”œâ”€â”€ DialogueTrigger.cs     # Interaction trigger
â”‚   â”‚   â”‚   â”œâ”€â”€ DialogEvents.cs        # Event system
â”‚   â”‚   â”‚   â””â”€â”€ Examples and Guides/   # Utility classes (not in main docs)
â”‚   â”‚   â”‚       â”œâ”€â”€ CreateLinearDialog.cs
â”‚   â”‚   â”‚       â””â”€â”€ DialogTreeFactory.cs
â”‚   â”‚   â”œâ”€â”€ PlayerScripts/       # Player-related scripts
â”‚   â”‚   â”‚   â”œâ”€â”€ Player_Controller.cs
â”‚   â”‚   â”‚   â”œâ”€â”€ PlayerInteraction.cs
â”‚   â”‚   â”‚   â””â”€â”€ Interactable.cs
â”‚   â”‚   â””â”€â”€ StageScripts/        # Stage environment scripts
â”‚   â”‚       â”œâ”€â”€ AudienceManager.cs
â”‚   â”‚       â”œâ”€â”€ LightingManager.cs
â”‚   â”‚       â”œâ”€â”€ Spotlight.cs
â”‚   â”‚       â””â”€â”€ SpotlightController.cs
â”‚   â””â”€â”€ World/                   # Data structures (ScriptableObjects)
â”‚       â”œâ”€â”€ Dialog Tree.cs       # Dialog tree container
â”‚       â”œâ”€â”€ Dialog Node.cs       # Individual dialog node
â”‚       â”œâ”€â”€ Dialog Choice.cs     # Dialog choice data
â”‚       â”œâ”€â”€ Character Content.cs # Base character data
â”‚       â””â”€â”€ Dream 1/             # Level-specific content
â”œâ”€â”€ Scenes/                      # Unity scenes
â”œâ”€â”€ Prefabs/                     # Reusable game objects
â””â”€â”€ UI/                          # UI Toolkit assets (UXML/USS)

Docs/                            # Documentation folder
â”œâ”€â”€ Class Hierarchy.md           # Complete architecture & class reference
â”œâ”€â”€ Project_Roadmap.md           # Feature planning & milestones
â”œâ”€â”€ TROUBLESHOOTING.MD           # Known issues & solutions
â””â”€â”€ Requirements.md              # Project requirements
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

### Singleton Pattern (for Global Managers)
```csharp
public class GameStateManager : MonoBehaviour
{
    private static GameStateManager _instance;
    public static GameStateManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<GameStateManager>();
                
                if (_instance == null)
                {
                    GameObject go = new GameObject("GameStateManager");
                    _instance = go.AddComponent<GameStateManager>();
                }
            }
            return _instance;
        }
    }
    
    private void Awake()
    {
        // Singleton enforcement
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
```

### GameState Integration Pattern
```csharp
// When implementing minigames or game features:

// 1. Start tracking
GameStateManager.Instance.StartMinigame("minigame_id");

// 2. Update state based on actions
GameStateManager.Instance.AdjustApplause(10f);
GameStateManager.Instance.AddSceneScore(50);

// 3. End tracking
GameStateManager.Instance.EndMinigame("minigame_id", success: true);

// 4. Record progression
GameStateManager.Instance.RecordSuccess();
GameStateManager.Instance.UnlockAbility("ability_id");
```

### Event-Driven UI Updates
```csharp
// Subscribe to GameState events for automatic UI updates
private void OnEnable()
{
    GameStateManager.Instance.OnApplauseScoreChanged += UpdateApplauseUI;
    GameStateManager.Instance.OnSceneScoreChanged += UpdateScoreUI;
}

private void OnDisable()
{
    GameStateManager.Instance.OnApplauseScoreChanged -= UpdateApplauseUI;
    GameStateManager.Instance.OnSceneScoreChanged -= UpdateScoreUI;
}

private void UpdateApplauseUI(float newScore)
{
    // Update UI elements
    applauseLabel.text = $"Applause: {newScore:F0}";
}
```

---

## Future Implementation Plans

### Phase 1: Core Systems (Current)
- âœ“ Dialog system with branching conversations
- âœ“ Player movement and interaction
- âœ“ Dialog event system refinement
- âœ“ Documentation structure

### Phase 2: Game Mechanics
- â³ Turn-based combat/action system
- â³ Audience mood and reaction system
- â³ Stage ability system (improvise, interact, perform)
- â³ Performance scoring system

### Phase 3: Content & Polish
- â³ Complete first "Dream" level
- â³ Multiple NPC interactions
- â³ Sound effects and music
- â³ Animations and visual effects
- â³ UI polish and transitions

### Phase 4: Advanced Features
- â³ Save/load system
- â³ Multiple "Dreams" (levels)
- â³ Character progression
- â³ Achievements/unlockables

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
- **Change â³ status to âœ“ when complete**

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
- **Use ASCII-safe characters in Debug.Log statements (no emojis)**

---

## Unity Best Practices

### Debug Logging Standards

Unity's Console window has limited Unicode support. **Always use ASCII-safe characters in debug logs.**

#### ✅ DO Use:
```csharp
Debug.Log("[OK] Operation successful");
Debug.LogWarning("[WARNING] Potential issue detected");
Debug.LogError("[ERROR] Operation failed");
Debug.Log("[INFO] Status update");
Debug.Log("[FIXED] Issue resolved");
```

#### ❌ DON'T Use:
```csharp
Debug.Log("✅ Operation successful");  // Displays as ?
Debug.LogWarning("⚠️ Warning");        // Displays as ?
Debug.LogError("❌ Error");            // Displays as ?
Debug.Log("📊 Statistics");            // Displays as ?
```

#### Emoji Replacement Guide

| Emoji | ASCII Replacement | Use Case |
|-------|------------------|----------|
| ✅ | `[OK]` or `[SUCCESS]` | Success/Valid |
| ❌ | `[ERROR]` or `[!]` | Error/Invalid |
| ⚠️ | `[WARNING]` or `[WARN]` | Warning |
| 🔧 | `[FIXED]` | Fixed/Repaired |
| 📊 | `[STATS]` or `[#]` | Statistics |
| ✨ | `[NEW]` or `[CREATED]` | Created |
| 🎯 | `[TARGET]` | Goal/Objective |
| 💡 | `[TIP]` or `[INFO]` | Information |
| 🚀 | `[START]` | Started |
| 🎉 | `[DONE]` or `[COMPLETE]` | Completed |
| 🐛 | `[BUG]` | Bug/Issue |
| 🔍 | `[SEARCH]` or `[FIND]` | Searching |
| ⏱️ | `[TIME]` | Timing/Duration |
| 🎭 | `[DIALOG]` or `[STAGE]` | Stage/Dialog system |

#### Example Implementations

**Before (with emojis)**:
```csharp
Debug.Log($"✅ DialogTree validated: {nodeCount} nodes");
Debug.LogWarning($"⚠️ Missing speaker name in node");
Debug.LogError($"❌ Failed to load dialog tree");
Debug.Log($"🎭 Starting dialog with {npcName}");
```

**After (ASCII-safe)**:
```csharp
Debug.Log($"[OK] DialogTree validated: {nodeCount} nodes");
Debug.LogWarning($"[WARNING] Missing speaker name in node");
Debug.LogError($"[ERROR] Failed to load dialog tree");
Debug.Log($"[DIALOG] Starting dialog with {npcName}");
```

---
