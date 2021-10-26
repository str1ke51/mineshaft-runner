using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int playerPoints = 0;
    public int playerHealth = 1;
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
        SpawnableItemSettings spawnableItemSettings = other.GetComponent<SpawnableItemSettings>();

        if(spawnableItemSettings)
        {
            if (spawnableItemSettings.isObstacle)
                DealDamage(spawnableItemSettings.points);
            else
                playerPoints += spawnableItemSettings.points;
                
            Destroy(spawnableItemSettings.gameObject);
        }
    }

    public void DealDamage(int damagePoints)
    {
        playerHealth -= damagePoints;

        if (playerHealth <= 0)
        {
            Debug.LogError("GAME OVER SCREEN");
            Application.Quit();
        }
    }
}
