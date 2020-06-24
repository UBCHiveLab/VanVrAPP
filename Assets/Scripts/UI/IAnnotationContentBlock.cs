using UnityEngine;

public interface IAnnotationContentBlock
{

    void Populate(string text, AnnotationDetailPanel panel);
    GameObject gameObject { get; }
}
