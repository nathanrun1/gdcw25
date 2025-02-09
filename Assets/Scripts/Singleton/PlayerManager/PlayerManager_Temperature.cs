using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public partial class PlayerManager : MonoSingleton<PlayerManager>
{
    [ReadOnly]
    public float PlayerTemperature = 25f;
    [SerializeField] GameObject gameStateMonitor;

    public float GetTemperatureRateOfChange()
    {
        float surroundingTemp = (float)GridManager.Instance.GetGridSquareAt(PlayerGridPosition).Temperature;
        float neutralTemp = playerConfig.playerNeutralTemperature;
        float rateOfChangeMultiplier = playerConfig.playerTemperatureROCMultiplier;

        // Check if temperature within ignore radius (i.e. too close to neutral temp to have any effect)
        if (surroundingTemp > neutralTemp - playerConfig.playerIgnoreTempRadius && surroundingTemp < neutralTemp + playerConfig.playerIgnoreTempRadius) return 0;

        return Mathf.Sqrt(Mathf.Abs(surroundingTemp - neutralTemp)) * Mathf.Sign(surroundingTemp - neutralTemp) * 0.1f * rateOfChangeMultiplier;
    }

    private void InitTemperature()
    {
        PlayerTemperature = playerConfig.playerInitialTemperature;
    }

    private void UpdateTemperature(float deltaTime)
    {
        float minTemp = playerConfig.playerDeathTemperature;
        float maxTemp = playerConfig.playerMaxTemperature;
        PlayerTemperature = Mathf.Clamp( PlayerTemperature + GetTemperatureRateOfChange() * deltaTime, minTemp, maxTemp);
        gameStateMonitor.GetComponent<GameStateMonitor>().setPlayerHealth(PlayerTemperature);
    }
}
