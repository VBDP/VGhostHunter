using Meta.XR.MRUtilityKit;
using UnityEngine;

public class GhostSpawner : MonoBehaviour
{
 public float spawnTime = 1;
    public GameObject prefabToSpawn;
    public float minEdgeDistance = 0.3f;
    public MRUKAnchor.SceneLabels spawnLabels;
    public float normalOffset;

    private float timer = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
 void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnTime)
        {
            timer = 0;
            SpawnGhost();
        }
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
        }
    }
}