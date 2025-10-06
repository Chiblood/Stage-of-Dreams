# Stage of Dreams - Class Hierarchy

## Scripts Folder: Core Classes

### Main Menu Events.cs
- **Properties:**
- `GameObject mainMenuUI`
  - `GameObject optionsMenuUI`
  - `GameObject loadingScreenUI`
  - `Slider loadingProgressBar`
  - `Text loadingProgressText`
- **Methods:**
  - `Start()`
  - `Update()`
  - `StartNewGame()`
  - `LoadGame()`
  - `OpenOptionsMenu()`
  - `CloseOptionsMenu()`
  - `OpenCreditsMenu()`
  - `CloseCreditsMenu()`
  - `QuitGame()`
  - `LoadSceneAsync(string sceneName)`

### GameManager.cs (TBD)
- **Properties:**
  - `GameState currentGameState`
  - `int currentDreamIndex`
  - `bool isInTurnBasedMode`
  - `DreamStage activeDreamStage`
  - `ActorStats playerStats`
- **Methods:**
  - `StartGame()`
  - `LoadDreamStage(int stageIndex)`
  - `SwitchGameState(GameState newState)`
	-	switch (newState)
  - `EndGame()`
  - `SaveGameProgress()`
  - `LoadGameProgress()`
- **Description:** Controls game state, scene transitions, and dream progression

## PlayerScripts Folder

### PlayerController.cs (PlayerScript.cs)
- **Properties:**
  - `float _moveSpeed = 5f`
  - `bool inSpotlight`
  - `Vector2 _moveDir`
  - `Rigidbody2D _rb`
  - `Animator _animator`
  - `SpriteRenderer _spriteRenderer`
  - `PlayerInput _playerInput`
  - `Spotlight _spotlight`
- **Methods:**
  - `Awake()`
  - `Update()`
  - `FixedUpdate()`
  - `GatherInput()`
  - `MovementUpdate()`
- **Description:** Handles movement, input, and spotlight positioning

## StageScripts Folder

### SpotlightController.cs (Spotlight.cs)
- **Properties:**
  - `float radius = 1.5f`
  - `PlayerScript _player`
  - `bool _wasInSpotlight`
  - `float _spotlightIntensity = 1f`
- **Methods:**
  - `Awake()`
  - `Update()`
  - `CheckPlayerInSpotlight(PlayerScript player)`
  - `OnPlayerEnteredSpotlight(PlayerScript player)`
  - `OnPlayerExitedSpotlight(PlayerScript player)`
  - `OnDrawGizmosSelected()`
- **Description:** Handles spotlight effects and transitions

### DialogTrigger.cs (DialogueTrigger.cs)
- **Properties:**
  - `string _npcName`
  - `string[] _dialogueLines`
  - `DialogueData[] dialogueSequence`
  - `bool triggerOnSpotlight = true`
  - `bool triggerOnInteraction = false`
  - `DialoguePopupSystem dialogueSystem`
  - `PlayerScript player`
  - `int currentDialogueIndex`
  - `bool hasTriggered`
- **Methods:**
  - `Start()`
  - `Update()`
  - `TriggerDialogue()`
  - `IsPlayerNearby()`
  - `ResetDialogue()`
- **Description:** Triggers dialogues and interactions with NPCs

### ActorStats.cs (TBD)
- **Properties:**
  - `int _health = 100`
  - `float _applauseMeter = 0f`
  - `float _booMeter = 0f`
  - `bool _hasMask = false`
  - `int maxHealth = 100`
  - `float maxApplause = 100f`
  - `float maxBoo = 100f`
- **Methods:**
  - `TakeDamage(int damage)`
  - `Heal(int amount)`
  - `AddApplause(float amount)`
  - `AddBoo(float amount)`
  - `ResetMeters()`
  - `GetHealthPercentage()`
  - `GetApplausePercentage()`
  - `GetBooPercentage()`
- **Description:** Stores health, applause meter, boo meter, and mask state

### Ability.cs (Optional)
- **Properties:**
  - `string abilityName`
  - `string description`
  - `float cooldown`
  - `bool isAvailable = true`
  - `ActorStats requiredStats`
- **Methods:**
  - `virtual Execute(ActorStats stats)`
  - `virtual CanExecute(ActorStats stats)`
  - `StartCooldown()`
  - `ResetCooldown()`
- **Description:** Base class for abilities like "PumpUpAudience", "ImprovedDialogue", "UseProp"

### DialogueSystem.cs (DialoguePopupSystem.cs)
- **Properties:**
  - `GameObject dialoguePopupPrefab`
  - `Canvas dialogueCanvas`
  - `Transform dialogueParent`
  - `float animationDuration = 0.3f`
  - `float fadeInAlpha = 0.9f`
  - `GameObject currentDialoguePopup`
  - `bool isDialogueActive`
- **Methods:**
  - `Awake()`
  - `CreateDialogueCanvas()`
  - `ShowDialogue(DialogueData dialogueData)`
  - `HideDialogue()`
  - `IsDialogueActive()`
  - `TestPlayerDialogue()`
  - `TestNPCDialogue()`
- **Description:** Controls NPC interactions, scripted lines, and branching choices

### AudienceManager.cs (TBD)
- **Properties:**
  - `List<AudienceMember> audienceMembers`
  - `float overallMood = 50f`
  - `float applauseThreshold = 75f`
  - `float booThreshold = 25f`
  - `AudioClip applauseSound`
  - `AudioClip booSound`
- **Methods:**
  - `UpdateAudienceMood(float change)`
  - `TriggerApplause()`
  - `TriggerBoos()`
  - `GetRandomAudienceReaction()`
  - `ResetAudienceMood()`
- **Description:** Tracks audience mood, applause/boo meters, and triggers effects

### DreamStage.cs (TBD)
- **Properties:**
  - `string stageName`
  - `Act[] acts` // Act I, II, III
  - `ClimaxBox climaxBox`
  - `int currentActIndex`
  - `bool isCompleted`
- **Methods:**
  - `StartStage()`
  - `ProgressToNextAct()`
  - `TriggerClimaxBox()`
  - `CompleteStage()`
  - `GetCurrentAct()`
- **Description:** Defines dream structure: Act I, II, III, ClimaxBox

### DreamStageManager.cs (TBD)
- **Properties:**
  - `DreamStage[] availableStages`
  - `int currentStageIndex`
  - `bool isTransitioning`
  - `float transitionDuration = 2f`
- **Methods:**
  - `LoadStage(int stageIndex)`
  - `TransitionToNextStage()`
  - `UnloadCurrentStage()`
  - `StartTransition()`
  - `CompleteTransition()`
- **Description:** Manages transitions between dream stages and loading scenes

### MinigameManager.cs (TBD)
- **Properties:**
  - `MinigameType currentMinigame`
  - `bool isMinigameActive`
  - `float minigameTimer`
  - `int score`
  - `int requiredScore`
  - `ActorStats playerStats`
- **Methods:**
  - `StartMinigame(MinigameType type)`
  - `EndMinigame(bool success)`
  - `CalmDialogue()` // 3 dialogue options, 1 correct
  - `RememberTheScript()` // Type out a word/phrase
  - `DancingCombat()` // Press arrow keys in rhythm
  - `DramaticLock()` // Button mash followed by typing script
  - `CheckMinigameSuccess()`
  - `GiveReward(int applause, int score)`
- **Description:** Handles minigame activation, success/failure conditions, and rewards

## Data Classes

### DialogueData.cs
- **Properties:**
  - `string characterName`
  - `string dialogueText`
  - `Sprite characterSprite`
  - `bool isPlayer`
  - `float displayDuration = 3f`
- **Description:** Serializable data structure for dialogue content

### AudienceMember.cs
- **Properties:**
  - `string memberName`
  - `float mood`
  - `Sprite memberSprite`
  - `Vector3 seatPosition`
- **Methods:**
  - `ReactToPerformance(float impact)`
  - `GetReactionSprite()`
- **Description:** Individual audience member with mood and reactions

### Act.cs
- **Properties:**
  - `string actName`
  - `List<DialogueData> actDialogue`
  - `List<MinigameType> requiredMinigames`
  - `bool isCompleted`
- **Methods:**
  - `StartAct()`
  - `CompleteAct()`
- **Description:** Represents one act within a dream stage

### ClimaxBox.cs
- **Properties:**
  - `MinigameType climaxMinigame`
  - `float timeLimit`
  - `int requiredScore`
- **Methods:**
  - `TriggerClimax()`
  - `CompleteClimax(bool success)`
- **Description:** Special climax encounter for each dream stage

## Enums

### public enum GameState
- `Exploration`// Player moving around stage
- `Dialogue` // in dialogue with NPC
- `TurnBased` // Turn-based combat mode
- `Minigame` // Playing one of the 4 minigames
- `Transition` // switching between scenes/acts

### public enum MinigameType
- `CalmDialogue` // Choose correct dialogue (1 of 3 options)
- `RememberTheScript` // Type out a word/phrase
- `DancingCombat` // Press arrow keys in rhythm
- `DramaticLock` // Button mash + type script combo

### public enum AudienceReaction
- `Applause` // Positive reaction from the audience
- `Boo` // Negative reaction from the audience
- `Confused` // Audience is unsure how to react