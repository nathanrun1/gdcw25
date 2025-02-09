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
    public virtual float TempChangeRate { get => 2f; }

    public bool isFunctional = true;

    protected virtual void Awake()
    {
        curTemperature = StartingTemperature;
        Debug.Log("temp" + StartingTemperature);
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

        curTemperature += Mathf.Sqrt(diff) * TempChangeRate;
    }
}
