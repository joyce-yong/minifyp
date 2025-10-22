using UnityEngine;
using UnityEngine.UI;

public class g_InventoryItem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private Text stackText;

    [Header("Item Data")]
    [SerializeField] private g_ItemSO _itemScriptableObject;
    public g_ItemSO itemScriptableObject
    {
        get => _itemScriptableObject;
        set
        {
            _itemScriptableObject = value;
            if (_itemScriptableObject != null)
            {
                stackMax = _itemScriptableObject.stackMax;
                stackCurrent = 1;

                if (iconImage != null)
                    iconImage.sprite = _itemScriptableObject.icon;

                UpdateStackText();
            }
        }
    }

    [Header("Stack Info")]
    public int stackCurrent;
    public int stackMax;

    public void AddToStack(int amount)
    {
        stackCurrent = Mathf.Min(stackCurrent + amount, stackMax);
        UpdateStackText();
    }

    private void UpdateStackText()
    {
        if (stackText != null)
        {
            stackText.text = stackCurrent > 1 ? stackCurrent.ToString() : "";
        }
    }
}
