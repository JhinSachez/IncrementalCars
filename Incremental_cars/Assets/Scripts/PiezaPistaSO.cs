using NUnit.Framework.Constraints;
using UnityEngine;

[CreateAssetMenu(fileName = "NuevaPieza", menuName = "Pieza", order = 1)]
public class PiezaPistaSO : ScriptableObject
{
    [Header("Información Básica")]
    public string nombrePieza;
    public Sprite icono;
    public GameObject prefabPieza;
    
    [Header("Tipo de Pieza")]
    public TipoPieza tipoPieza;
    
    [Header("Rotación")]
    public bool permiteRotacion = true;
    public int angulosPermitidos = 4; // 0, 90, 180, 270 grados
    
    [Header("Conexiones")]
    public DireccionConexion[] conexionesEntrada;
    public DireccionConexion[] conexionesSalida;
}

public enum TipoPieza
{
    Recta,
    Curva,
    Loop,
    Interseccion,
    Especial
}

public enum DireccionConexion
{
    Norte,
    Sur,
    Este,
    Oeste
}
