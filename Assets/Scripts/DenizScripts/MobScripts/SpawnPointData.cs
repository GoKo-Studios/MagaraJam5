using UnityEngine;

public struct SpawnPointData
{
    public SpawnPointData(Transform Location, int SpawnAmount) {
        this.Location = Location;
        this.SpawnAmount = SpawnAmount;
    }

    public Transform Location;
    public int SpawnAmount;
}