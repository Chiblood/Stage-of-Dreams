/* AudienceManager.cs
 * Manages audience reactions and applause during theater performances.
 * This class demonstrates how UnityEvents from dialog nodes can call methods with parameters.
 * 
 * How to use:
 * 1. Attach this script to a GameObject in your scene (like an "AudienceManager" empty GameObject)
 * 2. In your dialog nodes' UnityEvents, add this component and select the methods you want to call
 * 3. Configure the parameters directly in the inspector
 * 
 * Examples of calls from dialog events:
 * - OnDialogStart: AudienceApplause(5) - Light applause when dialog starts
 * - OnDialogEnd: AudienceApplause(10) - Heavy applause when dialog ends
 * - OnChoiceSelected: AudienceReaction("surprise") - Audience gasps at player choice
 */

using UnityEngine;
using System.Collections;

public class AudienceManager : MonoBehaviour
{
    [Header("Audience Settings")]
    [SerializeField] private AudioSource audienceAudioSource;
    [SerializeField] private ParticleSystem applauseParticles;
    [SerializeField] private Animator audienceAnimator;
    
    [Header("Audio Clips")]
    [SerializeField] private AudioClip lightApplauseClip;
    [SerializeField] private AudioClip heavyApplauseClip;
    [SerializeField] private AudioClip laughterClip;
    [SerializeField] private AudioClip gaspClip;
    [SerializeField] private AudioClip booClip;
    
    [Header("Settings")]
    [SerializeField] private float baseVolume = 0.7f;
    [SerializeField] private bool enableDebugLogs = true;
    
    /// <summary>
    /// Trigger audience applause with specified intensity
    /// This method can be called from dialog UnityEvents with an intensity parameter
    /// </summary>
    /// <param name="intensity">Applause intensity from 1-10 (1=light, 10=thunderous)</param>
    public void AudienceApplause(int intensity)
    {
        intensity = Mathf.Clamp(intensity, 1, 10);
        
        if (enableDebugLogs)
            Debug.Log($"Audience applause triggered with intensity: {intensity}");
        
        // Play appropriate audio clip
        AudioClip clipToPlay = intensity <= 5 ? lightApplauseClip : heavyApplauseClip;
        if (clipToPlay != null && audienceAudioSource != null)
        {
            audienceAudioSource.volume = baseVolume * (intensity / 10f);
            audienceAudioSource.PlayOneShot(clipToPlay);
        }
        
        // Trigger particle effects
        if (applauseParticles != null)
        {
            var emission = applauseParticles.emission;
            emission.rateOverTime = intensity * 10; // More particles for higher intensity
            applauseParticles.Play();
        }
        
        // Trigger animator
        if (audienceAnimator != null)
        {
            audienceAnimator.SetTrigger("Applause");
            audienceAnimator.SetFloat("ApplauseIntensity", intensity / 10f);
        }
        
        // Stop particles after a delay
        StartCoroutine(StopApplauseAfterDelay(2f + (intensity * 0.3f)));
    }
    
    /// <summary>
    /// Trigger specific audience reactions by name
    /// This method can be called from dialog UnityEvents with a reaction type
    /// </summary>
    /// <param name="reactionType">Type of reaction: "laugh", "gasp", "boo", "cheer"</param>
    public void AudienceReaction(string reactionType)
    {
        if (enableDebugLogs)
            Debug.Log($"Audience reaction triggered: {reactionType}");
        
        AudioClip clipToPlay = null;
        string animationTrigger = "";
        
        switch (reactionType.ToLower())
        {
            case "laugh":
            case "laughter":
                clipToPlay = laughterClip;
                animationTrigger = "Laugh";
                break;
            case "gasp":
            case "surprise":
                clipToPlay = gaspClip;
                animationTrigger = "Gasp";
                break;
            case "boo":
            case "disapproval":
                clipToPlay = booClip;
                animationTrigger = "Boo";
                break;
            case "cheer":
            case "excitement":
                clipToPlay = heavyApplauseClip;
                animationTrigger = "Cheer";
                break;
            default:
                Debug.LogWarning($"Unknown audience reaction type: {reactionType}");
                return;
        }
        
        // Play audio
        if (clipToPlay != null && audienceAudioSource != null)
        {
            audienceAudioSource.volume = baseVolume;
            audienceAudioSource.PlayOneShot(clipToPlay);
        }
        
        // Trigger animation
        if (audienceAnimator != null && !string.IsNullOrEmpty(animationTrigger))
        {
            audienceAnimator.SetTrigger(animationTrigger);
        }
    }
    
    /// <summary>
    /// Set the overall audience mood/energy level
    /// This can be called to build atmosphere during dialog
    /// </summary>
    /// <param name="moodLevel">Mood from 0.0 (hostile) to 1.0 (enthusiastic)</param>
    public void SetAudienceMood(float moodLevel)
    {
        moodLevel = Mathf.Clamp01(moodLevel);
        
        if (enableDebugLogs)
            Debug.Log($"Audience mood set to: {moodLevel:F2}");
        
        if (audienceAnimator != null)
        {
            audienceAnimator.SetFloat("MoodLevel", moodLevel);
        }
        
        // Adjust base volume based on mood
        if (audienceAudioSource != null)
        {
            audienceAudioSource.volume = baseVolume * (0.5f + (moodLevel * 0.5f));
        }
    }
    
    /// <summary>
    /// Create dramatic silence (stop all audience sounds)
    /// Useful for dramatic moments in dialog
    /// </summary>
    public void CreateSilence()
    {
        if (enableDebugLogs)
            Debug.Log("Audience falls silent...");
        
        if (audienceAudioSource != null)
        {
            audienceAudioSource.Stop();
        }
        
        if (applauseParticles != null)
        {
            applauseParticles.Stop();
        }
        
        if (audienceAnimator != null)
        {
            audienceAnimator.SetTrigger("Silence");
        }
    }
    
    /// <summary>
    /// Trigger standing ovation (maximum applause)
    /// For the end of an excellent performance
    /// </summary>
    public void StandingOvation()
    {
        if (enableDebugLogs)
            Debug.Log("Standing ovation!");
        
        AudienceApplause(10);
        
        if (audienceAnimator != null)
        {
            audienceAnimator.SetTrigger("StandingOvation");
        }
        
        // Extended applause duration
        StartCoroutine(StopApplauseAfterDelay(8f));
    }
    
    /// <summary>
    /// Increase audience engagement gradually
    /// Can be called multiple times during a performance to build energy
    /// </summary>
    /// <param name="amount">Amount to increase engagement (0.1 = 10% increase)</param>
    public void BoostEngagement(float amount = 0.1f)
    {
        if (audienceAnimator != null)
        {
            float currentMood = audienceAnimator.GetFloat("MoodLevel");
            float newMood = Mathf.Clamp01(currentMood + amount);
            SetAudienceMood(newMood);
        }
    }
    
    private IEnumerator StopApplauseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (applauseParticles != null)
        {
            applauseParticles.Stop();
        }
    }
    
    // Example of a static method that can also be called from UnityEvents
    public static void LogPerformanceEvent(string eventDescription)
    {
        Debug.Log($"[Performance Log] {eventDescription}");
    }
    
    #region Setup Helper Methods
    
    private void Start()
    {
        // Initialize audience to neutral mood
        SetAudienceMood(0.5f);
    }
    
    private void OnValidate()
    {
        // Ensure volume stays within reasonable bounds
        baseVolume = Mathf.Clamp01(baseVolume);
    }
    
    #endregion
}