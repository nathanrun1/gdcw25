using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Destroyable : MonoBehaviour
{
    public virtual float maxHealth => 100f;
    public float currentHealth;

    public event Action<float> OnDamaged;
    public event Action OnDestroyed;

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
            DestroyObject();
        }
    }

    public virtual void DestroyObject()
    {
        OnDestroyed?.Invoke();

        // destroy anim?
        Destroy(gameObject);
    }

}
