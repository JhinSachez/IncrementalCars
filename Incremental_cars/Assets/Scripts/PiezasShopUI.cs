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
            Image imagen = botonObj.GetComponent<Image>();
            Text texto = botonObj.GetComponentInChildren<Text>();
            
            // Configurar icono del item
            if (imagen != null && item.itemIcon != null)
            {
                imagen.sprite = item.itemIcon;
            }
            
            // Configurar texto del botón
            if (texto != null)
            {
                texto.text = item.itemName;
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
        
        // Crear el item en el inventario
        inventoryController.AddItemToInventory(item);
        
        Debug.Log($"Item obtenido: {item.name}");
    }
}
