using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class g_InventoryManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] GameObject[] hotbarSlots = new GameObject[3];
    [SerializeField] GameObject[] slots = new GameObject[6];
    [SerializeField] GameObject inventoryParent;
    [SerializeField] Transform handParent;
    [SerializeField] GameObject itemPrefab;
    [SerializeField] Camera cam;

    GameObject draggedObject;
    GameObject lastItemSlot;

    bool isInventoryOpened;

    int? selectedHotbarSlot = null;

    void Start()
    {
        HotbarItemChanged();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        CheckForHotbarInput();
        inventoryParent.SetActive(isInventoryOpened);

        // Move item
        if (draggedObject != null)
        {
            draggedObject.transform.position = Input.mousePosition;
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            if (isInventoryOpened)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                isInventoryOpened = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                isInventoryOpened = true;
            }
        }
    }

    private void CheckForHotbarInput()
    {
        int pressedSlotIndex = -1;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            pressedSlotIndex = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            pressedSlotIndex = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            pressedSlotIndex = 2;
        }

        if (pressedSlotIndex != -1)
        {
            // If the same key is pressed again, turn it off
            if (selectedHotbarSlot.HasValue && selectedHotbarSlot.Value == pressedSlotIndex)
            {
                selectedHotbarSlot = null;
            }
            // Otherwise, switch to the new slot / activate for the first time
            else
            {
                selectedHotbarSlot = pressedSlotIndex;
            }
            HotbarItemChanged();
        } 
    }

    private void HotbarItemChanged()
    {
        for (int i = 0; i < handParent.childCount; i++)
        {
            handParent.GetChild(i).gameObject.SetActive(false);
        }

        foreach(GameObject slot in hotbarSlots)
        {
            Vector3 scale;

            int currentSlotIndex = System.Array.IndexOf(hotbarSlots, slot);

            if (selectedHotbarSlot.HasValue && currentSlotIndex == selectedHotbarSlot.Value)
            {
                scale = new Vector3(1.1f, 1.1f, 1.1f);

                if (slot.GetComponent<g_InventorySlot>().heldItem != null)
                {
                    for (int i = 0; i < handParent.childCount; i++)
                    {
                        if (handParent.GetChild(i).GetComponent<g_ItemHand>().itemScriptableObject 
                            == slot.GetComponent<g_InventorySlot>().heldItem.GetComponent<g_InventoryItem>().itemScriptableObject)
                        {
                            handParent.GetChild(i).gameObject.SetActive(true);
                        }
                    }
                }
            }
            else
            {
                scale = new Vector3(0.9f, 0.9f, 0.9f);
            }

            slot.transform.localScale = scale;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;
            g_InventorySlot slot = clickedObject.GetComponent<g_InventorySlot>();

            if (slot != null && slot.heldItem != null)
            {
                draggedObject = slot.heldItem;
                slot.heldItem = null;
                lastItemSlot = clickedObject;
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (draggedObject != null && eventData.pointerCurrentRaycast.gameObject != null && eventData.button == PointerEventData.InputButton.Left)
        {
            GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;
            g_InventorySlot slot = clickedObject.GetComponent<g_InventorySlot>();

            // There isnt item in the slot - place item
            if (slot != null && slot.heldItem == null)
            {
                slot.SetHeldItem(draggedObject);
                draggedObject.transform.SetParent(slot.transform.parent.parent.GetChild(2));
            }
            // There is item in the slot - switch items
            else if (slot != null && slot.heldItem != null && slot.heldItem.GetComponent<g_InventoryItem>().stackCurrent == slot.heldItem.GetComponent<g_InventoryItem>().stackMax
                || slot != null && slot.heldItem != null && slot.heldItem.GetComponent<g_InventoryItem>().itemScriptableObject != draggedObject.GetComponent<g_InventoryItem>().itemScriptableObject)
            {
                // store the item from the target slot before overwriting it
                GameObject itemToSwitchBack = slot.heldItem;

                // place the target slot's item (itemToSwitchBack) back into the last slot
                lastItemSlot.GetComponent<g_InventorySlot>().SetHeldItem(itemToSwitchBack);
                // parent the returned item to the lastItemSlot's item container (Hotbar/Items or Inventory/Items)
                itemToSwitchBack.transform.SetParent(lastItemSlot.transform.parent.parent.GetChild(2));

                // place the dragged item into the target slot
                slot.SetHeldItem(draggedObject);
                // parent the dragged item to the target slot's item container
                draggedObject.transform.SetParent(slot.transform.parent.parent.GetChild(2));
            }

            // Fill stack
            else if (slot != null && slot.heldItem != null && slot.heldItem.GetComponent<g_InventoryItem>().stackCurrent < slot.heldItem.GetComponent<g_InventoryItem>().stackMax
                && slot.heldItem.GetComponent<g_InventoryItem>().itemScriptableObject == draggedObject.GetComponent<g_InventoryItem>().itemScriptableObject)
            {
                g_InventoryItem slotHeldItem = slot.heldItem.GetComponent<g_InventoryItem>();
                g_InventoryItem draggedItem = draggedObject.GetComponent<g_InventoryItem>();

                int itemsToFillStack = slotHeldItem.stackMax - slotHeldItem.stackCurrent;

                if (itemsToFillStack >= draggedItem.stackCurrent)
                {
                    slotHeldItem.stackCurrent += draggedItem.stackCurrent;
                    Destroy(draggedObject);
                }
                else
                {
                    slotHeldItem.stackCurrent += itemsToFillStack;
                    draggedItem.stackCurrent -= itemsToFillStack;
                    lastItemSlot.GetComponent<g_InventorySlot>().SetHeldItem(draggedObject);
                    // the item is being returned, so it must be reparented to the last slot's container
                    draggedObject.transform.SetParent(lastItemSlot.transform.parent.parent.GetChild(2));
                }
            }
            // Return item to last slot
            else if (clickedObject.name != "DropItem")
            {
                lastItemSlot.GetComponent<g_InventorySlot>().SetHeldItem(draggedObject);
                // parent the item back to its original slot's container (the lastItemSlot's container)
                draggedObject.transform.SetParent(lastItemSlot.transform.parent.parent.GetChild(2));
            }
            // Drop Item
            else
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                Vector3 position = ray.GetPoint(3);

                g_InventoryItem draggedItemComponent = draggedObject.GetComponent<g_InventoryItem>();

                GameObject newItem = Instantiate(draggedItemComponent.itemScriptableObject.prefab, position, new Quaternion());
                newItem.GetComponent<g_ItemPickable>().itemScriptableObject = draggedItemComponent.itemScriptableObject;

                draggedItemComponent.stackCurrent -= 1;

                if (draggedItemComponent.stackCurrent <= 0)
                {
                    lastItemSlot.GetComponent<g_InventorySlot>().heldItem = null;
                    Destroy(draggedObject);
                }
                else
                {
                    lastItemSlot.GetComponent<g_InventorySlot>().SetHeldItem(draggedObject);
                    draggedObject.transform.SetParent(lastItemSlot.transform.parent.parent.GetChild(2));
                }
            }

            HotbarItemChanged();
            draggedObject = null;
        }
    }

    public void ItemPicked(GameObject pickedItem)
    {
        g_ItemSO pickedSO = pickedItem.GetComponent<g_ItemPickable>().itemScriptableObject;

        // Try to find an existing stackable item of the same type
        foreach (GameObject slotObj in slots)
        {
            g_InventorySlot slot = slotObj.GetComponent<g_InventorySlot>();

            if (slot.heldItem != null)
            {
                g_InventoryItem heldItem = slot.heldItem.GetComponent<g_InventoryItem>();

                // If same type and not full stack
                if (heldItem.itemScriptableObject.name == pickedSO.name && heldItem.stackCurrent < heldItem.stackMax)
                {
                    heldItem.AddToStack(1);
                    Destroy(pickedItem);
                    return;
                }
            }
        }

        // If no existing stack, find an empty slot
        foreach (GameObject slotObj in slots)
        {
            g_InventorySlot slot = slotObj.GetComponent<g_InventorySlot>();

            if (slot.heldItem == null)
            {
                GameObject newItem = Instantiate(itemPrefab);
                g_InventoryItem invItem = newItem.GetComponent<g_InventoryItem>();
                invItem.itemScriptableObject = pickedSO;
                invItem.stackCurrent = 1;

                newItem.transform.SetParent(slot.transform.parent.parent.GetChild(2));
                newItem.transform.localScale = Vector3.one;
                slot.SetHeldItem(newItem);

                Destroy(pickedItem);
                return;
            }
        }

        Debug.Log("Inventory Full");
    }
}
 