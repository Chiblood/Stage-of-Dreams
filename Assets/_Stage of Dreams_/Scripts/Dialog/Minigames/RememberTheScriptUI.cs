/* RememberTheScriptUI.cs
 * UI component for RememberTheScript typing minigame.
 * Displays target phrase, typing progress, mistakes, and timer.
 * 
 * How it works:
 * 1. Initialize() is called with minigame parameters
 * 2. BuildUI() creates the visual structure
 * 3. UpdateProgress() is called as player types
 * 4. UpdateTimer() is called every frame
 * 5. ShowSuccess() or ShowFailure() displays final result
 */

using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// UI component for RememberTheScript typing minigame.
/// Displays target phrase, typing progress, mistakes, and timer.
/// Pure UI component - no game logic, just presentation.
/// </summary>
public class RememberTheScriptUI
{
    #region UI Elements
    private VisualElement rootElement;
    private Label titleLabel;
    private Label targetPhraseLabel;
    private Label typedTextLabel;
    private Label mistakeCountLabel;
    private Label timerLabel;
    private ProgressBar mistakeBar;
    private VisualElement feedbackContainer;
    private VisualElement statsContainer;
    #endregion

    #region Data
    private string targetPhrase;
    private int maxMistakes;
    private float timeLimit;
    #endregion

    #region Initialization

    /// <summary>
    /// Initialize the UI with minigame parameters
    /// </summary>
    public void Initialize(string phrase, int mistakes, float time)
    {
        targetPhrase = phrase;
        maxMistakes = mistakes;
        timeLimit = time;

        BuildUI();

        Debug.Log($"[RememberTheScriptUI] Initialized with phrase: '{phrase}' | Max mistakes: {mistakes} | Time: {time}s");
    }

    /// <summary>
    /// Build the complete UI structure
    /// </summary>
    private void BuildUI()
    {
        // Root container
        rootElement = new VisualElement();
        rootElement.name = "RememberTheScriptUI";
        rootElement.AddToClassList("minigame-root");

        // Title
        titleLabel = new Label("Remember the Script!");
        titleLabel.name = "MinigameTitle";
        titleLabel.AddToClassList("minigame-title");
        rootElement.Add(titleLabel);

        // Target phrase display (what player needs to type)
        targetPhraseLabel = new Label(targetPhrase);
        targetPhraseLabel.name = "TargetPhrase";
        targetPhraseLabel.AddToClassList("target-phrase");
        rootElement.Add(targetPhraseLabel);

        // Typed text display (what player has typed so far)
        typedTextLabel = new Label("");
        typedTextLabel.name = "TypedText";
        typedTextLabel.AddToClassList("typed-text");
        rootElement.Add(typedTextLabel);

        // Stats container (mistakes and timer)
        statsContainer = new VisualElement();
        statsContainer.name = "StatsContainer";
        statsContainer.AddToClassList("stats-container");
        rootElement.Add(statsContainer);

        // Mistake counter label
        mistakeCountLabel = new Label($"Mistakes: 0 / {maxMistakes}");
        mistakeCountLabel.name = "MistakeCount";
        mistakeCountLabel.AddToClassList("mistake-count");
        statsContainer.Add(mistakeCountLabel);

        // Mistake bar (visual progress bar)
        mistakeBar = new ProgressBar();
        mistakeBar.name = "MistakeBar";
        mistakeBar.value = 0;
        mistakeBar.title = "Mistakes";
        mistakeBar.AddToClassList("mistake-bar");
        statsContainer.Add(mistakeBar);

        // Timer label
        timerLabel = new Label($"Time: {timeLimit:F1}s");
        timerLabel.name = "TimerLabel";
        timerLabel.AddToClassList("timer-label");
        statsContainer.Add(timerLabel);

        // Feedback container (for correct/incorrect flashes and final result)
        feedbackContainer = new VisualElement();
        feedbackContainer.name = "FeedbackContainer";
        feedbackContainer.AddToClassList("feedback-container");
        rootElement.Add(feedbackContainer);

        Debug.Log("[RememberTheScriptUI] UI structure built");
    }

    #endregion

    #region Update Methods

    /// <summary>
    /// Update the display based on player's typing progress
    /// </summary>
    /// <param name="currentIndex">How many characters player has typed correctly</param>
    /// <param name="mistakeCount">Number of mistakes made</param>
    /// <param name="wasCorrect">Was the last character typed correct?</param>
    public void UpdateProgress(int currentIndex, int mistakeCount, bool wasCorrect)
    {
        // Update typed text with color coding (show what's been typed correctly)
        if (currentIndex > 0 && currentIndex <= targetPhrase.Length)
        {
            string typedPart = targetPhrase.Substring(0, currentIndex);
            // Use rich text to color the typed text green
            typedTextLabel.text = $"<color=#00FF00>{typedPart}</color>";
        }
        else
        {
            typedTextLabel.text = "";
        }

        // Flash feedback for correct/incorrect
        if (wasCorrect)
        {
            FlashFeedback("✓ CORRECT", "#00FF00"); // Green
        }
        else
        {
            FlashFeedback("✗ WRONG", "#FF0000"); // Red
        }

        // Update mistake counter
        mistakeCountLabel.text = $"Mistakes: {mistakeCount} / {maxMistakes}";

        // Update mistake bar
        if (maxMistakes > 0)
        {
            mistakeBar.value = (float)mistakeCount / maxMistakes * 100f;
        }

        // Add warning class if close to max mistakes
        if (mistakeCount >= maxMistakes - 1)
        {
            mistakeCountLabel.AddToClassList("warning");
            mistakeBar.AddToClassList("warning");
        }
        else
        {
            mistakeCountLabel.RemoveFromClassList("warning");
            mistakeBar.RemoveFromClassList("warning");
        }

        Debug.Log($"[RememberTheScriptUI] Progress updated: Index={currentIndex}, Mistakes={mistakeCount}, Correct={wasCorrect}");
    }

    /// <summary>
    /// Update the timer display
    /// </summary>
    public void UpdateTimer(float timeRemaining)
    {
        timerLabel.text = $"Time: {timeRemaining:F1}s";

        // Add warning class if time running out (5 seconds or less)
        if (timeRemaining <= 5f)
        {
            timerLabel.AddToClassList("warning");
        }
        else
        {
            timerLabel.RemoveFromClassList("warning");
        }
    }

    #endregion

    #region Success/Failure Display

    /// <summary>
    /// Show success message with score and time
    /// </summary>
    public void ShowSuccess(float score, float timeTaken)
    {
        // Clear feedback container
        feedbackContainer.Clear();

        // Create success message
        Label successLabel = new Label($"SUCCESS!\n\nScore: {score:F1}\nTime: {timeTaken:F1}s");
        successLabel.name = "SuccessMessage";
        successLabel.AddToClassList("success-message");
        feedbackContainer.Add(successLabel);

        Debug.Log($"[RememberTheScriptUI] Success displayed: Score={score}, Time={timeTaken}");
    }

    /// <summary>
    /// Show failure message with reason
    /// </summary>
    public void ShowFailure(string reason)
    {
        // Clear feedback container
        feedbackContainer.Clear();

        // Create failure message
        Label failureLabel = new Label($"FAILED!\n\n{reason}");
        failureLabel.name = "FailureMessage";
        failureLabel.AddToClassList("failure-message");
        feedbackContainer.Add(failureLabel);

        Debug.Log($"[RememberTheScriptUI] Failure displayed: {reason}");
    }

    #endregion

    #region Visual Feedback

    /// <summary>
    /// Flash a temporary feedback message (correct/incorrect)
    /// </summary>
    private void FlashFeedback(string text, string colorHex)
    {
        // Create feedback label
        Label feedback = new Label(text);
        feedback.name = "FeedbackFlash";
        feedback.AddToClassList("feedback-flash");

        // Set color using style
        feedback.style.color = new StyleColor(HexToColor(colorHex));

        // Add to container
        feedbackContainer.Add(feedback);

        // Remove after short delay (would ideally use a coroutine or timer)
        // For now, we'll clear old feedback when new feedback arrives
        // In future, could use DOTween or similar for automatic removal

        // Keep only the last 3 feedback messages to avoid clutter
        while (feedbackContainer.childCount > 3)
        {
            feedbackContainer.RemoveAt(0);
        }
    }

    /// <summary>
    /// Convert hex color string to Unity Color
    /// </summary>
    private Color HexToColor(string hex)
    {
        if (ColorUtility.TryParseHtmlString(hex, out Color color))
        {
            return color;
        }
        return Color.white;
    }

    #endregion

    #region Public Access

    /// <summary>
    /// Get the root visual element to add to the UI hierarchy
    /// </summary>
    public VisualElement GetRootElement()
    {
        return rootElement;
    }

    #endregion
}
