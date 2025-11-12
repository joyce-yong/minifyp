using UnityEngine;
using UnityEngine.UI;

public class CaptureVignetteEffect : MonoBehaviour
{
    public Image vignetteImage;
    public float vignetteIntensity = 0.8f;
    public float animationDuration = 0.3f;
    
    Material vignetteMaterial;
    
    void Start()
    {
        SetupVignette();
    }
    
    void SetupVignette()
    {
        if (vignetteImage == null) return;
        
        Shader vignetteShader = Shader.Find("UI/VignetteShader");
        if (vignetteShader == null)
        {
            Debug.LogWarning("Vignette shader not found, using solid color");
            vignetteImage.color = new Color(0, 0, 0, 0);
            return;
        }
        
        vignetteMaterial = new Material(vignetteShader);
        vignetteImage.material = vignetteMaterial;
        vignetteImage.color = Color.black;
        vignetteMaterial.SetFloat("_Intensity", 0);
    }
    
    public void ShowVignette()
    {
        if (vignetteMaterial)
        {
            vignetteMaterial.SetFloat("_Intensity", vignetteIntensity);
        }
        else
        {
            vignetteImage.color = new Color(0, 0, 0, vignetteIntensity);
        }
    }
    
    public void HideVignette()
    {
        if (vignetteMaterial)
        {
            vignetteMaterial.SetFloat("_Intensity", 0);
        }
        else
        {
            vignetteImage.color = new Color(0, 0, 0, 0);
        }
    }
}
