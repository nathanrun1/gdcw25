using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ice_Drill : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 targetPositionDelta = PlayerManager.Instance.PlayerWorldPosition - new Vector2(transform.position.x, transform.position.y);
        float angle = Mathf.Atan2(targetPositionDelta.y, targetPositionDelta.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private void FixedUpdate()
    {
        Vector2 targetPositionDelta = PlayerManager.Instance.PlayerWorldPosition - new Vector2(transform.position.x, transform.position.y);
        GetComponent<Rigidbody2D>().velocity = targetPositionDelta.normalized * _moveSpeed * Time.fixedDeltaTime;
    }
}
