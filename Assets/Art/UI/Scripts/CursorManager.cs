using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager instance;

    public Texture2D cursor;
    public Texture2D cursorClicked;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        ChangeCursor(cursor);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    public void ChangeCursor(Texture2D cursorType)
    {
        Vector2 hotspot = new Vector2(cursorType.width / 2f, cursorType.height / 2f);
        Cursor.SetCursor(cursorType, hotspot, CursorMode.Auto);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            ChangeCursor(cursorClicked);
        if (Input.GetMouseButtonUp(0))
            ChangeCursor(cursor);
    }
}
