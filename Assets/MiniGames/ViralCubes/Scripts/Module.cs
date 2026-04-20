using UnityEngine;

public enum ModuleState
{
    Normal,
    Unstable,
    Failing,
    Dead
}

public class Module : MonoBehaviour
{
    public ModuleState state = ModuleState.Normal;

    public Material greenMat;
    public Material yellowMat;
    public Material redMat;
    public Material blackMat;

    private Renderer rend;
    private float timer = 0f;
    private float maxTime = 2f;

    public Vector2Int gridPos;
    public GridManager grid;

    private ChainGameManager gameManager;

    void Start()
    {
        rend = GetComponent<Renderer>();
        gameManager = FindObjectOfType<ChainGameManager>();
        UpdateColor();
    }

    void Update()
    {
        if (state == ModuleState.Unstable || state == ModuleState.Failing)
        {
            timer += Time.deltaTime;

            if (timer >= maxTime)
            {
                if (state == ModuleState.Unstable)
                {
                    SetState(ModuleState.Failing);
                }
                else if (state == ModuleState.Failing)
                {
                    SetState(ModuleState.Dead);

                    gameManager.LoseLife(); // 💀 pierde vida
                    grid.SpreadFailure(gridPos);
                }

                timer = 0f;
            }
        }
    }

    public void SetState(ModuleState newState)
    {
        state = newState;
        UpdateColor();
    }

    public void Repair()
    {
        if (state == ModuleState.Unstable || state == ModuleState.Failing)
        {
            state = ModuleState.Normal;
            timer = 0f;
            UpdateColor();

            gameManager.AddScore(10); // ⭐ puntos
            grid.SpeedUpGame(); // 🔥 acelera juego
        }
    }

    void UpdateColor()
    {
        switch (state)
        {
            case ModuleState.Normal: rend.material = greenMat; break;
            case ModuleState.Unstable: rend.material = yellowMat; break;
            case ModuleState.Failing: rend.material = redMat; break;
            case ModuleState.Dead: rend.material = blackMat; break;
        }
    }
}