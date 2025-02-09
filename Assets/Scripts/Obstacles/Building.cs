using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Building : Obstacle
{
    public float curTemperature;

    public virtual float StartingTemperature {
        get => GridManager.Instance.GetGridSquareAt(pos).Temperature;
    }

    public virtual float FunctionalTemperature { get => 10f; }
    public virtual float IntegrityTemperature { get => -10f; }
    public virtual float TempChangeRate { get => 0.1f; }

    public bool isFunctional = true;

    protected virtual void Awake()
    {
        curTemperature = StartingTemperature;
    }

    protected virtual void FixedUpdate()
    {
        UpdateTemperature(Time.fixedDeltaTime);
        if (curTemperature < FunctionalTemperature) isFunctional = false;
        if (curTemperature < IntegrityTemperature) DestroyObstacle();
    }

    protected virtual void UpdateTemperature(float deltaTime)
    {
        float surroundTemp = GridManager.Instance.GetGridSquareAt(pos).Temperature;
        float diff = surroundTemp - curTemperature;
        //Debug.Log("updating: " + curTemperature + ", diff: " + diff);
        //Debug.Log("change: " + Mathf.Sqrt(Mathf.Abs(diff)) * 0.1f * TempChangeRate * Mathf.Sign(diff));
        curTemperature += Mathf.Sqrt(Mathf.Abs(diff)) * 0.1f * TempChangeRate * Mathf.Sign(diff);
        //Debug.Log("updated: " + curTemperature);
    }
}
