using UnityEngine;
using UnityEngine.UI;

public class g_InventoryItem : MonoBehaviour
{
    public g_ItemSO itemScriptableObject;

    [SerializeField] Image iconImage;

    void Update()
    {
        iconImage.sprite = itemScriptableObject.icon;
    }
}
