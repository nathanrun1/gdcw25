using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Camera : MonoBehaviour
{
    [SerializeField] private Camera _followCamera;

    private void CameraToPlayerPosition(Camera camera)
    {
        _followCamera.gameObject.transform.position = new Vector3(transform.position.x, transform.position.y, _followCamera.gameObject.transform.position.z);
    }

    private void Update()
    {
        CameraToPlayerPosition(_followCamera);
    }
}
