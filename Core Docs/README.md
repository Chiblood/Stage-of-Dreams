# Stage of Dreams

## 🎭 A 2D Top-Down Theater Performance Game

**Stage of Dreams** is a Unity 2D game where players navigate dream-like theater stages, engaging in dialogue, spotlight mechanics, and performance-based minigames. Experience the thrill of live theater with turn-based combat enhanced by audience reactions and applause meters.

---

## 🎮 Game Overview

Navigate through dream stages as a theater performer, mastering the art of live performance through:

- **Interactive Dialog System**: Branching conversations with NPCs using a sophisticated node-based dialog tree
- **Dynamic Spotlight Mechanics**: Performance areas triggered by entering spotlights on a theater stage
- **Audience Feedback System**: Real-time applause and boo meters that affect gameplay progression
- **Performance Minigames**: Four unique minigames testing different theatrical skills
- **Atmospheric Lighting**: Dynamic lighting effects that enhance the theatrical experience

---

## ⚡ Core Gameplay Features

### 💡 Spotlight Performance System
- Move through theater stages with dynamic spotlight detection
- Spotlights trigger performance opportunities and dialog encounters
- Advanced lighting system with smooth transitions and dramatic effects

### 💬 Integrated Dialog System
- **Node-based conversations** with branching choices and consequences
- **Minigame integration** - Seamless transition from dialog to gameplay challenges
- **Multiple trigger types**: Spotlight-based, proximity, and interaction triggers
- **Reusable dialog trees** stored as ScriptableObject assets
- **Event-driven architecture** for seamless game integration
- **Custom editor tools** - Specialized editing windows for each node type

### 🎯 Performance Minigames
- **CalmDialogue**: Choose correct dialog options based on the script (1 of 3 choices) ⏳
- **RememberTheScript**: Type scripted lines quickly and accurately ✅
- **DramaticLock**: Button masher followed by script recitation for dramatic tension ⏳
- **DancingCombat**: Rhythm-based arrow key sequences for choreographed combat ⏳

### 👥 Audience Interaction
- **Applause Meter**: Increases with successful performances
- **Boo Meter**: Penalty for poor performance, requires retry until success
- **Dynamic Reactions**: Audience responds with applause, laughter, gasps, and boos
- **Mood System**: Overall audience engagement affects difficulty and rewards

---

## 🏗️ Technical Architecture

### Core Systems
- **Unity 6000.2.6f2** with **Universal Render Pipeline (URP)** for advanced 2D lighting
- **.NET Standard 2.1** target for Unity compatibility
- **UI Toolkit** for modern, scalable user interfaces
- **Input System Package** for robust input handling

### Dialog System Architecture
```
DialogNode → DialogTree → NPCContent → DialogueTrigger → DialogManager (Singleton)
                ↓
         DialogNavigator (Game Logic)
```

**Key Innovation**: Minigames are integrated as **node types** within the dialog system, enabling seamless transitions and automatic outcome node creation.

### Key Components
- **DialogManager**: Singleton UI manager for all dialog display
- **DialogNavigator**: Pure navigation logic for dialog flow with event-driven architecture
- **DialogueTrigger**: Unified system supporting multiple trigger types
- **NPCContent**: Content definition with custom action support
- **GameStateManager**: Centralized state tracking with save/load infrastructure
- **Spotlight System**: Advanced character detection with Light2D integration
- **LightingManager**: Global lighting control for dramatic effects
- **AudienceManager**: Audience reaction and feedback system

---

## 📁 Project Structure

```
Assets/
└─ _Stage of Dreams_/
   ├─ Scripts/
   │  ├─ Dialog/               # Complete dialog system with minigame integration
   │  ├─ PlayerScripts/        # Player movement and interaction
   │  ├─ StageScripts/         # Spotlight, lighting, and stage logic
   │  └─ GameState/            # GameStateManager and save system
   ├─ World/                   # Dialog trees and NPC content
   ├─ Scenes/                  # Game scenes and levels
   └─ UI/                      # UI Toolkit assets and stylesheets

Core Docs/                     # Comprehensive documentation
```

---

## 🎨 Art & Audio Vision

- **Pixel Art Style**: Character sprites and tilesets for theater environments
- **Dynamic Lighting**: 2D lighting system for dramatic stage effects
- **Atmospheric Audio**: Stage-appropriate sound effects and ambient music
- **UI Design**: Clean, theater-themed interface using UI Toolkit

---

## 📊 Development Status

### ✅ Implemented Systems
- **Player Movement & Control**: Complete 2D movement with physics integration
- **Dialog System**: Full node-based dialog with multiple trigger types and minigame integration
- **Minigame Framework**: RememberTheScript minigame with core logic complete
  - Character-by-character validation
  - Mistake tracking and reset logic
  - Timer system with time-based scoring
  - GameStateManager integration
  - Event-driven architecture ready for UI integration
- **Node Type System**: Extensible dialog node types with specialized editors
- **Spotlight System**: Advanced spotlight detection and following
- **Lighting Management**: Dynamic lighting transitions and dramatic effects
- **Audience System**: Reaction management with audio and particle effects
- **Game State Management**: Centralized state tracking with save/load infrastructure
- **UI Framework**: Modern UI Toolkit implementation
- **Main Menu**: Scene transitions and application management
- **Documentation**: Comprehensive architecture documentation with Mermaid diagrams

### ⏳ In Development
- **RememberTheScript UI Integration**: UI components for minigame display
- **Additional Minigames**: CalmDialog, DramaticLock, DancingCombat
- **Actor Stats**: Health, applause, and performance metrics UI
- **Stage Management**: Multi-act stage progression system
- **Demo Scene**: Showcase scene for presentation

### 🎯 Technical Achievements
- **Minigame = Node Paradigm**: Unified data model eliminating separate minigame managers
- **Router Pattern**: Automatic editor routing based on node type
- **Clean Separation of Concerns**: Data (DialogNode) → Logic (DialogNavigator) → View (DialogManager)
- **Automatic Outcome Nodes**: Success/failure nodes created automatically for minigames
- **Event-Driven Architecture**: UI updates through events, no tight coupling

---

## 🎯 Target Platforms

- **Primary**: Windows PC
- **Secondary**: WebGL for browser play
- **Future**: Potential mobile adaptation

---

## 🗺️ Development Goals

### Phase 1: Core Mechanics ✅
- Player movement and basic interaction systems
- Dialog system with branching conversations
- Spotlight detection and lighting effects

### Phase 2: Performance Systems ⏳ (Current Phase)
- Implementation of all four minigames
- Audience feedback integration
- Stage progression mechanics

### Phase 3: Content Creation
- Multiple dream stages with unique themes
- Complete narrative integration
- Audio and visual polish

### Phase 4: Release Preparation
- Bug testing and performance optimization
- Build preparation for target platforms
- Documentation and player guides

---

## 🚀 Getting Started (Development)

### Prerequisites
- Unity 6000.2.6f2 or later
- .NET Standard 2.1
- UI Toolkit package
- Input System package
- Universal Render Pipeline (2D)

### Setup
1. Clone the repository
2. Open in Unity Hub
3. Ensure all required packages are installed
4. Open the Main Menu scene to start

### Key Scripts to Examine
- `DialogManager.cs` - Central dialog system management
- `DialogNavigator.cs` - Pure game logic for dialog flow and minigames
- `DialogNode.cs` - Node data structure with minigame support
- `Spotlight.cs` - Advanced spotlight detection
- `PlayerScript.cs` - Player movement and control
- `GameStateManager.cs` - Centralized state management
- `LightingManager.cs` - Global lighting control
- `AudienceManager.cs` - Audience reaction system

---

## 📚 Documentation

Comprehensive documentation is available in the `/Core Docs` folder:

- **[README.md](README.md)**: Project overview and quick start guide (this file)
- **[Presentation-Dialog-System-Architecture.md](Presentation-Dialog-System-Architecture.md)**: Detailed presentation on dialog system architecture
- **[Class Hierarchy](../Docs/Class%20Hierarchy.md)**: Complete system architecture overview with Mermaid diagrams
- **[Dialog System Guide](../Docs/Examples%20and%20Guides/Dialog%20System%20Guide.md)**: Detailed dialog implementation guide
- **[Project Roadmap](../Docs/Project_Roadmap.md)**: Development phases and progress tracking
- **[Requirements](../Docs/Requirements.md)**: Game design requirements and specifications

---

## 💡 About the Vision

**Stage of Dreams** explores the magical, surreal world of live theater through the lens of a performer's dreams. Each stage represents a different aspect of theatrical performance - from overcoming stage fright to mastering complex choreography. The game celebrates the art of live performance while providing engaging gameplay that captures the excitement, tension, and joy of being on stage.

The unique combination of turn-based mechanics, audience interaction, and performance minigames creates a gameplay experience that feels distinctly theatrical while remaining accessible to players of all backgrounds.

---

## 🤝 Contributing

This is a solo development project, but feedback and suggestions are welcome! Check the Issues tab for known bugs or feature requests.

---

## 📄 License

This project is developed as a personal game development showcase. All rights reserved.

---

**"The stage is set, the lights are bright, and the audience awaits your performance. Break a leg!"** 🎭✨
