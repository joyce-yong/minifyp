using UnityEngine;
using UnityEngine.UI;

public class g_InventoryItem : MonoBehaviour
{
    public g_ItemSO itemScriptableObject;

    public int stackCurrent = 1;
    public int stackMax;

    [SerializeField] Image iconImage;
    [SerializeField] Text stackText;

    private void Start()
    {
        stackMax = itemScriptableObject.stackMax;
    }

    void Update()
    {
        iconImage.sprite = itemScriptableObject.icon;

        if (stackMax > 1)
        {
            stackText.text = stackCurrent.ToString();
        }
    }
}
