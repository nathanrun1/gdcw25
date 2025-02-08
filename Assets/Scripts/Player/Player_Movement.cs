using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Movement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rigidBody;
    [SerializeField] private GameObject _playerObj;
    [SerializeField] private float _walkSpeed;


    private PlayerInput _input;
    private Vector2 _movementVector;

    private void OnMoveInput(InputAction.CallbackContext context)
    {
        _movementVector = context.ReadValue<Vector2>();
    }

    private void FaceMouse()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        //Vector2 diff = Camera.main.ScreenToWorldPoint(mousePos) - new Vector2(transform.position.x, transform.position.y); // diff from player pos
        Vector2 diff = mousePos - new Vector2(Screen.width / 2, Screen.height / 2);
        float angle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        _playerObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
        //_rigidBody.MoveRotation(Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg);
    }

    private void Start()
    {
        Debug.Log(GameManager.Instance.playerInput.Player.enabled);
        GameManager.Instance.playerInput.Player.Move.performed += OnMoveInput;
        GameManager.Instance.playerInput.Player.Move.canceled += OnMoveInput;
    }



    // Update is called once per frame
    private void Update()
    {
        FaceMouse();
    }

    private void FixedUpdate()
    {
        _rigidBody.velocity = _movementVector * _walkSpeed * Time.fixedDeltaTime;
        //FaceMouse();
    }
}
