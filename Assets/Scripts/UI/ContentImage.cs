using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ContentImage : MonoBehaviour, IAnnotationContentBlock
{
    public BlockType type => BlockType.IMAGE;
    public Button button;
    public AnnotationDetailPanel detailPanel;
    public RawImage canvas;
    public string url;
    public Transform homeParent { get; private set; }


    void Start()
    {
        button.onClick.AddListener(Click);

        if (url != null)
        {
            StartCoroutine(DownloadImage(url));
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    void Click()
    {
        detailPanel.ImageClicked(this);
    }


    public void Populate(ContentBlockData data, AnnotationDetailPanel panel)
    {
        if (data.type != BlockType.IMAGE) {
            throw new Exception("Must be text block to render text data");
        }

        url = data.content;
        detailPanel = panel;
        homeParent = detailPanel.contentTransform;
    }


    private IEnumerator DownloadImage(string url)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
        } else
        {
            Texture2D tex = ((DownloadHandlerTexture)request.downloadHandler).texture;

            // Ensures we preserve aspect ratio by locking width and setting size to the w/h ratio of the original.
            float cw = canvas.rectTransform.rect.width;
            float tw = tex.width;
            float ctRatio = cw / tw;
            float height = tex.height * ctRatio;
            canvas.rectTransform.sizeDelta = new Vector2(cw, height);

            canvas.texture = tex;

        }
    }
}