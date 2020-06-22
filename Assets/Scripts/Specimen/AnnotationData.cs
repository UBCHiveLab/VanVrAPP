using System;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class AnnotationData
{
    public string annotationId;
    public string title;
    public string content;
    public float[] position; // Local position of the annotation on the model
    public bool incorrectPosition;

    public Vector3 positionVector3
    {
        get
        {
            if (position.Length != 3)
            {
                Debug.LogWarning($"Position attribute for annotation id {annotationId} not given as 3-float array. Filling out v3 with 0s or clipping later values.");
                incorrectPosition = true;
                float[] p = {0f, 0f, 0f};
                for (int i = 0; i < position.Length; i++)
                {
                    p[i] = position[i];
                }
            }
            return new Vector3(position[0], position[1], position[2]);
        }
    }

    public AnnotationData(string annotationId, string title, string content, float[] position)
    {
        this.annotationId = annotationId;
        this.title = title;
        this.content = content;
        this.position = position;
    }
}
