using UnityEngine;
using System.Collections;

public class ParkingZone : MonoBehaviour
{
    [Header("Trigger Settings")]
    public string carTag = "Car";
    public bool triggerOnce = true;

    [Header("Car Stop Settings")]
    public float slowDownDuration = 3f;
    public float targetSpeed = 0f;

    private bool hasTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (triggerOnce && hasTriggered) return;

        if (other.CompareTag(carTag))
        {
            Player_Car_Movement carMovement = other.GetComponent<Player_Car_Movement>();
            if (carMovement != null)
            {
                StartCoroutine(SlowDownCar(carMovement));
                hasTriggered = true;
            }
        }
    }

    IEnumerator SlowDownCar(Player_Car_Movement carMovement)
    {
        // Lock player controls
        carMovement.LockControls();

        // Gradually slow down to stop
        float elapsed = 0f;
        float startSpeed = carMovement.GetCurrentSpeed();

        while (elapsed < slowDownDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / slowDownDuration;
            carMovement.SetSpeed(Mathf.Lerp(startSpeed, targetSpeed, t));
            yield return null;
        }

        // Ensure fully stopped
        carMovement.SetSpeed(targetSpeed);
    }

    public void ResetTrigger()
    {
        hasTriggered = false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = hasTriggered ? Color.gray : new Color(1f, 0.65f, 0f); // Orange
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
    }
}
