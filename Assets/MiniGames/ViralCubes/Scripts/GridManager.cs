using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int width = 5;
    public int height = 5;

    public float spacing = 2.8f; // 🔥 separación grande

    public GameObject modulePrefab;

    private Module[,] grid;

    private float failTimer = 0f;
    private float failInterval = 2f;

    void Start()
    {
        GenerateGrid();
    }

    void Update()
    {
        failTimer += Time.deltaTime;

        if (failTimer >= failInterval)
        {
            TriggerRandomFailure();
            failTimer = 0f;

            // dificultad progresiva
            failInterval *= 0.97f;
            failInterval = Mathf.Max(0.4f, failInterval);
        }
    }

    void GenerateGrid()
    {
        grid = new Module[width, height];

        float spacing = 2.5f;
        float offsetX = (width - 1) * spacing / 2f;
        float offsetZ = (height - 1) * spacing / 2f;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 pos = new Vector3(
                    x * spacing - offsetX,
                    1f,
                    z * spacing - offsetZ
                );

                GameObject obj = Instantiate(modulePrefab, pos, Quaternion.identity);

                // tamaño seguro
                obj.transform.localScale = Vector3.one * 1.5f;

                Module m = obj.GetComponent<Module>();

                if (m != null)
                {
                    m.gridPos = new Vector2Int(x, z);
                    m.grid = this;
                }

                grid[x, z] = m;
            }
        }
    }

    public void SpreadFailure(Vector2Int pos)
    {
        Vector2Int[] dirs = {
            new Vector2Int(1,0),
            new Vector2Int(-1,0),
            new Vector2Int(0,1),
            new Vector2Int(0,-1)
        };

        foreach (var d in dirs)
        {
            Vector2Int n = pos + d;

            if (n.x >= 0 && n.x < width && n.y >= 0 && n.y < height)
            {
                Module m = grid[n.x, n.y];

                if (m.state == ModuleState.Normal)
                    m.SetState(ModuleState.Unstable);
            }
        }
    }

    void TriggerRandomFailure()
    {
        int x = Random.Range(0, width);
        int z = Random.Range(0, height);

        Module m = grid[x, z];

        if (m.state == ModuleState.Normal)
            m.SetState(ModuleState.Unstable);
    }

    public void SpeedUpGame()
    {
        failInterval *= 0.9f;
        failInterval = Mathf.Max(0.2f, failInterval);
    }
}