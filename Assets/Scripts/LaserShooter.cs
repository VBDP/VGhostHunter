using UnityEngine;
using System.Collections;

public class LaserShooter : MonoBehaviour
{
    public Transform laserOrigin;
    public GameObject laserPrefab;
    public GameObject impactEffect;

    public float maxDistance = 60f;
    public float laserSpeed = 200f;
    public float fadeTime = 0.25f;

    public AudioSource laserAudioSource;
    public AudioClip laserClip;
    private GhostSpawner ghostSpawner;

    void Start()
    {
        ghostSpawner = FindFirstObjectByType<GhostSpawner>();
    }
    void Update()
    {
        if(ghostSpawner == null)
        {
           ghostSpawner = FindFirstObjectByType<GhostSpawner>(); 
        }
        
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
        {
            ShootLaser();
        }
    }

    void ShootLaser()
    {
        Vector3 origin = laserOrigin.position;
        Vector3 direction = laserOrigin.forward;
        Vector3 endPoint = origin + direction * maxDistance;

        // Raycast desde el mundo
        if (Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance))
        {
            endPoint = hit.point;

            // Efecto de impacto
            if (impactEffect != null)
            {
                GameObject impact = Instantiate(
                    impactEffect,
                    hit.point,
                    Quaternion.LookRotation(-hit.normal)
                );
                Destroy(impact, 0.5f);
            }

            // Destruir enemigo si tiene tag "Enemy"
            if (hit.collider.CompareTag("Enemy"))
            {
                Destroy(hit.collider.gameObject);
                ghostSpawner.RestarFantasma();
            }
        }

        // Instanciamos el láser
        GameObject laserInstance = Instantiate(laserPrefab, laserOrigin);
        laserInstance.transform.localPosition = Vector3.zero;
        laserInstance.transform.localRotation = Quaternion.identity;

        LineRenderer line = laserInstance.GetComponent<LineRenderer>();
        line.useWorldSpace = true; // Ojo: usar espacio mundial para que el raycast coincida con el LineRenderer

        // Reproducir sonido
        if (laserAudioSource != null && laserClip != null)
        {
            laserAudioSource.PlayOneShot(laserClip);
        }

        StartCoroutine(AnimateLaser(line, origin, endPoint, laserInstance));
    }

    IEnumerator AnimateLaser(LineRenderer line, Vector3 worldOrigin, Vector3 worldEndPoint, GameObject laserObj)
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