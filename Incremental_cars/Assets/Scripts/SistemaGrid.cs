using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class SistemaGrid : MonoBehaviour
{
   [Header("Configuración del Grid")]
    [SerializeField] private int anchoGrid = 10;
    [SerializeField] private int altoGrid = 10;
    [SerializeField] private float tamañoCelda = 1f;
    [SerializeField] private Transform contenedorGrid;
    [SerializeField] private GameObject celdaPrefab;
    
    [Header("Configuración Visual")]
    [SerializeField] public Color colorNormalCelda = new Color(1, 1, 1, 0.2f);
    [SerializeField] public Color colorHoverCelda = new Color(1, 1, 1, 0.4f);
    [SerializeField] public Color colorOcupada = new Color(0.5f, 1, 0.5f, 0.3f);
    
    private CeldaGrid[,] celdas;
    private Dictionary<Vector2Int, PiezaInstalada> piezasEnGrid;
    private PiezaPistaSO piezaSeleccionada;
    private Vector2Int? hoverCelda;
    
    void Start()
    {
        InicializarGrid();
    }
    
    void InicializarGrid()
    {
        celdas = new CeldaGrid[anchoGrid, altoGrid];
        piezasEnGrid = new Dictionary<Vector2Int, PiezaInstalada>();
        
        for (int x = 0; x < anchoGrid; x++)
        {
            for (int y = 0; y < altoGrid; y++)
            {
                GameObject nuevaCelda = Instantiate(celdaPrefab, contenedorGrid);
                CeldaGrid celda = nuevaCelda.GetComponent<CeldaGrid>();
                celda.Inicializar(x, y, tamañoCelda, this);
                celdas[x, y] = celda;
            }
        }
    }
    
    void Update()
    {
        ManejarInputGrid();
    }
    
    void ManejarInputGrid()
    {
        if (piezaSeleccionada == null) return;
        
        // Obtener posición del mouse en el grid
        Vector2 mousePos = Input.mousePosition;
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
        
        if (hit.collider != null)
        {
            CeldaGrid celda = hit.collider.GetComponent<CeldaGrid>();
            if (celda != null)
            {
                if (hoverCelda.HasValue && (hoverCelda.Value.x != celda.X || hoverCelda.Value.y != celda.Y))
                {
                    LimpiarHover();
                }
                
                hoverCelda = new Vector2Int(celda.X, celda.Y);
                celda.MostrarHover(true);
                
                // Click izquierdo para colocar
                if (Input.GetMouseButtonDown(0))
                {
                    ColocarPieza(celda.X, celda.Y);
                }
            }
        }
        else if (hoverCelda.HasValue)
        {
            LimpiarHover();
        }
        
        // Click derecho para rotar
        if (Input.GetMouseButtonDown(1) && hoverCelda.HasValue)
        {
            RotarPieza(hoverCelda.Value.x, hoverCelda.Value.y);
        }
        
        // Tecla R para rotar la pieza seleccionada
        if (Input.GetKeyDown(KeyCode.R) && hoverCelda.HasValue)
        {
            RotarPieza(hoverCelda.Value.x, hoverCelda.Value.y);
        }
        
        // Tecla Supr para eliminar
        if (Input.GetKeyDown(KeyCode.Delete) && hoverCelda.HasValue)
        {
            EliminarPieza(hoverCelda.Value.x, hoverCelda.Value.y);
        }
    }
    
    void LimpiarHover()
    {
        if (hoverCelda.HasValue && celdas[hoverCelda.Value.x, hoverCelda.Value.y] != null)
        {
            celdas[hoverCelda.Value.x, hoverCelda.Value.y].MostrarHover(false);
        }
        hoverCelda = null;
    }
    
    public void SeleccionarPieza(PiezaPistaSO pieza)
    {
        piezaSeleccionada = pieza;
        Debug.Log($"Pieza seleccionada: {pieza.nombrePieza}");
    }
    
    void ColocarPieza(int x, int y)
    {
        if (piezaSeleccionada == null) return;
        
        Vector2Int posicion = new Vector2Int(x, y);
        
        if (piezasEnGrid.ContainsKey(posicion))
        {
            Debug.Log("Ya hay una pieza en esta posición");
            return;
        }
        
        // Verificar conexiones válidas
        if (!VerificarConexionesValidas(posicion, piezaSeleccionada))
        {
            Debug.Log("Conexiones inválidas");
            return;
        }
        
        // Instanciar la pieza
        GameObject nuevaPiezaObj = Instantiate(piezaSeleccionada.prefabPieza, 
            ObtenerPosicionMundo(x, y), 
            Quaternion.identity, 
            contenedorGrid);
        
        PiezaInstalada nuevaPieza = nuevaPiezaObj.GetComponent<PiezaInstalada>();
        nuevaPieza.Inicializar(piezaSeleccionada, x, y);
        
        piezasEnGrid.Add(posicion, nuevaPieza);
        celdas[x, y].EstaOcupada = true;
        celdas[x, y].ActualizarColor();
        
        piezaSeleccionada = null;
    }
    
    void RotarPieza(int x, int y)
    {
        Vector2Int posicion = new Vector2Int(x, y);
        
        if (piezasEnGrid.TryGetValue(posicion, out PiezaInstalada pieza))
        {
            pieza.Rotar();
        }
    }
    
    void EliminarPieza(int x, int y)
    {
        Vector2Int posicion = new Vector2Int(x, y);
        
        if (piezasEnGrid.TryGetValue(posicion, out PiezaInstalada pieza))
        {
            Destroy(pieza.gameObject);
            piezasEnGrid.Remove(posicion);
            celdas[x, y].EstaOcupada = false;
            celdas[x, y].ActualizarColor();
        }
    }
    
    bool VerificarConexionesValidas(Vector2Int posicion, PiezaPistaSO pieza)
    {
        // Implementar lógica de verificación de conexiones
        return true;
    }
    
    Vector3 ObtenerPosicionMundo(int x, int y)
    {
        float offsetX = -((anchoGrid - 1) * tamañoCelda) / 2f;
        float offsetY = -((altoGrid - 1) * tamañoCelda) / 2f;
        
        return new Vector3(offsetX + (x * tamañoCelda), offsetY + (y * tamañoCelda), 0);
    }
}
