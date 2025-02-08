using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : Destroyable
{
    public override float maxHealth => 50f;

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
    }
}
