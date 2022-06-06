/**
* For deserializing courses and their lab data.
*/
[System.Serializable]
public class CourseData
{
    public string courseId;
    public LabData[] labs;
}

[System.Serializable]
public class LabData
{
    public string labName;
    public int labId;
    public string[] specimenList;
    public string imgUrl;
    public Manual[] manuals;
    public string description;
    public string[] links;
}

[System.Serializable]
public class Manual
{
    public string name;
    public string file;
}
