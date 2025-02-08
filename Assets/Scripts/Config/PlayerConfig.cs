using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Config/PlayerConfig", menuName = "ScriptableObjects/PlayerConfig", order = 1)]
public class PlayerConfig : ScriptableObject
{
    // Temperature system config
    public float playerInitialTemperature = 37f;
    public float playerTemperatureROCMultiplier = 0.1f;
    public float playerNeutralTemperature = 20f; // Surrounding temp above increases player temp, surrounding temp below decreases it
    public float playerMaxTemperature = 45f; // Highest possible temperature (for now this is like max health)
    public float playerDeathTemperature = 29f; // Once player reaches this, game over
    public float playerIgnoreTempRadius = 2f; // Minimum distance that surrounding temp needs to be from neutral temp for player temp to change
}
