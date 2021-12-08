using UnityEngine;

public class SpawnableItemSettings : MonoBehaviour
{
    public int points = 1;
    public SpawnableItemType itemType = SpawnableItemType.Collectible;

    private float rotationSpeed = 70.0f;

    private void Update()
    {
        if(!itemType.Equals(SpawnableItemType.Obstacle))
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    // Wiêksza iloœæ = rzadsze prawdopodobieñstwo zespawnowania
    public int GetProbability()
    {
        return itemType == SpawnableItemType.HealthBoost ? (points * 5) : points;
    }
}

public enum SpawnableItemType
{
    Collectible,
    Obstacle,
    HealthBoost
}