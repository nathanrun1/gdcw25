using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Obstacles that can be destroyed using the player's axe
/// </summary>
public abstract class Destroyable : Obstacle
{
    public virtual float maxHealth => 100f;
    [ReadOnly] public float currentHealth;

    public event Action<float> OnDamaged;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(float damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
        OnDamaged?.Invoke(damage);

        if (currentHealth == 0)
        {
            ObstacleManager.Instance.RemoveObstacleAt(pos);
        }
    }
}
