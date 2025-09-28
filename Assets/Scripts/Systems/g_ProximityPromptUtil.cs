using UnityEngine;

public class g_ProximityPromptUtil : MonoBehaviour
{
    [Header("Position Offset")]
    [SerializeField] private Vector3 positionOffset = Vector3.zero;
    
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
    
    public Quaternion GetAdjustedRotation(Quaternion originalRotation)
    {
        if (useCustomRotation)
            return Quaternion.Euler(customRotation);
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
        Gizmos.color = Color.yellow;
        Vector3 adjustedPos = GetAdjustedPosition(transform.position);
        Gizmos.DrawWireSphere(adjustedPos, 0.2f);
        Gizmos.DrawLine(transform.position, adjustedPos);
        
        if (useCustomRotation)
        {
            Gizmos.color = Color.red;
            Matrix4x4 oldMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(adjustedPos, Quaternion.Euler(customRotation), Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one * 0.3f);
            Gizmos.matrix = oldMatrix;
        }
        
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, proximityDistance);
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactDistance);
    }
}