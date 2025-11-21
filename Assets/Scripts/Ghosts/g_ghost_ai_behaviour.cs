using UnityEngine;

public class g_ghost_ai_behaviour : MonoBehaviour
{
    public Transform ghostMesh;

    public float wanderRadius = 10f;
    public float walkSpeed = 0.8f;
    public float runSpeed = 6f;
    public float minWalkTime = 2f;
    public float maxWalkTime = 5f;
    public float minIdleTime = 1f;
    public float maxIdleTime = 3f;
    public float rotationSpeed = 120f;
    public float chaseRotationSpeed = 300f;

    public float detectionRadius = 8f;
    public float chaseWaitTime = 1.5f;
    public float pushBackForce = 5f;
    public int damageAmount = 3;

    private Animator anim;
    private Vector3 targetPos;
    private float stateTimer;
    private bool isWalking;
    private bool isChasing;
    private float fixedY;
    private Vector3 spawnPos;
    private Quaternion targetRotation;
    private Vector3 meshStartPos;
    private Transform player;
    private bool waitingToChase;

    void Start()
    {
        if (ghostMesh != null)
        {
            anim = ghostMesh.GetComponent<Animator>();
            meshStartPos = ghostMesh.localPosition;
        }

        fixedY = transform.position.y;
        spawnPos = transform.position;

        if (ghostMesh != null)
        {
            targetRotation = ghostMesh.rotation;
        }

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        PickNewTarget();
        SetWalking();
    }

    void Update()
    {
        if (ghostMesh == null || anim == null) return;

        if (player != null && !isChasing && !waitingToChase)
        {
            float distToPlayer = Vector3.Distance(transform.position, player.position);
            if (distToPlayer < detectionRadius)
            {
                StartChaseSequence();
            }
        }

        if (waitingToChase)
        {
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            dirToPlayer.y = 0f;
            if (dirToPlayer.magnitude > 0.01f)
            {
                Quaternion lookRot = Quaternion.LookRotation(dirToPlayer);
                ghostMesh.rotation = Quaternion.RotateTowards(ghostMesh.rotation, lookRot, chaseRotationSpeed * Time.deltaTime);
            }
        }

        stateTimer -= Time.deltaTime;

        if (stateTimer <= 0f)
        {
            if (waitingToChase)
            {
                StartChase();
            }
            else if (isChasing)
            {

            }
            else if (isWalking)
            {
                SetIdle();
            }
            else
            {
                PickNewTarget();
                SetWalking();
            }
        }

        if (isChasing)
        {
            if (player != null)
            {
                Vector3 dirToPlayer = (player.position - transform.position).normalized;
                dirToPlayer.y = 0f;

                if (dirToPlayer.magnitude > 0.01f)
                {
                    Quaternion lookRot = Quaternion.LookRotation(dirToPlayer);
                    ghostMesh.rotation = Quaternion.RotateTowards(ghostMesh.rotation, lookRot, chaseRotationSpeed * Time.deltaTime);
                }

                Vector3 movement = ghostMesh.forward * runSpeed * Time.deltaTime;
                movement.y = 0f;
                transform.position += movement;

                float distToPlayer = Vector3.Distance(transform.position, player.position);
                if (distToPlayer < 1.5f)
                {
                    HitPlayer();
                }
            }
        }
        else if (isWalking)
        {
            ghostMesh.rotation = Quaternion.RotateTowards(ghostMesh.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            Vector3 movement = ghostMesh.forward * walkSpeed * Time.deltaTime;
            movement.y = 0f;
            transform.position += movement;

            float dist = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z),
                                         new Vector3(targetPos.x, 0, targetPos.z));

            if (dist < 0.5f)
            {
                PickNewTarget();
            }
        }

        Vector3 pos = transform.position;
        pos.y = fixedY;
        transform.position = pos;

        Vector3 localPos = ghostMesh.localPosition;
        localPos.x = meshStartPos.x;
        localPos.z = meshStartPos.z;
        ghostMesh.localPosition = localPos;
    }

    void StartChaseSequence()
    {
        waitingToChase = true;
        isWalking = false;
        isChasing = false;

        if (anim != null)
        {
            anim.SetBool("isWalking", false);
            anim.SetBool("isRunning", false);
        }

        stateTimer = chaseWaitTime;
    }

    void StartChase()
    {
        waitingToChase = false;
        isChasing = true;
        isWalking = false;

        if (anim != null)
        {
            anim.SetBool("isWalking", false);
            anim.SetBool("isRunning", true);
        }

        stateTimer = 999f;
    }

    void HitPlayer()
    {
        if (player != null)
        {
            g_PlayerHealth playerHealth = player.GetComponent<g_PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);

                Vector3 pushDir = (player.position - transform.position).normalized;
                pushDir.y = 0f;

                CharacterController cc = player.GetComponent<CharacterController>();
                if (cc != null)
                {
                    cc.Move(pushDir * pushBackForce * Time.deltaTime);
                }
                else
                {
                    Rigidbody rb = player.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.AddForce(pushDir * pushBackForce, ForceMode.Impulse);
                    }
                }
            }
        }

        Destroy(gameObject);
    }

    void PickNewTarget()
    {
        Vector2 randomCircle = Random.insideUnitCircle * wanderRadius;
        targetPos = spawnPos + new Vector3(randomCircle.x, 0f, randomCircle.y);
        targetPos.y = fixedY;

        Vector3 dir = (targetPos - transform.position).normalized;
        dir.y = 0f;
        if (dir.magnitude > 0.01f)
        {
            targetRotation = Quaternion.LookRotation(dir);
        }
    }

    void SetWalking()
    {
        isWalking = true;
        if (anim != null)
        {
            anim.SetBool("isWalking", true);
            anim.SetBool("isRunning", false);
        }
        stateTimer = Random.Range(minWalkTime, maxWalkTime);
    }

    void SetIdle()
    {
        isWalking = false;
        isChasing = false;
        waitingToChase = false;
        if (anim != null)
        {
            anim.SetBool("isWalking", false);
            anim.SetBool("isRunning", false);
        }
        stateTimer = Random.Range(minIdleTime, maxIdleTime);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        if (isChasing)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 1.5f);
        }
        else if (waitingToChase)
        {
            Gizmos.color = new Color(1f, 0.5f, 0f);
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }
}
