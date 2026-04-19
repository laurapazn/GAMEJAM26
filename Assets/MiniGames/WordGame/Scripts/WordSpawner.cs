using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordSpawner : MonoBehaviour
{
    public GameObject wordPrefab;

    public float spawnX = 6f;
    public float spawnY = 6f;

    public float interval = 2f;

    private WordCategory target;
    private List<WordCategory> all;

    public void Init(WordCategory t, List<WordCategory> a)
    {
        target = t;
        all = a;
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            Spawn();
            yield return new WaitForSeconds(interval);
        }
    }

    void Spawn()
    {
        bool correct = Random.value > 0.5f;

        string word;
        string cat;

        if (correct)
        {
            word = target.words[Random.Range(0, target.words.Count)];
            cat = target.categoryName;
        }
        else
        {
            WordCategory random = all[Random.Range(0, all.Count)];
            word = random.words[Random.Range(0, random.words.Count)];
            cat = random.categoryName;
            correct = false;
        }

        Vector3 pos = new Vector3(Random.Range(-spawnX, spawnX), spawnY, 0);

        GameObject obj = Instantiate(wordPrefab, pos, Quaternion.identity);
        obj.GetComponent<Word3D>().Setup(word, cat, correct);
    }
}