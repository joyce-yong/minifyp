using UnityEngine;

public class g_GameStartTrigger : MonoBehaviour
{
    [SerializeField] g_Girl gameManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (gameManager != null && !gameManager.IsGameStarted)
            {
                gameManager.StartGame();
                GetComponent<Collider>().enabled = false;
                Debug.Log("Game Started!");
            }
        }
    }

    // Reenable
    public void EnableTrigger()
    {
        GetComponent<Collider>().enabled = true;
        Debug.Log("Start Trigger Re-enabled");
    }
}