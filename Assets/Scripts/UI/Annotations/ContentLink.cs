using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ContentLink : MonoBehaviour, IAnnotationContentBlock
{
    public BlockType type => BlockType.TEXT;
    public TextMeshProUGUI tmp;
    public Button button;
    public Transform homeParent { get; private set; }
    public bool richMedia => false;
    public string title => "Text Content";
    public string url;

    private void Start()
    {
        button.onClick.AddListener(OnLinkClick);
    }
    public void Populate(ContentBlockData data, AnnotationDetailPanel panel)
    {
        if (data.type != BlockType.LINK)
        {
            throw new Exception("Must be text block to render text data");
        }
        url = data.content;
        if (data.title == "")
        {
            tmp.text = url;
        }
        else
        {
            tmp.text = data.title;
        }
        homeParent = panel.contentTransform;
    }
    private void OnLinkClick()
    {
        Application.OpenURL(url);
    }
}
