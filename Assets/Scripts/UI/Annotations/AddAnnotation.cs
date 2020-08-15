using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnnotationDatas
{
    public List<AnnotationData> annotationDatas;
}
[RequireComponent(typeof(MeshCollider))]
[System.Serializable]
public class AddAnnotation : MonoBehaviour
{
    //[SerializeField]
    //public List<AnnotationData> annotationDatas = new List<AnnotationData>();
    [SerializeField]
    private AnnotationDatas datas;
    [SerializeField]
    private Transform referenceT;
    string path = "/Resources/";
    public string specimenName;

    public GameObject annotationPlaceHolder;

    public void AddAnnotations(string id, string title,Vector3 position)
    {
        GameObject placeholder = Instantiate(annotationPlaceHolder, position, Quaternion.identity, this.transform);
        AnnotationPlaceHolderData placeHolderData = placeholder.GetComponent<AnnotationPlaceHolderData>();
        placeHolderData.id = id;
        placeHolderData.title = title;
        AnnotationNullablePosition annotationPosition = new AnnotationNullablePosition();
        Vector3 labelLocalPosition = referenceT.InverseTransformPoint(position);
        annotationPosition.x = labelLocalPosition.x;
        annotationPosition.y = labelLocalPosition.y;
        annotationPosition.z = labelLocalPosition.z;
        AnnotationData _data = new AnnotationData(id, title, "", annotationPosition);
        datas.annotationDatas.Add(_data);
    }
    public void SaveToJSON()
    { 
        string specimenAnnotationData = JsonUtility.ToJson(datas);
        Debug.Log(specimenAnnotationData);
        System.IO.File.WriteAllText(Application.dataPath + "/Resources/" + specimenName + ".json", specimenAnnotationData);

    }
    public void UpdateData()
    {
        
        AnnotationPlaceHolderData[] annotationPlaceHolderDatas = this.GetComponentsInChildren<AnnotationPlaceHolderData>();
        datas.annotationDatas.Clear();
       
        foreach(AnnotationPlaceHolderData placeHolderData in annotationPlaceHolderDatas)
        {
            AnnotationNullablePosition annotationPosition = new AnnotationNullablePosition();
            Vector3 labelLocalPosition = referenceT.InverseTransformPoint(placeHolderData.transform.position);
            annotationPosition.x = labelLocalPosition.x;
            annotationPosition.y = labelLocalPosition.y;
            annotationPosition.z = labelLocalPosition.z;
            datas.annotationDatas.Add(new AnnotationData(placeHolderData.id, placeHolderData.title, placeHolderData.contents, annotationPosition));
        }
    } 
}
