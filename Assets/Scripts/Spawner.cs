using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject[] obstaclePrefabs;
    [SerializeField] private Transform obstacleParent;
    public float obstacleSpawnTime = 3f;
    [Range(0, 1)] public float obstacleSpawnTimeFactor = 0.3f;
    public float minObstacleSpawnTime = 0.77f;
    public float obstacleSpeed = 10f;
    [Range(0, 1)] public float obstacleSpeedFactor = 0.1f;
    public float maxObstacleSpeed = 15.75f;

    private float _obstacleSpawnTime;
    private float _obstacleSpeed;

    private float timeAlive;
    private float timeUntilObstacleSpawn;

    private void Start()
    {
        GameManager.Instance.onGameOver.AddListener(ClearObstacles);
        GameManager.Instance.onPlay.AddListener(ResetFactors);
    }

    private void Update()
    {
        if (GameManager.Instance.isPlaying)
        {
            timeAlive += Time.deltaTime;

            CalculateFactors();

            SpawnLoop();
        }
    }

    private void SpawnLoop()
    {
        timeUntilObstacleSpawn += Time.deltaTime;

        if (timeUntilObstacleSpawn >= _obstacleSpawnTime)
        {
            Spawn();
            timeUntilObstacleSpawn = 0f;
        }
    }

    private void ClearObstacles()
    {
        foreach (Transform child in obstacleParent)
        {
            Destroy(child.gameObject);
        }
    }

    private void CalculateFactors()
    {
        _obstacleSpawnTime = Mathf.Clamp(obstacleSpawnTime / Mathf.Pow(timeAlive, obstacleSpawnTimeFactor), minObstacleSpawnTime, obstacleSpawnTime);
        _obstacleSpeed = Mathf.Clamp(obstacleSpeed * Mathf.Pow(timeAlive, obstacleSpeedFactor), obstacleSpeed, maxObstacleSpeed);
    }

    private void ResetFactors()
    {
        timeAlive = 1f;
        _obstacleSpawnTime = obstacleSpawnTime;
        _obstacleSpeed = obstacleSpeed;
    }

    private void Spawn()
    {
        GameObject obstacleToSpawn = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];

        GameObject spawnedObstacle = Instantiate(obstacleToSpawn, transform.position, Quaternion.identity);
        spawnedObstacle.transform.parent = obstacleParent;

        Rigidbody2D obstacleRB = spawnedObstacle.GetComponent<Rigidbody2D>();
        obstacleRB.velocity = Vector2.left * _obstacleSpeed;
    }
}
