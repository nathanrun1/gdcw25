using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowGenerator : MonoBehaviour
{
    [SerializeField] Camera camera;
    [SerializeField] ParticleSystem snowPS;
    private int snowEmission = 10;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(camera.transform.position.x, camera.transform.position.y + 4, -1);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(camera.transform.position.x, camera.transform.position.y + 4, -1);
    }

    public void updateEmission(int emission) {
        snowEmission = emission;
        var copy = snowPS.emission;
        copy.rateOverTime = emission;
    }
}
