using TMPro;
using UnityEngine;

public class ContentText : MonoBehaviour, IAnnotationContentBlock
{
    public TextMeshProUGUI tmp;
    public Transform homeParent { get; private set; }


    public void Populate(string text, AnnotationDetailPanel panel)
    {
        tmp.text = text;
        homeParent = panel.contentTransform;
    }
}