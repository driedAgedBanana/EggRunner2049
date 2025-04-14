using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 5f;
    public Rigidbody2D _rb2D;
    public Weapon weapon;

    private Vector2 _moveDirection;
    private Vector2 _mousePosition;

    [Header("Health")]
    [SerializeField] private int _playerHealth;
    [SerializeField] private int _currentHealth;
    private bool _isAlive;

    public int numberOfHearts;

    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;


    [Header("Dash Settings")]
    [SerializeField] private float dashForce = 10f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private TrailRenderer trailRenderer;

    private bool _canDash = true;
    private bool _isDashing = false;

    private void Start()
    {
        _isAlive = true;

        _playerHealth = numberOfHearts;
        _currentHealth = _playerHealth;

        if (trailRenderer != null)
            trailRenderer.emitting = false;
    }

    private void Update()
    {
        if (!_isAlive) return;

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        if (Input.GetMouseButtonDown(0))
        {
            weapon.Fire();
        }

        _moveDirection = new Vector2(moveX, moveY).normalized;
        _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetKeyDown(KeyCode.Space) && _canDash)
        {
            StartCoroutine(Dash());
        }

        CheckHealthHeart();

    }

    private void FixedUpdate()
    {
        if (!_isAlive) return;

        _rb2D.linearVelocity = new Vector2(_moveDirection.x * _moveSpeed, _moveDirection.y * _moveSpeed);

        Vector2 aimDirection = _mousePosition - _rb2D.position;
        float aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg - 90f;
        _rb2D.rotation = aimAngle;
    }

    private void CheckHealthHeart()
    {
        if(_currentHealth > numberOfHearts)
        {
            _currentHealth = numberOfHearts;
        }
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < _currentHealth)
            {
                hearts[i].sprite = fullHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }

            if (i < numberOfHearts)
            {
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
            }
        }
    }
    private void TakeDamageAndDie()
    {
        _currentHealth--;

        if (_currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamageAndDie();
        }
    }

    private IEnumerator Dash()
    {
        _canDash = false;
        _isDashing = true;

        // Enable trail
        if (trailRenderer != null)
            trailRenderer.emitting = true;

        // Store original speed and apply dash force
        float originalSpeed = _moveSpeed;
        _moveSpeed = dashForce;

        yield return new WaitForSeconds(dashDuration);

        // Reset speed and trail
        _moveSpeed = originalSpeed;
        _isDashing = false;

        if (trailRenderer != null)
            trailRenderer.emitting = false;
        trailRenderer.Clear();

        yield return new WaitForSeconds(dashCooldown);
        _canDash = true;
    }

}
