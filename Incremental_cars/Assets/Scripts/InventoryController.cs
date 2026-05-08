using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryController : MonoBehaviour
{
    [HideInInspector]
    private ItemGrid selectedItemGrid;

    public ItemGrid SelectedItemGrid { 
        get => selectedItemGrid;
        set {
            selectedItemGrid = value;
           //inventoryHighlight.SetParent(value);
        }
    }

    InventoryItem selectedItem;
    InventoryItem overlapItem;
    RectTransform rectTransform;

    [SerializeField] List<ItemData> items;
    [SerializeField] GameObject itemPrefab;
    [SerializeField] Transform canvasTransform;
    [SerializeField] private ItemGrid inventarioGrid;
    
    [Header("Nuevo Sistema de Slots")]
    [SerializeField] private Transform inventarioContainer;
    [SerializeField] private GameObject inventorySlotPrefab;
    private Dictionary<ItemData, InventorySlot> slotsPorItem;

    //InventoryHighlight inventoryHighlight;

    private void Awake()
    {
        //inventoryHighlight = GetComponent<InventoryHighlight>();
    }

    private void Update()
    {
        ItemIconDrag();

        if (Input.GetKeyDown(KeyCode.Q)) 
        {
            if (selectedItem == null) 
            {
                CreateRandomItem();
            }
            
        }

        if (Input.GetKeyDown(KeyCode.W)) 
        {
            InsertRandomItem();
        }

        if (Input.GetKeyDown(KeyCode.R)) 
        {
            RotateItem();
        }

        if (selectedItemGrid == null) 
        {
            //inventoryHighlight.Show(false);
            return; 
        }

        HandleHighlight();

        if (Input.GetMouseButtonDown(0))
        {
            LeftMouseButtonPress();
        }

    }
    
    private void InicializarSlots()
    {
        slotsPorItem = new Dictionary<ItemData, InventorySlot>();
    }
    
    public void CrearItemParaGrid(ItemData itemData)
    {
        // Crear el item igual que en CreateRandomItem() pero con el item específico
        InventoryItem inventoryItem = Instantiate(itemPrefab).GetComponent<InventoryItem>();
        selectedItem = inventoryItem;
    
        rectTransform = inventoryItem.GetComponent<RectTransform>();
        rectTransform.SetParent(canvasTransform);
        rectTransform.SetAsLastSibling();
    
        // Configurar el item con los datos del item seleccionado
        inventoryItem.Set(itemData);
    
        Debug.Log($"Item {itemData.name} creado para colocar en el grid");
    }
    
    public void AgregarItemAlInventario(ItemData item, int cantidad = 1)
    {
        if (slotsPorItem == null)
            slotsPorItem = new Dictionary<ItemData, InventorySlot>();
    
        // Verificar si el slot existe y no ha sido destruido
        if (slotsPorItem.ContainsKey(item) && slotsPorItem[item] != null)
        {
            slotsPorItem[item].AgregarCantidad(cantidad);
        }
        else
        {
            // Si la clave existe pero el slot es nulo, removerla
            if (slotsPorItem.ContainsKey(item))
                slotsPorItem.Remove(item);
        
            // Crear nuevo slot
            GameObject nuevoSlot = Instantiate(inventorySlotPrefab, inventarioContainer);
            InventorySlot slot = nuevoSlot.GetComponent<InventorySlot>();
            slot.Configurar(item, cantidad, this);
            slotsPorItem.Add(item, slot);
        }
    }

// Nuevo método para remover items
    public void RemoverItemDelInventario(ItemData item, int cantidad = 1)
    {
        if (slotsPorItem == null) return;
    
        if (slotsPorItem.ContainsKey(item))
        {
            if (slotsPorItem[item].RemoverCantidad(cantidad))
            {
                if (slotsPorItem[item].GetCantidad() <= 0)
                {
                    Destroy(slotsPorItem[item].gameObject);
                    slotsPorItem.Remove(item);
                }
            }
        }
    }
    
    public void LimpiarSlotNulo(ItemData item)
    {
        if (slotsPorItem != null && slotsPorItem.ContainsKey(item) && slotsPorItem[item] == null)
        {
            slotsPorItem.Remove(item);
        }
    }

// Método para manejar cuando se suelta un item desde el slot
    public void OnItemDroppedFromSlot(InventorySlot slot, PointerEventData eventData)
    {
        // Por ahora solo regresa el item (no hace nada extra)
        Debug.Log($"Item {slot.GetItemData().name} soltado");
    }

// Método para verificar si tienes cierto item
    public bool TieneItemEnInventario(ItemData item, int cantidadRequerida = 1)
    {
        if (slotsPorItem == null) return false;
        return slotsPorItem.ContainsKey(item) && slotsPorItem[item].GetCantidad() >= cantidadRequerida;
    }
    
    public bool AddItemToInventory(ItemData itemData)
    {
        if (inventarioGrid == null)
        {
            Debug.LogError("Inventario Grid no asignado en InventoryController");
            return false;
        }

        // Crear el item
        GameObject newItemObj = Instantiate(itemPrefab, canvasTransform);
        InventoryItem newItem = newItemObj.GetComponent<InventoryItem>();
        newItem.Set(itemData);
        
        // Intentar encontrar espacio en el grid del inventario
        Vector2Int? posicionLibre = inventarioGrid.FindSpaceForObject(newItem);
        
        if (posicionLibre.HasValue)
        {
            // Colocar el item en el inventario
            inventarioGrid.PlaceItem(newItem, posicionLibre.Value.x, posicionLibre.Value.y);
            newItemObj.transform.SetAsLastSibling();
            return true;
        }
        else
        {
            // No hay espacio, destruir el item
            Destroy(newItemObj);
            Debug.LogWarning($"No hay espacio en el inventario para: {itemData.name}");
            return false;
        }
    }

    // Método público para agregar item en posición específica (útil para cargar partidas)
    public bool AddItemToInventoryAtPosition(ItemData itemData, int posX, int posY, bool rotated = false)
    {
        if (inventarioGrid == null)
        {
            Debug.LogError("Inventario Grid no asignado en InventoryController");
            return false;
        }

        // Crear el item
        GameObject newItemObj = Instantiate(itemPrefab, canvasTransform);
        InventoryItem newItem = newItemObj.GetComponent<InventoryItem>();
        newItem.Set(itemData);
        
        // Aplicar rotación si es necesario
        if (rotated)
        {
            newItem.Rotate();
        }
        
        // Verificar si la posición es válida
        if (inventarioGrid.BoundryCheck(posX, posY, newItem.WIDTH, newItem.HEIGHT))
        {
            InventoryItem overlap = null;
            if (inventarioGrid.PlaceItem(newItem, posX, posY, ref overlap))
            {
                newItemObj.transform.SetAsLastSibling();
                return true;
            }
        }
        
        // Si no se pudo colocar, intentar encontrar espacio automáticamente
        Destroy(newItemObj);
        return AddItemToInventory(itemData);
    }

    private void RotateItem()
    {
        if (selectedItem == null) { return; }

        selectedItem.Rotate();
    }

    private void InsertRandomItem()
    {
        if (selectedItemGrid == null) { return; }

        CreateRandomItem();
        InventoryItem itemToInsert = selectedItem;
        selectedItem = null;
        InsertItem(itemToInsert);
    }

    private void InsertItem(InventoryItem itemToInsert)
    {
        Vector2Int? posOnGrid = selectedItemGrid.FindSpaceForObject(itemToInsert);

        if (posOnGrid == null) { return; }

        selectedItemGrid.PlaceItem(itemToInsert, posOnGrid.Value.x, posOnGrid.Value.y);
    }

    Vector2Int oldPosition;
    //InventoryItem itemToHighlight;
    private void HandleHighlight()
    {
        Vector2Int positionOnGrid = GetTileGridPosition();
        if (oldPosition == positionOnGrid) { return; }

        oldPosition = positionOnGrid;
        /*if (selectedItem == null)
        {
            itemToHighlight = selectedItemGrid.GetItem(positionOnGrid.x, positionOnGrid.y);

            if (itemToHighlight != null)
            {
                inventoryHighlight.Show(true);
                inventoryHighlight.SetSize(itemToHighlight);
                inventoryHighlight.SetPosition(selectedItemGrid, itemToHighlight);
            }
            else {
                inventoryHighlight.Show(false);
            }
        }
        else {
            inventoryHighlight.Show(selectedItemGrid.BoundryCheck(
                positionOnGrid.x, 
                positionOnGrid.y, 
                selectedItem.WIDTH,
                selectedItem.HEIGHT)
                );

            inventoryHighlight.SetSize(selectedItem);
            inventoryHighlight.SetPosition(selectedItemGrid, selectedItem, positionOnGrid.x, positionOnGrid.y);
        }*/
    }

    private void CreateRandomItem()
    {
        InventoryItem inventoryItem = Instantiate(itemPrefab).GetComponent<InventoryItem>();
        selectedItem = inventoryItem;

        rectTransform = inventoryItem.GetComponent<RectTransform>();
        rectTransform.SetParent(canvasTransform);
        rectTransform.SetAsLastSibling();

        int selectedItemID = UnityEngine.Random.Range(0, items.Count);
        inventoryItem.Set(items[selectedItemID]);
    }

    private void LeftMouseButtonPress()
    {
        Vector2Int tileGridPosition = GetTileGridPosition();

        if (selectedItem == null)
        {
            PickUpItem(tileGridPosition);
        }
        else
        {
            PlaceItem(tileGridPosition);
        }
    }

    private Vector2Int GetTileGridPosition()
    {
        Vector2 position = Input.mousePosition;

        if (selectedItem != null)
        {
            position.x -= (selectedItem.WIDTH - 1) * ItemGrid.tileSizeWidth / 2;
            position.y += (selectedItem.HEIGHT - 1) * ItemGrid.tileSizeHeight / 2;
        }

        return selectedItemGrid.GetTileGridPosition(position);
    }

    private void PlaceItem(Vector2Int tileGridPosition)
    {
        bool complete = selectedItemGrid.PlaceItem(selectedItem, tileGridPosition.x, tileGridPosition.y, ref overlapItem);
        if (complete) 
        {
            selectedItem = null;
            if (overlapItem != null) 
            {
                selectedItem = overlapItem;
                overlapItem = null;
                rectTransform = selectedItem.GetComponent<RectTransform>();
                rectTransform.SetAsLastSibling();
            }
        }
    }

    private void PickUpItem(Vector2Int tileGridPosition)
    {
        selectedItem = selectedItemGrid.PickUpItem(tileGridPosition.x, tileGridPosition.y);
        if (selectedItem != null)
        {
            rectTransform = selectedItem.GetComponent<RectTransform>();
        }
    }

    private void ItemIconDrag()
    {
        if (selectedItem != null)
        {
            rectTransform.position = Input.mousePosition;
        }
    }
}
