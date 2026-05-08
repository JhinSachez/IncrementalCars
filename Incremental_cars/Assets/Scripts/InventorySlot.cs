using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    [Header("Referencias UI")]
    [SerializeField] private Image iconoImagen;
    [SerializeField] private TMP_Text cantidadTexto;
    
    private ItemData itemData;
    private int cantidad;
    private InventoryController inventoryController;
    
    public void Configurar(ItemData item, int cantidadActual, InventoryController controller)
    {
        itemData = item;
        cantidad = cantidadActual;
        inventoryController = controller;
        
        if (iconoImagen != null && item.itemIcon != null)
            iconoImagen.sprite = item.itemIcon;
        
        ActualizarCantidad();
    }
    
    public void ActualizarCantidad()
    {
        if (cantidadTexto != null)
            cantidadTexto.text = cantidad.ToString();
    }
    
    public void AgregarCantidad(int cantidadAgregar)
    {
        cantidad += cantidadAgregar;
        ActualizarCantidad();
    }
    private void OnDestroy()
    {
        // Notificar al inventory controller que este slot fue destruido
        if (inventoryController != null)
        {
            inventoryController.LimpiarSlotNulo(itemData);
        }
    }
    public bool RemoverCantidad(int cantidadRemover)
    {
        if (cantidad >= cantidadRemover)
        {
            cantidad -= cantidadRemover;
            ActualizarCantidad();
            return true;
        }
        return false;
    }
    
    public ItemData GetItemData() => itemData;
    public int GetCantidad() => cantidad;
    
    // Al hacer click en el slot, crear el item para arrastrar como si fuera tecla Q
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && cantidad > 0)
        {
            // Remover 1 unidad del inventario
            if (RemoverCantidad(1))
            {
                // Crear el item para poner en el grid (como cuando presionas Q)
                inventoryController.CrearItemParaGrid(itemData);
                
                // Si la cantidad llega a 0, destruir el slot
                if (cantidad <= 0)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
