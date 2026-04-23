using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public float spawnAreaX = 7f;
    public float spawnZ = 20f;

    public float timeBetweenWaves = 8f;
    public int baseEnemiesPerWave = 4;
    public float timeBetweenSpawns = 0.4f;

    private int wave = 0;

    void Start()
    {
        StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        yield return new WaitForSeconds(2f);

        while (true)
        {
            wave++;
            int count = baseEnemiesPerWave + wave * 2;

            SpaceGameManager.Instance.ShowWaveText("Oleada " + wave);

            for (int i = 0; i < count; i++)
            {
                SpawnEnemy();
                yield return new WaitForSeconds(timeBetweenSpawns);
            }

            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    void SpawnEnemy()
    {
        float x = Random.Range(-spawnAreaX, spawnAreaX);
        Vector3 pos = new Vector3(x, 0, spawnZ);

        int i = Random.Range(0, enemyPrefabs.Length);
        Instantiate(enemyPrefabs[i], pos, Quaternion.identity);
    }
}