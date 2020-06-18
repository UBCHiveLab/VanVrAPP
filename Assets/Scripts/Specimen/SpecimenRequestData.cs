public class SpecimenRequestData
{
    public string id;
    public string path;
    public string organ;
    public string assetUrl;
    public int version;

    public SpecimenRequestData(string id, string path, string organ, string assetUrl, int version)
    {
        this.id = id;
        this.assetUrl = assetUrl;
        this.path = path;
        this.organ = organ;
        this.version = version;

    }
}
