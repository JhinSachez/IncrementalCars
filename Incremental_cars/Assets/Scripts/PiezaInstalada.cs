using UnityEngine;

public class PiezaInstalada : MonoBehaviour
{
    [Header("Referencias")]
    private PiezaPistaSO datosPieza;
    private int rotacionActual = 0;
    private int posicionX, posicionY;
    
    [Header("Visuales")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Transform puntoCentral;
    
    public void Inicializar(PiezaPistaSO pieza, int x, int y)
    {
        datosPieza = pieza;
        posicionX = x;
        posicionY = y;
        
        if (spriteRenderer != null && pieza.icono != null)
        {
            spriteRenderer.sprite = pieza.icono;
        }
    }
    
    public void Rotar()
    {
        if (!datosPieza.permiteRotacion) return;
        
        rotacionActual += 90;
        if (rotacionActual >= 360) rotacionActual = 0;
        
        transform.rotation = Quaternion.Euler(0, 0, rotacionActual);
        
        // Notificar al sistema sobre la rotación para verificar conexiones
        Debug.Log($"Pieza rotada a {rotacionActual} grados");
    }
    
    public DireccionConexion[] ObtenerConexionesActuales()
    {
        // Rotar las conexiones según la rotación actual
        DireccionConexion[] conexionesOriginales = datosPieza.conexionesSalida;
        DireccionConexion[] conexionesRotadas = new DireccionConexion[conexionesOriginales.Length];
        
        int rotaciones = rotacionActual / 90;
        
        for (int i = 0; i < conexionesOriginales.Length; i++)
        {
            conexionesRotadas[i] = RotarDireccion(conexionesOriginales[i], rotaciones);
        }
        
        return conexionesRotadas;
    }
    
    private DireccionConexion RotarDireccion(DireccionConexion dir, int rotaciones)
    {
        if (rotaciones == 0) return dir;
        
        DireccionConexion nuevaDir = dir;
        for (int i = 0; i < rotaciones; i++)
        {
            nuevaDir = (DireccionConexion)(((int)nuevaDir + 1) % 4);
        }
        return nuevaDir;
    }
}
