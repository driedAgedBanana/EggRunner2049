using System.Collections;
using TMPro;
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

    [SerializeField] private int _extraLifeEggsCollected = 0;
    [SerializeField] private int _maxHealth = 10;

    [Header("Dash Settings")]
    [SerializeField] private float dashForce = 10f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    public TrailRenderer trailRenderer;
    private float currentCoolDown;

    private bool _canDash = true;
    private bool _isDashing = false;
    private Coroutine _dashCoroutine;

    public Image wingedBoots;

    [SerializeField] private int _extraDashEggsCollected = 0;
    [SerializeField] private float _newDashCoolDown = 0.5f;

    [Header("Teleportation")]
    [SerializeField] private Teleportation teleport;
    private bool _canTeleport = true;
    private bool _isTeleporting = false;
    [SerializeField] private float _teleportCoolDown;
    public Image teleportImage;
    private Coroutine _teleportCoroutine;

    [Header("Text and UI")]
    public TextMeshProUGUI currentEggs;

    [Header("Firemodes")]
    private bool _singleMode;
    private bool _trippleMode;
    private bool _hasToggled;

    private void Start()
    {
        _isAlive = true;

        _playerHealth = numberOfHearts;
        _currentHealth = _playerHealth;

        currentCoolDown = dashCooldown;
        wingedBoots.gameObject.SetActive(false);
        teleportImage.gameObject.SetActive(true);

        _singleMode = true;
        _trippleMode = false;
        _hasToggled = false;

        currentEggs.text = "Ovum Tenes: 0";

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
            if(_dashCoroutine != null)
            {
                StopCoroutine(_dashCoroutine);
            }
            _dashCoroutine = StartCoroutine(Dash());
        }

        if (Input.GetKeyDown(KeyCode.T) && _canTeleport)
        {
            if(_teleportCoroutine != null)
            {
                StopCoroutine(_teleportCoroutine);
            }
            _teleportCoroutine = StartCoroutine(Teleport());
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

    public void TakeDamage(int amount = 1)
    {
        _currentHealth -= amount;
        if (_currentHealth < 0) Die();
    }

    private void Die()
    {
        _isAlive = false;
        GameManager.Instance.ShowDeathScreen();
    }


    public void HealPlayer(int amount)
    {
        if (_currentHealth < numberOfHearts)
        {
            _currentHealth += amount;

            if (_currentHealth > numberOfHearts)
                _currentHealth = numberOfHearts;
        }
    }

    public void CollectExtraLifeEgg(int requiredEggs)
    {
        // If we've already hit max health, hide UI and return early
        if (_playerHealth >= _maxHealth)
        {
            currentEggs.gameObject.SetActive(false);
            return;
        }

        _extraLifeEggsCollected++;
        currentEggs.text = _extraLifeEggsCollected == 1 ? "Ovum Tenes: 1" : "Ova Tenes: " + _extraLifeEggsCollected;

        if (_extraLifeEggsCollected >= requiredEggs)
        {
            _playerHealth++;
            _currentHealth = _playerHealth;
            numberOfHearts = _playerHealth;
            _extraLifeEggsCollected = 0;

            // If we've just reached max health, hide the UI
            if (_playerHealth >= _maxHealth)
            {
                currentEggs.gameObject.SetActive(false);
            }
        }
    }


    public void ReduceCoolDownDashEgg(int requiredEggs)
    {
        _extraDashEggsCollected++;

        if(_extraDashEggsCollected >= requiredEggs)
        {
            currentCoolDown = _newDashCoolDown;
            wingedBoots.gameObject.SetActive(true);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage();
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

        yield return new WaitForSeconds(currentCoolDown);
        _canDash = true;
    }

    public void CancelDash()
    {
        if (_dashCoroutine != null)
        {
            StopCoroutine(_dashCoroutine);
            _dashCoroutine = null;
        }

        _isDashing = false;
        _moveSpeed = 5f;

        if (trailRenderer != null)
        {
            trailRenderer.emitting = false;
            trailRenderer.Clear();
        }

        _canDash = true;
    }

    private IEnumerator Teleport()
    {
        _canTeleport = false;
        _isTeleporting = true;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color originalColor = sr.color;

        // Fade out
        for (float f = 1f; f >= 0; f -= 0.1f)
        {
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, f);
            yield return new WaitForSeconds(0.01f);
        }

        teleport.TeleportPlayer();

        // Fade in
        for (float f = 0; f <= 1f; f += 0.1f)
        {
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, f);
            yield return new WaitForSeconds(0.01f);
        }

        teleportImage.gameObject.SetActive(false);
        yield return new WaitForSeconds(_teleportCoolDown);
        teleportImage.gameObject.SetActive(true);

        _canTeleport = true;
        _isTeleporting = false;
    }


}
