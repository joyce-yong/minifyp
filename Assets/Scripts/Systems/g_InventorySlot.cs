using UnityEngine;

public class g_InventorySlot : MonoBehaviour
{
    public GameObject heldItem;

    public void SetHeldItem(GameObject item)
    {
        heldItem = item;
        heldItem.transform.position = transform.position;

    }
}
