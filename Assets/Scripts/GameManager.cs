using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    [Header("General Settings")]
    public PlayerController player;
    public float startMovementSpeed = 5.0f;
    public float neededSpawnedArea = 25.0f;
    public float speedLevelDuration = 60.0f;
    public float speedLevelIncrease = 0.5f;

    [Header("Track Sections")]
    public TrackSection startTrackSectionPrefab = null;
    public List<TrackSection> trackSectionPrefabs = new List<TrackSection>();

    [Header("Spawnable Items")]
    public float spawnRatePercent = 0.25f;
    public List<SpawnableItemSettings> spawnableItemPrefabs = new List<SpawnableItemSettings>();

    [Header("DEBUG")]
    [SerializeField]
    public List<TrackSection> spawnedSections = new List<TrackSection>();
    [SerializeField]
    private Vector3 targetSpawn = new Vector3();
    [SerializeField]
    private float spawnableItemsPointsRange = 0;
    [SerializeField]
    public float speedLevelTimeLeft = 0;
    [SerializeField]
    private float movementSpeed = 0;

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(gameObject);
        else
            _instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (var item in spawnableItemPrefabs)
            spawnableItemsPointsRange += (1.0f / item.GetProbability());

        targetSpawn = transform.position + Vector3.forward * startTrackSectionPrefab.totalLength / 2;

        TrackSection start = Instantiate(startTrackSectionPrefab, targetSpawn, Quaternion.identity);
        spawnedSections.Add(start);

        movementSpeed = startMovementSpeed;
        speedLevelTimeLeft = speedLevelDuration;
    }

    // Update is called once per frame
    void Update()
    {
        if (player.isPlaying)
        {
            if (spawnedSections.Count == 0 ||
                spawnedSections[spawnedSections.Count - 1].transform.position.z + spawnedSections[spawnedSections.Count - 1].totalLength / 2 < neededSpawnedArea)
            {
                SpawnNewSection();
            }

            if (spawnedSections.Count > 0)
            {
                foreach (var section in spawnedSections)
                {
                    if (section.transform.position.z + section.totalLength / 2 < neededSpawnedArea * -1)
                    {
                        section.toDelete = true;
                        Destroy(section.gameObject);
                    }
                    else
                    {
                        section.transform.position = section.transform.position - Vector3.forward * movementSpeed * Time.deltaTime;
                    }
                }

                spawnedSections.RemoveAll(ss => ss.toDelete);
            }

            speedLevelTimeLeft -= Time.deltaTime;
            if (speedLevelTimeLeft <= 0)
            {
                player.LevelUp(Mathf.FloorToInt(movementSpeed));
                speedLevelTimeLeft = speedLevelDuration;
                movementSpeed += speedLevelIncrease;
            }
        }
    }

    void SpawnNewSection()
    {
        // Spawn section model
        int index = Random.Range(0, trackSectionPrefabs.Count);
        TrackSection lastSection = spawnedSections[spawnedSections.Count - 1];

        if (lastSection)
        {
            targetSpawn = lastSection.transform.position + 
                          Vector3.forward * (lastSection.totalLength + trackSectionPrefabs[index].totalLength) / 2;
        }
        else
        {
            targetSpawn = transform.position + 
                          Vector3.forward * trackSectionPrefabs[index].totalLength / 2;
        }

        TrackSection section = Instantiate(trackSectionPrefabs[index], targetSpawn, Quaternion.identity);
        spawnedSections.Add(section);

        // Spawn collectibles along
        if (section.spawnPoints.Count > 0)
        {
            for (int i = 0; i < section.totalLength; i++)
            {
                float counterSpawnRate = spawnableItemsPointsRange / spawnRatePercent - spawnableItemsPointsRange;
                float spawn = Random.Range(-1 * counterSpawnRate, spawnableItemsPointsRange);

                if (spawn >= 0)
                {
                    int spawnToIndex = spawnableItemPrefabs.Count - 1;
                    for (int j = 0; j < spawnableItemPrefabs.Count; j++)
                    {
                        float itemRange = 1.0f / spawnableItemPrefabs[j].GetProbability();

                        if (spawn < itemRange)
                        {
                            spawnToIndex = j;
                            break;
                        }
                        else
                        {
                            spawn -= itemRange;
                        }
                    }

                    int spawnLength = spawnableItemPrefabs[spawnToIndex].itemType == SpawnableItemType.HealthBoost 
                                      ? 1 
                                      : Random.Range(1, Mathf.Min(4, Mathf.RoundToInt(section.totalLength - i)));

                    int spawnVectorIndex = Random.Range(0, section.spawnPoints.Count);
                    Vector2 spawnVector = section.spawnPoints[spawnVectorIndex];

                    for (int j = 0; j < spawnLength; j++)
                    {
                        targetSpawn = section.transform.position +
                                      Vector3.forward * ((section.totalLength / -2) + i + j) +
                                      Vector3.up * spawnVector.y +
                                      Vector3.right * spawnVector.x;

                        Instantiate(spawnableItemPrefabs[spawnToIndex], targetSpawn, Quaternion.identity, section.transform);
                    }

                    i += spawnLength + 1;
                }
            }
        }
    }

    public void StartNewGame()
    {
        foreach (var section in spawnedSections)
            Destroy(section.gameObject);
        spawnedSections.Clear();

        targetSpawn = transform.position + Vector3.forward * startTrackSectionPrefab.totalLength / 2;

        TrackSection start = Instantiate(startTrackSectionPrefab, targetSpawn, Quaternion.identity);
        spawnedSections.Add(start);
        
        movementSpeed = startMovementSpeed;
        speedLevelTimeLeft = speedLevelDuration;
        player.isPlaying = true;
    }

    public void QuitToMainMenu(AudioClip playingClip = null)
    {
        if (playingClip)
            StartCoroutine(DelayedLoad(playingClip.length, 0));
        else
            SceneManager.LoadScene(0);
    }

    IEnumerator DelayedLoad(float time, int sceneIndex)
    {
        yield return new WaitForSeconds(time);
        SceneManager.LoadScene(sceneIndex);
    }
}
