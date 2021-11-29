using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public int playerPoints = 0;
    public int playerHealth = 1;
    public bool isPlaying = true;
    public float tiltSensitivity = 2.0f;

    public TextMeshProUGUI pointsText;
    public TextMeshProUGUI gameOverPointsText;
    public GameObject gameOverSceen;
    public Transform cartDummy;

    private CameraPointer cameraPointer;
    private Vector3 startPosition;
    private float cartRotationSpeed = 40.0f;

    public void Start()
    {
        cameraPointer = GetComponent<CameraPointer>();
        startPosition = transform.position;

        gameOverSceen.SetActive(false);
    }

    public void Update()
    {
        cameraPointer.loader.transform.parent.gameObject.SetActive(!isPlaying);
        cameraPointer.enabled = !isPlaying;
        gameOverSceen.SetActive(!isPlaying);

        float zTilt = transform.rotation.z * tiltSensitivity;
        transform.position = startPosition + Vector3.left * zTilt;
        cartDummy.rotation = Quaternion.Euler(0, 0, zTilt * cartRotationSpeed);

        pointsText.text = isPlaying ? "Points: " + playerPoints : "";
    }

    public void OnTriggerEnter(Collider other)
    {
        SpawnableItemSettings spawnableItemSettings = other.GetComponent<SpawnableItemSettings>();

        if(spawnableItemSettings)
        {
            Destroy(spawnableItemSettings.gameObject);

            if (spawnableItemSettings.isObstacle)
                DealDamage(spawnableItemSettings.points);
            else
                playerPoints += spawnableItemSettings.points;
        }
    }

    public void DealDamage(int damagePoints)
    {
        playerHealth -= damagePoints;

        if (playerHealth <= 0)
            GameOver();
    }

    private void GameOver()
    {
        isPlaying = false;
        gameOverSceen.SetActive(true);
        gameOverPointsText.text = $"You collected {playerPoints} points";

        // Setting best scores
        string currentSceneName = SceneManager.GetActiveScene().name;
        int sceneBestScore = PlayerPrefs.GetInt(PlayerPrefsKeys.BestLevelScore(currentSceneName));
        if (playerPoints > sceneBestScore)
        {
            PlayerPrefs.SetInt(PlayerPrefsKeys.BestLevelScore(currentSceneName), playerPoints);
            gameOverPointsText.text += "\nNew high score!";
        }

        int bestScore = PlayerPrefs.GetInt(PlayerPrefsKeys.BestScore);
        if (playerPoints > bestScore)
        {
            PlayerPrefs.SetInt(PlayerPrefsKeys.BestScore, playerPoints);
            gameOverPointsText.text += "\nNew overall high score!";
        }

        int currentScore = PlayerPrefs.GetInt(PlayerPrefsKeys.OverallPoints);
        PlayerPrefs.SetInt(PlayerPrefsKeys.OverallPoints, currentScore + playerPoints);

        // Resetting points
        playerPoints = 0;
    }
}
