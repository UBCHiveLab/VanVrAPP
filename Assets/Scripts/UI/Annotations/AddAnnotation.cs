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
    
    [SerializeField]
    private AnnotationDatas datas;
    [SerializeField]
    private Transform referenceT;
    string path = "/Resources/";
    public string specimenName="";

    public GameObject annotationPlaceHolder;

    private void Reset()
    {
        referenceT = this.transform;
    }
    public void AddAnnotations(string id, string title,string contents, Vector3 position)
    {
        GameObject placeholder = Instantiate(annotationPlaceHolder, position, Quaternion.identity, this.transform);
        AnnotationPlaceHolderData placeHolderData = placeholder.GetComponent<AnnotationPlaceHolderData>();
        placeHolderData.id = id;
        placeHolderData.title = title;
        placeHolderData.contents = contents;
        AnnotationNullablePosition annotationPosition = new AnnotationNullablePosition();
        referenceT = referenceT == null ? this.transform : referenceT;
        Vector3 labelLocalPosition = referenceT.InverseTransformPoint(position);
        annotationPosition.x = labelLocalPosition.x;
        annotationPosition.y = labelLocalPosition.y;
        annotationPosition.z = labelLocalPosition.z;
        AnnotationData _data = new AnnotationData(id, title, contents, annotationPosition);
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
            referenceT = referenceT == null ? this.transform : referenceT;
            Vector3 labelLocalPosition = referenceT.InverseTransformPoint(placeHolderData.transform.position);
            annotationPosition.x = labelLocalPosition.x;
            annotationPosition.y = labelLocalPosition.y;
            annotationPosition.z = labelLocalPosition.z;
            datas.annotationDatas.Add(new AnnotationData(placeHolderData.id, placeHolderData.title, placeHolderData.contents, annotationPosition));
        }
    }
    public void LoadAnnotationData(string path)
    {
        specimenName = System.IO.Path.GetFileNameWithoutExtension(path);
        string json = System.IO.File.ReadAllText(path);
        datas = JsonUtility.FromJson<AnnotationDatas>(json);
        foreach(AnnotationData data in datas.annotationDatas)
        {
           
            if (data.positionVector3 != null)
            {
                Vector3 postion = data.positionVector3.Value;
                GameObject annotation = Instantiate(annotationPlaceHolder, referenceT.transform.InverseTransformPoint(postion), Quaternion.identity, this.transform);
                AnnotationPlaceHolderData annotationData = annotation.GetComponent<AnnotationPlaceHolderData>();
                annotationData.id = data.annotationId;
                annotationData.title = data.title;
                annotationData.contents = data.content;
            }
            
        }
        
    }
}
