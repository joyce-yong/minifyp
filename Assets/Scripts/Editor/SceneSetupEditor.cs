using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

public class SceneSetupEditor : EditorWindow
{
    private Color fogColor = new Color(0.05f, 0.05f, 0.1f);
    private float fogDensity = 0.01f;
    private Color skyTopColor = new Color(0.05f, 0.05f, 0.15f);
    private Color skyHorizonColor = new Color(0.1f, 0.05f, 0.2f);
    private int starCount = 1000;
    private float starSize = 0.01f;

    [MenuItem("Tools/Scene Setup/Night Sky & Fog Setup")]
    public static void ShowWindow()
    {
        GetWindow<SceneSetupEditor>("Night Sky Setup");
    }

    void OnGUI()
    {
        GUILayout.Label("Night Sky Skybox Settings", EditorStyles.boldLabel);

        skyTopColor = EditorGUILayout.ColorField("Sky Top Color", skyTopColor);
        skyHorizonColor = EditorGUILayout.ColorField("Sky Horizon Color", skyHorizonColor);
        starCount = EditorGUILayout.IntSlider("Star Count", starCount, 100, 5000);
        starSize = EditorGUILayout.Slider("Star Size", starSize, 0.001f, 0.05f);

        GUILayout.Space(20);
        GUILayout.Label("Fog Settings", EditorStyles.boldLabel);

        fogColor = EditorGUILayout.ColorField("Fog Color", fogColor);
        fogDensity = EditorGUILayout.Slider("Fog Density", fogDensity, 0.001f, 0.1f);

        GUILayout.Space(20);

        if (GUILayout.Button("Apply to Current Scene", GUILayout.Height(40)))
        {
            ApplySetup();
        }
    }

    void ApplySetup()
    {
        CreateNightSkybox();
        SetupFog();
        EditorUtility.DisplayDialog("Success", "Night sky skybox and fog applied to the scene!", "OK");
    }

    void CreateNightSkybox()
    {
        Material skyboxMat = new Material(Shader.Find("Skybox/Procedural"));

        skyboxMat.SetFloat("_SunSize", 0f);
        skyboxMat.SetFloat("_SunSizeConvergence", 0f);
        skyboxMat.SetFloat("_AtmosphereThickness", 0.5f);
        skyboxMat.SetColor("_SkyTint", skyTopColor);
        skyboxMat.SetColor("_GroundColor", skyHorizonColor);
        skyboxMat.SetFloat("_Exposure", 0.3f);

        string path = "Assets/Materials/NightSkybox.mat";
        AssetDatabase.CreateAsset(skyboxMat, path);
        AssetDatabase.SaveAssets();

        RenderSettings.skybox = skyboxMat;
        DynamicGI.UpdateEnvironment();

        CreateStarfield();
    }

    void CreateStarfield()
    {
        GameObject starfield = GameObject.Find("Starfield");
        if (starfield != null)
        {
            DestroyImmediate(starfield);
        }

        starfield = new GameObject("Starfield");

        Material starMat = new Material(Shader.Find("Particles/Standard Unlit"));
        starMat.SetColor("_Color", Color.white);
        starMat.EnableKeyword("_EMISSION");

        string matPath = "Assets/Materials/StarMaterial.mat";
        AssetDatabase.CreateAsset(starMat, matPath);

        ParticleSystem ps = starfield.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.loop = false;
        main.playOnAwake = true;
        main.maxParticles = starCount;
        main.startLifetime = Mathf.Infinity;
        main.startSpeed = 0f;
        main.startSize = starSize;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var emission = ps.emission;
        emission.enabled = true;
        emission.rateOverTime = 0f;
        var burst = new ParticleSystem.Burst(0f, starCount);
        emission.SetBurst(0, burst);

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 500f;

        var renderer = starfield.GetComponent<ParticleSystemRenderer>();
        renderer.material = starMat;
        renderer.renderMode = ParticleSystemRenderMode.Billboard;

        ps.Play();
    }

    void SetupFog()
    {
        RenderSettings.fog = true;
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogMode = FogMode.Exponential;
        RenderSettings.fogDensity = fogDensity;
    }
}
