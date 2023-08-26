//using MongoDB.Bson;
//using MongoDB.Driver;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Linq;
using Debug = UnityEngine.Debug;

/**
 * Extends DataLoader. For fetching a manifest located at manifestPath as an http resource.
 */
public class RemoteDataLoader : DataLoader
{

    //MongoDB
    //MongoClient client = new MongoClient("mongodb://hive:8afDe1K6XwY1W5cy@van-vr-shard-00-00.zr7vf.mongodb.net:27017,van-vr-shard-00-01.zr7vf.mongodb.net:27017,van-vr-shard-00-02.zr7vf.mongodb.net:27017/test?authSource=admin&replicaSet=atlas-afl4g3-shard-0&readPreference=primary&appname=MongoDB%20Compass&ssl=true");
    //IMongoDatabase database;

    // http request
    private string specimenUrl = "https://vvr-server-dbp.azurewebsites.net/specimens/get-specimens";
    private string regionUrl = "https://vvr-server-dbp.azurewebsites.net/specimens/get-regions";
    private string labCourseUrl = "https://vvr-server-dbp.azurewebsites.net/labs/get-labs-by-courses";

    //locl endpoints
    //private string specimenUrl = "http://localhost:8080/specimens/get-specimens";
    //private string regionUrl = "http://localhost:8080/specimens/get-regions";
    //private string labCourseUrl = "http://localhost:8080/labs/get-labs-by-courses";



    string otherSchool(string schoolName)
    {
        if(schoolName == "English")
        {
            return "";
        }else if(schoolName == "Spanish")
        {
            return "-esmx";
        }
        else
        {
            return "";
        }
    }

    string fixRegionJson(string value)
    {
        value = "{\"regions\":" + value + "}";
        return value;
    }

    string fixSpecimenJson(string value)
    {
        value = "{\"specimenData\":" + value + "}";
        return value;
    }
    string fixLabCourseJson(string value)
    {
        value = "{\"labCourses\":" + value + "}";
        return value;
    }

    public static class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.Items;
        }

        public static string ToJson<T>(T[] array)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper);
        }

        public static string ToJson<T>(T[] array, bool prettyPrint)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }
    }


    [Serializable]
    public class RegionData2
    {
        public RegionData[] regions;
    }

    [Serializable]
    public class SpecimenRequestData2
    {
        public SpecimenRequestData[] specimenData;
    }

    [Serializable]
    public class CourseData2
    {
        public CourseData[] labCourses;
    }

    protected override IEnumerator LoadManifest()
    {
        /*
        using (UnityWebRequest req =
            UnityWebRequest.Get(manifestPath))
        {
            req.SetRequestHeader("Cache-Control", "max-age=0, no-cache, no-store");
            req.SetRequestHeader("Pragma", "no-cache");
            yield return req.SendWebRequest();
            if (req.isNetworkError || req.isHttpError)
            {
                SendError($"Unable to get manifest. Please check your internet connection or contact the administrator.");
            } else
            {
                manifest = JsonUtility.FromJson<DataManifest>(req.downloadHandler.text);
                manifestLoaded = true;
            }
        }*/


        manifest = new DataManifest();


        Debug.Log("in remoteLoader"+school);

        if (otherSchool(school).Length > 0)
        {
            labCourseUrl = labCourseUrl + otherSchool(school);
            specimenUrl = specimenUrl + otherSchool(school);
            regionUrl = regionUrl + otherSchool(school);
            Debug.Log(labCourseUrl);
            Debug.Log(specimenUrl);
            Debug.Log(regionUrl);

        }


        UnityWebRequest _labCourseReq = UnityWebRequest.Get(labCourseUrl);
        yield return _labCourseReq.SendWebRequest();

        UnityWebRequest _specimenReq = UnityWebRequest.Get(specimenUrl);
        yield return _specimenReq.SendWebRequest();

        UnityWebRequest _regionReq = UnityWebRequest.Get(regionUrl);
        yield return _regionReq.SendWebRequest();

        if (_regionReq.isNetworkError || _regionReq.isHttpError) Debug.Log(_regionReq.error);
        if (_specimenReq.isNetworkError || _specimenReq.isHttpError) Debug.Log(_specimenReq.error);
        if (_labCourseReq.isNetworkError || _labCourseReq.isHttpError) Debug.Log(_labCourseReq.error);


        var specimenCollections = JsonUtility.FromJson<SpecimenRequestData2>(fixSpecimenJson(_specimenReq.downloadHandler.text));
        var regionCollections = JsonUtility.FromJson<RegionData2>(fixRegionJson(_regionReq.downloadHandler.text));
        var labCourseCollections = JsonUtility.FromJson<CourseData2>(fixLabCourseJson(_labCourseReq.downloadHandler.text));



        manifest.regions = new RegionData[regionCollections.regions.Length];
        manifest.regions = regionCollections.regions;
        //Debug.Log(specimenCollections.specimenData.Length);
        manifest.specimenData = new SpecimenRequestData[specimenCollections.specimenData.Length];
        manifest.specimenData = specimenCollections.specimenData;

        manifest.labCourses = new CourseData[labCourseCollections.labCourses.Length];
        manifest.labCourses = labCourseCollections.labCourses;

        //Debug.Log(manifest.specimenData.Length);
        //Debug.Log(manifest.labCourses.Length);
        //Debug.Log(manifest.regions.Length);
        

        manifestLoaded = true;
        yield break;
    }

    private void LoadManifestFromMongoDB()
    {
        /*
        IMongoCollection<BsonDocument> specimenCollection, courseCollection, regionCollection, labsCollection;
        database = client.GetDatabase("vanvr");
        specimenCollection = database.GetCollection<BsonDocument>("specimens");
        courseCollection = database.GetCollection<BsonDocument>("courses");
        regionCollection = database.GetCollection<BsonDocument>("regions");
        labsCollection = database.GetCollection<BsonDocument>("labs");

        var filter = Builders<BsonDocument>.Filter.Empty;

        var specimens = specimenCollection.Find(filter).ToList();
        var courses = courseCollection.Find(filter).ToList();
        var regions = regionCollection.Find(filter).ToList();

        manifest = new DataManifest();

        
        manifest.specimenData = new SpecimenRequestData[specimens.Count];
        for (int i = 0; i < specimens.Count; i++)
        {

            //Populate all fields
            manifest.specimenData[i] = new SpecimenRequestData();
            manifest.specimenData[i].id = specimens[i].GetValue("id").AsString;
            manifest.specimenData[i].name = specimens[i].GetValue("name").AsString;
            if (specimens[i].Contains("version")) manifest.specimenData[i].version = specimens[i].GetValue("version").AsInt32;
            if (specimens[i].Contains("organ")) manifest.specimenData[i].organ = specimens[i].GetValue("organ").AsString;
            if (specimens[i].Contains("assetUrl")) manifest.specimenData[i].assetUrl = specimens[i].GetValue("assetUrl").AsString;
            if (specimens[i].Contains("assetUrlWebGl")) manifest.specimenData[i].assetUrlWebGl = specimens[i].GetValue("assetUrlWebGl").AsString;
            if (specimens[i].Contains("assetUrlOsx")) manifest.specimenData[i].assetUrlOsx = specimens[i].GetValue("assetUrlOsx").AsString;
            if (specimens[i].Contains("altAssetUrl")) manifest.specimenData[i].altAssetUrl = specimens[i].GetValue("altAssetUrl").AsString;
            if (specimens[i].Contains("prefabPath")) manifest.specimenData[i].prefabPath = specimens[i].GetValue("prefabPath").AsString;
            if (specimens[i].Contains("scale")) manifest.specimenData[i].scale = (float)specimens[i].GetValue("scale").ToDecimal();
            if (specimens[i].Contains("yPos")) manifest.specimenData[i].yPos = (float)specimens[i].GetValue("yPos").ToDecimal();




            //Now populate SpecimenRequestData.AnnotationData[] array
            if (specimens[i].Contains("annotations"))
            {
                var annotations = specimens[i].GetValue("annotations").AsBsonArray;
                manifest.specimenData[i].annotations = new AnnotationData[annotations.Count];
                for (int y = 0; y < annotations.Count; y++)
                {
                    manifest.specimenData[i].annotations[y] = new AnnotationData(annotations[y].AsBsonDocument.GetValue("annotationId").AsString,
                                                                                 annotations[y].AsBsonDocument.GetValue("title").AsString,
                                                                                 annotations[y].AsBsonDocument.GetValue("content").AsString,
                                                                                 new AnnotationNullablePosition());

                    if (annotations[y].AsBsonDocument.GetValue("position").AsBsonDocument.Contains("global")) manifest.specimenData[i].annotations[y].position.global = annotations[y].AsBsonDocument.GetValue("position").AsBsonDocument.GetValue("global").AsBoolean;
                    if (annotations[y].AsBsonDocument.GetValue("position").AsBsonDocument.Contains("x")) manifest.specimenData[i].annotations[y].position.x = (float)annotations[y].AsBsonDocument.GetValue("position").AsBsonDocument.GetValue("x").ToDecimal(); // .AsDouble;
                    if (annotations[y].AsBsonDocument.GetValue("position").AsBsonDocument.Contains("y")) manifest.specimenData[i].annotations[y].position.y = (float)annotations[y].AsBsonDocument.GetValue("position").AsBsonDocument.GetValue("y").ToDecimal();
                    if (annotations[y].AsBsonDocument.GetValue("position").AsBsonDocument.Contains("z")) manifest.specimenData[i].annotations[y].position.z = (float)annotations[y].AsBsonDocument.GetValue("position").AsBsonDocument.GetValue("z").ToDecimal();

                }
            }

        }
        
        */
        /*
        manifest.labCourses = new CourseData[courses.Count];
        for (int i = 0; i < courses.Count; i++)
        {
            manifest.labCourses[i] = new CourseData();
            manifest.labCourses[i].courseId = courses[i].GetValue("courseId").AsString;

            var labs = courses[i].GetValue("labs").AsBsonArray;
            manifest.labCourses[i].labs = new LabData[labs.Count];

            for (int y = 0; y < labs.Count; y++)
            {
                var filterLab = Builders<BsonDocument>.Filter.Eq("labId", labs[y].AsInt32);
                var filteredLab = labsCollection.Find(filterLab);
                if (filteredLab.FirstOrDefault() != null)
                {
                    var lab = filteredLab.FirstOrDefault().AsBsonDocument;

                    manifest.labCourses[i].labs[y] = new LabData();

                    manifest.labCourses[i].labs[y].labName = lab.GetValue("labName").AsString;
                    manifest.labCourses[i].labs[y].labId = labs[y].AsInt32;
                    manifest.labCourses[i].labs[y].imgUrl = lab.GetValue("imgUrl").AsString;

                    var specimenLists = lab.GetValue("specimenList").AsBsonArray;
                    manifest.labCourses[i].labs[y].specimenList = new string[specimenLists.Count];
                    for (int k = 0; k < specimenLists.Count; k++)
                    {
                        manifest.labCourses[i].labs[y].specimenList[k] = specimenLists[k].AsString;
                    }
                }

            }
        }

        */

        //manifest.regions = new RegionData[regions.Count];

        /*
        for (int i = 0; i < regions.Count; i++)
        {
            manifest.regions[i] = new RegionData();
            manifest.regions[i].name = regions[i].GetValue("name").AsString;
            manifest.regions[i].order = regions[i].GetValue("order").AsInt32;

            var organs = regions[i].GetValue("organs").AsBsonArray;
            manifest.regions[i].organs = new string[organs.Count];
            for (int y = 0; y < organs.Count; y++)
            {
                manifest.regions[i].organs[y] = organs[y].AsString;
            }
        }
        */

    }

}