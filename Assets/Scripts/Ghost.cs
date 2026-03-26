using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Ghost : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform target;
    private Animator animator;

    private bool isDead = false;

    [Header("Distancia de animación")]
    public float attackDistance = 2f;  // Muy cerca del jugador → atacar
    public float runDistance = 5f;     // Rango de correr

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        if (Camera.main != null)
            target = Camera.main.transform;
    }

    void Update()
    {
        if (isDead) return;
        if (agent == null || !agent.enabled || !agent.isOnNavMesh) return;
        if (target == null) return;

        // Movimiento hacia el jugador
        Vector3 direction = (transform.position - target.position).normalized;
        Vector3 destination = target.position + direction * 2f;
        agent.SetDestination(destination);

        // Distancia al jugador
        float distance = Vector3.Distance(transform.position, target.position);

        if (animator != null)
        {
            if (distance <= attackDistance)
            {
                animator.SetTrigger("Attack");
            }
            else if (distance <= runDistance)
            {
                animator.SetTrigger("Run");
            }
            else
            {
                // Opcional: Idle si está muy lejos
                animator.ResetTrigger("Run");
                animator.ResetTrigger("Attack");
            }
        }
    }

    public void Kill()
    {
        if (isDead) return;
        isDead = true;

        // Detener movimiento
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        // Animación de muerte
        if (animator != null)
            animator.SetTrigger("Death");

        // Iniciar fade out visual mientras suena la animación
        StartCoroutine(FadeOutAndDestroy(1.2f));
    }

    private IEnumerator FadeOutAndDestroy(float duration)
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        Material[] mats = new Material[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            mats[i] = renderers[i].material;
            mats[i].SetFloat("_Mode", 2); // Transparent
            Color c = mats[i].color;
            c.a = 1f;
            mats[i].color = c;
        }

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / duration);
            foreach (Material mat in mats)
            {
                Color c = mat.color;
                c.a = alpha;
                mat.color = c;
            }
            yield return null;
        }

        Destroy(gameObject);
    }
}