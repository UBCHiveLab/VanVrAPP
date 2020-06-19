using System;
using UnityEngine;

[Serializable]
public class AnnotationData
{
    public string annotationId;
    public string title;
    public string content;
    public float[] position; // Local position of the annotation on the model

    public AnnotationData(string annotationId, string title, string content, float[] position)
    {
        this.annotationId = annotationId;
        this.title = title;
        this.content = content;
        this.position = position;
    }
}
