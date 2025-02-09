using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Crossbow : MonoBehaviour
{

    private GameObject crossbowBase;
    private GameObject crossbowBarrel;
    [SerializeField] private GameObject arrowPrefab; 
    [SerializeField] private double range;
    [SerializeField] private GameObject iceDrillPrefab;
    [SerializeField] private int damage;
    [SerializeField] private float coolDown;
    private float coolDownTimer;


    private GameObject closestEnemy;


    // Start is called before the first frame update
    void Start()
    {
        crossbowBase = transform.Find("Crossbow_Base").gameObject;
        crossbowBarrel = transform.Find("Crossbow_Barrel").gameObject;
        coolDownTimer = coolDown;
    }

    // Update is called once per frame
    void Update()
    {
        GameObject closestEnemy = getClosestEnemyInRange();
        if (closestEnemy != null) {
            Vector2 targetPositionDelta = new Vector2(closestEnemy.transform.position.x, closestEnemy.transform.position.y)
             - new Vector2(transform.position.x, transform.position.y);
            float angle = Mathf.Atan2(targetPositionDelta.y, targetPositionDelta.x) * Mathf.Rad2Deg;
            crossbowBarrel.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle-90));

            if (coolDownTimer <= 0) {
                shoot(closestEnemy);
                coolDownTimer = coolDown;
            } else {
                coolDownTimer -= Time.deltaTime;
            }
        }
    }

    private GameObject getClosestEnemyInRange() {
        GameObject[] enemies = PrefabUtility.FindAllInstancesOfPrefab(iceDrillPrefab);
        double minDistance = range + 1;
        GameObject closestEnemy =  null;
        for (int i = 0; i < enemies.Length; i++) {
            double distance = (transform.position - enemies[i].transform.position).magnitude;
            if (distance < minDistance) {
                closestEnemy = enemies[i];
                minDistance = distance;
            }
        }
        return closestEnemy;
    }


    private void shoot(GameObject targetedEnemy) {
        GameObject newArrow = PrefabUtility.InstantiatePrefab(arrowPrefab).GameObject();
        newArrow.transform.position = transform.position;
        newArrow.transform.rotation = transform.rotation;
        newArrow.GetComponent<Arrow>().init(targetedEnemy, damage);
    }
}
