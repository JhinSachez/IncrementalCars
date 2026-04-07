using UnityEngine;
using UnityEngine.UI;


public class UIPiezasManager : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private SistemaGrid sistemaGrid;
    [SerializeField] private Transform contenedorPiezas;
    [SerializeField] private GameObject botonPiezaPrefab;
    
    [Header("Piezas Disponibles")]
    [SerializeField] private PiezaPistaSO[] piezasDisponibles;
    
    void Start()
    {
        CrearBotonesPiezas();
    }
    
    void CrearBotonesPiezas()
    {
        foreach (PiezaPistaSO pieza in piezasDisponibles)
        {
            GameObject botonObj = Instantiate(botonPiezaPrefab, contenedorPiezas);
            Button boton = botonObj.GetComponent<Button>();
            Image imagen = botonObj.GetComponent<Image>();
            
            if (imagen != null && pieza.icono != null)
            {
                imagen.sprite = pieza.icono;
            }
            
            PiezaPistaSO piezaLocal = pieza;
            boton.onClick.AddListener(() => sistemaGrid.SeleccionarPieza(piezaLocal));
            
            // Agregar texto del nombre
            Text texto = botonObj.GetComponentInChildren<Text>();
            if (texto != null)
            {
                texto.text = pieza.nombrePieza;
            }
        }
    }
}
