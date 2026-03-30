using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

        UpdateTarget();

        if (target == null) return;

        // Movimiento hacia el objetivo
        Vector3 direction = (transform.position - target.position).normalized;
        Vector3 destination = target.position;

        // Si el objetivo es el jugador, mantener distancia. Si es un orbe, ir directo a él.
        if (target == Camera.main.transform)
        {
            destination = target.position + direction * 2f;
        }

        agent.SetDestination(destination);

        // Distancia al objetivo
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
                animator.ResetTrigger("Run");
                animator.ResetTrigger("Attack");
            }
        }
    }

    void UpdateTarget()
    {
        Orb[] orbs = Object.FindObjectsByType<Orb>(FindObjectsSortMode.None);
        if (orbs.Length > 0)
        {
            Orb closestOrb = null;
            float minDistance = Mathf.Infinity;
            foreach (Orb orb in orbs)
            {
                float dist = Vector3.Distance(transform.position, orb.transform.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    closestOrb = orb;
                }
            }
            if (closestOrb != null)
            {
                target = closestOrb.transform;
            }
        }
        else
        {
            if (Camera.main != null)
                target = Camera.main.transform;
            else
                target = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isDead) return;

        Orb orb = other.GetComponent<Orb>();
        if (orb != null)
        {
            Destroy(orb.gameObject);
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
        List<Material> mats = new List<Material>();

        foreach (Renderer renderer in renderers)
        {
            if (renderer.material != null)
            {
                mats.Add(renderer.material);
                // Intento de poner el modo transparente si el shader lo soporta
                if (renderer.material.HasProperty("_Mode"))
                    renderer.material.SetFloat("_Mode", 2);
            }
        }

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / duration);
            foreach (Material mat in mats)
            {
                if (mat == null) continue;

                // Soporte para shader estándar (_Color) y shader graph (_BaseColor)
                if (mat.HasProperty("_Color"))
                {
                    Color c = mat.color;
                    c.a = alpha;
                    mat.color = c;
                }
                else if (mat.HasProperty("_BaseColor"))
                {
                    Color c = mat.GetColor("_BaseColor");
                    c.a = alpha;
                    mat.SetColor("_BaseColor", c);
                }
            }
            yield return null;
        }

        Destroy(gameObject);
    }
}