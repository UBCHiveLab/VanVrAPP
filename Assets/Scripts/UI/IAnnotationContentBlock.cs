using UnityEngine;

public interface IAnnotationContentBlock
{
    GameObject gameObject { get; }
    BlockType type { get; }
    void Populate(ContentBlockData data, AnnotationDetailPanel panel);
}
