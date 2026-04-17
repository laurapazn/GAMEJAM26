// Define los 4 estados posibles de cada módulo.
// Se usa como referencia en todos los demás scripts.
public enum ModuleState
{
    Normal,     // Verde  - funcionando bien
    Unstable,   // Amarillo - comenzando a fallar
    Failing,    // Rojo - a punto de contagiar
    Dead        // Negro - ya no funciona
}