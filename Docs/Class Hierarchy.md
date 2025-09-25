- GameManager.cs
  - Controls game state, scene transitions, and dream progression

- PlayerController.cs
  - Handles movement, input, and spotlight positioning

- CombatManager.cs
  - Manages turn-based combat flow, audience reactions, and climax box logic

- ActorStats.cs
  - Stores health, applause meter, boo meter, and mask state

- Ability.cs
  - Base class for abilities like "PumpUpAudience", "ImprovedDialogue", "UseProp"

- DialogueSystem.cs
  - Controls NPC interactions, scripted lines, and branching choices

- AudienceManager.cs
  - Tracks audience mood, applause/boo meters, and triggers effects

- DreamStage.cs
  - Defines dream structure: Act I, II, III, ClimaxBox
	
- DreamStageManager.cs
  - Manages transitions between dream stages and loading scenes
	
- MinigameManager.cs
  - Handles minigame activation, success/failure conditions, and rewards
	- Minigame: CalmDialogue() - 3 dialogue options, 1 correct
	- Minigame: RememberTheScript() - Type out a word/phrase
	- Minigame: DancingCombat() - Press arrow keys in rhythm
	- Minigame: DramaticLock() - Button mash followed by typing script