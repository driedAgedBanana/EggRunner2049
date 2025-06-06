using UnityEngine;

[System.Serializable]
public class LootDrop
{
    public GameObject itemPrefab;
    [Range(0f, 100f)] public float dropChance;
}

public class Enemies : MonoBehaviour
{
    private WaveGenerator waveGenerator;

    private Transform _player;
    private Rigidbody2D _rb2D;
    private Vector3 _direction;

    private Vector2 _movement;
    [SerializeField] private float _moveSpeed;

    // Health
    [SerializeField] private int _healthAmount = 3;
    [SerializeField] private int _currentHealth;
    private bool _isAlive;

    [Header("Loot Drop Settings")]
    [SerializeField] private LootDrop[] _lootTable;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb2D = GetComponent<Rigidbody2D>();
        _isAlive = true;

        _currentHealth = _healthAmount;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            _player = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("Player not found! Make sure the player object is tagged 'Player'");
        }

        Debug.Log($"Enemy {_lootTable.Length} loot items loaded.");
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isAlive) return;

        LookAtPlayer();
    }

    private void LookAtPlayer()
    {
        _direction = _player.position - transform.position;

        float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        _rb2D.rotation = angle - 90f;
        _direction.Normalize();
        _movement = _direction;
    }

    private void FixedUpdate()
    {
        if (!_isAlive) return;

        MoveToPlayer(_movement);
    }

    private void MoveToPlayer(Vector2 MoveDirection)
    {
        _rb2D.MovePosition((Vector2)transform.position + (MoveDirection * _moveSpeed * Time.deltaTime));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            TakeDamageAndDie();
        }
    }

    public void Init(WaveGenerator manager)
    {
        waveGenerator = manager;
    }

    private void TakeDamageAndDie()
    {
        _currentHealth--;
        if (_currentHealth <= 0)
        {
            TryDropLoot();

            waveGenerator.OnEnemyKilled();
            Destroy(gameObject);
        }
    }

    private void TryDropLoot()
    {
        float roll = Random.Range(0f, 100f);
        float cumulative = 0f;

        for (int i = 0; i < _lootTable.Length; i++)
        {
            LootDrop loot = _lootTable[i];
            cumulative += loot.dropChance;

            if (roll <= cumulative)
            {
                if(loot.itemPrefab != null)
                {
                    Instantiate(loot.itemPrefab, transform.position, Quaternion.identity);
                }
                break;
            }
        }
    }
}
