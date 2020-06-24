using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class ContentVideo : MonoBehaviour, IAnnotationContentBlock
{
    public Button button;
    public AnnotationDetailPanel detailPanel;
    public RawImage canvas;
    public string url;



    void Start()
    {
        button.onClick.AddListener(Click);

    }

    void Click()
    {
        detailPanel.VideoClicked(this);
    }


    public void Populate(string url, AnnotationDetailPanel panel)
    {
        this.url = url;
        this.detailPanel = panel;
    }
}
