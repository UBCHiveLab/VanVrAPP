using System;

/**
 * For deserializing the root of manifest.json
 */
[Serializable]
public class DataManifest
{

    public SpecimenRequestData[] specimenData;
    public LabData[] labs;
    public RegionData[] regions;

}
