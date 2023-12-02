using UnityEngine;

/// <summary>
/// <para>
/// Abstract class of an object moving from the spawn point to the point of its destruction.
/// The projectile performs an action implemented in the implementation
/// when it comes in contact with <see cref="PlayerBodypart"/>
/// </para>
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public abstract class AbstractProjectile : MonoBehaviour
{
    /// <summary>
    /// <para><see cref="LevelManager"/> reference</para>
    /// </summary>
    protected LevelManager levelManager;

    private Transform destroyPoint;
    private float movementSpeed = 6f;
    private ProjectileSpawner projectileSpawner;
    private bool isBrickMove;

    protected virtual void Start()
    {
        isBrickMove = true;
    }

    protected virtual void FixedUpdate()
    {
        if (isBrickMove)
        {
            Move();
        }

        CheckDestination();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out PlayerBodypart player) && isBrickMove)
        {
            isBrickMove = false;
            PerformAction();
        }
    }

    private void OnDestroy()
    {
        projectileSpawner.RemoveProjectileFromSpawnedList(this);
    }

    private void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, destroyPoint.position, movementSpeed);
    }

    private void CheckDestination()
    {
        if (transform.position == destroyPoint.position)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// <para>Set level manager reference</para>
    /// </summary>
    /// <param name="levelManager">The level manager reference</param>
    public void SetLevelManager(LevelManager levelManager)
    {
        this.levelManager = levelManager;
    }

    /// <summary>
    /// <para>Set projectile spawner</para>
    /// </summary>
    /// <param name="projectileSpawner">The projectile spawner</param>
    public void SetSpawner(ProjectileSpawner projectileSpawner)
    {
        this.projectileSpawner = projectileSpawner;
    }

    /// <summary>
    /// <para>Set projectile movement speed</para>
    /// </summary>
    /// <param name="movementSpeed">The projectile movement speed</param>
    public void SetMovementSpeed(float movementSpeed)
    {
        this.movementSpeed = movementSpeed;
    }

    /// <summary>
    /// <para>Set point of projectile destruction</para>
    /// </summary>
    /// <param name="destroyPoint">Phe point of projectile destruction</param>
    public void SetDestroyPoint(Transform destroyPoint)
    {
        this.destroyPoint = destroyPoint;
    }

    /// <summary>
    /// <para>The action that will be performed when a projectile comes into contact with <see cref="PlayerBodypart"/></para>
    /// </summary>
    protected abstract void PerformAction();
}