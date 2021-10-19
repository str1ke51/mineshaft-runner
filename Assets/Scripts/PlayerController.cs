using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public int playerPoints = 0;
    public bool isPlaying = true;
    public float tiltSensitivity = 2.0f;

    public TextMeshProUGUI text;

    private CameraPointer cameraPointer;
    private Vector3 startPosition;

    public void Start()
    {
        cameraPointer = GetComponent<CameraPointer>();
        startPosition = transform.position;
    }

    public void Update()
    {
        if (cameraPointer.enabled == isPlaying)
            cameraPointer.enabled = !isPlaying;

        float zTilt = transform.rotation.z * -1;
        transform.position = startPosition + Vector3.right * zTilt * tiltSensitivity;

        text.text = "Points: " + playerPoints;
    }

    public void OnTriggerEnter(Collider other)
    {
        CollectibleItemSettings collectibleItemSettings = other.GetComponent<CollectibleItemSettings>();

        if(collectibleItemSettings)
        {
            playerPoints += collectibleItemSettings.points;
            Destroy(collectibleItemSettings.gameObject);
        }
    }
}
