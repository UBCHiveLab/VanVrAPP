using System;
using UnityEngine;

public class ContentSeparator : MonoBehaviour, IAnnotationContentBlock
{
    public BlockType type => BlockType.SEPARATOR;
    public bool richMedia => false;
    public string title => "Separator";
    public void Populate(ContentBlockData data, AnnotationDetailPanel panel) {
        if (data.type != BlockType.SEPARATOR) {
            throw new Exception("Must be separator block to render separator data");
        }
    }
}
