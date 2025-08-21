using UnityEngine;

public class anim_ceiling_Fan : MonoBehaviour
{
    [SerializeField] private Transform fanBlades;
    [SerializeField] private float speed = 100f;
    [SerializeField] private bool clockwise = true;

    void Update()
    {
        if (fanBlades != null)
        {
            float rotationDirection = clockwise ? 1f : -1f;
            fanBlades.Rotate(0f, 0f, speed * rotationDirection * Time.deltaTime);
        }
    }
}