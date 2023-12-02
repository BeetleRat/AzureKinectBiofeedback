using DG.Tweening;
using UnityEngine;

/// <summary>
/// <para>
/// A <see cref="AbstractProjectile">projectile</see> that looks like a brick.
/// If it comes in contact with player, it will increase the player's score
/// </para>
/// </summary>
[RequireComponent(typeof(MeshRenderer))]
public class PositiveBrick : AbstractProjectile
{
    [Tooltip("The number of points a player will get")] [Range(1, 100)] 
    [SerializeField] private int _scorePoints;

    [Header("Vanish settings")] 
    [SerializeField] private float _transformMultiplier;
    [SerializeField] private float _vanishSpeed;

    private bool isVanishing;

    /// <summary>
    /// <inheritdoc cref="AbstractProjectile.PerformAction"/>
    /// </summary>
    protected override void PerformAction()
    {
        levelManager.AddScore(_scorePoints);
        Vanishing();
    }

    private void Vanishing()
    {
        transform.DOScale(transform.localScale * _transformMultiplier, _vanishSpeed);

        if (TryGetComponent(out MeshRenderer meshRenderer))
        {
            meshRenderer.material.DOFade(0f, _vanishSpeed).OnComplete(() => Destroy(gameObject));
        }
    }
}