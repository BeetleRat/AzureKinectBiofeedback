using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// <para>Specific spavn speed settings for a particular game speed state</para>
/// </summary>
[Serializable]
struct SpawnerSpeed
{
    public Speed ForSpeed;
    public float MovementSpeed;
    [Range(1, 300)] 
    public float SpawnSpeed;
}

/// <summary>
/// <para>Spawned projectile info</para>
/// </summary>
[Serializable]
struct SpawnedProjectile
{
    public AbstractProjectile SpawnedObject;
    [Range(0, 100)] 
    public int SpawnChance;
    public ObjectMovement[] MovementPoints;
}

/// <summary>
/// <para>The component that performs projectile spawning</para>
/// It is a <see cref="AbstractSpeedChangingComponent"/>
/// </summary>
public class ProjectileSpawner : AbstractSpeedChangingComponent
{
    private const float SPEED_DIVIDER = 70;

    [SerializeField] private LevelManager _levelManager;
    [SerializeField] private SpawnedProjectile[] _projectiles;

    [SerializeField] private List<SpawnerSpeed> _speedSettings;

    private SpawnerSpeed currentSpeed;
    private bool isSpawnerActive = false;
    private float timeAfterSpawn;
    private List<AbstractProjectile> spawnedProjectiles;

    private void Awake()
    {
        spawnedProjectiles = new List<AbstractProjectile>();
        ValidateSS();
    }

    private void Start()
    {
        if (_speedSettings.Count == 0)
        {
            Debug.LogError($"[{name}]: Speed Settings List is empty.");
        }
        else
        {
            currentSpeed = _speedSettings[0];
        }

        _levelManager.GameStarted += StartSpawn;
        _levelManager.GameEnded += StopSpawnAndDestroyAllObjects;
        timeAfterSpawn = 200;
    }

    private void Update()
    {
        if (isSpawnerActive)
        {
            PreformSpawn();
        }
    }

    private void OnDestroy()
    {
        _levelManager.GameStarted -= StartSpawn;
        _levelManager.GameEnded -= StopSpawnAndDestroyAllObjects;
    }

    /// <summary>
    /// <para>Remove the specified projectile from the list of tracked projectiles</para>
    /// </summary>
    /// <param name="abstractProjectile">Removed projectile</param>
    public void RemoveProjectileFromSpawnedList(AbstractProjectile abstractProjectile)
    {
        spawnedProjectiles.Remove(abstractProjectile);
    }

    private void ValidateSS()
    {
        _speedSettings.Sort((ss1, ss2) => ss1.ForSpeed - ss2.ForSpeed);
        RemoveDuplicatesFromSS();
    }

    private void RemoveDuplicatesFromSS()
    {
        for (int i = _speedSettings.Count - 1; i > 0; i--)
        {
            if (_speedSettings[i].ForSpeed == _speedSettings[i - 1].ForSpeed)
            {
                _speedSettings.Remove(_speedSettings[i]);
            }
        }
    }

    private void StartSpawn()
    {
        isSpawnerActive = true;
    }

    private void StopSpawnAndDestroyAllObjects()
    {
        isSpawnerActive = false;
        for (int i = spawnedProjectiles.Count - 1; i >= 0; i--)
        {
            Destroy(spawnedProjectiles[i].gameObject);
        }
    }

    private void EndSpawn()
    {
        isSpawnerActive = false;
        _levelManager.EndGame();
    }

    private void PreformSpawn()
    {
        timeAfterSpawn += Time.deltaTime;
        if (timeAfterSpawn >= (100 / currentSpeed.SpawnSpeed))
        {
            SpawnRandomBrick();
            timeAfterSpawn = 0;
        }
    }

    private void SpawnRandomBrick()
    {
        int totalNumbers = 0;

        foreach (SpawnedProjectile brick in _projectiles)
        {
            totalNumbers += brick.SpawnChance;
        }

        int winner = Random.Range(1, totalNumbers);
        foreach (SpawnedProjectile brick in _projectiles)
        {
            totalNumbers -= brick.SpawnChance;
            if (winner >= totalNumbers)
            {
                SpawnBrick(brick);
                break;
            }
        }
    }

    private void SpawnBrick(SpawnedProjectile projectile)
    {
        ObjectMovement brickMovement = projectile.MovementPoints[Random.Range(0, projectile.MovementPoints.Length)];
        Transform spawnPoint = brickMovement.GetStartPoint();
        Transform destroyPoint = brickMovement.GetEndPoint();
        AbstractProjectile spawnProjectile =
            Instantiate(projectile.SpawnedObject, spawnPoint.position, brickMovement.transform.rotation);
        spawnProjectile.SetMovementSpeed(currentSpeed.MovementSpeed / SPEED_DIVIDER);
        spawnProjectile.SetDestroyPoint(destroyPoint);
        spawnProjectile.SetLevelManager(_levelManager);
        spawnProjectile.SetSpawner(this);
        spawnedProjectiles.Add(spawnProjectile);
    }

    /// <summary>
    /// <inheritdoc cref="AbstractSpeedChangingComponent.ChangeSpeed"/>
    /// </summary>
    /// <param name="speed"><inheritdoc cref="AbstractSpeedChangingComponent.ChangeSpeed"/></param>
    public override void ChangeSpeed(Speed speed)
    {
        if (speed == Speed.STOP)
        {
            EndSpawn();
            return;
        }

        if (_speedSettings.Count == 0)
        {
            Debug.LogError($"[{name}]: Speed Settings List is empty.");
            return;
        }

        foreach (SpawnerSpeed spawnerSpeed in _speedSettings)
        {
            if (spawnerSpeed.ForSpeed == speed)
            {
                currentSpeed = spawnerSpeed;
                return;
            }
        }

        LogNoSpeedSettingsFor(speed);
    }
}