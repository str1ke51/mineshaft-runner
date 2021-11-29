using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("General Settings")]
    public int playerPoints = 0;
    public int playerHealth = 1;
    public bool isPlaying = true;
    public float tiltSensitivity = 2.0f;

    [Header("Object References")]
    public TextMeshProUGUI pointsText;
    public TextMeshProUGUI gameOverPointsText;
    public GameObject gameOverSceen;
    public Transform cartDummy;

    [Header("Audio References")]
    public AudioSource ambientAudioSource;
    public AudioClip collectAudioClip;
    public AudioClip levelUpAudioClip;
    public AudioClip crashAudioClip;

    private CameraPointer cameraPointer;
    private AudioSource effectAudioSource;
    private Vector3 startPosition;
    private float cartRotationSpeed = 40.0f;

    public void Start()
    {
        cameraPointer = GetComponent<CameraPointer>();
        effectAudioSource = GetComponent<AudioSource>();
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

        if (isPlaying ^ ambientAudioSource.isPlaying)
            if (ambientAudioSource.isPlaying)
                ambientAudioSource.Stop();
            else
                ambientAudioSource.Play();
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
                CollectCoin(spawnableItemSettings.points);
        }
    }

    public void CollectCoin(int points)
    {
        playerPoints += points;

        effectAudioSource.PlayOneShot(collectAudioClip);
    }

    public void DealDamage(int damagePoints)
    {
        playerHealth -= damagePoints;

        if (playerHealth <= 0)
            GameOver();
    }

    public void LevelUp(int bonusPoints)
    {
        playerPoints += bonusPoints;
        effectAudioSource.PlayOneShot(levelUpAudioClip);
    }

    private void GameOver()
    {
        isPlaying = false;
        effectAudioSource.PlayOneShot(crashAudioClip);
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
