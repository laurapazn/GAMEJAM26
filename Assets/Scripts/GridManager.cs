using UnityEngine;

// Genera la grilla 5x5, maneja la propagación de fallos
// y detecta el Game Over cuando todos los módulos están muertos.
public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    [Header("Grilla")]
    public int width = 5;
    public int height = 5;
    public float spacing = 1.5f;
    public GameObject modulePrefab;

    private GridModule[,] _grid;

    void Awake()
    {
        Instance = this;
        BuildGrid();
    }

    void BuildGrid()
    {
        _grid = new GridModule[width, height];
        Vector3 origin = new Vector3(
            -(width - 1) * spacing * 0.5f,
            0f,
            -(height - 1) * spacing * 0.5f
        );

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                Vector3 pos = origin + new Vector3(x * spacing, 0f, y * spacing);
                GameObject go = Instantiate(modulePrefab, pos, Quaternion.identity, transform);
                go.name = $"Module_{x}_{y}";

                GridModule mod = go.GetComponent<GridModule>();
                mod.gridX = x;
                mod.gridY = y;
                _grid[x, y] = mod;
            }
    }

    // Llamado por un módulo cuando muere: contagia sus 4 vecinos
    public void PropagateFrom(int x, int y)
    {
        TryInfect(x + 1, y);
        TryInfect(x - 1, y);
        TryInfect(x, y + 1);
        TryInfect(x, y - 1);
    }

    void TryInfect(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height) return;
        _grid[x, y].TriggerFailure();
    }

    // Devuelve un módulo Normal aleatorio para iniciar fallas
    public GridModule GetRandomNormalModule()
    {
        var normals = new System.Collections.Generic.List<GridModule>();
        foreach (var m in _grid)
            if (m.State == ModuleState.Normal) normals.Add(m);

        if (normals.Count == 0) return null;
        return normals[Random.Range(0, normals.Count)];
    }

    // Detecta si todos están muertos → Game Over
    public bool AllDead()
    {
        foreach (var m in _grid)
            if (m.IsAlive) return false;
        return true;
    }

    // Click del jugador: lanzar raycast y reparar módulo tocado
    void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GridModule mod = hit.collider.GetComponent<GridModule>();
            if (mod != null) mod.Repair();
        }
    }
}