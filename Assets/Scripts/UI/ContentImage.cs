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
    public Texture2D image;
    public RawImage canvas;
    public string url;
    public Transform homeParent { get; private set; }
    public bool richMedia => true;
    public string title { get; set; }


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

        title = data.title;
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
            image = ((DownloadHandlerTexture)request.downloadHandler).texture;
             

            // Ensures we preserve aspect ratio by locking width and setting size to the w/h ratio of the original.
            float cw = canvas.rectTransform.rect.width;
            float tw = image.width;
            float whRatio = cw / tw;
            float height = image.height * whRatio;
            canvas.rectTransform.sizeDelta = GetConstrainedRect(canvas.rectTransform.rect.width, -1f); //new Vector2(cw, height);

            canvas.texture = image;

        }
    }

    // Set a value to negative to flex
    public Vector2 GetConstrainedRect(float cWidth, float cHeight)
    {
        if (cHeight < 0)
        {
            float tw = image.width;
            float whRatio = cWidth / tw;
            float height = image.height * whRatio;
            return new Vector2(cWidth, height);
        } else if (cWidth < 0)
        {
            float th = image.height;
            float hwRatio = cHeight / th;
            float width = image.width * hwRatio;
            return new Vector2(width, cHeight);
        }

        //TODO: allow for maxs that aren't -1
        return new Vector2(cWidth, cHeight);
    }
}