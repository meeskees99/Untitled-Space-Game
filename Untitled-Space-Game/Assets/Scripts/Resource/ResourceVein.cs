using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceVein : MonoBehaviour
{
    [SerializeField] Resource _resource;
    public Resource Resource
    {
        get { return _resource; }
    }

    public int resourceIndex;

    public bool acceptsMiner;

}
