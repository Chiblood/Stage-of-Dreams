using UnityEngine;
using UnityEngine.InputSystem;

[SelectionBase]
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
    #endregion

    #region Internal Fields
    private Vector2 _moveDir = Vector2.zero;
    private PlayerInput _playerInput;
    #endregion

    public bool inSpotlight { get; set; }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.freezeRotation = true;
        _playerInput = GetComponent<PlayerInput>();
        
        // Find spotlight if not assigned
        if (_spotlight == null)
            _spotlight = FindObjectOfType<Spotlight>();
    }

    #region Input System
    private void GatherInput()
    {
        if (_playerInput != null)
        {
            _moveDir = _playerInput.actions["Move"].ReadValue<Vector2>();
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
                Debug.Log("Player entered spotlight - Begin turn-based mode!");
                // TODO: Start turn-based gameplay
            }
        }
    }
    #endregion

    #region Movement Logic
    private void MovementUpdate()
    {
        if (_rb != null)
        {
            Vector2 movement = _moveDir * _moveSpeed * Time.deltaTime;
            _rb.MovePosition(_rb.position + movement);
        }
    }
    #endregion
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //void Start()
    //{

    //}
}
