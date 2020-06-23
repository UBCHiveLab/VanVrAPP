using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AnnotationDetailPanel : MonoBehaviour
{
    public TextMeshProUGUI title;
    public TextMeshProUGUI content;

    public void Populate(AnnotationData data)
    {
        title.text = data.title;
        content.text = data.content;
    }
}
