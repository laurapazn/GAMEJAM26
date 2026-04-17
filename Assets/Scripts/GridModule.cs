using UnityEngine;

// Controla un módulo individual: su estado, timer,
// contagio a vecinos y respuesta al click del jugador.
public class GridModule : MonoBehaviour
{
    [Header("Config")]
    public float failTime = 2f;          // Segundos antes de contagiar
    public int gridX, gridY;             // Posición en la grilla

    [Header("Materiales")]
    public Material matNormal;
    public Material matUnstable;
    public Material matFailing;
    public Material matDead;

    private ModuleState _state = ModuleState.Normal;
    private float _timer = 0f;
    private Renderer _renderer;

    public ModuleState State => _state;
    public bool IsAlive => _state != ModuleState.Dead;

    void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }

    void Update()
    {
        if (_state == ModuleState.Normal || _state == ModuleState.Dead)
            return;

        _timer += Time.deltaTime;

        // Transición Unstable → Failing a mitad del timer
        if (_state == ModuleState.Unstable && _timer >= failTime * 0.5f)
            SetState(ModuleState.Failing);

        // Al terminar el timer: contagiar y morir
        if (_timer >= failTime)
        {
            SpreadToNeighbors();
            SetState(ModuleState.Dead);
        }
    }

    // Inicia el proceso de falla en este módulo
    public void TriggerFailure()
    {
        if (_state != ModuleState.Normal) return;
        SetState(ModuleState.Unstable);
        _timer = 0f;
    }

    // El jugador hace click: repara si está fallando
    public void Repair()
    {
        if (_state == ModuleState.Dead) return;
        SetState(ModuleState.Normal);
        _timer = 0f;
    }

    void SpreadToNeighbors()
    {
        GridManager.Instance.PropagateFrom(gridX, gridY);
    }

    void SetState(ModuleState newState)
    {
        _state = newState;
        UpdateVisual();
    }

    void UpdateVisual()
    {
        switch (_state)
        {
            case ModuleState.Normal: _renderer.material = matNormal; break;
            case ModuleState.Unstable: _renderer.material = matUnstable; break;
            case ModuleState.Failing: _renderer.material = matFailing; break;
            case ModuleState.Dead: _renderer.material = matDead; break;
        }
    }
}