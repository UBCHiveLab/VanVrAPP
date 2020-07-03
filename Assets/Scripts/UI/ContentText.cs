using System;
using TMPro;
using UnityEngine;

public class ContentText : MonoBehaviour, IAnnotationContentBlock
{
    public BlockType type => BlockType.TEXT;
    public TextMeshProUGUI tmp;
    public Transform homeParent { get; private set; }

    public void Populate(ContentBlockData data, AnnotationDetailPanel panel)
    {
        if (data.type != BlockType.TEXT) {
            throw new Exception("Must be text block to render text data");
        }
        tmp.text = data.content;
        homeParent = panel.contentTransform;
    }
}