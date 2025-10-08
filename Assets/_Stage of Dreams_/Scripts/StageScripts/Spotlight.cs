/* Spotlight.cs
 *  Handles the movement of the spotlight and checks if the player is within its radius.
 *  When the player or NPC enters and exits the spotlight, it updates their state accordingly.
 *  
 *  Movement: by Default remains static, but can be extended to move along a path or follow waypoints.
 *  
 *  Rotation: The spotlight can be rotated to face a specific direction or follow a target.
 */

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages spotlight movement and tracks which characters are within its radius.
/// Focuses purely on spotlight positioning and character state management.
/// Movement patterns: Static, Random, Target Following
/// </summary>
public class Spotlight : MonoBehaviour
{
    [Header("Spotlight Settings")]
    public float radius = 1.5f;
    [SerializeField] private LayerMask characterLayer = -1; // Which layers contain characters
    
    [Header("Movement Settings")]
    [SerializeField] private SpotlightMovementType movementType = SpotlightMovementType.Static;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private Transform targetToFollow; // Specific target to follow
    
    [Header("Random Movement")]
    [SerializeField] private Vector2 movementBounds = new Vector2(10f, 6f); // Random movement area
    [SerializeField] private float randomMoveInterval = 3f; // How often to pick new random target
    [SerializeField] private float arrivalThreshold = 0.5f; // How close to get to target
    
    [Header("Character Detection")]
    [SerializeField] private bool trackAllCharacters = true; // Track all characters or just player
    [SerializeField] private PlayerScript specificPlayer; // Specific player to track if not tracking all
    
    // Movement state
    private Vector2 randomTarget;
    private float lastRandomMoveTime;
    private Vector2 originalPosition;
    
    // Character tracking
    private HashSet<ISpotlightCharacter> charactersInSpotlight = new HashSet<ISpotlightCharacter>();
    private List<ISpotlightCharacter> allTrackedCharacters = new List<ISpotlightCharacter>();
    
    // Events for external systems to subscribe to
    public System.Action<ISpotlightCharacter> OnCharacterEnteredSpotlight;
    public System.Action<ISpotlightCharacter> OnCharacterExitedSpotlight;
    public System.Action<Vector2> OnSpotlightMoved;
    
    public enum SpotlightMovementType
    {
        Static,         // Doesn't move
        Random,         // Moves to random positions within bounds
        FollowTarget    // Follows a specific target
    }
    
    private void Awake()
    {
        // Store original position for reference
        originalPosition = transform.position;
        
        // Find specific player if not assigned and not tracking all characters
        if (!trackAllCharacters && specificPlayer == null)
        {
            specificPlayer = FindFirstObjectByType<PlayerScript>();
        }
        
        // Set up initial random target
        if (movementType == SpotlightMovementType.Random)
        {
            GenerateNewRandomTarget();
        }
    }
    
    private void Start()
    {
        // Find all characters to track
        RefreshTrackedCharacters();
    }
    
    private void Update()
    {
        // Handle movement
        UpdateSpotlightMovement();
        
        // Check character positions
        UpdateCharacterDetection();
    }
    
    #region Movement Methods
    
    /// <summary>
    /// Update spotlight movement based on current movement type
    /// </summary>
    private void UpdateSpotlightMovement()
    {
        Vector2 currentPos = transform.position;
        Vector2 newPos = currentPos;
        bool moved = false;
        
        switch (movementType)
        {
            case SpotlightMovementType.Static:
                // No movement
                break;
                
            case SpotlightMovementType.Random:
                newPos = UpdateRandomMovement(currentPos);
                moved = Vector2.Distance(currentPos, newPos) > 0.01f;
                break;
                
            case SpotlightMovementType.FollowTarget:
                newPos = UpdateTargetFollowing(currentPos);
                moved = Vector2.Distance(currentPos, newPos) > 0.01f;
                break;
        }
        
        if (moved)
        {
            transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
            OnSpotlightMoved?.Invoke(newPos);
        }
    }
    
    /// <summary>
    /// Handle random movement pattern
    /// </summary>
    private Vector2 UpdateRandomMovement(Vector2 currentPos)
    {
        // Check if we need a new random target
        if (Time.time - lastRandomMoveTime > randomMoveInterval || 
            Vector2.Distance(currentPos, randomTarget) < arrivalThreshold)
        {
            GenerateNewRandomTarget();
            lastRandomMoveTime = Time.time;
        }
        
        // Move towards random target
        return Vector2.MoveTowards(currentPos, randomTarget, moveSpeed * Time.deltaTime);
    }
    
    /// <summary>
    /// Handle target following movement
    /// </summary>
    private Vector2 UpdateTargetFollowing(Vector2 currentPos)
    {
        if (targetToFollow == null) return currentPos;
        
        Vector2 targetPos = targetToFollow.position;
        return Vector2.MoveTowards(currentPos, targetPos, moveSpeed * Time.deltaTime);
    }
    
    /// <summary>
    /// Generate a new random target position within bounds
    /// </summary>
    private void GenerateNewRandomTarget()
    {
        float randomX = Random.Range(originalPosition.x - movementBounds.x/2, originalPosition.x + movementBounds.x/2);
        float randomY = Random.Range(originalPosition.y - movementBounds.y/2, originalPosition.y + movementBounds.y/2);
        randomTarget = new Vector2(randomX, randomY);
    }
    
    #endregion
    
    #region Character Detection Methods
    
    /// <summary>
    /// Update which characters are in the spotlight
    /// </summary>
    private void UpdateCharacterDetection()
    {
        Vector2 spotlightPos = transform.position;
        
        // Check each tracked character
        foreach (var character in allTrackedCharacters)
        {
            if (character == null) continue;
            
            bool wasInSpotlight = charactersInSpotlight.Contains(character);
            bool isInSpotlight = IsCharacterInSpotlight(character, spotlightPos);
            
            // Handle state changes
            if (isInSpotlight && !wasInSpotlight)
            {
                // Character entered spotlight
                charactersInSpotlight.Add(character);
                character.SetInSpotlight(true);
                OnCharacterEnteredSpotlight?.Invoke(character);
                
                Debug.Log($"{character.GetCharacterName()} entered spotlight: {gameObject.name}");
            }
            else if (!isInSpotlight && wasInSpotlight)
            {
                // Character exited spotlight
                charactersInSpotlight.Remove(character);
                character.SetInSpotlight(false);
                OnCharacterExitedSpotlight?.Invoke(character);
                
                Debug.Log($"{character.GetCharacterName()} exited spotlight: {gameObject.name}");
            }
        }
    }
    
    /// <summary>
    /// Check if a specific character is within spotlight radius
    /// </summary>
    private bool IsCharacterInSpotlight(ISpotlightCharacter character, Vector2 spotlightPos)
    {
        if (character == null) return false;
        
        Vector2 characterPos = character.GetPosition();
        return Vector2.Distance(spotlightPos, characterPos) <= radius;
    }
    
    /// <summary>
    /// Refresh the list of characters to track
    /// </summary>
    public void RefreshTrackedCharacters()
    {
        allTrackedCharacters.Clear();
        
        if (trackAllCharacters)
        {
            // Find all characters in the scene
            var players = FindObjectsByType<PlayerScript>(FindObjectsSortMode.None);
            foreach (var player in players)
            {
                allTrackedCharacters.Add(new PlayerCharacterWrapper(player));
            }
            
            // Add other character types here as needed
            // var npcs = FindObjectsOfType<NPCCharacter>();
            // foreach (var npc in npcs)
            // {
            //     allTrackedCharacters.Add(new NPCCharacterWrapper(npc));
            // }
        }
        else if (specificPlayer != null)
        {
            // Track only the specific player
            allTrackedCharacters.Add(new PlayerCharacterWrapper(specificPlayer));
        }
    }
    
    #endregion
    
    #region Public Interface
    
    /// <summary>
    /// Set the movement type and target
    /// </summary>
    public void SetMovementType(SpotlightMovementType type, Transform target = null)
    {
        movementType = type;
        if (type == SpotlightMovementType.FollowTarget)
        {
            targetToFollow = target;
        }
    }
    
    /// <summary>
    /// Move spotlight to a specific position
    /// </summary>
    public void MoveToPosition(Vector2 position)
    {
        transform.position = new Vector3(position.x, position.y, transform.position.z);
        OnSpotlightMoved?.Invoke(position);
    }
    
    /// <summary>
    /// Set a specific target to follow
    /// </summary>
    public void SetTarget(Transform target)
    {
        targetToFollow = target;
        if (movementType != SpotlightMovementType.FollowTarget)
        {
            movementType = SpotlightMovementType.FollowTarget;
        }
    }
    
    /// <summary>
    /// Start random movement within specified bounds
    /// </summary>
    public void StartRandomMovement(Vector2 bounds)
    {
        movementBounds = bounds;
        movementType = SpotlightMovementType.Random;
        GenerateNewRandomTarget();
    }
    
    /// <summary>
    /// Stop all movement (set to static)
    /// </summary>
    public void StopMovement()
    {
        movementType = SpotlightMovementType.Static;
    }
    
    /// <summary>
    /// Get all characters currently in spotlight
    /// </summary>
    public ISpotlightCharacter[] GetCharactersInSpotlight()
    {
        var result = new ISpotlightCharacter[charactersInSpotlight.Count];
        charactersInSpotlight.CopyTo(result);
        return result;
    }
    
    /// <summary>
    /// Check if any character is in the spotlight
    /// </summary>
    public bool HasCharacterInSpotlight()
    {
        return charactersInSpotlight.Count > 0;
    }
    
    /// <summary>
    /// Check if a specific character is in this spotlight
    /// </summary>
    public bool IsCharacterInSpotlight(ISpotlightCharacter character)
    {
        return charactersInSpotlight.Contains(character);
    }
    
    /// <summary>
    /// Force update character detection (useful after moving spotlight manually)
    /// </summary>
    public void ForceUpdateCharacterDetection()
    {
        UpdateCharacterDetection();
    }
    
    #endregion
    
    #region Gizmos and Debugging
    
    private void OnDrawGizmosSelected()
    {
        // Draw spotlight radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
        
        // Draw movement bounds for random movement
        if (movementType == SpotlightMovementType.Random)
        {
            Gizmos.color = Color.cyan;
            Vector2 center = Application.isPlaying ? originalPosition : (Vector2)transform.position;
            Vector3 boundsSize = new Vector3(movementBounds.x, movementBounds.y, 0);
            Gizmos.DrawWireCube(center, boundsSize);
            
            // Draw current random target
            if (Application.isPlaying)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(randomTarget, 0.2f);
                Gizmos.DrawLine(transform.position, randomTarget);
            }
        }
        
        // Draw connection to follow target
        if (movementType == SpotlightMovementType.FollowTarget && targetToFollow != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, targetToFollow.position);
        }
        
        // Draw characters in spotlight
        if (Application.isPlaying)
        {
            Gizmos.color = Color.magenta;
            foreach (var character in charactersInSpotlight)
            {
                if (character != null)
                {
                    Gizmos.DrawWireSphere(character.GetPosition(), 0.3f);
                }
            }
        }
    }
    
    #endregion
}

/// <summary>
/// Interface for characters that can be tracked by spotlights
/// </summary>
public interface ISpotlightCharacter
{
    Vector2 GetPosition();
    string GetCharacterName();
    void SetInSpotlight(bool inSpotlight);
    bool IsInSpotlight();
}

/// <summary>
/// Wrapper to make PlayerScript compatible with ISpotlightCharacter
/// </summary>
public class PlayerCharacterWrapper : ISpotlightCharacter
{
    private PlayerScript player;
    
    public PlayerCharacterWrapper(PlayerScript player)
    {
        this.player = player;
    }
    
    public Vector2 GetPosition()
    {
        return player.transform.position;
    }
    
    public string GetCharacterName()
    {
        return player.gameObject.name;
    }
    
    public void SetInSpotlight(bool inSpotlight)
    {
        player.inSpotlight = inSpotlight;
    }
    
    public bool IsInSpotlight()
    {
        return player.inSpotlight;
    }
}