# 🎮 Stage of Dreams – Development Roadmap

**🚨 PRESENTATION DEADLINE: 2 Working Days Remaining**

---

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

## Phase 3: Stage Construction ✅ COMPLETE
- [x] Create NPCContent to contain Dialog Trees, Inventories, Abilities, Quests, and other NPC-specific data
- [x] Build first dream stage
- [x] Implement stage transition mechanics (Main Menu → Main Stage)
- [x] Create advanced stage lighting and atmosphere system
	- [x] LightingManager for global lighting control
	- [x] SpotlightController for coordinated spotlight effects
	- [x] Dynamic lighting transitions and dramatic effects
- [x] Audience reaction system with audio and particle effects

## Phase 4: Game State & Minigames 🔄 IN PROGRESS - PRIORITY
- [x] **GameStateManager singleton** - Centralized state tracking (⚠️ COMPLETED)
	- [x] Audience metrics (applause, boo, mood)
	- [x] Performance scores (scene, dream, total)
	- [x] Player progression (abilities, achievements)
	- [x] Turn-based state tracking
	- [x] Session data and minigame statistics
	- [x] Event-driven UI updates
	- [x] Save/load infrastructure with ScriptableObjects
- [x] **GameStateData ScriptableObject** - Persistent state container
- [x] AudienceManager with applause/boo reaction system (⚠️ NEEDS GAMESTATE INTEGRATION)
- [ ] **CalmDialog Minigame** - NEXT PRIORITY
	- [ ] Implement in DialogNode system
	- [ ] Create UI for phrase selection
	- [ ] Integrate with GameStateManager
	- [ ] Add audience feedback
- [ ] **RememberTheScript Minigame** - NEXT PRIORITY
	- [ ] Implement typing mechanics
	- [ ] Create real-time letter validation UI
	- [ ] Integrate with GameStateManager
	- [ ] Add mistake tracking and reset logic
- [ ] MinigameManager for coordinating minigames (⏸️ DEFERRED - not critical for demo)
- [ ] Additional minigames (⏸️ DEFERRED):
	- [ ] DancingCombat minigame (arrow key rhythm sequences)
	- [ ] DramaticLock minigame (button mash + script typing)
- [ ] Quest trigger system for performances (⏸️ DEFERRED)

## Phase 5: Performance Systems 🔄 FOUNDATION COMPLETE
- [x] **GameStateManager** - Core performance tracking
- [x] Audience feedback system (reactions, mood, applause)
- [ ] Minigame integration with audience feedback (⏸️ DEPENDS ON PHASE 4)
- [ ] Performance rewards system (⏸️ DEFERRED)
- [ ] Complete game state management for performance flow (⏸️ DEFERRED)

## Phase 6: Content & Systems Integration 🔄 PARTIALLY COMPLETE
- [x] Main menu system with scene transitions
- [x] Dialog system with comprehensive validation and events
- [x] Spotlight system with character detection and movement
- [x] Lighting effects with smooth transitions
- [x] **GameStateManager** - Global state coordination
- [ ] GameManager for overall game state coordination (⏸️ OPTIONAL - GameStateManager covers core needs)
- [ ] DreamStage system for Act I, II, III structure (⏸️ DEFERRED)
- [ ] DreamStageManager for stage transitions (⏸️ DEFERRED)

## Phase 7: Audio & Visual Polish 🔄 FOUNDATION READY
- [ ] Add sound effects and music (⏸️ NICE-TO-HAVE)
- [x] Advanced lighting system with dramatic effects
- [x] UI Toolkit implementation for modern UI
- [ ] Implement screen transitions and effects (⏸️ NICE-TO-HAVE)
- [ ] Polish visual feedback systems (⏸️ NICE-TO-HAVE)

## Phase 8: Content & Narrative 🔄 STRUCTURE READY
- [x] Dialog tree structure and tooling
- [ ] Complete narrative content for demo scene (🚨 PRIORITY)
- [x] Title screen and main menu (basic implementation)
- [ ] Credits and complete game flow (⏸️ DEFERRED)
- [ ] Multiple dream stages with unique themes (⏸️ DEFERRED)

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

## 🚨 2-DAY PRESENTATION PLAN

### Current Status Summary

#### ✅ **Fully Implemented (70% Complete)**
- **Core Systems**: Player movement, input handling, scene management
- **Dialog System**: Complete with validation, events, and UI integration
- **Spotlight System**: Advanced detection, movement patterns, and integration
- **Lighting System**: Global lighting control and dramatic effects
- **Audience System**: Reaction management with audio/visual feedback
- **UI Framework**: Modern UI Toolkit implementation
- **GameStateManager**: Centralized state tracking (✅ COMPLETE)
- **Integration**: Cross-system communication and event handling

#### 🔄 **In Progress (20% Started - PRESENTATION FOCUS)**
- **Minigame Systems**: Architecture planned, implementation needed
	- GameStateManager ready for integration ✅
	- CalmDialog proposal complete, ready to implement
	- RememberTheScript proposal complete, ready to implement
- **Demo Content**: Need one complete playable scene
- **GameState Integration**: Connect existing systems to GameStateManager

#### 🟡 **Deferred (10% - Post-Presentation)**
- **Advanced Features**: MinigameManager, additional minigames, multi-act system
- **Content Creation**: Multiple stages, full narrative
- **Audio Integration**: Sound effects and music
- **Polish & Optimization**: Performance tuning, visual effects
- **Deployment**: Build pipeline and platform optimization

---

### Minimum Viable Demo Features

**MUST HAVE (for presentation):**
- ✅ Player movement and basic interaction
- ✅ Dialog system working
- ⏳ CalmDialog minigame functional
- ⏳ RememberTheScript minigame functional
- ⏳ GameState showing scores/applause
- ⏳ One complete demo scene (5-10 minutes)
- ✅ Title screen → Main scene transition

**NICE TO HAVE (if time permits):**
- ⏸️ Minigame failure states
- ⏸️ Advanced audience reactions
- ⏸️ Sound effects and music
- ⏸️ Visual polish and effects

**CAN SKIP (post-presentation):**
- ❌ Multiple dream stages
- ❌ Full narrative content
- ❌ Achievement system
- ❌ Save/load functionality
- ❌ DancingCombat and DramaticLock minigames

---

### Risk Mitigation

**Potential Blockers:**
1. **UI Integration Issues**: Have fallback simple text-based UI ready
2. **GameState Event Problems**: Fallback to direct method calls
3. **Input System Bugs**: Test early and often, have backup input handling
4. **Time Overruns**: CalmDialog is simpler - prioritize it, RememberTheScript is optional

**Backup Plan:**
- If RememberTheScript takes too long → Skip it, focus on CalmDialog only
- If both minigames problematic → Use basic dialog choices to demonstrate system
- If UI polish not ready → Focus on functionality over aesthetics

---

## Technical Achievements

- ✅ **Modern Unity Architecture**: UI Toolkit, Input System, URP 2D
- ✅ **Robust Dialog System**: Event-driven with comprehensive validation
- ✅ **Advanced Lighting**: Smooth transitions and dramatic effects
- ✅ **Flexible Spotlight System**: Multiple movement patterns and detection
- ✅ **Audience Integration**: Dynamic reactions with audio/visual feedback
- ✅ **Clean Code Architecture**: Separation of concerns and maintainable design
- ✅ **GameStateManager Singleton**: Centralized state tracking with events (⚠️ NEWLY COMPLETE)
- ✅ **Save/Load Infrastructure**: ScriptableObject-based persistence (⚠️ NEWLY COMPLETE)

---

## Next Immediate Actions

### Right Now (Next 30 Minutes)
1. Review CalmDialog proposal (Docs\CalmDialog-Minigame-Proposal.md)
2. Review RememberTheScript proposal (Docs\RememberTheScript-Minigame-Proposal.md)
3. Verify GameStateManager is in scene and configured
4. Backup current project state

---

**Current Branch**: Feature/Minigames  
**Last Updated**: January 2025 (Pre-Presentation Push)  
**Status**: 🚨 CRUNCH MODE - 2 Days to Presentation

The project has a solid technical foundation. The focus for the next 48 hours is implementing the two core minigames (CalmDialog and RememberTheScript) and creating one polished demo scene that showcases the systems.

**You can do this! The foundation is strong - now it's time to bring it to life!** 💪🎭
