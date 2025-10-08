using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controls the player's movement, interactions, and state within the game world.
/// </summary>
/// <remarks>This class handles player movement using Unity's physics system, processes input for movement,  and
/// manages interactions with other game systems such as the spotlight and dialogue manager.  It provides methods to
/// enable or disable movement and to check if the player is currently moving.  The player's movement is restricted
/// under certain conditions, such as when the player is in a spotlight  or when a dialogue sequence is active. These
/// conditions are managed dynamically during gameplay.</remarks>

[SelectionBase] // Ensures that clicking on child objects selects the parent GameObject in the editor
public class PlayerScript : MonoBehaviour
{
    #region Editor Data
    [Header("Movement Attributes")]
    [SerializeField] private float _moveSpeed = 5f;

    [Header("Dependencies")]
    [SerializeField] Rigidbody2D _rb;
    [SerializeField] Animator _animator;
    [SerializeField] SpriteRenderer _spriteRenderer;
    [SerializeField] Spotlight _spotlight; // Reference to spotlight
    [SerializeField] PlayerInteraction _playerInteraction; // Reference to interaction system
    #endregion

    #region Internal Fields
    private Vector2 _moveDir = Vector2.zero;
    private PlayerInput _playerInput;
    #endregion

    public bool inSpotlight { get; set; }
    public bool canMove { get; set; } = true; // Allow external control of movement

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.freezeRotation = true;
        _playerInput = GetComponent<PlayerInput>();
        
        // Get PlayerInteraction if not assigned
        if (_playerInteraction == null)
            _playerInteraction = GetComponent<PlayerInteraction>();
        
        // Find spotlight if not assigned
        if (_spotlight == null)
            _spotlight = FindAnyObjectByType<Spotlight>();
    }

    #region Input System
    private void GatherInput()
    {
        if (_playerInput != null && canMove)
        {
            _moveDir = _playerInput.actions["Move"].ReadValue<Vector2>();
        }
        else
        {
            _moveDir = Vector2.zero;
        }
    }

    private void FixedUpdate() // fixed update is called at a fixed interval and is independent of frame rate. Put physics code here.
    {
        MovementUpdate();
    }
    #endregion

    #region Tick
    private void Update() // update is called once per frame
    {
        GatherInput();
        
        // Check spotlight status each frame
        if (_spotlight != null)
        {
            _spotlight.CheckPlayerInSpotlight(this);
            
            // React to spotlight state change
            if (inSpotlight)
            {
                // TODO: Start turn-based gameplay
                
                // Disable movement during dialogue/turn-based mode
                if (DialogManager.Instance != null && DialogManager.Instance.IsDialogueActive())
                {
                    canMove = false;
                }
            }
            else
            {
                canMove = true; // Re-enable movement when not in spotlight
            }
        }
        
        // Disable movement during dialogue
        if (DialogManager.Instance != null && DialogManager.Instance.IsDialogueActive())
        {
            canMove = false;
        }
        else if (!inSpotlight) // Only re-enable if not in spotlight mode
        {
            canMove = true;
        }
    }
    #endregion

    #region Movement Logic
    private void MovementUpdate()
    {
        if (_rb != null && canMove)
        {
            Vector2 movement = _moveDir * _moveSpeed * Time.deltaTime;
            _rb.MovePosition(_rb.position + movement);
        }
    }
    #endregion

    #region Public Interface
    public void DisableMovement()
    {
        canMove = false;
    }

    public void EnableMovement()
    {
        // Only enable if not in dialogue
        if (DialogManager.Instance == null || !DialogManager.Instance.IsDialogueActive())
        {
            canMove = true;
        }
    }

    public bool IsMoving()
    {
        return _moveDir.magnitude > 0.1f && canMove;
    }
    #endregion
}
