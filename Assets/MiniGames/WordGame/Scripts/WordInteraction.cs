using UnityEngine;

public class WordInteraction : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Word3D w = hit.collider.GetComponent<Word3D>();
                if (w != null)
                    w.OnClicked();
            }
        }
    }
}