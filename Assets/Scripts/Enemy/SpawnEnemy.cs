using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using UnityEngine.SceneManagement;



public class SpawnEnemy : MonoBehaviour
{
    [SerializeField] private int playerSafeZoneRadius;
    [SerializeField] private int spawnZoneWidth;
    [SerializeField] private int buildingSafeZoneRadius;
    [SerializeField] private GameObject enemyType;
    [SerializeField] private GameObject campfirePrefab;
    [SerializeField] private GameObject crossbowPrefab;
    [SerializeField] private GameObject snowGenerator;

    // key is wave length, value is the number of enemies spawns
    private Dictionary<int, Tuple<float, int>> spawnWaveInfo = new Dictionary<int, Tuple<float, int>>();
    private float waveTimer;
    private int waveNumber;


    // Start is called before the first frame update
    void Start()
    {
        spawnWaveInfo.Add(1, new Tuple<float, int>(5f, 10));
<<<<<<< HEAD
        spawnWaveInfo.Add(2, new Tuple<float, int>(60f, 20));
        spawnWaveInfo.Add(3, new Tuple<float, int>(60f, 35));
        spawnWaveInfo.Add(4, new Tuple<float, int>(60f, 65));
        spawnWaveInfo.Add(5, new Tuple<float, int>(60f, 100));
=======
        spawnWaveInfo.Add(2, new Tuple<float, int>(5f, 20));
        spawnWaveInfo.Add(3, new Tuple<float, int>(5f, 35));
        spawnWaveInfo.Add(4, new Tuple<float, int>(5f, 65));
        spawnWaveInfo.Add(5, new Tuple<float, int>(5f, 100));
>>>>>>> f3dfbcee9a66629eebd86593086765366f90984f



        transform.position = PlayerManager.Instance.PlayerWorldPosition;
        waveNumber = 1;
        waveTimer = spawnWaveInfo[waveNumber].Item1;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = PlayerManager.Instance.PlayerWorldPosition;
        if (Input.GetKeyDown(KeyCode.E)) {
            Spawn(1);
        }

        waveTimer -= Time.deltaTime;
        if (waveTimer <= 0) {
            waveTimer = spawnWaveInfo[waveNumber].Item1;
            Spawn(spawnWaveInfo[waveNumber].Item2);



            waveNumber += 1;
            snowGenerator.GetComponent<SnowGenerator>().updateEmission(waveNumber * 20);
            if (waveNumber == 5) {
                Destroy(gameObject);
            }
        }
        
    }


    private void Spawn(int amount) {
        System.Random rng = new System.Random();

        float degree, distance;
        Vector2 spawnPosition;

        for (int i = 0; i < amount; i++) {
            int attempts = 4;
            do {
                degree = (float)(rng.NextDouble() * 360);
                distance = playerSafeZoneRadius + (float)(rng.NextDouble() * spawnZoneWidth);
                spawnPosition = new Vector2(transform.position.x, transform.position.y)
                + new Vector2(Mathf.Cos(degree)*distance, Mathf.Sin(degree)*distance);
                attempts -= 1;
                if (attempts == 0) break;
            } while (!checkPosValidity(spawnPosition));
            if (attempts == 0) continue;
            GameObject newEnemy = (GameObject)PrefabUtility.InstantiatePrefab(enemyType);
            Debug.Log("new enemy" + newEnemy);
            newEnemy.transform.position = spawnPosition;
            newEnemy.transform.rotation = Quaternion.identity;
        }

        
    }


    private bool checkPosValidity(Vector2 spawnPosition) {

        GameObject[] campfires = PrefabUtility.FindAllInstancesOfPrefab(campfirePrefab);
        GameObject[] crossbows = PrefabUtility.FindAllInstancesOfPrefab(crossbowPrefab);

        for (int i = 0; i < campfires.Length; i++) {
            if ((campfires[i].transform.position - transform.position).magnitude <= buildingSafeZoneRadius) {
                return false;
            }
        }
        for (int i = 0; i < crossbows.Length; i++) {
            if ((crossbows[i].transform.position - transform.position).magnitude <= buildingSafeZoneRadius) {
                return false;
            }
        }
        return true;
    } 
}
