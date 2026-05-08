using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PiezasShopUI : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform botonesContainer;
    [SerializeField] private GameObject botonItemPrefab;
    [SerializeField] private InventoryController inventoryController;
    
    [Header("Items Disponibles")]
    [SerializeField] private List<ItemData> itemsDisponibles;
    
    private void Start()
    {
        CrearBotonesItems();
    }
    
    private void CrearBotonesItems()
    {
        // Limpiar botones existentes
        foreach (Transform child in botonesContainer)
        {
            Destroy(child.gameObject);
        }
    
        // Crear un botón por cada item disponible
        foreach (ItemData item in itemsDisponibles)
        {
            GameObject botonObj = Instantiate(botonItemPrefab, botonesContainer);
            Button boton = botonObj.GetComponent<Button>();
        
            // Buscar la imagen adicional del item (no la del botón)
            Image imagenItem = botonObj.transform.Find("ItemImage")?.GetComponent<Image>();
            if (imagenItem != null && item.itemIcon != null)
            {
                imagenItem.sprite = item.itemIcon;
            }
        
            // Buscar el texto del nombre del objeto
            Text textoNombre = botonObj.transform.Find("ItemName")?.GetComponent<Text>();
            if (textoNombre != null)
            {
                textoNombre.text = item.itemName;
            }
        
            // Agregar evento de click
            ItemData itemLocal = item;
            boton.onClick.AddListener(() => ObtenerItem(itemLocal));
        }
    }
    
    private void ObtenerItem(ItemData item)
    {
        if (inventoryController == null)
        {
            Debug.LogError("InventoryController no asignado en PiezasShopUI");
            return;
        }
    
        // Usar el nuevo método
        inventoryController.AgregarItemAlInventario(item, 1);
    
        Debug.Log($"Item obtenido: {item.name}");
    }
}
