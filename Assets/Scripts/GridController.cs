using UnityEngine;
using System.Collections.Generic;

public class GridController : MonoBehaviour
{
    [Header("Configuración de Grilla")]
    public int gridWidth = 5;
    public int gridHeight = 5;
    public float spacing = 1.2f;
    public Vector3 gridOrigin = new Vector3(-2.4f, 0f, -2.4f);

    [Header("Prefab")]
    public GameObject modulePrefab;

    [Header("Generación de Fallos")]
    public float initialFailInterval = 3f;
    public float minFailInterval = 0.8f;
    public float difficultyIncreaseRate = 0.95f;

    // Matriz de módulos
    private Module[,] modules;

    // Tiempos
    private float nextFailTime = 0f;
    private float difficultyTimer = 0f;
    private float currentFailInterval;

    // Estado del juego
    public bool isGameActive = true;

    // Evento para Game Over
    public System.Action OnGameOver;

    void Start()
    {
        Debug.Log("GridController INICIADO");
        CreateGrid();
        currentFailInterval = initialFailInterval;
        nextFailTime = Time.time + currentFailInterval;
        Debug.Log($" Primer fallo en {currentFailInterval} segundos");
    }

    void Update()
    {
        if (!isGameActive) return;

        // Generar fallos aleatorios
        if (Time.time >= nextFailTime)
        {
            SpawnRandomFailure();
            nextFailTime = Time.time + currentFailInterval;
        }

        // Aumentar dificultad con el tiempo
        difficultyTimer += Time.deltaTime;
        if (difficultyTimer >= 10f)
        {
            difficultyTimer = 0f;
            currentFailInterval = Mathf.Max(minFailInterval, currentFailInterval * difficultyIncreaseRate);
            Debug.Log($" DIFICULTAD AUMENTADA: Fallos cada {currentFailInterval:F2} segundos");
        }

        // Verificar Game Over
        CheckGameOver();
    }

    void CreateGrid()
    {
        Debug.Log($" Creando grilla de {gridWidth}x{gridHeight}");

        if (modulePrefab == null)
        {
            Debug.LogError("ERROR: modulePrefab no está asignado en GridController");
            return;
        }

        modules = new Module[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 position = gridOrigin + new Vector3(x * spacing, 0f, y * spacing);
                GameObject moduleObj = Instantiate(modulePrefab, position, Quaternion.identity);
                moduleObj.transform.parent = this.transform;

                Module module = moduleObj.GetComponent<Module>();
                if (module != null)
                {
                    module.gridX = x;
                    module.gridY = y;
                    modules[x, y] = module;
                }
                else
                {
                    Debug.LogError($"El prefab no tiene el script Module en posición ({x},{y})");
                }
            }
        }
        Debug.Log($"Grilla creada. Total módulos: {gridWidth * gridHeight}");
    }

    void SpawnRandomFailure()
    {
        Debug.Log(" Intentando generar fallo aleatorio...");

        if (!isGameActive)
        {
            Debug.Log("Juego no activo, no se generan fallos");
            return;
        }

        if (modules == null)
        {
            Debug.LogError(" modules es NULL, la grilla no se creó correctamente");
            return;
        }

        // Obtener todos los módulos normales
        List<Module> normalModules = new List<Module>();

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (modules[x, y] != null && modules[x, y].currentState == Module.ModuleState.Normal)
                {
                    normalModules.Add(modules[x, y]);
                }
            }
        }

        Debug.Log($"Módulos normales disponibles: {normalModules.Count}");

        if (normalModules.Count > 0)
        {
            int randomIndex = Random.Range(0, normalModules.Count);
            Module target = normalModules[randomIndex];
            Debug.Log($" Fallo generado en módulo ({target.gridX}, {target.gridY})");
            target.SetState(Module.ModuleState.Unstable);
        }
        else
        {
            Debug.Log("No hay módulos normales para fallar - GAME OVER inminente");
        }
    }

    public void PropagateFrom(int x, int y)
    {
        if (!isGameActive) return;

        Debug.Log($" Propagando desde ({x},{y})");

        // Vecinos: Arriba, Abajo, Izquierda, Derecha
        Vector2Int[] neighbors = new Vector2Int[]
        {
            new Vector2Int(x, y + 1), // Arriba
            new Vector2Int(x, y - 1), // Abajo
            new Vector2Int(x - 1, y), // Izquierda
            new Vector2Int(x + 1, y)  // Derecha
        };

        foreach (Vector2Int neighbor in neighbors)
        {
            if (IsInsideGrid(neighbor.x, neighbor.y))
            {
                Module neighborModule = modules[neighbor.x, neighbor.y];
                if (neighborModule != null && neighborModule.currentState == Module.ModuleState.Normal)
                {
                    Debug.Log($" Infectando vecino ({neighbor.x},{neighbor.y})");
                    neighborModule.SetState(Module.ModuleState.Unstable);
                }
            }
        }
    }

    bool IsInsideGrid(int x, int y)
    {
        return x >= 0 && x < gridWidth && y >= 0 && y < gridHeight;
    }

    void CheckGameOver()
    {
        if (modules == null) return;

        int failingOrUnstableCount = 0;
        int totalModules = gridWidth * gridHeight;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (modules[x, y] != null &&
                    (modules[x, y].currentState == Module.ModuleState.Failing ||
                     modules[x, y].currentState == Module.ModuleState.Unstable))
                {
                    failingOrUnstableCount++;
                }
            }
        }

        if (failingOrUnstableCount >= totalModules && totalModules > 0)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        isGameActive = false;
        Debug.Log(" GAME OVER - Todos los módulos han fallado ");
        OnGameOver?.Invoke();
    }

    public void StartNewGame()
    {
        Debug.Log("Iniciando nuevo juego");
        isGameActive = true;
        currentFailInterval = initialFailInterval;
        difficultyTimer = 0f;
        nextFailTime = Time.time + currentFailInterval;

        if (modules != null)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    if (modules[x, y] != null)
                    {
                        modules[x, y].SetState(Module.ModuleState.Normal);
                    }
                }
            }
        }
    }

    public void RestartGame()
    {
        StartNewGame();
    }

    public void StopGame()
    {
        isGameActive = false;
    }

    public float GetCurrentFailInterval()
    {
        return currentFailInterval;
    }

    public int GetAliveModulesCount()
    {
        if (modules == null) return 0;

        int alive = 0;
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (modules[x, y] != null &&
                    modules[x, y].currentState == Module.ModuleState.Normal)
                {
                    alive++;
                }
            }
        }
        return alive;
    }
}