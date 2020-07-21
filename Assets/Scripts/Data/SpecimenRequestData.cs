using System.Collections.Generic;

[System.Serializable]
public class SpecimenRequestData
{
    public string id;
    public string name;
    public int version;
    public string organ;
    public string assetUrl;
    public string assetUrlWebGl;
    public string meshPath;
    public string matPath;
    public string prefabPath;
    public float scale;
    public AnnotationData[] annotations;

}
