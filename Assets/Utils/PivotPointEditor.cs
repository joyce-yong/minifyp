using UnityEngine;
using UnityEditor;

[System.Serializable]
public class PivotPoint : MonoBehaviour
{
    [SerializeField] private Vector3 customPivot = Vector3.zero; // Local-space pivot

    public Vector3 GetPivotWorldPosition() => transform.TransformPoint(customPivot);
    public Vector3 GetPivotLocalPosition()  => customPivot;
    public void    SetPivotLocalPosition(Vector3 position) => customPivot = position;

    public void RotateAroundPivot(Vector3 axis, float angle)
    {
        transform.RotateAround(GetPivotWorldPosition(), axis, angle);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(PivotPoint))]
public class PivotPointEditor : Editor
{
    private SerializedProperty customPivotProp;
    private float customAngle = 90f;

    void OnEnable()
    {
        customPivotProp = serializedObject.FindProperty("customPivot");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(customPivotProp, new GUIContent("Custom Pivot (Local)"));
        EditorGUILayout.Space();

        var pivot = (PivotPoint)target;

        EditorGUILayout.LabelField("Rotate Around Pivot (Y Axis)", EditorStyles.boldLabel);
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("-90°")) RotateY(pivot, -90f);
            if (GUILayout.Button("+90°")) RotateY(pivot, +90f);
        }

        EditorGUILayout.Space(4);
        using (new EditorGUILayout.HorizontalScope())
        {
            customAngle = EditorGUILayout.FloatField("Custom Angle", customAngle);
            if (GUILayout.Button("Rotate Y")) RotateY(pivot, customAngle);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void RotateY(PivotPoint pivot, float angle)
    {
        Undo.RecordObject(pivot.transform, "Rotate Around Pivot (Y)");
        pivot.RotateAroundPivot(Vector3.up, angle);
        EditorUtility.SetDirty(pivot.transform);
    }
}
#endif
