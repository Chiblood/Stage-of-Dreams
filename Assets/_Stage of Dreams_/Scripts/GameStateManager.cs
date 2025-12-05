/* GameStateManager.cs
 * Centralized game state management system for Stage of Dreams.
 * 
 * Responsibilities:
 * - Track audience metrics (applause, boo, mood)
 * - Manage performance scores (scene, dream, total)
 * - Handle player progression (abilities, achievements, completed scenes)
 * - Coordinate turn-based state
 * - Monitor session data and minigame statistics
 * 
 * Architecture:
 * - Singleton pattern for global access
 * - Event-driven for UI updates
 * - ScriptableObject backing for save/load
 * 
 * Usage:
 * GameStateManager.Instance.AdjustApplause(10f);
 * GameStateManager.Instance.AddSceneScore(50);
 * GameStateManager.Instance.Instance.OnApplauseScoreChanged += UpdateUI;
 */

using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Centralized game state manager - tracks audience metrics, scores, progression, and session data.
/// Singleton pattern ensures one source of truth across all systems.
/// </summary>
public class GameStateManager : MonoBehaviour
{
    #region Singleton

    private static GameStateManager _instance;
    public static GameStateManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<GameStateManager>();

                if (_instance == null)
                {
                    GameObject go = new GameObject("GameStateManager");
                    _instance = go.AddComponent<GameStateManager>();
                }
            }
            return _instance;
        }
    }

    #endregion

    #region Editor Settings

    [Header("State Configuration")]
    [SerializeField] private GameStateData defaultState;
    [SerializeField] private bool resetOnSceneLoad = false;
    [SerializeField] private bool enableDebugLogs = true;

    [Header("Audience Settings")]
    [SerializeField] private float audienceDecayRate = 1f; // Points per second of inactivity
    [SerializeField] private float audienceMoodSmoothTime = 2f; // Smooth dampening for mood changes

    [Header("Performance Settings")]
    [SerializeField] private int maxScorePerScene = 1000;
    [SerializeField] private int scoreThresholdForNextDream = 500;

    #endregion

    #region State Data

    // Audience Metrics
    private float _currentApplauseScore = 50f; // 0-100 scale
    private float _currentBooScore = 0f; // 0-100 scale
    private float _audienceMood = 0.5f; // 0.0 (hostile) to 1.0 (enthusiastic)
    private float _audienceMoodVelocity; // For smooth dampening

    // Performance Metrics
    private int _currentSceneScore = 0;
    private int _currentDreamScore = 0;
    private int _totalGameScore = 0;
    private int _consecutiveSuccesses = 0;
    private int _consecutiveFailures = 0;

    // Progression
    private HashSet<string> _completedScenes = new HashSet<string>();
    private HashSet<string> _unlockedAbilities = new HashSet<string>();
    private HashSet<string> _earnedAchievements = new HashSet<string>();
    private int _currentDreamIndex = 0;
    private int _currentActIndex = 0; // Act I, II, III

    // Turn-Based State
    private bool _isInTurnBasedMode = false;
    private int _currentTurn = 0;
    private int _playerActionsThisTurn = 0;
    private int _maxActionsPerTurn = 3;

    // Session Data
    private float _sessionStartTime;
    private int _totalMinigamesAttempted = 0;
    private int _totalMinigamesCompleted = 0;
    private Dictionary<string, int> _minigameRetries = new Dictionary<string, int>();

    #endregion

    #region Properties - Audience

    public float CurrentApplauseScore
    {
        get => _currentApplauseScore;
        private set
        {
            float oldValue = _currentApplauseScore;
            _currentApplauseScore = Mathf.Clamp(value, 0f, 100f);

            if (Math.Abs(oldValue - _currentApplauseScore) > 0.01f)
            {
                OnApplauseScoreChanged?.Invoke(_currentApplauseScore);
                UpdateAudienceMood();
            }
        }
    }

    public float CurrentBooScore
    {
        get => _currentBooScore;
        private set
        {
            float oldValue = _currentBooScore;
            _currentBooScore = Mathf.Clamp(value, 0f, 100f);

            if (Math.Abs(oldValue - _currentBooScore) > 0.01f)
            {
                OnBooScoreChanged?.Invoke(_currentBooScore);
                UpdateAudienceMood();
            }
        }
    }

    public float AudienceMood
    {
        get => _audienceMood;
        private set
        {
            float oldValue = _audienceMood;
            _audienceMood = Mathf.Clamp01(value);

            if (Math.Abs(oldValue - _audienceMood) > 0.01f)
            {
                OnAudienceMoodChanged?.Invoke(_audienceMood);
            }
        }
    }

    /// <summary>
    /// Get audience reaction category based on current mood
    /// </summary>
    public AudienceReaction CurrentAudienceReaction
    {
        get
        {
            if (_audienceMood >= 0.8f) return AudienceReaction.Enthusiastic;
            if (_audienceMood >= 0.6f) return AudienceReaction.Supportive;
            if (_audienceMood >= 0.4f) return AudienceReaction.Neutral;
            if (_audienceMood >= 0.2f) return AudienceReaction.Disappointed;
            return AudienceReaction.Hostile;
        }
    }

    #endregion

    #region Properties - Performance

    public int CurrentSceneScore
    {
        get => _currentSceneScore;
        private set
        {
            int oldValue = _currentSceneScore;
            _currentSceneScore = Mathf.Clamp(value, 0, maxScorePerScene);

            if (oldValue != _currentSceneScore)
            {
                OnSceneScoreChanged?.Invoke(_currentSceneScore);
            }
        }
    }

    public int CurrentDreamScore
    {
        get => _currentDreamScore;
        private set
        {
            int oldValue = _currentDreamScore;
            _currentDreamScore = Mathf.Max(0, value);

            if (oldValue != _currentDreamScore)
            {
                OnDreamScoreChanged?.Invoke(_currentDreamScore);
            }
        }
    }

    public int TotalGameScore
    {
        get => _totalGameScore;
        private set
        {
            int oldValue = _totalGameScore;
            _totalGameScore = Mathf.Max(0, value);

            if (oldValue != _totalGameScore)
            {
                OnTotalScoreChanged?.Invoke(_totalGameScore);
            }
        }
    }

    public int ConsecutiveSuccesses => _consecutiveSuccesses;
    public int ConsecutiveFailures => _consecutiveFailures;

    #endregion

    #region Properties - Progression

    public int CurrentDreamIndex
    {
        get => _currentDreamIndex;
        private set
        {
            int oldValue = _currentDreamIndex;
            _currentDreamIndex = Mathf.Max(0, value);

            if (oldValue != _currentDreamIndex)
            {
                OnDreamIndexChanged?.Invoke(_currentDreamIndex);
            }
        }
    }

    public int CurrentActIndex
    {
        get => _currentActIndex;
        private set
        {
            int oldValue = _currentActIndex;
            _currentActIndex = Mathf.Clamp(value, 0, 2); // Act I (0), II (1), III (2)

            if (oldValue != _currentActIndex)
            {
                OnActIndexChanged?.Invoke(_currentActIndex);
            }
        }
    }

    public bool IsSceneCompleted(string sceneId) => _completedScenes.Contains(sceneId);
    public bool HasAbility(string abilityId) => _unlockedAbilities.Contains(abilityId);
    public bool HasAchievement(string achievementId) => _earnedAchievements.Contains(achievementId);

    #endregion

    #region Properties - Turn-Based

    public bool IsInTurnBasedMode => _isInTurnBasedMode;
    public int CurrentTurn => _currentTurn;
    public int PlayerActionsThisTurn => _playerActionsThisTurn;
    public int MaxActionsPerTurn => _maxActionsPerTurn;
    public int RemainingActions => Mathf.Max(0, _maxActionsPerTurn - _playerActionsThisTurn);

    #endregion

    #region Properties - Session

    public float SessionDuration => Time.time - _sessionStartTime;
    public int TotalMinigamesAttempted => _totalMinigamesAttempted;
    public int TotalMinigamesCompleted => _totalMinigamesCompleted;
    public float MinigameSuccessRate => _totalMinigamesAttempted > 0
        ? (float)_totalMinigamesCompleted / _totalMinigamesAttempted
        : 0f;

    #endregion

    #region Events

    // Audience Events
    public event Action<float> OnApplauseScoreChanged;
    public event Action<float> OnBooScoreChanged;
    public event Action<float> OnAudienceMoodChanged;

    // Performance Events
    public event Action<int> OnSceneScoreChanged;
    public event Action<int> OnDreamScoreChanged;
    public event Action<int> OnTotalScoreChanged;

    // Progression Events
    public event Action<string> OnSceneCompleted;
    public event Action<string> OnAbilityUnlocked;
    public event Action<string> OnAchievementEarned;
    public event Action<int> OnDreamIndexChanged;
    public event Action<int> OnActIndexChanged;

    // Turn-Based Events
    public event Action OnTurnBasedModeStarted;
    public event Action OnTurnBasedModeEnded;
    public event Action<int> OnTurnChanged;
    public event Action<int> OnActionsRemainingChanged;

    // Minigame Events
    public event Action<string> OnMinigameStarted;
    public event Action<string, bool> OnMinigameEnded; // (minigame ID, success)

    #endregion

    #region Initialization

    private void Awake()
    {
        // Singleton enforcement
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeGameState();
    }

    private void InitializeGameState()
    {
        _sessionStartTime = Time.time;

        if (defaultState != null)
        {
            LoadFromData(defaultState);
        }
        else
        {
            ResetToDefaults();
        }

        LogDebug("GameStateManager initialized");
    }

    private void ResetToDefaults()
    {
        _currentApplauseScore = 50f;
        _currentBooScore = 0f;
        _audienceMood = 0.5f;
        _currentSceneScore = 0;
        _currentDreamScore = 0;
        _totalGameScore = 0;
        _consecutiveSuccesses = 0;
        _consecutiveFailures = 0;
        _completedScenes.Clear();
        _unlockedAbilities.Clear();
        _earnedAchievements.Clear();
        _currentDreamIndex = 0;
        _currentActIndex = 0;
        _isInTurnBasedMode = false;
        _currentTurn = 0;
        _playerActionsThisTurn = 0;
        _totalMinigamesAttempted = 0;
        _totalMinigamesCompleted = 0;
        _minigameRetries.Clear();
    }

    #endregion

    #region Update Loop

    private void Update()
    {
        // Smooth audience mood changes
        if (audienceMoodSmoothTime > 0)
        {
            float targetMood = CalculateTargetMood();
            _audienceMood = Mathf.SmoothDamp(_audienceMood, targetMood, ref _audienceMoodVelocity, audienceMoodSmoothTime);
        }

        // Audience decay over time (if enabled)
        if (audienceDecayRate > 0 && !_isInTurnBasedMode)
        {
            AdjustApplause(-audienceDecayRate * Time.deltaTime);
        }
    }

    /// <summary>
    /// Calculate target mood based on applause and boo scores
    /// </summary>
    private float CalculateTargetMood()
    {
        // Mood is influenced by ratio of applause to boo
        float totalReaction = _currentApplauseScore + _currentBooScore;

        if (totalReaction < 0.01f)
        {
            return 0.5f; // Neutral if no reactions
        }

        float applauseRatio = _currentApplauseScore / totalReaction;
        return applauseRatio;
    }

    /// <summary>
    /// Force immediate mood update (called after score changes)
    /// </summary>
    private void UpdateAudienceMood()
    {
        float targetMood = CalculateTargetMood();
        AudienceMood = targetMood;
    }

    #endregion

    #region Audience Methods

    /// <summary>
    /// Add or subtract from applause score
    /// </summary>
    public void AdjustApplause(float amount)
    {
        CurrentApplauseScore += amount;
        LogDebug($"Applause adjusted by {amount:F1} -> Now: {CurrentApplauseScore:F1}");
    }

    /// <summary>
    /// Add or subtract from boo score
    /// </summary>
    public void AdjustBoo(float amount)
    {
        CurrentBooScore += amount;
        LogDebug($"Boo adjusted by {amount:F1} -> Now: {CurrentBooScore:F1}");
    }

    /// <summary>
    /// Set applause score directly
    /// </summary>
    public void SetApplause(float value)
    {
        CurrentApplauseScore = value;
        LogDebug($"Applause set to {CurrentApplauseScore:F1}");
    }

    /// <summary>
    /// Set boo score directly
    /// </summary>
    public void SetBoo(float value)
    {
        CurrentBooScore = value;
        LogDebug($"Boo set to {CurrentBooScore:F1}");
    }

    /// <summary>
    /// Reset both audience scores to defaults
    /// </summary>
    public void ResetAudienceScores()
    {
        CurrentApplauseScore = 50f;
        CurrentBooScore = 0f;
        LogDebug("Audience scores reset to defaults");
    }

    #endregion

    #region Performance Methods

    /// <summary>
    /// Add points to current scene score
    /// </summary>
    public void AddSceneScore(int points)
    {
        CurrentSceneScore += points;
        CurrentDreamScore += points;
        TotalGameScore += points;

        LogDebug($"Added {points} points -> Scene: {CurrentSceneScore}, Dream: {CurrentDreamScore}, Total: {TotalGameScore}");
    }

    /// <summary>
    /// Record a successful action/minigame
    /// </summary>
    public void RecordSuccess()
    {
        _consecutiveSuccesses++;
        _consecutiveFailures = 0;

        LogDebug($"Success recorded! Consecutive successes: {_consecutiveSuccesses}");
    }

    /// <summary>
    /// Record a failed action/minigame
    /// </summary>
    public void RecordFailure()
    {
        _consecutiveFailures++;
        _consecutiveSuccesses = 0;

        LogDebug($"Failure recorded! Consecutive failures: {_consecutiveFailures}");
    }

    /// <summary>
    /// Reset scene score (called when starting a new scene)
    /// </summary>
    public void ResetSceneScore()
    {
        CurrentSceneScore = 0;
        LogDebug("Scene score reset");
    }

    /// <summary>
    /// Reset dream score (called when starting a new dream)
    /// </summary>
    public void ResetDreamScore()
    {
        CurrentDreamScore = 0;
        LogDebug("Dream score reset");
    }

    #endregion

    #region Progression Methods

    /// <summary>
    /// Mark a scene as completed
    /// </summary>
    public void CompleteScene(string sceneId)
    {
        if (!_completedScenes.Contains(sceneId))
        {
            _completedScenes.Add(sceneId);
            OnSceneCompleted?.Invoke(sceneId);
            LogDebug($"Scene completed: {sceneId}");
        }
    }

    /// <summary>
    /// Unlock a new ability
    /// </summary>
    public void UnlockAbility(string abilityId)
    {
        if (!_unlockedAbilities.Contains(abilityId))
        {
            _unlockedAbilities.Add(abilityId);
            OnAbilityUnlocked?.Invoke(abilityId);
            LogDebug($"Ability unlocked: {abilityId}");
        }
    }

    /// <summary>
    /// Earn an achievement
    /// </summary>
    public void EarnAchievement(string achievementId)
    {
        if (!_earnedAchievements.Contains(achievementId))
        {
            _earnedAchievements.Add(achievementId);
            OnAchievementEarned?.Invoke(achievementId);
            LogDebug($"Achievement earned: {achievementId}");
        }
    }

    /// <summary>
    /// Progress to the next dream
    /// </summary>
    public void AdvanceToDream(int dreamIndex)
    {
        CurrentDreamIndex = dreamIndex;
        ResetDreamScore();
        LogDebug($"Advanced to Dream {dreamIndex}");
    }

    /// <summary>
    /// Progress to the next act within current dream
    /// </summary>
    public void AdvanceToAct(int actIndex)
    {
        CurrentActIndex = actIndex;
        LogDebug($"Advanced to Act {actIndex + 1}"); // +1 because 0-indexed
    }

    #endregion

    #region Turn-Based Methods

    /// <summary>
    /// Start turn-based mode
    /// </summary>
    public void StartTurnBasedMode()
    {
        if (_isInTurnBasedMode)
        {
            LogWarning("Already in turn-based mode");
            return;
        }

        _isInTurnBasedMode = true;
        _currentTurn = 0;
        _playerActionsThisTurn = 0;

        OnTurnBasedModeStarted?.Invoke();
        LogDebug("Turn-based mode started");
    }

    /// <summary>
    /// End turn-based mode
    /// </summary>
    public void EndTurnBasedMode()
    {
        if (!_isInTurnBasedMode)
        {
            LogWarning("Not in turn-based mode");
            return;
        }

        _isInTurnBasedMode = false;
        _currentTurn = 0;
        _playerActionsThisTurn = 0;

        OnTurnBasedModeEnded?.Invoke();
        LogDebug("Turn-based mode ended");
    }

    /// <summary>
    /// Advance to the next turn
    /// </summary>
    public void NextTurn()
    {
        if (!_isInTurnBasedMode)
        {
            LogWarning("Not in turn-based mode - cannot advance turn");
            return;
        }

        _currentTurn++;
        _playerActionsThisTurn = 0;

        OnTurnChanged?.Invoke(_currentTurn);
        OnActionsRemainingChanged?.Invoke(RemainingActions);

        LogDebug($"Advanced to turn {_currentTurn}");
    }

    /// <summary>
    /// Use one player action
    /// </summary>
    public bool UsePlayerAction()
    {
        if (!_isInTurnBasedMode)
        {
            LogWarning("Not in turn-based mode");
            return false;
        }

        if (_playerActionsThisTurn >= _maxActionsPerTurn)
        {
            LogWarning("No actions remaining this turn");
            return false;
        }

        _playerActionsThisTurn++;
        OnActionsRemainingChanged?.Invoke(RemainingActions);

        LogDebug($"Player action used. Remaining: {RemainingActions}");
        return true;
    }

    #endregion

    #region Minigame Tracking

    /// <summary>
    /// Called when a minigame starts
    /// </summary>
    public void StartMinigame(string minigameId)
    {
        _totalMinigamesAttempted++;

        if (!_minigameRetries.ContainsKey(minigameId))
        {
            _minigameRetries[minigameId] = 0;
        }

        OnMinigameStarted?.Invoke(minigameId);
        LogDebug($"Minigame started: {minigameId} (Attempt #{_minigameRetries[minigameId] + 1})");
    }

    /// <summary>
    /// Called when a minigame ends
    /// </summary>
    public void EndMinigame(string minigameId, bool success)
    {
        if (success)
        {
            _totalMinigamesCompleted++;
            RecordSuccess();
        }
        else
        {
            if (_minigameRetries.ContainsKey(minigameId))
            {
                _minigameRetries[minigameId]++;
            }
            else
            {
                _minigameRetries[minigameId] = 1;
            }
            RecordFailure();
        }

        OnMinigameEnded?.Invoke(minigameId, success);
        LogDebug($"Minigame ended: {minigameId} - Success: {success}");
    }

    /// <summary>
    /// Get retry count for a specific minigame
    /// </summary>
    public int GetMinigameRetries(string minigameId)
    {
        return _minigameRetries.ContainsKey(minigameId) ? _minigameRetries[minigameId] : 0;
    }

    #endregion

    #region Save/Load

    /// <summary>
    /// Save current state to a GameStateData object
    /// </summary>
    public GameStateData SaveToData()
    {
        GameStateData data = ScriptableObject.CreateInstance<GameStateData>();

        // Audience
        data.applauseScore = _currentApplauseScore;
        data.booScore = _currentBooScore;
        data.audienceMood = _audienceMood;

        // Performance
        data.sceneScore = _currentSceneScore;
        data.dreamScore = _currentDreamScore;
        data.totalScore = _totalGameScore;
        data.consecutiveSuccesses = _consecutiveSuccesses;
        data.consecutiveFailures = _consecutiveFailures;

        // Progression
        data.completedScenes = new List<string>(_completedScenes);
        data.unlockedAbilities = new List<string>(_unlockedAbilities);
        data.earnedAchievements = new List<string>(_earnedAchievements);
        data.currentDreamIndex = _currentDreamIndex;
        data.currentActIndex = _currentActIndex;

        // Session
        data.totalMinigamesAttempted = _totalMinigamesAttempted;
        data.totalMinigamesCompleted = _totalMinigamesCompleted;

        LogDebug("Game state saved to data");
        return data;
    }

    /// <summary>
    /// Load state from a GameStateData object
    /// </summary>
    public void LoadFromData(GameStateData data)
    {
        if (data == null)
        {
            LogError("Cannot load from null data");
            return;
        }

        // Audience
        _currentApplauseScore = data.applauseScore;
        _currentBooScore = data.booScore;
        _audienceMood = data.audienceMood;

        // Performance
        _currentSceneScore = data.sceneScore;
        _currentDreamScore = data.dreamScore;
        _totalGameScore = data.totalScore;
        _consecutiveSuccesses = data.consecutiveSuccesses;
        _consecutiveFailures = data.consecutiveFailures;

        // Progression
        _completedScenes = new HashSet<string>(data.completedScenes);
        _unlockedAbilities = new HashSet<string>(data.unlockedAbilities);
        _earnedAchievements = new HashSet<string>(data.earnedAchievements);
        _currentDreamIndex = data.currentDreamIndex;
        _currentActIndex = data.currentActIndex;

        // Session
        _totalMinigamesAttempted = data.totalMinigamesAttempted;
        _totalMinigamesCompleted = data.totalMinigamesCompleted;

        LogDebug("Game state loaded from data");
    }

    #endregion

    #region Debugging

    [ContextMenu("Print Current State")]
    public void PrintCurrentState()
    {
        Debug.Log("=== GAME STATE ===");
        Debug.Log($"Applause: {CurrentApplauseScore:F1} | Boo: {CurrentBooScore:F1} | Mood: {AudienceMood:F2} ({CurrentAudienceReaction})");
        Debug.Log($"Scores - Scene: {CurrentSceneScore} | Dream: {CurrentDreamScore} | Total: {TotalGameScore}");
        Debug.Log($"Streaks - Successes: {ConsecutiveSuccesses} | Failures: {ConsecutiveFailures}");
        Debug.Log($"Progress - Dream: {CurrentDreamIndex} | Act: {CurrentActIndex + 1}");
        Debug.Log($"Completed Scenes: {_completedScenes.Count} | Unlocked Abilities: {_unlockedAbilities.Count}");
        Debug.Log($"Minigames - Attempted: {TotalMinigamesAttempted} | Completed: {TotalMinigamesCompleted} | Success Rate: {MinigameSuccessRate:P0}");
        Debug.Log($"Session Duration: {SessionDuration:F1}s");
        Debug.Log($"Turn-Based Active: {IsInTurnBasedMode}");
    }

    [ContextMenu("Reset All State")]
    public void ResetAllState()
    {
        ResetToDefaults();
        LogDebug("All game state reset to defaults");
    }

    [ContextMenu("Test Applause Increase")]
    private void TestApplauseIncrease()
    {
        AdjustApplause(10f);
    }

    [ContextMenu("Test Boo Increase")]
    private void TestBooIncrease()
    {
        AdjustBoo(10f);
    }

    [ContextMenu("Test Score Add")]
    private void TestScoreAdd()
    {
        AddSceneScore(50);
    }

    private void LogDebug(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"[GameStateManager] {message}");
        }
    }

    private void LogWarning(string message)
    {
        Debug.LogWarning($"[GameStateManager] {message}");
    }

    private void LogError(string message)
    {
        Debug.LogError($"[GameStateManager] {message}");
    }

    #endregion
}

#region Enums

public enum AudienceReaction
{
    Hostile,       // 0.0 - 0.2
    Disappointed,  // 0.2 - 0.4
    Neutral,       // 0.4 - 0.6
    Supportive,    // 0.6 - 0.8
    Enthusiastic   // 0.8 - 1.0
}

#endregion
