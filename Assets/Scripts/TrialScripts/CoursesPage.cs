using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoursesPage : MonoBehaviour
{
    MongoClient client = new MongoClient("mongodb://hive:8afDe1K6XwY1W5cy@van-vr-shard-00-00.zr7vf.mongodb.net:27017,van-vr-shard-00-01.zr7vf.mongodb.net:27017,van-vr-shard-00-02.zr7vf.mongodb.net:27017/test?authSource=admin&replicaSet=atlas-afl4g3-shard-0&readPreference=primary&appname=MongoDB%20Compass&ssl=true");
    IMongoDatabase database;
    IMongoCollection<BsonDocument> collection;

    // Start is called before the first frame update
    void Start()
    {
        database = client.GetDatabase("vanvr");
        collection = database.GetCollection<BsonDocument>("courses");
        var courses = collection.Find(new BsonDocument()).FirstOrDefault();

        Debug.Log(courses.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
