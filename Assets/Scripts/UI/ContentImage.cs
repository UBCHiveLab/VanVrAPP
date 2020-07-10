using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ContentImage : MultimediaContent, IAnnotationContentBlock
{
    public readonly int DEFAULT_WIDTH = 900;
    public readonly int DEFAULT_HEIGHT = 900;

    public BlockType type => BlockType.IMAGE;
    public Texture2D image;
    public string url;
    public Transform homeParent { get; private set; }
    public string title { get; set; }
    public int width;
    public int height;
    public TextMeshProUGUI titleLabel;
    public TextMeshProUGUI citation;


    protected override void PrepareContent()
    {
        if (url != null)
        {
            StartCoroutine(DownloadImage(url));
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void Populate(ContentBlockData data, AnnotationDetailPanel panel)
    {
        if (data.type != BlockType.IMAGE) {
            throw new Exception("Must be text block to render text data");
        }

        contentBlock = this;
        title = data.title;
        url = data.content;
        if (data.widthHeight == new Vector2(-1, -1)) {
            sizeRect = new Vector2(DEFAULT_WIDTH, DEFAULT_HEIGHT);
        } else {
            sizeRect = data.widthHeight;
        }

        if (data.cite == "") {
            citation.gameObject.SetActive(false);
        } else {
            citation.text = data.cite;
        }

        if (data.title == "")
        {
            titleLabel.gameObject.SetActive(false);
        }
        else
        {
            titleLabel.text = data.title;
        }

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
            image = ((DownloadHandlerTexture)request.downloadHandler).texture;
            sizeRect = new Vector2(image.width, image.height);


            // Ensures we preserve aspect ratio by locking width and setting size to the w/h ratio of the original.
            float cw = canvas.rectTransform.rect.width;
            float tw = image.width;
            float whRatio = cw / tw;
            float height = image.height * whRatio;
            canvas.rectTransform.sizeDelta = GetConstrainedRec(canvas.rectTransform.rect.width, -1f); //new Vector2(cw, height);

            canvas.texture = image;

        }
    }
}