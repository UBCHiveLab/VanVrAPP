using MongoDB.Driver;
using MongoDB.Bson;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System;
using UnityEngine.UI;

/**
 * Extends DataLoader. For fetching a manifest located at manifestPath as an http resource.
 */
public class RemoteDataLoader : DataLoader
{
    //MongoDB
    MongoClient client = new MongoClient("mongodb://hive:8afDe1K6XwY1W5cy@van-vr-shard-00-00.zr7vf.mongodb.net:27017,van-vr-shard-00-01.zr7vf.mongodb.net:27017,van-vr-shard-00-02.zr7vf.mongodb.net:27017/test?authSource=admin&replicaSet=atlas-afl4g3-shard-0&readPreference=primary&appname=MongoDB%20Compass&ssl=true");
    IMongoDatabase database;

    //UI
    public Text courseName;

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

        //Using MongoDB
        LoadManifestFromMongoDB();

        manifestLoaded = true;
        yield break;
    }

    private void LoadManifestFromMongoDB()
    {
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
        manifest.labCourses = new CourseData[courses.Count];

        manifest.labCourses[0] = new CourseData();
        manifest.labCourses[0].courseId = courses[0].GetValue("courseId").AsString;
        courseName.text = manifest.labCourses[0].courseId;

        /*
        for (int i = 0; i < courses.Count; i++)
        {
            manifest.labCourses[i] = new CourseData();
            manifest.labCourses[i].courseId = courses[i].GetValue("courseId").AsString;

        }*/
    }
}
