using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecimenData
{

    public string Id;
    public GameObject Prefab;
    public string OrganType;

    public SpecimenData(string id, GameObject prefab, string organType)
    {
        Id = id;
        Prefab = prefab;
        OrganType = organType;
    }
}
