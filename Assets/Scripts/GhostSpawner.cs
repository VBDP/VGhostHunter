using Meta.XR.MRUtilityKit;
using TMPro;
using UnityEngine;

public class GhostSpawner : MonoBehaviour
{
 public float spawnTime = 1;
    public GameObject prefabToSpawn;
    public float minEdgeDistance = 1f;
    public MRUKAnchor.SceneLabels spawnLabels;
    public float normalOffset;

    private float timer = 0;
    public int fantasmas;
    public int matado;
    public TextMeshProUGUI fantasmasRestantesTexto;
     public TextMeshProUGUI fantasmasAsesinadosTexto;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fantasmas = 0;
        matado = 0;
        
        fantasmasRestantesTexto.text = fantasmas.ToString();
        fantasmasAsesinadosTexto.text = matado.ToString();
    }

    // Update is called once per frame
 void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnTime)
        {
            timer = 0;
            SpawnGhost();
            fantasmasRestantesTexto.text = fantasmas.ToString();
        }

                    // Mira hacia la cámara
            transform.LookAt(Camera.main.transform);
            // Opcional: rotar 180° si el texto se ve al revés
            transform.Rotate(0, 180f, 0);
    }

    // Method to spawn a ghost at the spawner's position
   public void SpawnGhost()
    {
        MRUKRoom room = MRUK.Instance.GetCurrentRoom();
        bool hasFoundPosition = room.GenerateRandomPositionOnSurface(MRUK.SurfaceType.VERTICAL, minEdgeDistance,
            LabelFilter.Included(spawnLabels), out Vector3 pos, out Vector3 norm);
        if (hasFoundPosition)
        {
            Vector3 randomPositionNormalOffset = pos + norm*normalOffset;
            randomPositionNormalOffset.y = 0;
            Instantiate(prefabToSpawn, randomPositionNormalOffset, Quaternion.identity);
            fantasmas += 1;
        }
    }

    public void RestarFantasma()
    {
        fantasmas -= 1;
        fantasmasRestantesTexto.text = fantasmas.ToString();
        matado += 1;
        fantasmasAsesinadosTexto.text = matado.ToString();
    }
}