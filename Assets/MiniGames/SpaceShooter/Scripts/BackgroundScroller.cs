using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    public Transform[] tiles;
    public float speed = 5f;
    public float resetZ = 40f;

    void Update()
    {
        foreach (Transform t in tiles)
        {
            t.Translate(Vector3.back * speed * Time.deltaTime);

            if (t.position.z < -20)
                t.position += new Vector3(0, 0, resetZ);
        }
    }
}