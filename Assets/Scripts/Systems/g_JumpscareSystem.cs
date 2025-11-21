using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class g_JumpscareSystem : MonoBehaviour
{
    public GameObject jumpscareMesh;
    public float fadeInDuration = 0.3f;
    public float fadeOutDuration = 0.5f;
    public float displayDuration = 2f;
    public bool triggerOnce = true;

    private Animator jumpscareAnimator;
    private Renderer[] jumpscareRenderers;
    private bool hasTriggered = false;
    private Dictionary<Material, Material> originalMaterials = new Dictionary<Material, Material>();
    private Dictionary<Material, Shader> originalShaders = new Dictionary<Material, Shader>();

    void Start()
    {
        if (jumpscareMesh != null)
        {
            jumpscareAnimator = jumpscareMesh.GetComponent<Animator>();
            jumpscareRenderers = jumpscareMesh.GetComponentsInChildren<Renderer>();

            SetupTransparentMaterials();

            jumpscareMesh.SetActive(false);
        }
    }

    void SetupTransparentMaterials()
    {
        foreach (Renderer rend in jumpscareRenderers)
        {
            Material[] mats = rend.materials;
            for (int i = 0; i < mats.Length; i++)
            {
                Material originalMat = mats[i];
                Material newMat = new Material(originalMat);

                originalShaders[newMat] = originalMat.shader;

                if (newMat.HasProperty("_Mode"))
                {
                    newMat.SetFloat("_Mode", 3);
                }

                newMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                newMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                newMat.SetInt("_ZWrite", 0);
                newMat.DisableKeyword("_ALPHATEST_ON");
                newMat.EnableKeyword("_ALPHABLEND_ON");
                newMat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                newMat.renderQueue = 3000;

                mats[i] = newMat;
            }
            rend.materials = mats;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (triggerOnce && hasTriggered) return;

            StartCoroutine(PlayJumpscare());
            hasTriggered = true;
        }
    }

    IEnumerator PlayJumpscare()
    {
        if (jumpscareMesh == null) yield break;

        jumpscareMesh.SetActive(true);

        SetMeshAlpha(0f);

        yield return StartCoroutine(FadeIn());

        if (jumpscareAnimator != null)
        {
            jumpscareAnimator.SetTrigger("jump_scare");
        }

        yield return new WaitForSeconds(displayDuration);

        yield return StartCoroutine(FadeOut());

        jumpscareMesh.SetActive(false);
    }

    IEnumerator FadeIn()
    {
        float elapsed = 0f;

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);
            SetMeshAlpha(alpha);
            yield return null;
        }

        SetMeshAlpha(1f);
    }

    IEnumerator FadeOut()
    {
        float elapsed = 0f;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeOutDuration);
            SetMeshAlpha(alpha);
            yield return null;
        }

        SetMeshAlpha(0f);
    }

    void SetMeshAlpha(float alpha)
    {
        if (jumpscareRenderers == null) return;

        foreach (Renderer rend in jumpscareRenderers)
        {
            foreach (Material mat in rend.materials)
            {
                if (mat.HasProperty("_Color"))
                {
                    Color color = mat.GetColor("_Color");
                    color.a = alpha;
                    mat.SetColor("_Color", color);
                }

                if (mat.HasProperty("_BaseColor"))
                {
                    Color baseColor = mat.GetColor("_BaseColor");
                    baseColor.a = alpha;
                    mat.SetColor("_BaseColor", baseColor);
                }

                if (mat.HasProperty("_MainTex"))
                {
                    Color mainColor = mat.color;
                    mainColor.a = alpha;
                    mat.color = mainColor;
                }
            }
        }
    }
}
