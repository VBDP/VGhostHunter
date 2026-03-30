using UnityEngine;
using System.Collections;

public class LaserShooter : MonoBehaviour
{
    [Header("References")]
    public Transform laserOrigin;
    public GameObject laserPrefab;
    public GameObject impactEffect;
    public GhostSpawner ghostSpawner; // Asignar en Inspector opcionalmente

    [Header("Laser Settings")]
    public float maxDistance = 60f;
    public float laserSpeed = 200f;
    public float fadeTime = 0.25f;

    [Header("Audio")]
    public AudioSource laserAudioSource;
    public AudioClip laserClip;

    void Start()
    {
        // Si no se asignó en el inspector, lo buscamos en la escena
        if (ghostSpawner == null)
        {
            ghostSpawner = Object.FindFirstObjectByType<GhostSpawner>();
            
            // Si después de buscar sigue siendo null, usamos el método antiguo por si acaso
            if (ghostSpawner == null)
            {
                ghostSpawner = GameObject.FindObjectOfType<GhostSpawner>();
            }
        }

        if (ghostSpawner != null)
        {
            Debug.Log("GhostSpawner successfully linked to LaserShooter!");
        }
        else
        {
            Debug.LogError("GhostSpawner NOT found! Please add it to the scene or the LaserShooter inspector.");
        }
    }

    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
        {
            ShootLaser();
        }
    }

    void ShootLaser()
    {
        if (laserOrigin == null || laserPrefab == null) return;

        Vector3 origin = laserOrigin.position;
        Vector3 direction = laserOrigin.forward;
        Vector3 endPoint = origin + direction * maxDistance;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance))
        {
            endPoint = hit.point;

            if (impactEffect != null)
            {
                GameObject impact = Instantiate(
                    impactEffect,
                    hit.point,
                    Quaternion.LookRotation(-hit.normal)
                );
                Destroy(impact, 0.5f);
            }

            Ghost hitGhost = hit.collider.GetComponentInParent<Ghost>();
            if (hitGhost != null)
            {
                Debug.Log("Ghost Hit! Calling Kill.");
                hitGhost.Kill();

                if (ghostSpawner != null)
                {
                    Debug.Log("GhostSpawner found! Calling RestarFantasma.");
                    ghostSpawner.RestarFantasma();
                }
                else
                {
                    Debug.LogWarning("GhostSpawner NOT found in LaserShooter!");
                }
            }
            else
            {
                Debug.Log("Hit something that is not a Ghost: " + hit.collider.name);
            }
        }

        // Crear láser visual
        GameObject laserInstance = Instantiate(laserPrefab);
        if (laserInstance != null)
        {
            LineRenderer line = laserInstance.GetComponent<LineRenderer>();
            if (line != null)
            {
                line.useWorldSpace = true;
                StartCoroutine(AnimateLaser(line, origin, endPoint, laserInstance));
            }
            else
            {
                Destroy(laserInstance);
            }
        }

        // Reproducir sonido
        if (laserAudioSource != null && laserClip != null)
        {
            laserAudioSource.PlayOneShot(laserClip);
        }
    }

    private IEnumerator AnimateLaser(LineRenderer line, Vector3 worldOrigin, Vector3 worldEndPoint, GameObject laserObj)
    {
        float distance = Vector3.Distance(worldOrigin, worldEndPoint);
        float currentDistance = 0;

        line.SetPosition(0, worldOrigin);

        while (currentDistance < distance)
        {
            currentDistance += laserSpeed * Time.deltaTime;
            Vector3 point = Vector3.Lerp(worldOrigin, worldEndPoint, currentDistance / distance);
            line.SetPosition(1, point);
            yield return null;
        }

        line.SetPosition(1, worldEndPoint);

        // Fade out
        float t = 0;
        Color startColor = line.startColor;

        while (t < fadeTime)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, t / fadeTime);
            Color c = startColor;
            c.a = alpha;
            line.startColor = c;
            line.endColor = c;
            yield return null;
        }

        Destroy(laserObj);
    }
}