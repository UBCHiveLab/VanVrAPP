public class SpecimenRequestData
{
    public string id;
    public string path;
    public string organ;
    public string assetUrl;

    public SpecimenRequestData(string id, string path, string organ, string assetUrl)
    {
        this.id = id;
        this.assetUrl = assetUrl;
        this.path = path;
        this.organ = organ;
    }
}
