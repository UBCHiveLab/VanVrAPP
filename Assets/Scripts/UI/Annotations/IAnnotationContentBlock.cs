using UnityEngine;

public interface IAnnotationContentBlock
{
    GameObject gameObject { get; }
    BlockType type { get; }
    bool richMedia { get; } // If true, will show up in full screen pager.
    string title { get; }

    void Populate(ContentBlockData data, AnnotationDetailPanel panel);

}
