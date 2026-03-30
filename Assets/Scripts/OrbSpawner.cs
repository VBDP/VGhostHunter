using Meta.XR.MRUtilityKit;
using UnityEngine;

public class OrbSpawner : MonoBehaviour
{
    public float spawnTime = 5f;
    public GameObject orbPrefab;
    public float minEdgeDistance = 0.5f;
    public MRUKAnchor.SceneLabels spawnLabels = MRUKAnchor.SceneLabels.FLOOR; // Default to spawn on the floor
    public float normalOffset = 0.1f;

    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnTime)
        {
            timer = 0;
            SpawnOrb();
        }
    }

    public void SpawnOrb()
    {
        if (MRUK.Instance == null) return;

        MRUKRoom room = MRUK.Instance.GetCurrentRoom();
        if (room == null) return;

        bool hasFoundPosition = room.GenerateRandomPositionOnSurface(MRUK.SurfaceType.FACING_UP, minEdgeDistance,
            LabelFilter.Included(spawnLabels), out Vector3 pos, out Vector3 norm);
        
        if (hasFoundPosition)
        {
            Vector3 spawnPosition = pos + norm * normalOffset;
            Instantiate(orbPrefab, spawnPosition, Quaternion.identity);
        }
    }
}
