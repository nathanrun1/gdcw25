using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Ice_Drill : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float maxHealth;

    [SerializeField] private Camera camera;
    [SerializeField] private GameObject crossbowPrefab;
    [SerializeField] private GameObject campfirePrefab;
    [SerializeField] private GameObject emblemPrefab;
    private float hp;
    private GameObject closestTarget;

    

    // Start is called before the first frame update
    void Start()
    {
        hp = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        closestTarget = getClosestTarget();
        Vector2 targetPositionDelta = new Vector2(closestTarget.transform.position.x, closestTarget.transform.position.y) - new Vector2(transform.position.x, transform.position.y);
        float angle = Mathf.Atan2(targetPositionDelta.y, targetPositionDelta.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private void FixedUpdate()
    {
        closestTarget = getClosestTarget();
        Vector2 targetPositionDelta = new Vector2(closestTarget.transform.position.x, closestTarget.transform.position.y) - new Vector2(transform.position.x, transform.position.y);
        GetComponent<Rigidbody2D>().velocity = targetPositionDelta.normalized * _moveSpeed * Time.fixedDeltaTime;
    }


    public void takeDamage(int damage) {
        hp -= damage;

        Debug.Log(hp);
        // TODO: flash red
        gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        StartCoroutine(whitecolor());


        if (hp <= 0) {
            terminate();
        }
    }

    private IEnumerator whitecolor() {
        yield return new WaitForSeconds((float)0.08);
        GetComponent<SpriteRenderer> ().color = Color.white;
    }

    private GameObject getClosestTarget() {
        GameObject[] crossbows = PrefabUtility.FindAllInstancesOfPrefab(crossbowPrefab);
        GameObject[] campfires = PrefabUtility.FindAllInstancesOfPrefab(campfirePrefab);
        // GameObject emblem = PrefabUtility.FindAllInstancesOfPrefab(emblemPrefab)[0];
        double minDistance = double.MaxValue;
        GameObject closestTarget =  null;
        for (int i = 0; i < crossbows.Length; i++) {
            double distance = (transform.position - crossbows[i].transform.position).magnitude;
            if (distance < minDistance) {
                closestTarget = crossbows[i];
                minDistance = distance;
            }
        }
        for (int i = 0; i < campfires.Length; i++) {
            double distance = (transform.position - campfires[i].transform.position).magnitude;
            if (distance < minDistance) {
                closestTarget = campfires[i];
                minDistance = distance;
            }
        }
        // double emblemDistance = (transform.position - emblem.transform.position).magnitude;
        // if (emblemDistance < minDistance) {
        //     closestTarget = emblem;
        //     minDistance = emblemDistance;
        // }

        Vector3 playerPosition = new Vector3(PlayerManager.Instance.PlayerWorldPosition.x,  PlayerManager.Instance.PlayerWorldPosition.y, 0);
        double playerDistance = (transform.position - playerPosition).magnitude;
        if (playerDistance < minDistance) {
            closestTarget = PlayerManager.Instance.gameObject;
            minDistance = playerDistance;
        }
        
        return closestTarget;
    }


    private void terminate() {
        Destroy(gameObject);
    }
}
