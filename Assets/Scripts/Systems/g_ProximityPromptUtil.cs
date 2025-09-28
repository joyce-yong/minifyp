using UnityEngine;

public class g_ProximityPromptUtil : MonoBehaviour
{
    [Header("Position Offset")]
    [SerializeField] private Vector3 positionOffset = Vector3.zero;
    
    [Header("Icon Positions")]
    [SerializeField] private Vector3 indicatorPositionOffset = Vector3.zero;
    [SerializeField] private Vector3 interactPositionOffset = Vector3.zero;
    
    [Header("Icon Rotations")]
    [SerializeField] private bool useCustomIndicatorRotation = false;
    [SerializeField] private Vector3 customIndicatorRotation = Vector3.zero;
    [SerializeField] private bool useCustomInteractRotation = false;
    [SerializeField] private Vector3 customInteractRotation = Vector3.zero;
    
    [Header("Rotation Override")]
    [SerializeField] private bool useCustomRotation = false;
    [SerializeField] private Vector3 customRotation = Vector3.zero;
    
    [Header("Scale Override")]
    [SerializeField] private bool useCustomScale = false;
    [SerializeField] private Vector3 customScale = Vector3.one;
    
    [Header("Distance Settings")]
    [SerializeField] private float proximityDistance = 5f;
    [SerializeField] private float interactDistance = 2f;
    
    [Header("Scale Settings")]
    [SerializeField] private float indicatorMaxScale = 0.5f;
    [SerializeField] private float interactMaxScale = 0.6f;
    
    [Header("Look At Color")]
    [SerializeField] private Color normalInteractColor = Color.white;
    [SerializeField] private Color lookingAtColor = new Color(1f, 0.86f, 0.38f, 1f);
    
    private ProximityInteractableUI proximityUI;
    
    public Vector3 GetAdjustedPosition(Vector3 originalPosition)
    {
        return originalPosition + positionOffset;
    }
    
    public Vector3 GetIndicatorPosition(Vector3 basePosition)
    {
        return basePosition + indicatorPositionOffset;
    }
    
    public Vector3 GetInteractPosition(Vector3 basePosition)
    {
        return basePosition + interactPositionOffset;
    }
    
    public Quaternion GetIndicatorRotation(Quaternion originalRotation)
    {
        if (useCustomIndicatorRotation)
            return Quaternion.Euler(customIndicatorRotation);
        return originalRotation;
    }
    
    public Quaternion GetInteractRotation(Quaternion originalRotation)
    {
        if (useCustomInteractRotation)
            return Quaternion.Euler(customInteractRotation);
        return originalRotation;
    }
    
    public Vector3 GetAdjustedScale()
    {
        if (useCustomScale)
            return customScale;
        return Vector3.one;
    }
    
    public float GetProximityDistance()
    {
        return proximityDistance;
    }
    
    public float GetInteractDistance()
    {
        return interactDistance;
    }
    
    public float GetIndicatorScale()
    {
        return indicatorMaxScale;
    }
    
    public float GetInteractScale()
    {
        return interactMaxScale;
    }
    
    public Color GetNormalColor()
    {
        return normalInteractColor;
    }
    
    public Color GetLookingColor()
    {
        return lookingAtColor;
    }
    
    public void ApplyAdjustments(ProximityInteractableUI ui)
    {
        proximityUI = ui;
        if (proximityUI != null)
        {
            if (useCustomScale)
                proximityUI.transform.localScale = customScale;
        }
    }
    
    void OnDrawGizmosSelected()
    {
        Vector3 basePos = transform.position;
        Vector3 adjustedPos = GetAdjustedPosition(basePos);
        Vector3 indicatorPos = GetIndicatorPosition(adjustedPos);
        Vector3 interactPos = GetInteractPosition(adjustedPos);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(adjustedPos, 0.2f);
        Gizmos.DrawLine(basePos, adjustedPos);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(indicatorPos, 0.15f);
        Gizmos.DrawLine(adjustedPos, indicatorPos);
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(interactPos, 0.15f);
        Gizmos.DrawLine(adjustedPos, interactPos);
        
        if (useCustomRotation)
        {
            Gizmos.color = Color.red;
            Matrix4x4 oldMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(adjustedPos, Quaternion.Euler(customRotation), Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one * 0.3f);
            Gizmos.matrix = oldMatrix;
        }
        
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(basePos, proximityDistance);
        
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(basePos, interactDistance);
    }
}