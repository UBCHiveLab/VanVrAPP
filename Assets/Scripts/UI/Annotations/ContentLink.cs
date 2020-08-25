using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class ContentLink : MonoBehaviour, IAnnotationContentBlock
{
#if UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern void OpenNewTab(string url);
#endif
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
#if!UNITY_EDITOR && UNITY_WEBGL

        OpenNewTab(url);
#else
        Application.OpenURL(url);
#endif

    }
}
