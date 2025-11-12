using UnityEngine;
using UnityEditor;

public class BatchGameObjectRenamer : EditorWindow
{
    private string baseName = "GameObject";
    private int startingNumber = 1;
    private bool preserveHierarchyOrder = true;
    
    [MenuItem("Tools/Batch GameObject Renamer")]
    public static void ShowWindow()
    {
        GetWindow<BatchGameObjectRenamer>("Batch Renamer");
    }
    
    void OnGUI()
    {
        GUILayout.Label("Batch GameObject Renamer", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        GUILayout.Label("Base Name:");
        baseName = EditorGUILayout.TextField(baseName);
        
        GUILayout.Space(5);
        
        GUILayout.Label("Starting Number:");
        startingNumber = EditorGUILayout.IntField(startingNumber);
        
        GUILayout.Space(5);
        
        preserveHierarchyOrder = EditorGUILayout.Toggle("Preserve Hierarchy Order", preserveHierarchyOrder);
        
        GUILayout.Space(10);
        
        EditorGUILayout.HelpBox("Select multiple GameObjects in the hierarchy, then click 'Rename Selected Objects'.", MessageType.Info);
        
        GUILayout.Space(5);
        
        if (Selection.gameObjects != null && Selection.gameObjects.Length > 0)
        {
            GUILayout.Label($"Selected Objects: {Selection.gameObjects.Length}");
            
            GUILayout.Space(5);
            GUILayout.Label("Preview:", EditorStyles.boldLabel);
            
            var selectedObjects = GetSortedSelection();
            for (int i = 0; i < Mathf.Min(selectedObjects.Length, 5); i++)
            {
                string newName = $"{baseName}_{startingNumber + i}";
                GUILayout.Label($"• {selectedObjects[i].name} → {newName}");
            }
            
            if (selectedObjects.Length > 5)
            {
                GUILayout.Label($"• ... and {selectedObjects.Length - 5} more objects");
            }
        }
        else
        {
            GUILayout.Label("No objects selected");
        }
        
        GUILayout.Space(10);
        
        GUI.enabled = Selection.gameObjects != null && Selection.gameObjects.Length > 0 && !string.IsNullOrEmpty(baseName);
        
        if (GUILayout.Button("Rename Selected Objects", GUILayout.Height(30)))
        {
            RenameSelectedObjects();
        }
        
        GUI.enabled = true;
        
        GUILayout.Space(10);
        
        GUILayout.Label("Quick Actions:", EditorStyles.boldLabel);
        
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Health_Bar"))
        {
            baseName = "Health_Bar";
            startingNumber = 1;
        }
        if (GUILayout.Button("Enemy"))
        {
            baseName = "Enemy";
            startingNumber = 1;
        }
        if (GUILayout.Button("Platform"))
        {
            baseName = "Platform";
            startingNumber = 1;
        }
        GUILayout.EndHorizontal();
    }
    
    private GameObject[] GetSortedSelection()
    {
        var selectedObjects = Selection.gameObjects;
        
        if (preserveHierarchyOrder)
        {
            System.Array.Sort(selectedObjects, (a, b) =>
            {
                int depthA = GetHierarchyDepth(a.transform);
                int depthB = GetHierarchyDepth(b.transform);
                
                if (depthA != depthB)
                    return depthA.CompareTo(depthB);
                
                return a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex());
            });
        }
        
        return selectedObjects;
    }
    
    private int GetHierarchyDepth(Transform transform)
    {
        int depth = 0;
        while (transform.parent != null)
        {
            depth++;
            transform = transform.parent;
        }
        return depth;
    }
    
    private void RenameSelectedObjects()
    {
        if (Selection.gameObjects == null || Selection.gameObjects.Length == 0)
        {
            EditorUtility.DisplayDialog("No Selection", "Please select one or more GameObjects to rename.", "OK");
            return;
        }
        
        if (string.IsNullOrEmpty(baseName))
        {
            EditorUtility.DisplayDialog("Invalid Base Name", "Please enter a valid base name.", "OK");
            return;
        }
        
        Undo.RecordObjects(Selection.gameObjects, "Batch Rename GameObjects");
        
        var sortedObjects = GetSortedSelection();
        
        for (int i = 0; i < sortedObjects.Length; i++)
        {
            string newName = $"{baseName}_{startingNumber + i}";
            sortedObjects[i].name = newName;
            EditorUtility.SetDirty(sortedObjects[i]);
        }
        
        Debug.Log($"Renamed {sortedObjects.Length} GameObjects with base name '{baseName}' starting from {startingNumber}");
        
        EditorApplication.RepaintHierarchyWindow();
    }
}