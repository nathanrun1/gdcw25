using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Obstacle : MonoBehaviour
{
    /// <summary>
    /// Id of this obstacle type
    /// </summary>
    [ReadOnly] public int id;
    /// <summary>
    /// Grid position of this obstacle
    /// </summary>
    [ReadOnly] public Vector2Int pos;

    public event Action OnDestroyed;

    public virtual void DestroyObstacle()
    {
        OnDestroyed?.Invoke();

        // destroy anim?
        Destroy(gameObject);
    }
}
