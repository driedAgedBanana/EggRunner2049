using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class WaveEnemyConfig
{
    public GameObject enemyPrefab; // The enemy prefab to spawn
    public int unlockAtWave = 1;   // This enemy becomes available at this wave number
    public int spawnWeight = 1;    // Higher number = higher chance to spawn
}

public class WaveGenerator : MonoBehaviour
{
    [Header("Enemy Settings")]
    public List<WaveEnemyConfig> enemyTypes;  // List of all enemy configurations
    public Collider2D spawnArea;              // Defines where enemies can spawn
    public int enemiesPerWave = 5;            // Number of enemies per wave
    public float timeBetweenWaves = 3f;       // Delay between waves

    [Header("Obstacle Settings")]
    public GameObject[] obstaclePrefabs;      // Pool of obstacles to spawn
    public Transform[] obstacleSpawnPoints;   // Fixed points where obstacles can spawn

    [Header("Spawn Settings")]
    public LayerMask obstacleLayer;           // LayerMask for obstacles (including the player)
    public float minSpawnDistance = 5f;       // Minimum distance from player and obstacles

    private List<GameObject> _activeObstacles = new List<GameObject>(); // Currently active obstacles
    private bool _isSpawning = false;         // Is a wave currently spawning?
    private int _enemiesAlive;                // Number of enemies currently alive
    private int _currentWave = 1;             // Current wave number

    [Header("Wave Settings")]
    public TextMeshProUGUI waveText;
    private int _currentWaveInt;

    void Start()
    {
        // Start the first wave at game start
        StartCoroutine(StartNextWave());

        // Update wave text UI
        if (waveText != null)
        {
            Debug.Log("Call wave!");
            waveText.text = "Impulsus: " + 1;
        }
    }

    void Update()
    {
        // When all enemies are dead and we're not already spawning, start a new wave
        if (_enemiesAlive <= 0 && !_isSpawning)
        {
            StartCoroutine(StartNextWave());
        }
    }

    IEnumerator StartNextWave()
    {
        _isSpawning = true;

        // Wait before spawning next wave
        yield return new WaitForSeconds(timeBetweenWaves);

        // Reset obstacles for the new wave
        ClearObstacles();
        SpawnRandomObstacles();

        _enemiesAlive = enemiesPerWave;

        // Spawn all enemies for this wave
        for (int i = 0; i < enemiesPerWave; i++)
        {
            SpawnEnemy();
        }

        // Increase wave number and enemy count for scaling
        _currentWave++;
        _currentWaveInt = _currentWave;
        enemiesPerWave += 1;

        // Update wave text UI
        if (waveText != null)
        {
            Debug.Log("Call wave!");
            waveText.text = "Impulsus: " + _currentWaveInt;
        }

        _isSpawning = false;
    }

    void SpawnEnemy()
    {
        // Pick an enemy prefab based on the current wave config
        GameObject selectedEnemy = GetRandomUnlockedEnemy();

        // Get a valid random position within the spawn area, avoiding obstacles
        Vector2 spawnPosition = GetValidSpawnPosition();

        // Instantiate and initialize the enemy
        GameObject enemy = Instantiate(selectedEnemy, spawnPosition, Quaternion.identity);
        enemy.GetComponent<Enemies>().Init(this); // Ensure enemy calls OnEnemyKilled() on death
    }

    GameObject GetRandomUnlockedEnemy()
    {
        // List to hold all enemies that are allowed to spawn this wave
        List<WaveEnemyConfig> unlockedEnemies = new List<WaveEnemyConfig>();

        // Loop through all enemy configs
        for (int i = 0; i < enemyTypes.Count; i++)
        {
            WaveEnemyConfig config = enemyTypes[i];

            // Only include enemies that are unlocked for this wave
            if (_currentWave >= config.unlockAtWave)
            {
                // Use spawnWeight to duplicate the entry for higher chance
                for (int w = 0; w < config.spawnWeight; w++)
                {
                    unlockedEnemies.Add(config);
                }
            }
        }

        // If somehow no enemies are unlocked, fallback to first defined type
        if (unlockedEnemies.Count == 0)
        {
            Debug.LogWarning("No enemies unlocked for current wave!");
            return enemyTypes[0].enemyPrefab;
        }

        // Pick one at random
        int randomIndex = Random.Range(0, unlockedEnemies.Count);
        WaveEnemyConfig chosen = unlockedEnemies[randomIndex];
        return chosen.enemyPrefab;
    }

    Vector2 GetValidSpawnPosition()
    {
        Vector2 spawnPosition = GetRandomPositionInZone();

        // Keep checking until a valid position is found
        while (IsTooCloseToPlayerOrObstacle(spawnPosition))
        {
            spawnPosition = GetRandomPositionInZone(); // Try a different position
        }

        return spawnPosition;
    }

    Vector2 GetRandomPositionInZone()
    {
        // Get a random point within the spawn area bounds
        Bounds bounds = spawnArea.bounds;
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);
        return new Vector2(x, y);
    }

    bool IsTooCloseToPlayerOrObstacle(Vector2 position)
    {
        // Check if the position is too close to the player or any obstacles
        Collider2D hit = Physics2D.OverlapCircle(position, minSpawnDistance, obstacleLayer);
        return hit != null; // Return true if something is in the way
    }

    void SpawnRandomObstacles()
    {
        // Choose how many obstacles to spawn (between 1 and 3)
        int numObstacles = Random.Range(1, 4);

        for (int i = 0; i < numObstacles; i++)
        {
            // Make sure we have valid obstacle and spawn data
            if (obstaclePrefabs.Length == 0 || obstacleSpawnPoints.Length == 0)
                return;

            // Pick random prefab and spawn location
            GameObject prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
            Transform spawnPoint = obstacleSpawnPoints[Random.Range(0, obstacleSpawnPoints.Length)];

            // Instantiate and track the obstacle
            GameObject obstacle = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
            _activeObstacles.Add(obstacle);
        }
    }

    void ClearObstacles()
    {
        // Destroy all previously spawned obstacles
        for (int i = 0; i < _activeObstacles.Count; i++)
        {
            GameObject obj = _activeObstacles[i];
            if (obj != null)
            {
                Destroy(obj);
            }
        }

        _activeObstacles.Clear(); // Reset list
    }

    public void OnEnemyKilled()
    {
        // Called by enemy when it dies to track alive count
        _enemiesAlive--;
    }
}
