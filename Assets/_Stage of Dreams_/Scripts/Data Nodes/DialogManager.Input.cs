/* DialogManager.Input.cs
 * 
 * Partial class containing input handling logic for DialogManager.
 * Handles both standard dialog input and minigame-specific input routing.
 */

using UnityEngine.InputSystem;

/// <summary>
/// Input handling portion of DialogManager.
/// Handles dialog advancement and minigame input routing.
/// </summary>
public partial class DialogManager
{
    /// <summary>
    /// Main update loop - routes input based on current state
    /// </summary>
    private void Update()
    {
        // Check if minigame is active and handle its input
        if (minigameUIManager != null && minigameUIManager.IsMinigameActive)
        {
            HandleMinigameInput();
            return; // Don't process standard dialog input
        }

        // Handle input for advancing dialog (when no choices)
        if (navigator != null && interactAction != null)
        {
            var state = navigator.GetCurrentState();

            if (state.isActive && !state.hasChoices && !state.shouldAutoAdvance)
            {
                if (enableInputLogging && interactAction.WasPressedThisFrame())
                {
                    LogDebug("Interact action pressed - advancing dialog");
                }

                if (interactAction.WasPressedThisFrame())
                {
                    AdvanceDialog();
                }
            }
        }
    }

    /// <summary>
    /// Handle input for active minigames
    /// Routes input to appropriate minigame handler based on minigame type
    /// </summary>
    private void HandleMinigameInput()
    {
        if (minigameUIManager == null || navigator == null)
        {
            return;
        }

        // Route to specific minigame input handler
        switch (minigameUIManager.CurrentMinigameType)
        {
            case MinigameType.RememberTheScript:
                HandleRememberTheScriptInput();
                break;

                // Future minigames:
                // case MinigameType.CalmDialog:
                //     HandleCalmDialogInput();
                //     break;
        }
    }

    /// <summary>
    /// Handle keyboard input for RememberTheScript minigame
    /// Captures character typing and passes to DialogNavigator for validation
    /// </summary>
    private void HandleRememberTheScriptInput()
    {
        // Use new Input System Keyboard API instead of old Input.inputString
        var keyboard = Keyboard.current;

        if (keyboard == null)
        {
            // No keyboard available
            return;
        }

        // Check all character keys for typing input
        // Process printable characters (letters, numbers, punctuation, space)
        string typedChars = "";

        // Get text input from the new Input System
        // We need to check onTextInput event or iterate through keys
        // For now, we'll check individual keys that were pressed this frame

        // ALTERNATIVE APPROACH: Subscribe to text input event in OnEnable
        // But for simplicity, we'll check key presses directly

        // Check for alphabetic keys (a-z)
        for (int i = (int)Key.A; i <= (int)Key.Z; i++)
        {
            var key = (Key)i;
            if (keyboard[key].wasPressedThisFrame)
            {
                char c = GetCharFromKey(key, keyboard);
                if (c != '\0')
                {
                    typedChars += c;
                }
            }
        }

        // Check for number keys (0-9)
        for (int i = (int)Key.Digit0; i <= (int)Key.Digit9; i++)
        {
            var key = (Key)i;
            if (keyboard[key].wasPressedThisFrame)
            {
                char c = GetCharFromKey(key, keyboard);
                if (c != '\0')
                {
                    typedChars += c;
                }
            }
        }

        // Check for space
        if (keyboard[Key.Space].wasPressedThisFrame)
        {
            typedChars += ' ';
        }

        // Check for common punctuation
        if (keyboard[Key.Period].wasPressedThisFrame) typedChars += '.';
        if (keyboard[Key.Comma].wasPressedThisFrame) typedChars += ',';
        if (keyboard[Key.Quote].wasPressedThisFrame) typedChars += '\'';
        if (keyboard[Key.Semicolon].wasPressedThisFrame) typedChars += ';';
        if (keyboard[Key.Slash].wasPressedThisFrame) typedChars += '/';
        if (keyboard[Key.Backslash].wasPressedThisFrame) typedChars += '\\';
        if (keyboard[Key.LeftBracket].wasPressedThisFrame) typedChars += '[';
        if (keyboard[Key.RightBracket].wasPressedThisFrame) typedChars += ']';
        if (keyboard[Key.Minus].wasPressedThisFrame) typedChars += '-';
        if (keyboard[Key.Equals].wasPressedThisFrame) typedChars += '=';

        // Process any typed characters
        if (string.IsNullOrEmpty(typedChars))
        {
            return;
        }

        // Process each character typed this frame
        foreach (char c in typedChars)
        {
            // Pass to navigator for processing
            if (navigator.IsActive)
            {
                if (enableInputLogging)
                {
                    LogDebug($"[RememberTheScript] Character typed: '{c}'");
                }

                try
                {
                    bool correct = navigator.ProcessRememberScriptInput(c);

                    if (enableInputLogging)
                    {
                        LogDebug($"[RememberTheScript] Input result: {(correct ? "Correct" : "Mistake")}");
                    }
                }
                catch (System.Exception ex)
                {
                    LogError($"Error processing typing input: {ex.Message}");
                }
            }
        }
    }

    /// <summary>
    /// Convert a Key enum to a character, respecting shift state
    /// </summary>
    private char GetCharFromKey(Key key, Keyboard keyboard)
    {
        bool shiftPressed = keyboard.shiftKey.isPressed;

        // Handle letter keys (a-z)
        if (key >= Key.A && key <= Key.Z)
        {
            char letter = (char)('a' + (key - Key.A));
            return shiftPressed ? char.ToUpper(letter) : letter;
        }

        // Handle number keys (0-9) - top row
        if (key >= Key.Digit0 && key <= Key.Digit9)
        {
            if (shiftPressed)
            {
                // Shifted symbols for number keys
                return key switch
                {
                    Key.Digit0 => ')',
                    Key.Digit1 => '!',
                    Key.Digit2 => '@',
                    Key.Digit3 => '#',
                    Key.Digit4 => '$',
                    Key.Digit5 => '%',
                    Key.Digit6 => '^',
                    Key.Digit7 => '&',
                    Key.Digit8 => '*',
                    Key.Digit9 => '(',
                    _ => '\0'
                };
            }
            else
            {
                return (char)('0' + (key - Key.Digit0));
            }
        }

        return '\0';
    }
}
