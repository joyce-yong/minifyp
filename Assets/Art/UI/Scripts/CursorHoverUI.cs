using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CursorHoverUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Texture2D hoverCursor;
    private Texture2D normalCursor;

    private void Start()
    {
        normalCursor = CursorManager.instance != null ? CursorManager.instance.cursor : null;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverCursor != null)
        {
            Cursor.SetCursor(hoverCursor, new Vector2(hoverCursor.width / 2f, hoverCursor.height / 2f), CursorMode.Auto);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (normalCursor != null)
        {
            Cursor.SetCursor(normalCursor, new Vector2(normalCursor.width / 2f, normalCursor.height / 2f), CursorMode.Auto);
        }
    }
}

