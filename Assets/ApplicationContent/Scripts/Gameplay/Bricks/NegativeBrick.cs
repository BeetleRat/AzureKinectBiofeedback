using UnityEngine;

/// <summary>
/// <para>
/// A <see cref="AbstractProjectile">projectile</see> that looks like a brick.
/// If it comes in contact with player, it will take away part of his health points.
/// </para>
/// </summary>
public class NegativeBrick : AbstractProjectile
{
    [Tooltip("The amount of health that will be taken away from the player when the projectile comes into contact with the projectile")]
    [Range(0, 100)]
    [SerializeField] private int _hpDrain = 1;
    [Header("Blast settings")]
    [SerializeField] private float _crashForce = 6f;
    [SerializeField] private float _crashRadius = 6f;
    [SerializeField] private float _destructionTimeSec = 2.5f;

    /// <summary>
    /// <inheritdoc cref="AbstractProjectile.PerformAction"/>
    /// </summary>
    protected override void PerformAction()
    {
        levelManager?.DrainHealthPoint(_hpDrain);
        Crash(_crashForce, _crashRadius);
    }

    private void Crash(float force, float radius)
    {
        if (TryGetComponent(out Rigidbody rigidbody))
        {
            // Вернуть объекту возможность физического взаимодействия
            rigidbody.isKinematic = false;
            rigidbody.useGravity = true;
            // Растолкать объект в разные стороны
            rigidbody.AddExplosionForce(force,
                transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f)), radius);
        }
        Destroy(gameObject,_destructionTimeSec);
    }
}