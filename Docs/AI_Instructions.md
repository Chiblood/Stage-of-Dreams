# Stage of Dreams – AI Instructions

## Overview
Create a top-down, turn-based RPG where the player is an actor performing on a stage. Each "Dream" is a level with its own story. The player interacts with the audience and uses stage-based abilities to progress.

## Core Features
- **Turn-Based Gameplay:** Player and NPCs (audience, other actors) take turns.
- **Stage Abilities:** Actor can interact with the audience, improvise dialog, and perform unique actions.
- **Audience Interaction UI:** Display audience reactions, allow choices to influence their mood.
- **Story Progression:** Each level is a new "Dream" with its own narrative and objectives.

## Implementation Steps

1. **Project Setup**
   - Create a new Unity 2D project.
   - Set up folders for Scripts, Prefabs, Scenes, and UI.

2. **Player Actor**
   - Implement a player character with movement and turn-based actions.
   - Add abilities: interact with audience, improvise dialog, perform.

3. **Stage & Audience**
   - Design a stage scene with audience seats.
   - Create audience NPCs with mood states (happy, bored, excited, etc.).

4. **Turn System**
   - Implement a turn manager to handle player and NPC turns.

5. **UI**
   - Create UI panels for audience interaction and feedback.
   - Show dialog choices and audience reactions.

6. **Story & Levels**
   - Script a simple story for the first "Dream"/level.
   - Add objectives and win/lose conditions.

7. **Polish**
   - Add sound effects, simple animations, and transitions.

## Tips
- Focus on core mechanics first; polish later.
- Use Unity’s UI system for audience interaction.
- Keep code modular for quick iteration.
