using Meta.XR.MRUtilityKit;
using TMPro;
using UnityEngine;

public class ScoreCanvasSpawner : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public float minEdgeDistance;
    public MRUKAnchor.SceneLabels spawnLabels;
    public float normalOffset;
    private bool spawned = false;


    public void Start()
    {
        MRUK.Instance.RegisterSceneLoadedCallback(SpawnUI);

    }
    public void SpawnUI()
    {
        MRUKRoom room = MRUK.Instance.GetCurrentRoom();


        bool hasFoundPosition = room.GenerateRandomPositionOnSurface(MRUK.SurfaceType.VERTICAL, minEdgeDistance, LabelFilter.Included(spawnLabels), out Vector3 pos, out Vector3 norm);
        if (hasFoundPosition)
        {
            Vector3 randomPositionNormalOffset = pos + norm * normalOffset;
            randomPositionNormalOffset.y = 2;

            Instantiate(prefabToSpawn, randomPositionNormalOffset, Quaternion.identity);
            spawned = true;
            return;
        }
        else
        {
            Debug.Log("Failed to instantiate canvas");
        }

    }

}