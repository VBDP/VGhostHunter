using UnityEngine;

public class LaserShooter : MonoBehaviour
{
    public Transform laserOrigin;          // Punto desde donde sale el láser
    public GameObject laserPrefab;         // Prefab del Line Renderer rojo
    public float MaxLineDistance = 50f;         // Alcance del láser

    void Update()
    {
        // Detecta trigger derecho
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
        Vector3 endPoint = origin + direction * MaxLineDistance;

        // Raycast para detectar impacto
        if (Physics.Raycast(origin, direction, out hit, MaxLineDistance))
        {
            endPoint = hit.point;
            Debug.Log("Impacto con: " + hit.collider.name);
        }

        // Instanciar prefab de línea
        GameObject laserInstance = Instantiate(laserPrefab);
        LineRenderer line = laserInstance.GetComponent<LineRenderer>();

        line.SetPosition(0, origin);
        line.SetPosition(1, endPoint);

        // Destruir después de 0.2 segundos
        Destroy(laserInstance, 0.2f);
    }
}