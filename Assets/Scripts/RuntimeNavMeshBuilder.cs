using System.Collections;
using Meta.XR.MRUtilityKit;
using UnityEngine;
using Unity.AI.Navigation;

public class RuntimeNavMeshBuilder : MonoBehaviour
{
    private NavMeshSurface surface;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        surface = GetComponent<NavMeshSurface>();
        MRUK.Instance.RegisterSceneLoadedCallback(BuildNavMesh);
    }

    // Update is called once per frame
    private void BuildNavMesh()
    {
        StartCoroutine(BuildNavMeshRoutine());
    }

    public IEnumerator BuildNavMeshRoutine()
    {
        yield return new WaitForEndOfFrame();
        surface.BuildNavMesh();
    }
}
