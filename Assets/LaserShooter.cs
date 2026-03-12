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

    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
        {
            ShootLaser();
        }
    }

    void ShootLaser()
    {
        RaycastHit hit;

        Vector3 origin = laserOrigin.position;
        Vector3 direction = laserOrigin.forward;
        Vector3 endPoint = origin + direction * maxDistance;

        if (Physics.Raycast(origin, direction, out hit, maxDistance))
        {
            endPoint = hit.point;

            // Crear efecto de impacto
            GameObject impact = Instantiate(
                impactEffect,
                hit.point,
                Quaternion.LookRotation(hit.normal)
            );
            Destroy(impact, 1f);
        }

        // Instanciamos el láser como hijo del arma
        GameObject laserInstance = Instantiate(laserPrefab, laserOrigin);
        laserInstance.transform.localPosition = Vector3.zero;
        laserInstance.transform.localRotation = Quaternion.identity;

        LineRenderer line = laserInstance.GetComponent<LineRenderer>();
        line.useWorldSpace = false; // <-- Muy importante

        // Convertimos el punto final a local del arma
        Vector3 localEndPoint = laserOrigin.InverseTransformPoint(endPoint);

        StartCoroutine(AnimateLaser(line, Vector3.zero, localEndPoint, laserInstance));
    }

    IEnumerator AnimateLaser(LineRenderer line, Vector3 localOrigin, Vector3 localEndPoint, GameObject laserObj)
    {
        float distance = Vector3.Distance(localOrigin, localEndPoint);
        float currentDistance = 0;

        line.SetPosition(0, localOrigin);

        while (currentDistance < distance)
        {
            currentDistance += laserSpeed * Time.deltaTime;
            Vector3 point = Vector3.Lerp(localOrigin, localEndPoint, currentDistance / distance);
            line.SetPosition(1, point);
            yield return null;
        }

        line.SetPosition(1, localEndPoint);

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