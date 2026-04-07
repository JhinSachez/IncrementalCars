using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PanelLateralManager : MonoBehaviour
{
    [Header("Configuración del Panel")]
    [SerializeField] private RectTransform panelLateral;
    [SerializeField] private float anchoAbierto = 200;
    
    [Header("Botones de Pestañas")]
    [SerializeField] private Button botonMejoras;
    [SerializeField] private Button botonPiezas;
    [SerializeField] private Button botonInventario;
    
    [Header("Ventanas de Contenido")]
    [SerializeField] private GameObject ventanaMejoras;
    [SerializeField] private GameObject ventanaPiezas;
    [SerializeField] private GameObject ventanaInventario;
    
    private GameObject ventanaActiva;
    
    void Start()
    {
        // Configurar listeners de los botones
        if (botonMejoras != null)
            botonMejoras.onClick.AddListener(() => CambiarVentana(ventanaMejoras));
            
        if (botonPiezas != null)
            botonPiezas.onClick.AddListener(() => CambiarVentana(ventanaPiezas));
            
        if (botonInventario != null)
            botonInventario.onClick.AddListener(() => CambiarVentana(ventanaInventario));
        
        // El panel siempre está abierto
        if (panelLateral != null)
        {
            Vector2 sizeDelta = panelLateral.sizeDelta;
            sizeDelta.x = anchoAbierto;
            panelLateral.sizeDelta = sizeDelta;
        }
        
        // Iniciar con la ventana de mejoras activa
        InicializarConVentanaMejoras();
    }
    
    void InicializarConVentanaMejoras()
    {
        // Asegurar que todas las ventanas están ocultas primero
        if (ventanaMejoras != null) ventanaMejoras.SetActive(false);
        if (ventanaPiezas != null) ventanaPiezas.SetActive(false);
        if (ventanaInventario != null) ventanaInventario.SetActive(false);
        
        // Activar ventana de mejoras
        if (ventanaMejoras != null)
        {
            ventanaMejoras.SetActive(true);
            ventanaActiva = ventanaMejoras;
        }
    }
    
    void CambiarVentana(GameObject nuevaVentana)
    {
        // Desactivar ventana actual
        if (ventanaActiva != null)
            ventanaActiva.SetActive(false);
        
        // Activar nueva ventana
        if (nuevaVentana != null)
        {
            nuevaVentana.SetActive(true);
            ventanaActiva = nuevaVentana;
        }
    }
}
