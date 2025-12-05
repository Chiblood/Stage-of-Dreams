# Stage of Dreams - Class Hierarchy

## Architecture Diagrams

### Dialog System Architecture (MVC Pattern)

```mermaid
graph TB
    subgraph "Data Layer - Model"
        DT[DialogTree<br/>ScriptableObject]
        DN[DialogNode<br/>ScriptableObject]
        DC[DialogChoice<br/>ScriptableObject]
        NPC[NPCContent<br/>ScriptableObject]
        
        DT -->|contains| DN
        DN -->|has choices| DC
        DC -->|references| DN
        NPC -->|contains| DT
    end
    
    subgraph "Logic Layer - Controller"
        DNAV[DialogNavigator<br/>Navigation Logic]
        DTRIG[DialogueTrigger<br/>Interaction Handler]
        
        DNAV -->|fires events| DEVT[DialogEvents]
        DTRIG -->|initiates| DNAV
    end
    
    subgraph "View Layer"
        DMGR[DialogManager<br/>UI Controller]
        UI[UI Toolkit<br/>Visual Elements]
        
        DMGR -->|updates| UI
    end
    
    subgraph "Player System"
        PC[Player_Controller]
        PI[PlayerInteraction]
        
        PC -.->|enables/disables| PI
        PI -->|triggers| DTRIG
    end
    
    DTRIG -->|passes| NPC
    NPC -->|provides data| DMGR
    DMGR -->|coordinates| DNAV
    DNAV -->|reads| DT
    DNAV -->|notifies| DMGR
    
    style DT fill:#90EE90,stroke:#333,stroke-width:2px,color:#000
    style DN fill:#90EE90,stroke:#333,stroke-width:2px,color:#000
    style DC fill:#90EE90,stroke:#333,stroke-width:2px,color:#000
    style NPC fill:#90EE90,stroke:#333,stroke-width:2px,color:#000
    style DNAV fill:#FFB6C1,stroke:#333,stroke-width:2px,color:#000
    style DTRIG fill:#FFB6C1,stroke:#333,stroke-width:2px,color:#000
    style DMGR fill:#87CEEB,stroke:#333,stroke-width:2px,color:#000
    style UI fill:#87CEEB,stroke:#333,stroke-width:2px,color:#000
```

### Dialog Flow Sequence

```mermaid
sequenceDiagram
    participant Player
    participant PlayerInteraction
    participant DialogueTrigger
    participant DialogManager
    participant DialogNavigator
    participant DialogTree
    participant NPCContent
    
    Player->>PlayerInteraction: Press Interact Key
    PlayerInteraction->>DialogueTrigger: Trigger Dialog
    DialogueTrigger->>DialogueTrigger: ValidateNPCContent()
    DialogueTrigger->>NPCContent: GetDialogTree()
    NPCContent-->>DialogueTrigger: DialogTree
    DialogueTrigger->>DialogManager: StartDialog(NPCContent)
    
    DialogManager->>DialogManager: DisablePlayerMovement()
    DialogManager->>DialogNavigator: StartDialog(DialogTree)
    DialogNavigator->>DialogTree: Get Starting Node
    DialogTree-->>DialogNavigator: DialogNode
    DialogNavigator->>DialogNavigator: Fire OnNodeChanged Event
    DialogNavigator-->>DialogManager: Node Changed Event
    
    DialogManager->>DialogManager: DisplayNode(DialogNode)
    DialogManager->>DialogManager: ShowChoices()
    
    Note over Player,DialogManager: Player sees dialog UI
    
    Player->>DialogManager: SelectChoice(index)
    DialogManager->>DialogNavigator: SelectChoice(index)
    DialogNavigator->>DialogNode: Execute Choice Events
    DialogNavigator->>DialogTree: Navigate to Next Node
    
    alt Has Next Node
        DialogTree-->>DialogNavigator: Next DialogNode
        DialogNavigator->>DialogNavigator: Fire OnNodeChanged Event
        DialogNavigator-->>DialogManager: Node Changed Event
    else Dialog Complete
        DialogNavigator->>DialogNavigator: Fire OnDialogEnded Event
        DialogNavigator-->>DialogManager: Dialog Ended Event
        DialogManager->>DialogManager: EnablePlayerMovement()
        DialogManager->>DialogManager: HideDialog()
    end
```

### Dialog Event System

```mermaid
classDiagram
    class DialogEvent {
        <<abstract>>
        +Execute()
        +IsValid() bool
        +SetExecuteDelegate(Action)
    }
    
    class MethodCallEvent {
        -UnityEvent methodCall
        +Execute()
        +IsValid() bool
    }
    
    class ParameterizedMethodEvent~T~ {
        -UnityEvent~T~ methodCall
        -T parameter
        +Execute()
        +IsValid() bool
    }
    
    class StaticMethodCallEvent {
        -string className
        -string methodName
        +Execute()
        +IsValid() bool
    }
    
    class DialogNode {
        -List~DialogEvent~ startEvents
        -List~DialogEvent~ endEvents
        +ExecuteStartEvents()
        +ExecuteEndEvents()
    }
    
    class DialogChoice {
        -List~DialogEvent~ choiceEvents
        +ExecuteChoiceEvents()
    }
    
    DialogEvent <|-- MethodCallEvent
    DialogEvent <|-- ParameterizedMethodEvent
    DialogEvent <|-- StaticMethodCallEvent
    
    DialogNode *-- DialogEvent
    DialogChoice *-- DialogEvent
```

### Complete Class Hierarchy Overview

```mermaid
graph TB
    subgraph "Core Systems"
        MENU[Main Menu Events]
        GSM[GameStateManager]
        GSD[GameStateData SO]
        GM[GameManager - TBD]
        
        GSM -->|saves/loads| GSD
    end
    
    subgraph "Player Systems"
        PC[Player_Controller]
        PI[PlayerInteraction]
        INTER[Interactable Interface]
        
        PC --> PI
        PI -.implements.- INTER
    end
    
    subgraph "Dialog System"
        DMGR[DialogManager]
        DNAV[DialogNavigator]
        DTRIG[DialogueTrigger]
        
        DT[DialogTree SO]
        DN[DialogNode SO]
        DCHC[DialogChoice SO]
        NPC[NPCContent SO]
        
        DMGR --> DNAV
        DTRIG --> DMGR
        DTRIG --> NPC
        
        NPC --> DT
        DT --> DN
        DN --> DCHC
        
        DNAV -.updates.- GSM
    end
    
    subgraph "Stage Systems"
        SPOT[Spotlight]
        SPOTCTRL[SpotlightController]
        LIGHT[LightingManager]
        AUD[AudienceManager]
        
        SPOTCTRL --> SPOT
        SPOTCTRL --> LIGHT
        LIGHT --> SPOT
        
        AUD -.updates.- GSM
    end
    
    subgraph "Interfaces"
        ISC[ISpotlightCharacter]
        PCW[PlayerCharacterWrapper]
        
        PC -.implements via.- PCW
        PCW -.implements.- ISC
        SPOT --> ISC
    end
    
    subgraph "TBD Systems"
        AS[ActorStats - TBD]
        ABL[Ability - TBD]
        DS[DreamStage - TBD]
        DSM[DreamStageManager - TBD]
        MGM[MinigameManager - TBD]
    end
    
    DTRIG -.integrates.- PC
    DMGR -.controls.- PC
    SPOTCTRL -.integrates.- DMGR
    
    GSM -.provides state.- DNAV
    GSM -.provides state.- AUD
    GSM -.provides state.- LIGHT
    
    style GSM fill:#FFD700,stroke:#333,stroke-width:3px,color:#000
    style GSD fill:#90EE90,stroke:#333,stroke-width:2px,color:#000
    style DMGR fill:#90EE90,stroke:#333,stroke-width:2px,color:#000
    style DNAV fill:#90EE90,stroke:#333,stroke-width:2px,color:#000
    style DTRIG fill:#90EE90,stroke:#333,stroke-width:2px,color:#000
    style DT fill:#90EE90,stroke:#333,stroke-width:2px,color:#000
    style DN fill:#90EE90,stroke:#333,stroke-width:2px,color:#000
    style DCHC fill:#90EE90,stroke:#333,stroke-width:2px,color:#000
    style NPC fill:#90EE90,stroke:#333,stroke-width:2px,color:#000
    
    style AS fill:#FFB6C1,stroke:#333,stroke-width:2px,color:#000
    style ABL fill:#FFB6C1,stroke:#333,stroke-width:2px,color:#000
    style DS fill:#FFB6C1,stroke:#333,stroke-width:2px,color:#000
    style DSM fill:#FFB6C1,stroke:#333,stroke-width:2px,color:#000
    style MGM fill:#FFB6C1,stroke:#333,stroke-width:2px,color:#000
```

### Spotlight & Lighting Integration

```mermaid
graph LR
    subgraph "Spotlight System"
        SC[SpotlightController]
        S1[Spotlight 1]
        S2[Spotlight 2]
        S3[Spotlight 3]
        
        SC -->|controls| S1
        SC -->|controls| S2
        SC -->|controls| S3
    end
    
    subgraph "Lighting System"
        LM[LightingManager]
        GL[Global Light2D]
        CAM[Camera]
        
        LM -->|adjusts| GL
        LM -->|modifies| CAM
    end
    
    subgraph "Character Tracking"
        ISC[ISpotlightCharacter<br/>Interface]
        PLAYER[Player Character]
        NPC1[NPC Character]
        
        PLAYER -.implements.- ISC
        NPC1 -.implements.- ISC
    end
    
    subgraph "Dialog Integration"
        DM[DialogManager]
        
        DM -->|calls| SC
        DM -->|calls| LM
    end
    
    S1 -->|detects| ISC
    S2 -->|detects| ISC
    S3 -->|detects| ISC
    
    SC -->|coordinates with| LM
    SC -->|named spotlights| LM
    
    style DM fill:#90EE90,stroke:#333,stroke-width:2px,color:#000
```

### Data Structure: Dialog Tree

```mermaid
graph TD
    DT[DialogTree<br/>treeName: string<br/>startingNode: DialogNode]
    
    DN1[DialogNode 1<br/>speakerName: string<br/>dialogText: string<br/>isPlayerSpeaking: bool]
    DN2[DialogNode 2<br/>autoAdvance: bool<br/>advanceDelay: float]
    DN3[DialogNode 3<br/>nodeName: string]
    DN4[DialogNode 4 - Convergent]
    
    DC1[DialogChoice 1<br/>choiceText: string]
    DC2[DialogChoice 2<br/>choiceText: string]
    DC3[DialogChoice 3<br/>targetNodeName: string]
    
    EVT1[Start Events]
    EVT2[End Events]
    EVT3[Choice Events]
    
    DT -->|starting node| DN1
    DN1 -->|children| DN2
    DN1 -->|children| DN3
    DN2 -->|choices| DC1
    DN2 -->|choices| DC2
    DN3 -->|choices| DC3
    
    DC1 -->|next node| DN4
    DC2 -->|next node| DN4
    DC3 -->|references by name| DN4
    
    DN1 -.->|triggers| EVT1
    DN2 -.->|triggers| EVT2
    DC1 -.->|triggers| EVT3
    
    style DT fill:#FFD700,stroke:#333,stroke-width:2px,color:#000
    style DN1 fill:#90EE90,stroke:#333,stroke-width:2px,color:#000
    style DN2 fill:#90EE90,stroke:#333,stroke-width:2px,color:#000
    style DN3 fill:#90EE90,stroke:#333,stroke-width:2px,color:#000
    style DN4 fill:#87CEEB,stroke:#333,stroke-width:2px,color:#000
    style DC1 fill:#FFB6C1,stroke:#333,stroke-width:2px,color:#000
    style DC2 fill:#FFB6C1,stroke:#333,stroke-width:2px,color:#000
    style DC3 fill:#FFB6C1,stroke:#333,stroke-width:2px,color:#000
```

---

## Dialog System - Detailed Architecture

### Pattern: Model-View-Controller (MVC) with Event-Driven Communication

**Clean Architecture (Current Implementation):**
```
DialogNode (Data - Model)
    ↓
DialogNavigator (Game Logic - Controller)
    ├─ Fires: OnNodeChanged
    ├─ Fires: OnRememberScriptStarted
    ├─ Fires: OnRememberScriptProgress
    ├─ Fires: OnRememberScriptSuccess/Failure
    └─ Fires: OnDialogEnded
         ↓
    DialogManager (View - UI Controller)
         └─ Subscribes to events and renders UI
```

**Key Principle**: 
- DialogNavigator focuses purely on game logic (validation, scoring, state)
- DialogNavigator fires events when state changes
- DialogManager (or future UI controllers) subscribe to events and handle presentation
- **No UI code in DialogNavigator** - complete separation of concerns

**Current Status**:
- ✅ DialogNavigator has full minigame logic with events
- ✅ RememberTheScript events defined and functional
- ⏳ DialogManager needs to subscribe to minigame events (when UI is added)
- ⏳ Minigame UI components need to be created in DialogManager

### Documentation References

**📚 Complete Documentation:**
- **User Guide**: [`Docs/Dialog-System-Complete-Guide.md`](./Dialog-System-Complete-Guide.md) - For designers and content creators
- **Technical Reference**: [`Docs/Dialog-System-Technical-Reference.md`](./Dialog-System-Technical-Reference.md) - For developers and system integrators
- **DialogEvents Guide**: [`Docs/DialogEvents-Scaffolding-Guide.md`](./DialogEvents-Scaffolding-Guide.md) - Event system documentation
- **Workflow Guide**: [`Docs/DialogTree-Editing-Workflow-Guide.md`](./DialogTree-Editing-Workflow-Guide.md) - Detailed editor workflow
- **Node ID Guide**: [`Docs/DialogNodeID-NamingConvention.md`](./DialogNodeID-NamingConvention.md) - Naming conventions

**📦 Archived Documentation:**
- Historical development documents: [`Docs/Archive/Dialog-Development/`](./Archive/Dialog-Development/)

### Components

#### A. Data Layer (Model)
- **`DialogTree`** (ScriptableObject): Container for entire conversation tree
  - Location: `Assets/_Stage of Dreams_/World/Dialog Tree.cs`
  - Stores starting node and manages node collection
  - Provides tree traversal and validation methods
  - Supports named nodes for convergent dialog paths
  - Properties:
    - `string treeName` - Display name
    - `string description` - Tree description
    - `DialogNode startingNode` - Entry point
    - `List<DialogNode> allNodes` - All nodes in tree
    - `bool autoUpdateNodeList` - Auto-refresh setting
    - `bool validateOnSave` - Validation on save
  - Key Methods:
    - `GetStartingNode()` - Get entry point
    - `GetAllNodes()` - Get all nodes
    - `FindNodeByName(string)` - Find by ID
    - `GetConvergentNodes()` - Get nodes with multiple parents
    - `GetEndNodes()` - Get terminal nodes
    - `ValidateTree()` - Comprehensive validation
    - `RefreshNodeList()` - Update node collection
    - `ResolveNamedReferences()` - Resolve named node references
    - `CreateStartingNode()` - Create entry point
    - `AddChoiceNode()` - Add branching node
    - `AddSequentialNode()` - Add auto-advance node
  
- **`DialogNode`** (Serializable Class): Individual conversation node
  - Location: `Assets/_Stage of Dreams_/World/Dialog Node.cs`
  - Properties:
    - Node Identification:
      - `string NodeName` - Unique identifier (e.g., `act1_director_intro_01`)
    - Dialog Content:
      - `string CharacterName` - Speaker name
      - `string DialogText` - Dialog content (TextArea 3-6 lines)
      - `bool IsPlayerSpeaking` - Player dialog flag
    - Flow Control:
      - `float AutoAdvanceDelay` - Auto-advance timing (0 = manual advance)
    - Tree Structure:
      - `List<DialogNode> ParentNodes` - Incoming connections (supports convergent paths)
      - `DialogNode ChildNode` - Auto-advance target (null if has choices)
      - `List<DialogChoice> Choices` - Player choice options
    - Events:
      - `List<DialogEvent> StartEvents` - Events triggered on node start
      - `List<DialogEvent> EndEvents` - Events triggered on node end
      - `UnityEvent OnDialogStart` - Legacy start event (backwards compatibility)
      - `UnityEvent OnDialogEnd` - Legacy end event (backwards compatibility)
    - Computed Properties:
      - `bool IsRootNode` - True if no parent nodes
      - `bool HasChoices` - True if has choice options
      - `bool HasAutoAdvance` - True if has child node
  - Key Methods:
    - Hierarchy Management:
      - `SetChildNode(DialogNode)` - Set auto-advance target
      - `CreateChildNode(string speaker, string text, ...)` - Create and link child node
      - `AddParentNode(DialogNode)` - Add parent reference
      - `RemoveParentNode(DialogNode)` - Remove parent reference
    - Choice Management:
      - `AddChoice(string text, DialogNode target, string choiceId)` - Add player choice
      - `RemoveChoice(int index)` - Remove choice by index
    - Event Management:
      - `AddStartEvent(DialogEvent)` - Add start event
      - `AddEndEvent(DialogEvent)` - Add end event
      - `RemoveStartEvent(int)` - Remove start event
      - `RemoveEndEvent(int)` - Remove end event
      - `ExecuteStartEvents()` - Trigger all start events
      - `ExecuteEndEvents()` - Trigger all end events
    - Validation:
      - `IsValid()` - Validate node configuration
      - `GetDisplayName()` - Get display name for UI
  
- **`DialogChoice`** (Serializable Class): Choice option in dialog
  - Location: `Assets/_Stage of Dreams_/World/Dialog Choice.cs`
  - Properties:
    - Choice Data:
      - `string ChoiceText` - Display text for choice
      - `string ChoiceId` - Unique identifier for custom actions
      - `List<bool> Conditions` - Conditions to show choice (not implemented yet)
    - References:
      - `DialogNode ParentNode` - Node this choice belongs to
      - `DialogNode TargetNode` - Direct target node reference
      - `string TargetNodeName` - Named target for convergent paths
    - Events:
      - `List<DialogEvent> ChoiceEvents` - Events triggered on selection
      - `UnityEvent OnChoiceSelected` - Legacy choice event (backwards compatibility)
    - Computed Properties:
      - `bool HasNamedTarget` - True if using named reference
      - `bool HasValidTarget` - True if has target (direct or named)
      - `bool HasEvents` - True if has events to execute
  - Key Methods:
    - Event Management:
      - `AddChoiceEvent(DialogEvent)` - Add choice event
      - `RemoveChoiceEvent(int)` - Remove choice event
      - `ExecuteChoiceEvents()` - Trigger all choice events
    - Target Management:
      - `CreateTargetNode(string speaker, string text, ...)` - Create and set target node
      - `SetTarget(DialogNode)` - Set direct target reference
      - `SetTargetByName(string)` - Set named target for convergent paths
      - `ResolveNamedTarget(DialogTree)` - Resolve named target to actual node
    - Validation:
      - `IsValid()` - Validate choice configuration
      - `IsTargetResolved()` - Check if named target is resolved
      - `GetDisplayName()` - Get display name for UI
      - `GetTargetInfo()` - Get target information for debugging
- **`DialogEvent`** (Abstract Base Class): Polymorphic event system
  - Location: `Assets/_Stage of Dreams_/Scripts/Dialog/DialogEvents.cs`
  - Base Properties:
    - `string eventName` - Event identifier
    - `bool isEnabled` - Enable/disable flag
    - `string description` - Event description
    - `Action onExecuteDelegate` - Method delegate support
    - `Func<bool> validationDelegate` - Custom validation
  - Base Methods:
    - `Execute()` - Trigger event
    - `IsValid()` - Validate event
    - `SetExecuteDelegate(Action)` - Set method delegate
    - `SetValidationDelegate(Func<bool>)` - Set validation method
  - Implemented Event Types:
    - `MethodCallEvent` - Execute Action delegates
    - `ParameterizedMethodEvent<T>` - Execute Action<T> with parameters
    - `StaticMethodCallEvent` - Call static methods via reflection
  - Usage: See [`DialogEvents-Scaffolding-Guide.md`](./DialogEvents-Scaffolding-Guide.md)

#### B. Logic Layer (Controller)
- **`DialogNavigator`**: Navigation logic engine
  - Location: `Assets/_Stage of Dreams_/Scripts/Dialog/DialogNavigator.cs`
  - Properties:
    - `DialogNode currentNode` - Current position in tree
    - `NPCContent currentNPC` - Current NPC context
    - `DialogTree currentTree` - Current tree
    - `bool IsActive` - Navigation active flag
  - Events:
    - `Action<DialogNode> OnNodeChanged` - Node navigation event
    - `Action<DialogChoice, NPCContent> OnCustomActionTriggered` - Custom action event
    - `Action OnDialogEnded` - Dialog completion event
  - Key Methods:
    - `StartDialog(NPCContent, string)` - Begin navigation
    - `NavigateToNode(DialogNode)` - Move to specific node
    - `SelectChoice(int)` - Handle choice selection
    - `AdvanceDialog()` - Advance to next node
    - `EndDialog()` - Complete navigation
    - `SwitchToTree(string)` - Change dialog tree
    - `GetCurrentState()` - Get navigation state
  
- **`DialogueTrigger`**: Interaction trigger component
  - Location: `Assets/_Stage of Dreams_/Scripts/Dialog/DialogueTrigger.cs`
  - Properties:
    - `bool triggerOnSpotlight` - Spotlight trigger flag
    - `bool triggerOnInteraction` - Interaction trigger flag
    - `bool triggerOnProximity` - Proximity trigger flag
    - `float interactionRadius` - Interaction range
    - `float proximityRadius` - Proximity range
    - `bool requireInteractionInput` - Require key press
    - `NPCContent targetNPC` - Target NPC reference
    - `string specificDialogTree` - Specific tree name
    - `bool canRetrigger` - Allow retriggering
    - `float retriggerDelay` - Retrigger cooldown
  - Events:
    - `Action OnDialogTriggered` - Dialog started event
    - `Action OnDialogEnded` - Dialog ended event
  - Key Methods:
    - `ManualTrigger()` - Force trigger
    - `SetTriggerType(bool, bool, bool)` - Configure trigger types
    - `ValidateSetup()` - Validate configuration
    - `IsReadyToTrigger()` - Check trigger readiness
    - `GetSetupStatus()` - Get setup status string
    - `GetTargetDialogTree()` - Get target tree

#### C. View Layer
- **`DialogManager`**: UI controller (Singleton)
  - Location: `Assets\_Stage of Dreams_\Scripts\Dialog\DialogManager.cs`
  - Properties:
    - `static DialogManager Instance` - Singleton instance
    - `UIDocument uiDocument`
    - `VisualTreeAsset dialogVisualTree`
    - `string interactActionName = "Interact"`
    - `bool enableInputLogging`, `enableDebugLogs`, `validateOnStart`
    - `GroupBox dialogBox`, `Label dialogLabel`
    - `Button choiceButton1-5`
    - `PlayerInput playerInput`, `InputAction interactAction`
    - `DialogNavigator navigator`
    - `bool isUIActive`, `isInitialized`
    - `NPCContent currentNPC`, `DialogTree currentTree`
    - `int dialogSessionCount`
  - Methods:
    - `Awake()`, `Start()`, `Update()`, `OnDestroy()`
    - `InitializeDialogManager()`, `InitializeNavigator()`, `InitializeUI()`, `InitializeInput()`
    - `ValidateSetup()`, `ValidateNPCContent()`
    - `StartDialog(NPCContent)`, `StartDialog(NPCContent, string)`
    - `HandleNodeChanged(DialogNode)`, `HandleCustomAction()`, `HandleDialogEnded()`
    - `DisplayNode(DialogNode)`, `ShowChoices()`, `HideChoices()`, `HideDialog()`
    - `OnChoiceClicked(int)`, `AdvanceDialog()`, `EndDialog()`
    - `IsDialogActive()`, `GetNavigationState()`, `IsReady()`
  - Events:
    - `OnDialogStarted`, `OnDialogEnded`, `OnNodeDisplayed`, `OnCustomActionHandled`
  - Description: Singleton dialog manager handling UI display and integration with DialogNavigator, supports UI Toolkit
  - **Future Enhancement**: Will subscribe to DialogNavigator minigame events (OnRememberScriptStarted, OnRememberScriptProgress, etc.) when minigame UI is implemented

- **`DialogNavigator`**: Navigation logic engine
  - Location: `Assets/_Stage of Dreams_/Scripts/Dialog/DialogNavigator.cs`
  - Properties:
    - `DialogNode currentNode` - Current position in tree
    - `NPCContent currentNPC` - Current NPC context
    - `DialogTree currentTree` - Current tree being navigated
    - `bool IsActive` - Whether navigation is active
    - `DialogNode CurrentNode` - Public accessor for current node
    - `NPCContent CurrentNPC` - Public accessor for current NPC
  - Methods:
    - `StartDialog(NPCContent, string)` - Begin dialog navigation
    - `NavigateToNode(DialogNode)` - Move to specific node
    - `SelectChoice(int)` - Handle player choice selection
    - `AdvanceDialog()` - Advance to next node (auto-advance or manual)
    - `EndDialog()` - Complete dialog session
    - `SwitchToTree(string)` - Change to different dialog tree
    - `ForceNavigateToNode(DialogNode)` - Force navigation to specific node
    - `GetCurrentState()` - Get current navigation state
    - `ExecuteNodeStartEvents(DialogNode)` - Execute node start events
    - `ExecuteNodeEndEvents(DialogNode)` - Execute node end events
    - `ExecuteChoiceEvents(DialogChoice)` - Execute choice events
  - Events:
    - `OnNodeChanged` - Fired when navigating to new node
    - `OnCustomActionTriggered` - Fired for custom choice actions
    - `OnDialogEnded` - Fired when dialog completes
  - Description: Handles dialog tree navigation logic, separate from UI concerns. Pure logic component that coordinates with DialogManager.

- **`DialogueTrigger`**: Interaction trigger component
  - Location: `Assets/_Stage of Dreams_/Scripts/Dialog/DialogueTrigger.cs`
  - Properties:
    - `bool triggerOnSpotlight`, `triggerOnInteraction`, `triggerOnProximity`
    - `float interactionRadius = 2f`, `proximityRadius = 1.5f`
    - `bool requireInteractionInput = true`
    - `KeyCode interactionKey = KeyCode.E`
    - `NPCContent targetNPC`
    - `string specificDialogTree`
    - `bool canRetrigger`, `float retriggerDelay = 10f`
    - `DialogManager dialogManager`, `PlayerScript player`
    - `bool hasTriggered`, `isWaitingForInput`
  - Methods:
    - `Start()`, `Update()`, `OnDestroy()`, `OnDisable()`
    - `InitializeReferences()`, `ValidateSetup()`, `ValidateNPCContent()`
    - `IsReadyToTrigger()`, `CheckSpotlightTrigger()`, `CheckInteractionTrigger()`, `CheckProximityTrigger()`
    - `PassNPCContent()`, `PrepareNPCForDialog()`
    - `SubscribeToDialogEvents()`, `HandleDialogManagerEnded()`
    - `ResetTrigger()`, `ManualTrigger()`
    - `SetTriggerType(bool spotlight, bool interaction, bool proximity)`, `SetTargetNPC(NPCContent)`
    - `GetTargetDialogTree()`, `GetSetupStatus()`
  - Events:
    - `OnDialogTriggered`, `OnDialogEnded`
  - Description: Unified dialog trigger system supporting multiple trigger conditions (spotlight, interaction, proximity) and comprehensive validation

### ✅ DialogEvents.cs
- **Pattern:** Polymorphic Event System
- **Location:** `Assets/_Stage of Dreams_/Scripts/Dialog/DialogEvents.cs`
- **Classes:**
  - **DialogEvent** (Abstract Base):
    - Properties: `eventName`, `isEnabled`, `description`
    - Methods: `Execute()`, `IsValid()`, `SetExecuteDelegate(Action)`, `SetValidationDelegate(Func<bool>)`
  - **MethodCallEvent**:
    - Executes Action delegates
    - Properties: `methodToCall`, `methodDescription`
    - Methods: `SetMethod(Action, string)`
  - **ParameterizedMethodEvent<T>**:
    - Executes Action<T> delegates with parameters
    - Properties: `methodToCall`, `parameter`, `methodDescription`
    - Methods: `SetMethod(Action<T>, T, string)`
  - **StaticMethodCallEvent**:
    - Calls static methods via reflection
    - Properties: `className`, `methodName`, `stringParameters`
- **Description:** Type-safe event system for dialog integration with game systems. Supports method delegates and custom event types.

### Integration Flow
```
Player Interaction → DialogueTrigger → DialogManager → DialogNavigator
                                            ↓
                                       NPC Content
                                            ↓
                                       Dialog Tree
```

### Key Features
- **Convergent Paths**: Multiple choices can lead to same node via named references
- **Event System**: Custom events can be triggered at node start/end or choice selection
- **Validation**: Comprehensive validation at multiple levels (tree, node, choice)
- **Auto-Advance**: Nodes can automatically progress after delay
- **Method Delegates**: Events support both UnityEvents and direct method calls
- **Editor Windows**: Dedicated windows for complex tree editing
- **Tree Navigation**: Navigate between nodes using parent/child/choice references
- **Named Nodes**: Reference nodes by name for convergent dialog paths

### Event System Pattern
**Observer pattern with polymorphic event types**

#### Usage Pattern:
```csharp
// At node start/end
node.AddStartEvent(new MethodCallEvent());
node.AddEndEvent(new MethodCallEvent());

// At choice selection
choice.AddChoiceEvent(new ParameterizedMethodEvent<string>());

// With method delegates
var evt = new MethodCallEvent();
evt.SetMethod(() => GameStateManager.Instance.AdjustApplause(10f));
node.AddStartEvent(evt);
```

### Editor Tools

#### DialogTreeEditor
- Location: `Assets\_Stage of Dreams_\Editor\DialogTreeEditor.cs`
- Custom inspector for DialogTree
- Read-only workflow with edit buttons
- Quick tree builder
- Validation and debugging tools

#### DialogNodeEditorWindow
- Location: `Assets\_Stage of Dreams_\Editor\DialogNodeEditorWindow.cs`
- Dedicated window for editing standard dialog nodes
- Tree navigation buttons (parents, children, choice targets)
- Choice management interface
- Unity Events and DialogEvents integration
- **Smart routing**: Detects specialized node types and redirects to appropriate editor

#### RememberTheScriptNodeEditor
- Location: `Assets\_Stage of Dreams_\Editor\RememberTheScriptNodeEditor.cs`
- **NEW**: Specialized editor for RememberTheScript minigame nodes
- Focused UI showing only minigame-relevant settings:
  - Target phrase configuration with character count
  - Difficulty settings (max mistakes, time limit, case sensitivity)
  - Score settings (success reward, mistake penalty)
  - Success/failure node navigation
  - Estimated difficulty rating calculator
  - Score preview for perfect/worst runs
- Test minigame button for quick validation
- Automatic visual preview of target phrase
- Integration with tree navigation system

#### DialogChoiceEditorWindow
- Location: `Assets\_Stage of Dreams_\Editor\DialogChoiceEditorWindow.cs`
- Dedicated window for editing dialog choices
- Choice-specific UI and validation

#### DialogTreeUtilities
- Location: `Assets\_Stage of Dreams_\Editor\DialogTreeUtilities.cs`
- Menu items: `Tools > Dialog System`
- Node ID generator
- Tree validation
- Batch operations

### Editor Architecture Pattern

**Specialized Editor Windows for Minigames**

The editor system uses a **router pattern** where `DialogNodeEditorWindow.OpenWindow()` detects node types and routes to specialized editors:

```
Node Edit Request
    ↓
DialogNodeEditorWindow.OpenWindow()
    ├─ IsRememberTheScriptNode? → RememberTheScriptNodeEditor
    ├─ IsCalmDialogNode? → CalmDialogNodeEditor (future)
    └─ Default → DialogNodeEditorWindow (standard dialog)
```

**Benefits:**
- Single Responsibility: Each editor handles one node type
- Scalability: New minigames = new editors (no modification to existing code)
- Clean UI: Only relevant fields shown for each node type
- Easy Maintenance: Changes isolated to specific editors

**Adding New Minigame Editors:**
1. Create new editor class (e.g., `CalmDialogNodeEditor.cs`)
2. Add detection method in `DialogNodeEditorWindow` (e.g., `IsCalmDialogNode()`)
3. Add routing logic in `OpenWindow()` method
4. Update documentation

---
