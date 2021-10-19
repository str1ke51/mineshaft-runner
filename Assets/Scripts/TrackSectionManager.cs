using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackSectionManager : MonoBehaviour
{
    [Header("General Settings")]
    public float movementSpeed = 5.0f;
    public float neededSpawnedArea = 25.0f;

    [Header("Track Sections")]
    public TrackSection startTrackSectionPrefab = null;
    public List<TrackSection> trackSectionPrefabs = new List<TrackSection>();

    [Header("Collectible Items")]
    public float spawnRatePercent = 0.50f;
    public List<CollectibleItemSettings> collectibleItemPrefabs = new List<CollectibleItemSettings>();

    [Header("DEBUG")]
    public List<TrackSection> spawnedSections = new List<TrackSection>();
    private Vector3 targetSpawn = new Vector3();

    // Start is called before the first frame update
    void Start()
    {
        targetSpawn = transform.position + Vector3.forward * startTrackSectionPrefab.totalLength / 2;

        TrackSection start = Instantiate(startTrackSectionPrefab, targetSpawn, Quaternion.identity);
        spawnedSections.Add(start);
    }

    // Update is called once per frame
    void Update()
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
                int counterSpawnRate = Mathf.RoundToInt(collectibleItemPrefabs.Count / spawnRatePercent) - collectibleItemPrefabs.Count;
                int spawn = Random.Range(-1 * counterSpawnRate, collectibleItemPrefabs.Count);

                if (spawn >= 0)
                {
                    int spawnLength = Random.Range(1, Mathf.Min(4, Mathf.RoundToInt(section.totalLength - i)));
                    int spawnVectorIndex = Random.Range(0, section.spawnPoints.Count);
                    Vector2 spawnVector = section.spawnPoints[spawnVectorIndex];

                    for (int j = 0; j < spawnLength; j++)
                    {
                        targetSpawn = section.transform.position +
                                      Vector3.forward * ((section.totalLength / -2) + i + j) +
                                      Vector3.up * spawnVector.y +
                                      Vector3.right * spawnVector.x;

                        Instantiate(collectibleItemPrefabs[spawn], targetSpawn, Quaternion.identity, section.transform);
                    }

                    i += spawnLength;
                }
            }
        }
    }
}
