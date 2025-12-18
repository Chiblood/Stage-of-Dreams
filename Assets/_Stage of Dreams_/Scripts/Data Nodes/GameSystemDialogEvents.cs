/* GameSystemDialogEvents.cs
 * Pre-built DialogEvent implementations for game systems like crowd reactions, applause meter, etc.
 * 
 * How to use:
 * 1. These events can be added to DialogNode start/end events or DialogChoice events
 * 2. Add via inspector by clicking "+" on the Dialog Events list
 * 3. Select the specific event type (CrowdReactionEvent, ApplauseMeterEvent, etc.)
 * 4. Configure the event parameters in the inspector
 * 
 * Status: SCAFFOLDING - Systems not yet implemented
 * When systems are ready, update the OnExecute() methods to call actual game systems
 */

using System;
using UnityEngine;

#region Crowd System Events

/// <summary>
/// Event that triggers a crowd reaction (cheering, booing, silence, etc.)
/// SCAFFOLDING: Update when CrowdManager is implemented
/// </summary>
[System.Serializable]
public class CrowdReactionEvent : DialogEvent
{
    public enum CrowdReaction
    {
        Cheer,
        Applause,
        Boo,
        Silence,
        GaspInAwe,
        Laughter,
        Mixed
    }

    [SerializeField] private CrowdReaction reactionType = CrowdReaction.Applause;
    [SerializeField, Range(0f, 1f)] private float intensity = 0.5f;
    [SerializeField] private float duration = 2f;

    public CrowdReaction ReactionType
    {
        get => reactionType;
        set => reactionType = value;
    }

    public float Intensity
    {
        get => intensity;
        set => intensity = Mathf.Clamp01(value);
    }

    public float Duration
    {
        get => duration;
        set => duration = Mathf.Max(0f, value);
    }

    protected override void OnExecute()
    {
        // TODO: When CrowdManager is implemented, call:
        // CrowdManager.Instance.TriggerReaction(reactionType, intensity, duration);

        Debug.Log($"[CrowdReactionEvent] SCAFFOLDING: Crowd reaction '{reactionType}' at {intensity * 100}% intensity for {duration}s");
    }

    public override string GetDisplayName()
    {
        return $"Crowd Reaction: {reactionType} ({intensity * 100:F0}%)";
    }

    public override bool IsValid()
    {
        return base.IsValid() && duration > 0;
    }
}

/// <summary>
/// Event that makes the crowd "go wild" with maximum reaction
/// SCAFFOLDING: Update when CrowdManager is implemented
/// </summary>
[System.Serializable]
public class CrowdGoWildEvent : DialogEvent
{
    [SerializeField] private float duration = 3f;
    [SerializeField] private bool includeStandingOvation = true;

    public float Duration
    {
        get => duration;
        set => duration = Mathf.Max(0f, value);
    }

    public bool IncludeStandingOvation
    {
        get => includeStandingOvation;
        set => includeStandingOvation = value;
    }

    protected override void OnExecute()
    {
        // TODO: When CrowdManager is implemented, call:
        // CrowdManager.Instance.GoWild(duration, includeStandingOvation);

        Debug.Log($"[CrowdGoWildEvent] SCAFFOLDING: Crowd going wild for {duration}s! Standing ovation: {includeStandingOvation}");
    }

    public override string GetDisplayName()
    {
        return $"Crowd Go Wild! ({duration}s)";
    }
}

#endregion

#region Applause Meter Events

/// <summary>
/// Event that adds or removes points from the applause meter
/// SCAFFOLDING: Update when ApplauseMeter system is implemented
/// </summary>
[System.Serializable]
public class ApplauseMeterEvent : DialogEvent
{
    [SerializeField] private int pointsToAdd = 10;
    [SerializeField] private bool showFeedback = true;
    [SerializeField] private string feedbackMessage = "Great choice!";

    public int PointsToAdd
    {
        get => pointsToAdd;
        set => pointsToAdd = value;
    }

    public bool ShowFeedback
    {
        get => showFeedback;
        set => showFeedback = value;
    }

    public string FeedbackMessage
    {
        get => feedbackMessage;
        set => feedbackMessage = value;
    }

    protected override void OnExecute()
    {
        // TODO: When ApplauseMeter is implemented, call:
        // ApplauseMeter.Instance.AddPoints(pointsToAdd, showFeedback ? feedbackMessage : null);

        string sign = pointsToAdd >= 0 ? "+" : "";
        Debug.Log($"[ApplauseMeterEvent] SCAFFOLDING: {sign}{pointsToAdd} applause points. Feedback: {(showFeedback ? feedbackMessage : "none")}");
    }

    public override string GetDisplayName()
    {
        string sign = pointsToAdd >= 0 ? "+" : "";
        return $"Applause: {sign}{pointsToAdd} points";
    }
}

/// <summary>
/// Event that sets the applause meter to a specific value
/// SCAFFOLDING: Update when ApplauseMeter system is implemented
/// </summary>
[System.Serializable]
public class SetApplauseMeterEvent : DialogEvent
{
    [SerializeField, Range(0, 100)] private int targetValue = 50;
    [SerializeField] private bool animate = true;

    public int TargetValue
    {
        get => targetValue;
        set => targetValue = Mathf.Clamp(value, 0, 100);
    }

    public bool Animate
    {
        get => animate;
        set => animate = value;
    }

    protected override void OnExecute()
    {
        // TODO: When ApplauseMeter is implemented, call:
        // ApplauseMeter.Instance.SetValue(targetValue, animate);

        Debug.Log($"[SetApplauseMeterEvent] SCAFFOLDING: Set applause meter to {targetValue}%. Animate: {animate}");
    }

    public override string GetDisplayName()
    {
        return $"Set Applause: {targetValue}%";
    }
}

#endregion

#region Performance Quality Events

/// <summary>
/// Event that affects the player's performance rating
/// SCAFFOLDING: Update when Performance system is implemented
/// </summary>
[System.Serializable]
public class PerformanceQualityEvent : DialogEvent
{
    public enum QualityAspect
    {
        Timing,
        Delivery,
        Emotion,
        ChemistryWithPartner,
        Overall
    }

    [SerializeField] private QualityAspect aspect = QualityAspect.Overall;
    [SerializeField, Range(-10, 10)] private int qualityChange = 1;

    public QualityAspect Aspect
    {
        get => aspect;
        set => aspect = value;
    }

    public int QualityChange
    {
        get => qualityChange;
        set => qualityChange = Mathf.Clamp(value, -10, 10);
    }

    protected override void OnExecute()
    {
        // TODO: When Performance system is implemented, call:
        // PerformanceManager.Instance.AdjustQuality(aspect, qualityChange);

        string sign = qualityChange >= 0 ? "+" : "";
        Debug.Log($"[PerformanceQualityEvent] SCAFFOLDING: {aspect} {sign}{qualityChange}");
    }

    public override string GetDisplayName()
    {
        string sign = qualityChange >= 0 ? "+" : "";
        return $"Performance {aspect}: {sign}{qualityChange}";
    }
}

#endregion

#region Mood/Atmosphere Events

/// <summary>
/// Event that changes the stage mood/atmosphere
/// SCAFFOLDING: Update when Mood system is implemented
/// </summary>
[System.Serializable]
public class StageMoodEvent : DialogEvent
{
    public enum MoodType
    {
        Tense,
        Joyful,
        Melancholic,
        Suspenseful,
        Triumphant,
        Intimate,
        Chaotic
    }

    [SerializeField] private MoodType moodType = MoodType.Joyful;
    [SerializeField, Range(0f, 1f)] private float intensity = 0.5f;
    [SerializeField] private float transitionDuration = 1f;

    public MoodType Mood
    {
        get => moodType;
        set => moodType = value;
    }

    public float Intensity
    {
        get => intensity;
        set => intensity = Mathf.Clamp01(value);
    }

    public float TransitionDuration
    {
        get => transitionDuration;
        set => transitionDuration = Mathf.Max(0f, value);
    }

    protected override void OnExecute()
    {
        // TODO: When MoodManager is implemented, call:
        // MoodManager.Instance.SetMood(moodType, intensity, transitionDuration);

        Debug.Log($"[StageMoodEvent] SCAFFOLDING: Set mood to {moodType} at {intensity * 100}% over {transitionDuration}s");
    }

    public override string GetDisplayName()
    {
        return $"Stage Mood: {moodType} ({intensity * 100:F0}%)";
    }
}

#endregion

#region Audio Events

/// <summary>
/// Event that plays a sound effect
/// SCAFFOLDING: Update when AudioManager is implemented
/// </summary>
[System.Serializable]
public class PlaySoundEffectEvent : DialogEvent
{
    [SerializeField] private string soundEffectName = "";
    [SerializeField, Range(0f, 1f)] private float volume = 1f;
    [SerializeField] private bool interruptOthers = false;

    public string SoundEffectName
    {
        get => soundEffectName;
        set => soundEffectName = value;
    }

    public float Volume
    {
        get => volume;
        set => volume = Mathf.Clamp01(value);
    }

    public bool InterruptOthers
    {
        get => interruptOthers;
        set => interruptOthers = value;
    }

    protected override void OnExecute()
    {
        // TODO: When AudioManager is implemented, call:
        // AudioManager.Instance.PlaySFX(soundEffectName, volume, interruptOthers);

        Debug.Log($"[PlaySoundEffectEvent] SCAFFOLDING: Play '{soundEffectName}' at {volume * 100}% volume. Interrupt: {interruptOthers}");
    }

    public override string GetDisplayName()
    {
        return $"Play SFX: {soundEffectName}";
    }

    public override bool IsValid()
    {
        return base.IsValid() && !string.IsNullOrEmpty(soundEffectName);
    }
}

/// <summary>
/// Event that changes the background music
/// SCAFFOLDING: Update when AudioManager is implemented
/// </summary>
[System.Serializable]
public class ChangeMusicEvent : DialogEvent
{
    [SerializeField] private string musicTrackName = "";
    [SerializeField] private float fadeInDuration = 1f;
    [SerializeField] private float fadeOutDuration = 1f;

    public string MusicTrackName
    {
        get => musicTrackName;
        set => musicTrackName = value;
    }

    public float FadeInDuration
    {
        get => fadeInDuration;
        set => fadeInDuration = Mathf.Max(0f, value);
    }

    public float FadeOutDuration
    {
        get => fadeOutDuration;
        set => fadeOutDuration = Mathf.Max(0f, value);
    }

    protected override void OnExecute()
    {
        // TODO: When AudioManager is implemented, call:
        // AudioManager.Instance.ChangeMusic(musicTrackName, fadeInDuration, fadeOutDuration);

        Debug.Log($"[ChangeMusicEvent] SCAFFOLDING: Change music to '{musicTrackName}' (FadeOut: {fadeOutDuration}s, FadeIn: {fadeInDuration}s)");
    }

    public override string GetDisplayName()
    {
        return $"Change Music: {musicTrackName}";
    }

    public override bool IsValid()
    {
        return base.IsValid() && !string.IsNullOrEmpty(musicTrackName);
    }
}

#endregion

#region Lighting/Visual Events

/// <summary>
/// Event that changes spotlight focus
/// SCAFFOLDING: Update when Spotlight system is enhanced
/// </summary>
[System.Serializable]
public class SpotlightFocusEvent : DialogEvent
{
    public enum SpotlightTarget
    {
        Player,
        NPC,
        Both,
        None,
        Custom
    }

    [SerializeField] private SpotlightTarget target = SpotlightTarget.Player;
    [SerializeField] private string customTargetName = "";
    [SerializeField] private float transitionSpeed = 1f;

    public SpotlightTarget Target
    {
        get => target;
        set => target = value;
    }

    public string CustomTargetName
    {
        get => customTargetName;
        set => customTargetName = value;
    }

    public float TransitionSpeed
    {
        get => transitionSpeed;
        set => transitionSpeed = Mathf.Max(0.1f, value);
    }

    protected override void OnExecute()
    {
        // TODO: When Spotlight system is enhanced, call:
        // SpotlightController.Instance.FocusOn(target, transitionSpeed);

        string targetName = target == SpotlightTarget.Custom ? customTargetName : target.ToString();
        Debug.Log($"[SpotlightFocusEvent] SCAFFOLDING: Focus spotlight on '{targetName}' (speed: {transitionSpeed})");
    }

    public override string GetDisplayName()
    {
        string targetName = target == SpotlightTarget.Custom ? customTargetName : target.ToString();
        return $"Spotlight: {targetName}";
    }
}

#endregion

#region Game State Events

/// <summary>
/// Event that updates a game variable/flag
/// SCAFFOLDING: Update when GameState system is implemented
/// </summary>
[System.Serializable]
public class SetGameFlagEvent : DialogEvent
{
    [SerializeField] private string flagName = "";
    [SerializeField] private bool flagValue = true;

    public string FlagName
    {
        get => flagName;
        set => flagName = value;
    }

    public bool FlagValue
    {
        get => flagValue;
        set => flagValue = value;
    }

    protected override void OnExecute()
    {
        // TODO: When GameState system is implemented, call:
        // GameState.Instance.SetFlag(flagName, flagValue);

        Debug.Log($"[SetGameFlagEvent] SCAFFOLDING: Set flag '{flagName}' = {flagValue}");
    }

    public override string GetDisplayName()
    {
        return $"Set Flag: {flagName} = {flagValue}";
    }

    public override bool IsValid()
    {
        return base.IsValid() && !string.IsNullOrEmpty(flagName);
    }
}

#endregion

/* 
 * USAGE EXAMPLES:
 * 
 * 1. In DialogTree inspector, expand a DialogNode
 * 2. Go to "Dialog Events" section
 * 3. Add to "Start Events" or "End Events" list
 * 4. Click "+" and select event type (e.g., CrowdReactionEvent)
 * 5. Configure parameters
 * 
 * OR for choices:
 * 
 * 1. In DialogNode inspector, expand a choice
 * 2. Go to "Dialog Events" section
 * 3. Add to "Choice Events" list
 * 4. Click "+" and select event type
 * 5. Configure parameters
 * 
 * WHEN SYSTEMS ARE IMPLEMENTED:
 * - Update each OnExecute() method to call actual game systems
 * - Remove "SCAFFOLDING" debug logs
 * - Test each event type
 * - Add additional events as needed
 */
