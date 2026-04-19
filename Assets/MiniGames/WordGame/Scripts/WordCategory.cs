using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WordCategory", menuName = "WordGame/Category")]
public class WordCategory : ScriptableObject
{
    public string categoryName;
    public Color categoryColor = Color.white;
    public List<string> words = new List<string>();
}