# Dialog System - Quick Reference Diagrams

**Purpose**: Quick visual reference for presentation and development  
**Last Updated**: January 2025

---

## System Architecture Overview

```mermaid
graph TB
    subgraph "Data Layer"
        DT[DialogTree<br/>ScriptableObject]
        DN[DialogNode<br/>Can be Standard or Minigame]
        DC[DialogChoice]
    end
    
    subgraph "Editor Layer"
        TREEED[DialogTreeEditor<br/>Tree Management]
        ROUTER[DialogNodeEditorWindow<br/>Router + Standard Editor]
        RTSED[RememberTheScriptNodeEditor<br/>Specialized Minigame Editor]
    end
    
    subgraph "Runtime Layer"
        DNAV[DialogNavigator<br/>Game Logic + Events]
        DMGR[DialogManager<br/>UI Rendering]
    end
    
    DT --> DN
    DN --> DC
    
    TREEED -->|Edit Node| ROUTER
    ROUTER -->|Detect Type| RTSED
    ROUTER -->|Standard| ROUTER
    
    DN -->|Provides Data| DNAV
    DNAV -->|Fires Events| DMGR
    
    style DT fill:#FFD700,stroke:#333,stroke-width:2px,color:#000
    style DN fill:#90EE90,stroke:#333,stroke-width:2px,color:#000
    style ROUTER fill:#FFA500,stroke:#333,stroke-width:2px,color:#000
    style RTSED fill:#90EE90,stroke:#333,stroke-width:2px,color:#000
    style DNAV fill:#FFB6C1,stroke:#333,stroke-width:2px,color:#000
    style DMGR fill:#87CEEB,stroke:#333,stroke-width:2px,color:#000
```

---

## Editor Router Pattern

```mermaid
graph LR
    EDIT[User Clicks<br/>Edit Node]
    
    OPEN[OpenWindow<br/>Router Method]
    
    CHECK{Node Type?}
    
    STANDARD[DialogNodeEditorWindow<br/>Standard Editor]
    RTS[RememberTheScriptNodeEditor<br/>Minigame Editor]
    CALM[CalmDialogNodeEditor<br/>Future]
    
    EDIT --> OPEN
    OPEN --> CHECK
    
    CHECK -->|StandardDialog| STANDARD
    CHECK -->|RememberTheScript| RTS
    CHECK -->|CalmDialog| CALM
    
    style EDIT fill:#87CEEB,stroke:#333,stroke-width:2px,color:#000
    style OPEN fill:#FFD700,stroke:#333,stroke-width:3px,color:#000
    style CHECK fill:#FFA500,stroke:#333,stroke-width:2px,color:#000
    style RTS fill:#90EE90,stroke:#333,stroke-width:2px,color:#000
```

---

## Minigame Node Flow

```mermaid
graph TB
    START[Dialog Start]
    
    INTRO[Standard Node<br/>Let's test your memory]
    
    MINIGAME[Minigame Node<br/>RememberTheScript<br/>🎭 Type the phrase correctly]
    
    SUCCESS[Success Node<br/>Excellent work!<br/>+20 Applause]
    
    FAILURE[Failure Node<br/>Lets try again<br/>-5 per mistake]
    
    NEXT[Continue Story]
    
    START --> INTRO
    INTRO --> MINIGAME
    
    MINIGAME -->|Player Succeeds<br/>ChildNode| SUCCESS
    MINIGAME -.->|Player Fails<br/>FailureNode| FAILURE
    
    SUCCESS --> NEXT
    FAILURE -.->|Retry| MINIGAME
    
    style MINIGAME fill:#FFD700,stroke:#333,stroke-width:3px,color:#000
    style SUCCESS fill:#90EE90,stroke:#333,stroke-width:2px,color:#000
    style FAILURE fill:#FFB6C1,stroke:#333,stroke-width:2px,color:#000
```

---

## Node Type Hierarchy

```mermaid
classDiagram
    class DialogNode {
        +DialogNodeType NodeType
        +string NodeName
        +string CharacterName
        +string DialogText
        +DialogNode ChildNode
        +DialogNode FailureNode
        +List~DialogChoice~ Choices
    }
    
    class StandardDialog {
        <<NodeType>>
        +Regular conversation
        +Choices or auto-advance
    }
    
    class RememberTheScript {
        <<NodeType>>
        +string TargetPhrase
        +int MaxMistakes
        +float TimeLimit
        +float ScoreOnSuccess
        +float ScorePerMistake
    }
    
    class CalmDialog {
        <<NodeType>>
        +Future implementation
        +Choice-based minigame
    }
    
    DialogNode <|-- StandardDialog
    DialogNode <|-- RememberTheScript
    DialogNode <|-- CalmDialog
    
    StandardDialog : Uses ChildNode
    StandardDialog : Uses Choices
    
    RememberTheScript : Uses ChildNode for success
    RememberTheScript : Uses FailureNode for failure
    
    CalmDialog : Future design
```

---

## Event Flow: Minigame Execution

```mermaid
sequenceDiagram
    participant Player
    participant Input
    participant DialogNavigator
    participant GameStateManager
    participant DialogManager
    
    Player->>DialogNavigator: Start Minigame Node
    DialogNavigator->>DialogManager: OnRememberScriptStarted
    DialogManager->>DialogManager: Show Typing UI
    
    loop Each Character
        Player->>Input: Type Character
        Input->>DialogNavigator: ValidateInput(char)
        DialogNavigator->>DialogNavigator: Check if correct
        
        alt Correct
            DialogNavigator->>DialogManager: OnRememberScriptProgress
            DialogManager->>DialogManager: Update UI (fill character)
        else Mistake
            DialogNavigator->>DialogManager: OnRememberScriptProgress
            DialogManager->>DialogManager: Update mistake counter
            
            alt Max Mistakes Reached
                DialogNavigator->>GameStateManager: AdjustApplause(penalty)
                DialogNavigator->>DialogManager: OnRememberScriptFailure
                DialogManager->>DialogNavigator: Navigate to FailureNode
            end
        end
        
        alt Phrase Complete
            DialogNavigator->>GameStateManager: AdjustApplause(reward)
            DialogNavigator->>DialogManager: OnRememberScriptSuccess
            DialogManager->>DialogNavigator: Navigate to SuccessNode
        end
    end
```

---

## Content Creator Workflow

```mermaid
graph TD
    START[Start]
    
    CREATE[Create DialogTree Asset<br/>Assets → Create → Dialog System]
    
    OPEN[Open in Inspector<br/>DialogTreeEditor appears]
    
    BUILD[Use Quick Tree Builder<br/>Set Node Type to RememberTheScript]
    
    AUTO[System Auto-Creates:<br/>• Minigame Node<br/>• Success Node<br/>• Failure Node]
    
    EDIT[Click Edit on Minigame Node<br/>Router opens RememberTheScriptNodeEditor]
    
    CONFIG[Configure Minigame:<br/>• Target phrase<br/>• Difficulty<br/>• Scoring]
    
    TEST[Test Minigame<br/>Validate settings]
    
    CUSTOM[Edit Outcome Nodes<br/>Customize success/failure dialog]
    
    DONE[Done! Ready to use in game]
    
    START --> CREATE
    CREATE --> OPEN
    OPEN --> BUILD
    BUILD --> AUTO
    AUTO --> EDIT
    EDIT --> CONFIG
    CONFIG --> TEST
    TEST --> CUSTOM
    CUSTOM --> DONE
    
    style AUTO fill:#90EE90,stroke:#333,stroke-width:2px,color:#000
    style EDIT fill:#FFA500,stroke:#333,stroke-width:2px,color:#000
    style TEST fill:#FFD700,stroke:#333,stroke-width:2px,color:#000
```

---

## Key Concepts

### Minigame = Node

**Traditional Approach**:
```
Dialog System ← → Minigame Manager ← → Individual Minigames
(Complex integration, separate systems)
```

**Our Approach**:
```
DialogNode (with NodeType = Minigame)
(Unified system, seamless integration)
```

### Dual-Path System

**Success Path**:
```
Minigame Node → ChildNode → Success Node → Continue
```

**Failure Path**:
```
Minigame Node → FailureNode → Retry Node → (Back to Minigame OR Continue)
```

### Editor Routing

```
Single Entry Point (DialogNodeEditorWindow.OpenWindow)
    ↓
Type Detection
    ↓
Route to Appropriate Editor
    • StandardDialog → DialogNodeEditorWindow
    • RememberTheScript → RememberTheScriptNodeEditor
    • Future types → Future editors
```

---

## File Locations

**Core Scripts**:
- `Assets/_Stage of Dreams_/World/Dialog Node.cs` - Node data structure
- `Assets/_Stage of Dreams_/World/Dialog Tree.cs` - Tree container
- `Assets/_Stage of Dreams_/Scripts/Dialog/DialogNavigator.cs` - Game logic
- `Assets/_Stage of Dreams_/Scripts/Dialog/DialogManager.cs` - UI rendering

**Editor Scripts**:
- `Assets/_Stage of Dreams_/Editor/DialogTreeEditor.cs` - Tree inspector
- `Assets/_Stage of Dreams_/Editor/DialogNodeEditorWindow.cs` - Standard editor + router
- `Assets/_Stage of Dreams_/Editor/RememberTheScriptNodeEditor.cs` - Minigame editor

**Documentation**:
- `Docs/Class Hierarchy.md` - Complete system documentation
- `Docs/Presentation-Dialog-System-Architecture.md` - Presentation deck
- `Docs/Dialog-System-Quick-Reference.md` - This file

---

## Color Legend

- 🟡 **Gold** - Router/Decision Points
- 🟢 **Green** - Success/Positive Outcomes
- 🔴 **Pink** - Failure/Retry/Negative Outcomes
- 🔵 **Blue** - Standard/Neutral Elements
- 🟠 **Orange** - Active/Processing Elements

---

## Quick Stats

**System Metrics**:
- ✅ 2 Node Types Implemented (StandardDialog, RememberTheScript)
- ✅ 1 Specialized Editor (RememberTheScriptNodeEditor)
- ✅ Dual-path system (ChildNode + FailureNode)
- ✅ Event-driven architecture (8+ events)
- ✅ Automatic outcome node creation
- ✅ Router pattern for editor selection
- ⏳ 2+ Future Node Types (CalmDialog, DancingCombat)

**Code Stats**:
- DialogNode: ~600 lines (data + logic)
- DialogTree: ~400 lines (tree management)
- DialogNavigator: ~800 lines (game logic)
- DialogManager: ~600 lines (UI control)
- Editor Scripts: ~2000+ lines combined

---

**End of Quick Reference**

Use these diagrams in presentations, documentation, or as development reference!
