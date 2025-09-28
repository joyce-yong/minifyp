using UnityEngine;

public class g_ProximityPromptUtil : MonoBehaviour
{
    [System.Serializable]
    public class PositionSettings
    {
        [SerializeField] private Vector3 positionOffset = Vector3.zero;
        [SerializeField] private Vector3 indicatorPositionOffset = Vector3.zero;
        [SerializeField] private Vector3 interactPositionOffset = Vector3.zero;
        
        public Vector3 PositionOffset => positionOffset;
        public Vector3 IndicatorPositionOffset => indicatorPositionOffset;
        public Vector3 InteractPositionOffset => interactPositionOffset;
    }
    
    [System.Serializable]
    public class RotationSettings
    {
        [SerializeField] private bool useCustomIndicatorRotation = false;
        [SerializeField] private Vector3 customIndicatorRotation = Vector3.zero;
        [SerializeField] private bool useCustomInteractRotation = false;
        [SerializeField] private Vector3 customInteractRotation = Vector3.zero;
        [SerializeField] private bool useCustomRotation = false;
        [SerializeField] private Vector3 customRotation = Vector3.zero;
        
        public bool UseCustomIndicatorRotation => useCustomIndicatorRotation;
        public Vector3 CustomIndicatorRotation => customIndicatorRotation;
        public bool UseCustomInteractRotation => useCustomInteractRotation;
        public Vector3 CustomInteractRotation => customInteractRotation;
        public bool UseCustomRotation => useCustomRotation;
        public Vector3 CustomRotation => customRotation;
    }
    
    [System.Serializable]
    public class ScaleSettings
    {
        [SerializeField] private bool useCustomScale = false;
        [SerializeField] private Vector3 customScale = Vector3.one;
        [SerializeField] private float indicatorMaxScale = 0.5f;
        [SerializeField] private float interactMaxScale = 0.6f;
        
        public bool UseCustomScale => useCustomScale;
        public Vector3 CustomScale => customScale;
        public float IndicatorMaxScale => indicatorMaxScale;
        public float InteractMaxScale => interactMaxScale;
    }
    
    [System.Serializable]
    public class DistanceSettings
    {
        [SerializeField] private float proximityDistance = 5f;
        [SerializeField] private float interactDistance = 2f;
        
        public float ProximityDistance => proximityDistance;
        public float InteractDistance => interactDistance;
    }
    
    [System.Serializable]
    public class ColorSettings
    {
        [SerializeField] private Color normalInteractColor = Color.white;
        [SerializeField] private Color lookingAtColor = new Color(1f, 0.86f, 0.38f, 1f);
        
        public Color NormalInteractColor => normalInteractColor;
        public Color LookingAtColor => lookingAtColor;
    }
    
    [System.Serializable]
    public class DynamicPositioning
    {
        [SerializeField] private bool enableDynamicPositioning = false;
        [SerializeField] private PositionSettings interactedPositions = new PositionSettings();
        [SerializeField] private float transitionDelay = 0.3f;
        
        public bool EnableDynamicPositioning => enableDynamicPositioning;
        public PositionSettings InteractedPositions => interactedPositions;
        public float TransitionDelay => transitionDelay;
    }
    
    [Header("Position Configuration")]
    [SerializeField] private PositionSettings defaultPositions = new PositionSettings();
    
    [Header("Rotation Configuration")]
    [SerializeField] private RotationSettings rotationSettings = new RotationSettings();
    
    [Header("Scale Configuration")]
    [SerializeField] private ScaleSettings scaleSettings = new ScaleSettings();
    
    [Header("Distance Configuration")]
    [SerializeField] private DistanceSettings distanceSettings = new DistanceSettings();
    
    [Header("Color Configuration")]
    [SerializeField] private ColorSettings colorSettings = new ColorSettings();
    
    [Header("Dynamic Positioning")]
    [SerializeField] private DynamicPositioning dynamicPositioning = new DynamicPositioning();
    
    private ProximityInteractableUI proximityUI;
    private bool isInAlternatePosition = false;
    
    public Vector3 GetAdjustedPosition(Vector3 originalPosition)
    {
        PositionSettings currentPositions = (isInAlternatePosition && dynamicPositioning.EnableDynamicPositioning) 
            ? dynamicPositioning.InteractedPositions 
            : defaultPositions;
        return originalPosition + currentPositions.PositionOffset;
    }
    
    public Vector3 GetIndicatorPosition(Vector3 basePosition)
    {
        PositionSettings currentPositions = (isInAlternatePosition && dynamicPositioning.EnableDynamicPositioning) 
            ? dynamicPositioning.InteractedPositions 
            : defaultPositions;
        return basePosition + currentPositions.IndicatorPositionOffset;
    }
    
    public Vector3 GetInteractPosition(Vector3 basePosition)
    {
        PositionSettings currentPositions = (isInAlternatePosition && dynamicPositioning.EnableDynamicPositioning) 
            ? dynamicPositioning.InteractedPositions 
            : defaultPositions;
        return basePosition + currentPositions.InteractPositionOffset;
    }
    
    public Quaternion GetIndicatorRotation(Quaternion originalRotation)
    {
        if (rotationSettings.UseCustomIndicatorRotation)
            return Quaternion.Euler(rotationSettings.CustomIndicatorRotation);
        return originalRotation;
    }
    
    public Quaternion GetInteractRotation(Quaternion originalRotation)
    {
        if (rotationSettings.UseCustomInteractRotation)
            return Quaternion.Euler(rotationSettings.CustomInteractRotation);
        return originalRotation;
    }
    
    public Vector3 GetAdjustedScale()
    {
        if (scaleSettings.UseCustomScale)
            return scaleSettings.CustomScale;
        return Vector3.one;
    }
    
    public float GetProximityDistance()
    {
        return distanceSettings.ProximityDistance;
    }
    
    public float GetInteractDistance()
    {
        return distanceSettings.InteractDistance;
    }
    
    public float GetIndicatorScale()
    {
        return scaleSettings.IndicatorMaxScale;
    }
    
    public float GetInteractScale()
    {
        return scaleSettings.InteractMaxScale;
    }
    
    public Color GetNormalColor()
    {
        return colorSettings.NormalInteractColor;
    }
    
    public Color GetLookingColor()
    {
        return colorSettings.LookingAtColor;
    }
    
    public void ApplyAdjustments(ProximityInteractableUI ui)
    {
        proximityUI = ui;
        if (proximityUI != null)
        {
            if (scaleSettings.UseCustomScale)
                proximityUI.transform.localScale = scaleSettings.CustomScale;
        }
    }
    
    public void OnObjectInteracted()
    {
        if (dynamicPositioning.EnableDynamicPositioning)
        {
            isInAlternatePosition = !isInAlternatePosition;
        }
    }
    
    public void ResetInteractionState()
    {
        isInAlternatePosition = false;
    }
    
    public float GetTransitionDelay()
    {
        return dynamicPositioning.TransitionDelay;
    }
    
    public bool IsDynamicPositioningEnabled()
    {
        return dynamicPositioning.EnableDynamicPositioning;
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
        
        if (rotationSettings.UseCustomRotation)
        {
            Gizmos.color = Color.red;
            Matrix4x4 oldMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(adjustedPos, Quaternion.Euler(rotationSettings.CustomRotation), Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one * 0.3f);
            Gizmos.matrix = oldMatrix;
        }
        
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(basePos, distanceSettings.ProximityDistance);
        
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(basePos, distanceSettings.InteractDistance);
        
        if (dynamicPositioning.EnableDynamicPositioning)
        {
            Vector3 dynamicAdjustedPos = basePos + dynamicPositioning.InteractedPositions.PositionOffset;
            Vector3 dynamicIndicatorPos = dynamicAdjustedPos + dynamicPositioning.InteractedPositions.IndicatorPositionOffset;
            Vector3 dynamicInteractPos = dynamicAdjustedPos + dynamicPositioning.InteractedPositions.InteractPositionOffset;
            
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(dynamicAdjustedPos, 0.15f);
            Gizmos.DrawLine(basePos, dynamicAdjustedPos);
            
            Gizmos.color = new Color(0.5f, 0.5f, 1f);
            Gizmos.DrawWireSphere(dynamicIndicatorPos, 0.1f);
            Gizmos.DrawLine(dynamicAdjustedPos, dynamicIndicatorPos);
            
            Gizmos.color = new Color(0.5f, 1f, 0.5f);
            Gizmos.DrawWireSphere(dynamicInteractPos, 0.1f);
            Gizmos.DrawLine(dynamicAdjustedPos, dynamicInteractPos);
            
            if (isInAlternatePosition)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(dynamicAdjustedPos, Vector3.one * 0.2f);
            }
        }
    }
}