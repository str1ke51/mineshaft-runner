using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackSectionManager : MonoBehaviour
{
    public float movementSpeed = 5.0f;
    public float neededSpawnedArea = 25.0f;
    public TrackSection startTrackSectionPrefab = null;
    public List<TrackSection> trackSectionPrefabs = new List<TrackSection>();

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
    }
}
