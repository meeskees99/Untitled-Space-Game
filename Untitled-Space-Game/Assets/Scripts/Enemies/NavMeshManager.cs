using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;
using Unity.VisualScripting;

public class NavMeshManager : MonoBehaviour
{
    public static NavMeshManager Instance;

    [SerializeField] NavMeshSurface[] _navSurfaces;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(this);
    }

    public void UpdateNavMesh()
    {
        _navSurfaces = FindObjectsOfType<NavMeshSurface>();
        for (int i = 0; i < _navSurfaces.Length; i++)
        {
            _navSurfaces[i].BuildNavMesh();
        }
    }
}
