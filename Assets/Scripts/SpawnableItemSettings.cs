using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnableItemSettings : MonoBehaviour
{
    public int points = 1;
    public bool isObstacle = false;

    private float rotationSpeed = 70.0f;
    private float floatAmplitude = 0.001f;

    private void Update()
    {
        if(!isObstacle)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

            float newY = Mathf.Sin(Time.fixedTime * Mathf.PI) * floatAmplitude + transform.position.y;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }
}
