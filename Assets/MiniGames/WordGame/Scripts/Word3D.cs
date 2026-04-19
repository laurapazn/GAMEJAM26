using UnityEngine;
using TMPro;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class Word3D : MonoBehaviour
{
    public TextMeshPro textMesh;

    public string wordText;
    public string categoryName;
    public bool isCorrect;

    private bool clicked = false;

    public float speed = 2f;

    void Awake()
    {
        GetComponent<Rigidbody>().useGravity = false;
    }

    public void Setup(string word, string category, bool correct)
    {
        wordText = word;
        categoryName = category;
        isCorrect = correct;

        textMesh.text = word.ToUpper();
    }

    void Update()
    {
        if (clicked) return;

        transform.Translate(Vector3.down * speed * Time.deltaTime);

        if (transform.position.y < -6)
        {
            if (isCorrect)
                WordGameManager.Instance.OnWordMissed();

            Destroy(gameObject);
        }
    }

    public void OnClicked()
    {
        if (clicked) return;
        clicked = true;

        if (isCorrect)
            WordGameManager.Instance.OnCorrectWord(this);
        else
            WordGameManager.Instance.OnWrongWord(this);
    }
}