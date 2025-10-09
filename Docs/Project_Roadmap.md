# 🎮 Stage of Dreams – Development Roadmap

## Phase 1: Foundation & Setup
- [x] Create Unity 2D project
- [x] Organize folders: Scripts, Art, Audio, UI, Scenes
- [x] Download assets like Character sprites, UI

## Phase 2: Core Movement & Interaction
- [x] Implement player movement
- [ ] Implement spotlight logic
- [ ] Create Dialogue system with nodes-based structure
		- [ ] DialogManager Class to display UI and manage flow
		- [ ] DialogNavigator Class to handle Dialog node transitions and pass what to display to the DialogManager
		- [ ] DialogNode.cs with DialogChoice class to provide the structure for dialog text, choices, and method calls
		- [ ] DialogTree.cs to hold a collection of DialogNodes and provide methods to navigate between them, will act as my basis for specific NPC Dialog
		- [ ] DialogTrigger.cs to attach to NPCs, Spotlight, or other conditions to trigger dialog trees from NPC Content
	- [ ] Implement Dialogue UI
- [ ] Create basic interaction framework

## Phase 3: Stage Construction
- [ ] Create NPCContent to contain Dialog Trees, Inventories, Abilities, Quests, and other NPC-specific data
- [ ] Build first dream stage
- [ ] Implement stage transition mechanics
- [ ] Create stage lighting and atmosphere system

## Phase 4: Combat Minigames
- [ ] Create turn-based combat system with applause/boo meters
- [ ] Implement audience interaction mechanics
- [ ] Add quest trigger system for performances

## Phase 5: Audience Interaction and Performance Systems
- [ ] Implement CalmDialogue minigame
- [ ] Create audience feedback system
- [ ] Add performance rewards (applause, boos)

## Phase 6: Audio & Visual Polish
- [ ] Add sound effects and music
- [ ] Polish UI and visual feedback
- [ ] Implement screen transitions and effects

## Phase 7: Content & Narrative
- [ ] Add final dialogue and story elements
- [ ] Create title screen and main menu
- [ ] Add credits and game flow

## Phase 8: Testing & Deployment
- [ ] Fix bugs and test all dream transitions
- [ ] Optimize performance
- [ ] Export and submit to itch.io

## Phase 9: Extra Features & Expansion
- [ ] Implement mask system and prop-based abilities
- [ ] Add 2–3 additional dream stages with unique tones
- [ ] Create character progression/unlocks
