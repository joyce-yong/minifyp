using UnityEngine;
using UnityEditor;

public class TransparencyConverter : EditorWindow
{
    GameObject targetObject;
    float alphaValue = 0.3f;

    [MenuItem("Tools/Transparency Converter")]
    static void Init()
    {
        TransparencyConverter window = (TransparencyConverter)EditorWindow.GetWindow(typeof(TransparencyConverter));
        window.titleContent = new GUIContent("Transparency Converter");
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Make Ghost Materials Transparent", EditorStyles.boldLabel);
        GUILayout.Space(10);

        targetObject = (GameObject)EditorGUILayout.ObjectField("Target Mesh", targetObject, typeof(GameObject), true);

        GUILayout.Space(10);
        alphaValue = EditorGUILayout.Slider("Alpha Value", alphaValue, 0f, 1f);

        GUILayout.Space(20);

        if (GUILayout.Button("Convert to Transparent", GUILayout.Height(40)))
        {
            ConvertToTransparent();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Reset to Opaque", GUILayout.Height(30)))
        {
            ResetToOpaque();
        }
    }

    void ConvertToTransparent()
    {
        if (targetObject == null)
        {
            EditorUtility.DisplayDialog("Error", "Please assign a target object first!", "OK");
            return;
        }

        int materialCount = 0;

        Renderer[] renderers = targetObject.GetComponentsInChildren<Renderer>(true);

        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.sharedMaterials;

            for (int i = 0; i < materials.Length; i++)
            {
                if (materials[i] != null)
                {
                    SetMaterialTransparent(materials[i]);
                    materialCount++;
                }
            }
        }

        EditorUtility.DisplayDialog("Success", $"Converted {materialCount} materials to transparent mode!", "OK");
    }

    void SetMaterialTransparent(Material material)
    {
        if (material.HasProperty("_Surface"))
        {
            material.SetFloat("_Surface", 1);
        }

        if (material.HasProperty("_Mode"))
        {
            material.SetFloat("_Mode", 3);
        }

        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = 3000;

        if (material.HasProperty("_BaseColor"))
        {
            Color color = material.GetColor("_BaseColor");
            color.a = alphaValue;
            material.SetColor("_BaseColor", color);
        }

        if (material.HasProperty("_Color"))
        {
            Color color = material.GetColor("_Color");
            color.a = alphaValue;
            material.SetColor("_Color", color);
        }

        EditorUtility.SetDirty(material);
    }

    void ResetToOpaque()
    {
        if (targetObject == null)
        {
            EditorUtility.DisplayDialog("Error", "Please assign a target object first!", "OK");
            return;
        }

        int materialCount = 0;

        Renderer[] renderers = targetObject.GetComponentsInChildren<Renderer>(true);

        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.sharedMaterials;

            for (int i = 0; i < materials.Length; i++)
            {
                if (materials[i] != null)
                {
                    SetMaterialOpaque(materials[i]);
                    materialCount++;
                }
            }
        }

        EditorUtility.DisplayDialog("Success", $"Reset {materialCount} materials to opaque mode!", "OK");
    }

    void SetMaterialOpaque(Material material)
    {
        material.SetFloat("_Mode", 0);
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        material.SetInt("_ZWrite", 1);
        material.DisableKeyword("_ALPHATEST_ON");
        material.DisableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = -1;

        if (material.HasProperty("_Color"))
        {
            Color color = material.color;
            color.a = 1f;
            material.color = color;
        }

        EditorUtility.SetDirty(material);
    }
}
