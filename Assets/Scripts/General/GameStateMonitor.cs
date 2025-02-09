using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameStateMonitor : MonoBehaviour
{

    private int waveCounter = 1;
    private float playerHealth;
    [SerializeField] GameObject iceDrillPrefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (waveCounter == 5) {
            GameObject[] enemies = PrefabUtility.FindAllInstancesOfPrefab(iceDrillPrefab);
            if (enemies.Length == 0) {
                // Game Won
            }
        }

        if (playerHealth <= 30) {
            // game lost
        }
    }


    public void setWaveCounter(int waveCounter) {
        this.waveCounter = waveCounter;
    }

    public void setPlayerHealth(float playerHealth) {
        this.playerHealth = playerHealth;
    }
}
