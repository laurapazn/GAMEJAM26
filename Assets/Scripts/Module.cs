using UnityEngine;

public class Module : MonoBehaviour
{
    public enum ModuleState
    {
        Normal,     // Verde
        Unstable,   // Amarillo (va a fallar)
        Failing,    // Rojo (fallando)
        Dead        // Negro (muerto)
    }

    [Header("Configuración")]
    public float unstableDuration = 1.5f;
    public float failingPropagationTime = 2f;

    [Header("Materiales (arrastra desde carpeta Materials)")]
    public Material normalMat;
    public Material unstableMat;
    public Material failingMat;
    public Material deadMat;

    [Header("Estado actual")]
    public ModuleState currentState = ModuleState.Normal;

    private Renderer moduleRenderer;
    private GridController gridController;
    private float unstableTimer = 0f;
    private float failingTimer = 0f;

    public int gridX { get; set; }
    public int gridY { get; set; }

    void Start()
    {
        moduleRenderer = GetComponent<Renderer>();
        gridController = FindObjectOfType<GridController>();

        // DEBUG: Verificar que el módulo se inició correctamente
        Debug.Log($"✅ Módulo {gameObject.name} iniciado en posición ({gridX}, {gridY})");

        if (gridController == null)
        {
            Debug.LogError("❌ ERROR: No se encontró GridController en la escena");
        }

        if (normalMat == null)
        {
            Debug.LogError($"❌ ERROR: Módulo ({gridX},{gridY}) - Material NORMAL no asignado");
        }

        SetState(ModuleState.Normal);
    }

    void Update()
    {
        if (gridController == null || !gridController.isGameActive) return;

        switch (currentState)
        {
            case ModuleState.Unstable:
                unstableTimer -= Time.deltaTime;
                if (unstableTimer <= 0f)
                {
                    Debug.Log($"⚠️ Módulo ({gridX},{gridY}) - De UNSTABLE a FAILING");
                    SetState(ModuleState.Failing);
                }
                break;

            case ModuleState.Failing:
                failingTimer -= Time.deltaTime;
                if (failingTimer <= 0f)
                {
                    Debug.Log($"💀 Módulo ({gridX},{gridY}) - PROPAGANDO a vecinos");
                    if (gridController != null)
                        gridController.PropagateFrom(gridX, gridY);
                    failingTimer = failingPropagationTime;
                }
                break;
        }
    }

    public void SetState(ModuleState newState)
    {
        ModuleState oldState = currentState;
        currentState = newState;

        // DEBUG: Registrar cambio de estado
        Debug.Log($"🔄 Módulo ({gridX},{gridY}) cambió de {oldState} a {newState}");

        switch (newState)
        {
            case ModuleState.Normal:
                if (normalMat != null)
                    moduleRenderer.material = normalMat;
                else
                    Debug.LogError($"❌ Módulo ({gridX},{gridY}) - normalMat es NULL");
                break;
            case ModuleState.Unstable:
                if (unstableMat != null)
                    moduleRenderer.material = unstableMat;
                else
                    Debug.LogError($" Módulo ({gridX},{gridY}) - unstableMat es NULL");
                unstableTimer = unstableDuration;
                break;
            case ModuleState.Failing:
                if (failingMat != null)
                    moduleRenderer.material = failingMat;
                else
                    Debug.LogError($"Módulo ({gridX},{gridY}) - failingMat es NULL");
                failingTimer = failingPropagationTime;
                break;
            case ModuleState.Dead:
                if (deadMat != null)
                    moduleRenderer.material = deadMat;
                else
                    Debug.LogError($" Módulo ({gridX},{gridY}) - deadMat es NULL");
                break;
        }
    }

    public void Repair()
    {
        if (gridController == null)
        {
            Debug.LogError("Repair: gridController es NULL");
            return;
        }

        if (!gridController.isGameActive)
        {
            Debug.Log("Juego no activo, no se puede reparar");
            return;
        }

        if (currentState == ModuleState.Unstable || currentState == ModuleState.Failing)
        {
            Debug.Log($"Módulo ({gridX},{gridY}) REPARADO por el jugador");
            SetState(ModuleState.Normal);
        }
        else
        {
            Debug.Log($" Módulo ({gridX},{gridY}) está en estado {currentState}, no se puede reparar");
        }
    }

    void OnMouseDown()
    {
        Debug.Log($" CLICK en módulo ({gridX},{gridY}) - Estado actual: {currentState}");
        Repair();
    }
}