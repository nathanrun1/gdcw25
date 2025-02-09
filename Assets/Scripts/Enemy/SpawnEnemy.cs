using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using UnityEngine.SceneManagement;



public class script : MonoBehaviour
{
    [SerializeField] private int playerSafeZoneRadius;
    [SerializeField] private int spawnZoneWidth;
    [SerializeField] private int buildingSafeZoneRadius;
    [SerializeField] private GameObject enemyType;
    [SerializeField] private GameObject campfirePrefab;
    [SerializeField] private GameObject crossbowPrefab;
    

    // Start is called before the first frame update
    void Start()
    {
        transform.position = PlayerManager.Instance.PlayerWorldPosition;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = PlayerManager.Instance.PlayerWorldPosition;
        if (Input.GetKeyDown(KeyCode.E)) {
            Spawn();
        }
    }


    private void Spawn() {
        System.Random rng = new System.Random();

        float degree, distance;
        Vector2 spawnPosition;

        do {
            degree = (float)(rng.NextDouble() * 360);
            distance = playerSafeZoneRadius + (float)(rng.NextDouble() * spawnZoneWidth);
            spawnPosition = new Vector2(transform.position.x, transform.position.y)
             + new Vector2(Mathf.Cos(degree)*distance, Mathf.Sin(degree)*distance);
        } while (!checkPosValidity(spawnPosition));

        Instantiate(enemyType, spawnPosition, Quaternion.identity);
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
