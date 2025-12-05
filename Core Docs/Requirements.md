# 📋 Game Requirements – Stage of Dreams

## Gameplay
- Top-down movement on a 15x20 grid with spotlight mechanic
- NPC dialogue with branching dialog trees
- Minigames can be chained with DialogTrees and nodes for the 
- Script display at the beginning of the scene corresponding with line minigame in the scene
- Minigames activated by standing in the Spotlighted cell, almost always in the first half of the stage
- Successful Completions of minigames increase Applause meter and progress the story, failures increase Boo meter and must be retried until successful
- Minigame: CalmDialogue() = Choosing the right dialogue options to keep the audience engaged, (1 option is correct based on the script)
- Minigame: RememberTheScript() = Reacting to scripted lines by typing out a word or phrase on the keyboard
- Minigame: DramaticLock() = Button masher for building tension (as if two swords are locked)
- Minigame: DancingCombat() = press the arrow keys in the correct order and rhythm, timing and accuracy is key! Missing a button press will increase the Boo meter
- Minigame: ScriptWriting() = Create a dialog tree in game, pre structured based on Genre
- Minigame: ReadScript() = available as a UI element and a button press to view the current script for practices
- 

## Art & Audio
- Pixel art tilesets and character sprites
- UI elements for combat, dialogue, and meters
- Sound effects for applause, boos
- Ambient music per dream stage
- Scripted dream stages with 3-act structure

## Technical
- Unity 2D project with C# scripts
- Organized folder structure
- Build for Windows and WebGL
- No symlinks or external dependencies
