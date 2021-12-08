using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("General Settings")]
    public int playerPoints = 0;
    public int playerHealth = 1;
    public int maxPlayerHealt = 4;
    public bool isPlaying = true;
    public float tiltSensitivity = 2.0f;

    [Header("Object References")]
    public TextMeshProUGUI pointsText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI gameOverPointsText;
    public GameObject gameOverSceen;
    public Transform cartDummy;

    [Header("Audio References")]
    public AudioSource ambientAudioSource;
    public AudioClip collectAudioClip;
    public AudioClip levelUpAudioClip;
    public AudioClip crashAudioClip;
    public AudioClip healthPickupAudioClip;
    public AudioClip damageAudioClip;

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

        pointsText.text = isPlaying ? $"Points: {playerPoints}" : "";
        healthText.text = isPlaying ? $"Health: {playerHealth}" : "";

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

            switch(spawnableItemSettings.itemType)
            {
                case SpawnableItemType.Collectible:
                    CollectCoin(spawnableItemSettings.points); break;
                case SpawnableItemType.Obstacle:
                    DealDamage(spawnableItemSettings.points); break;
                case SpawnableItemType.HealthBoost:
                    BoostHealt(spawnableItemSettings.points); break;
                default:
                    CollectCoin(spawnableItemSettings.points); break;
            }
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
        else
            effectAudioSource.PlayOneShot(damageAudioClip, 4f);
    }

    public void BoostHealt(int healtBoost)
    {
        playerHealth += healtBoost;

        if (playerHealth > maxPlayerHealt)
            playerHealth = maxPlayerHealt;

        Debug.Log("PLAYING");
        effectAudioSource.PlayOneShot(healthPickupAudioClip, 4f);
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
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int sceneBestScore = PlayerPrefs.GetInt(PlayerPrefsKeys.BestLevelScore(currentSceneIndex));
        if (playerPoints > sceneBestScore)
        {
            PlayerPrefs.SetInt(PlayerPrefsKeys.BestLevelScore(currentSceneIndex), playerPoints);
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
