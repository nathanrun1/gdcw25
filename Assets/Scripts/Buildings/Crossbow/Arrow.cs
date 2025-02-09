using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Arrow : MonoBehaviour
{

    private GameObject targetedEnemy;
    [SerializeField] private int speed;
    private int damage;

    private Vector2 targetPositionDelta;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (targetedEnemy.IsDestroyed()) {
            return;
        }

        targetPositionDelta = new Vector2(targetedEnemy.transform.position.x, targetedEnemy.transform.position.y)
            - new Vector2(transform.position.x, transform.position.y);
        float angle = Mathf.Atan2(targetPositionDelta.y, targetPositionDelta.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle-90));
    }


    void FixedUpdate() {
        double playerDistance = (PlayerManager.Instance.PlayerWorldPosition - new Vector2(transform.position.x, transform.position.y)).magnitude;
        
        if (!targetedEnemy.IsDestroyed()) {
            targetPositionDelta = new Vector2(targetedEnemy.transform.position.x, targetedEnemy.transform.position.y)
            - new Vector2(transform.position.x, transform.position.y);
        }

        if (targetedEnemy.IsDestroyed() && playerDistance > Camera.main.pixelWidth) {
            Destroy(gameObject);
        }

        GetComponent<Rigidbody2D>().velocity = targetPositionDelta.normalized * speed * Time.fixedDeltaTime;

    }

    public void init(GameObject targetedEnemy, int damage) {
        this.targetedEnemy = targetedEnemy;
        this.damage = damage;
    }


    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject == targetedEnemy.gameObject) {
            collider.gameObject.GetComponent<Ice_Drill>().takeDamage(damage);
            Destroy(gameObject);
        }


    }
}
