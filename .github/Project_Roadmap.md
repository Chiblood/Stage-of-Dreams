# 🎮 Stage of Dreams – Development Roadmap

## Phase 1: Foundation & Setup ✅ COMPLETE
- [x] Create Unity 2D project
- [x] Organize folders: Scripts, Art, Audio, UI, Scenes
- [x] Download assets like Character sprites, UI

## Phase 2: Core Movement & Interaction ✅ COMPLETE
- [x] Implement player movement
- [x] Implement spotlight logic with advanced features
- [x] Create Dialogue system with nodes-based structure
	- [x] DialogManager Class to display UI and manage flow
	- [x] DialogNavigator Class to handle Dialog node transitions and pass what to display to the DialogManager
	- [x] DialogNode.cs with DialogChoice class to provide the structure for dialog text, choices, and method calls
	- [x] DialogTree.cs to hold a collection of DialogNodes and provide methods to navigate between them, will act as my basis for specific NPC Dialog
	- [x] DialogTrigger.cs to attach to NPCs, Spotlight, or other conditions to trigger dialog trees from NPC Content
- [x] Implement Dialogue UI using UI Toolkit
- [x] Create comprehensive interaction framework

## Phase 3: Stage Construction ✅ MOSTLY COMPLETE
- [x] Create NPCContent to contain Dialog Trees, Inventories, Abilities, Quests, and other NPC-specific data
- [x] Build first dream stage
- [x] Implement stage transition mechanics (Main Menu → Main Stage)
- [x] Create advanced stage lighting and atmosphere system
	- [x] LightingManager for global lighting control
	- [x] SpotlightController for coordinated spotlight effects
	- [x] Dynamic lighting transitions and dramatic effects
- [x] Audience reaction system with audio and particle effects

## Phase 4: Combat Minigames 🔄 IN PROGRESS
- [ ] Create turn-based combat system with applause/boo meters
	- [x] AudienceManager with applause/boo reaction system
	- [ ] ActorStats for health, applause, and performance tracking
	- [ ] MinigameManager for coordinating all minigames
- [ ] Implement minigame systems:
	- [ ] CalmDialogue minigame (3 dialogue options, 1 correct)
	- [ ] RememberTheScript minigame (type out words/phrases)
	- [ ] DancingCombat minigame (arrow key rhythm sequences)
	- [ ] DramaticLock minigame (button mash + script typing)
- [ ] Add quest trigger system for performances

## Phase 5: Performance Systems 🔄 IN PROGRESS
- [ ] Complete minigame integration with audience feedback
- [x] Audience feedback system (reactions, mood, applause)
- [ ] Performance rewards system
- [ ] Game state management for performance flow

## Phase 6: Content & Systems Integration 🔄 PARTIALLY COMPLETE
- [x] Main menu system with scene transitions
- [x] Dialog system with comprehensive validation and events
- [x] Spotlight system with character detection and movement
- [x] Lighting effects with smooth transitions
- [ ] GameManager for overall game state coordination
- [ ] DreamStage system for Act I, II, III structure
- [ ] DreamStageManager for stage transitions

## Phase 7: Audio & Visual Polish 🔄 FOUNDATION READY
- [ ] Add sound effects and music
- [x] Advanced lighting system with dramatic effects
- [x] UI Toolkit implementation for modern UI
- [ ] Implement screen transitions and effects
- [ ] Polish visual feedback systems

## Phase 8: Content & Narrative 🔄 STRUCTURE READY
- [x] Dialog tree structure and tooling
- [ ] Complete narrative content for all stages
- [x] Title screen and main menu (basic implementation)
- [ ] Credits and complete game flow
- [ ] Multiple dream stages with unique themes

## Phase 9: Testing & Deployment 🟡 PENDING
- [ ] Fix bugs and test all dream transitions
- [ ] Performance optimization
- [ ] Complete build system for Windows and WebGL
- [ ] Export and submit to itch.io

## Phase 10: Extra Features & Expansion 🟡 FUTURE
- [ ] Implement mask system and prop-based abilities (Ability.cs)
- [ ] Add 2–3 additional dream stages with unique tones
- [ ] Create character progression/unlocks system
- [ ] Advanced audience member system (individual reactions)

---

## Current Status Summary

### ✅ **Fully Implemented (60% Complete)**
- **Core Systems**: Player movement, input handling, scene management
- **Dialog System**: Complete with validation, events, and UI integration
- **Spotlight System**: Advanced detection, movement patterns, and integration
- **Lighting System**: Global lighting control and dramatic effects
- **Audience System**: Reaction management with audio/visual feedback
- **UI Framework**: Modern UI Toolkit implementation
- **Integration**: Cross-system communication and event handling

### 🔄 **In Progress (30% Started)**
- **Game State Management**: Basic structure, needs completion
- **Minigame Systems**: Architecture planned, implementation needed
- **Stage Management**: Framework ready, content integration needed
- **Performance Metrics**: Audience system ready, stat tracking needed

### 🟡 **Planned (10% Not Started)**
- **Content Creation**: Narrative content and multiple stages
- **Audio Integration**: Sound effects and music implementation
- **Polish & Optimization**: Performance tuning and visual effects
- **Deployment**: Build pipeline and platform optimization

---

## Next Priority Tasks

1. **Implement MinigameManager and ActorStats** to complete the performance system foundation
2. **Create GameManager** for overall state coordination
3. **Develop the four core minigames** with audience integration
4. **Build DreamStage system** for multi-act progression
5. **Add audio integration** to enhance theatrical atmosphere

---

## Technical Achievements

- **Modern Unity Architecture**: UI Toolkit, Input System, URP 2D
- **Robust Dialog System**: Event-driven with comprehensive validation
- **Advanced Lighting**: Smooth transitions and dramatic effects
- **Flexible Spotlight System**: Multiple movement patterns and detection
- **Audience Integration**: Dynamic reactions with audio/visual feedback
- **Clean Code Architecture**: Separation of concerns and maintainable design

The project has established a solid technical foundation with most core systems implemented and integrated. The focus now shifts to gameplay content creation and minigame implementation.
