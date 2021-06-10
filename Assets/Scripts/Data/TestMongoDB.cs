using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


public class TestMongoDB : MonoBehaviour
{

    MongoClient client = new MongoClient("mongodb://hive:8afDe1K6XwY1W5cy@van-vr-shard-00-00.zr7vf.mongodb.net:27017,van-vr-shard-00-01.zr7vf.mongodb.net:27017,van-vr-shard-00-02.zr7vf.mongodb.net:27017/test?authSource=admin&replicaSet=atlas-afl4g3-shard-0&readPreference=primary&appname=MongoDB%20Compass&ssl=true");
    IMongoDatabase database;
    IMongoCollection<BsonDocument> _courseCollection;



    // Start is called before the first frame update
    void Start()
    {
        database = client.GetDatabase("vanvr");
        _courseCollection = database.GetCollection<BsonDocument>("courses");

        var projection = Builders<BsonDocument>.Projection.Exclude("_id");
        var course = _courseCollection.Find(new BsonDocument()).Project(projection).ToCursor();
        List<string> _courses = new List<string>();

        foreach (var document in course.ToEnumerable())
        {
            _courses.Add(document["courseId"].ToString());
            Debug.Log(document);
        }


    }


}
