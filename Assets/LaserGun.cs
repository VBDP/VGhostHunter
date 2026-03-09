using UnityEngine;

public class LaserContinuous : MonoBehaviour
{
    public Transform laserOrigin;        // Empty GameObject en la punta del arma
    public GameObject laserPrefab;       // Prefab con Line Renderer rojo
    public float laserRange = 50f;       // Alcance del láser

    private GameObject currentLaser;     // Instancia actual del láser
    private LineRenderer line;           // Line Renderer actual

    void Update()
    {
        // Trigger derecho presionado
        if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch) > 0.1f)
        {
            if (currentLaser == null)
            {
                // Instanciar láser
                currentLaser = Instantiate(laserPrefab);
                line = currentLaser.GetComponent<LineRenderer>();
            }

            UpdateLaser();
        }
        else
        {
            // Si se suelta el trigger, destruir láser
            if (currentLaser != null)
            {
                Destroy(currentLaser);
                currentLaser = null;
            }
        }
    }

    void UpdateLaser()
    {
        if (line == null) return;

        Vector3 origin = laserOrigin.position;
        Vector3 direction = laserOrigin.forward;
        Vector3 endPoint = origin + direction * laserRange;

        // Raycast para detectar impacto
        if (Physics.Raycast(origin, direction, out RaycastHit hit, laserRange))
        {
            endPoint = hit.point;
        }

        // Actualizar posiciones del Line Renderer
        line.SetPosition(0, origin);
        line.SetPosition(1, endPoint);
    }
}