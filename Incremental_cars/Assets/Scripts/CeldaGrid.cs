using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CeldaGrid : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Componentes")]
    [SerializeField] private Image fondo;
    [SerializeField] private Outline outline;
    
    private int x;
    private int y;
    private SistemaGrid sistemaGrid;
    private bool estaOcupada = false;
    
    public int X => x;
    public int Y => y;
    public bool EstaOcupada 
    { 
        get => estaOcupada;
        set 
        { 
            estaOcupada = value;
            ActualizarColor();
        }
    }
    
    public void Inicializar(int x, int y, float tamaño, SistemaGrid sistema)
    {
        this.x = x;
        this.y = y;
        this.sistemaGrid = sistema;
        
        RectTransform rect = GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(tamaño, tamaño);
        
        ActualizarColor();
    }
    
    public void ActualizarColor()
    {
        if (fondo == null) return;
        
        if (estaOcupada)
        {
            fondo.color = sistemaGrid.colorOcupada;
        }
        else
        {
            fondo.color = sistemaGrid.colorNormalCelda;
        }
    }
    
    public void MostrarHover(bool mostrar)
    {
        if (outline != null)
        {
            outline.enabled = mostrar;
        }
        
        if (!mostrar && !estaOcupada && fondo != null)
        {
            fondo.color = sistemaGrid.colorNormalCelda;
        }
        else if (mostrar && !estaOcupada && fondo != null)
        {
            fondo.color = sistemaGrid.colorHoverCelda;
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!estaOcupada)
        {
            MostrarHover(true);
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        MostrarHover(false);
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // El sistema grid maneja la colocación
        }
    }
}
