using UnityEngine;

public class TrackManager : MonoBehaviour
{
    public GameObject[] trackPrefabs;
    public Transform player;
    public float spawnZ = 0;
    public float trackLength = 20f;
    public int numberOfTracks = 5;

    void Start()
    {
        for (int i = 0; i < numberOfTracks; i++)
        {
            SpawnTrack();
        }
    }

    void Update()
    {
        if (player.position.z > spawnZ - (numberOfTracks * trackLength))
        {
            SpawnTrack();
        }
    }

    void SpawnTrack()
    {
        int index = Random.Range(0, trackPrefabs.Length);
        Instantiate(trackPrefabs[index], Vector3.forward * spawnZ, Quaternion.identity);
        spawnZ += trackLength;
    }
}