using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ContentImage : MonoBehaviour, IAnnotationContentBlock
{
    public Button button;
    public AnnotationDetailPanel detailPanel;
    public RawImage canvas;
    public string url;



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


    public void Populate(string url, AnnotationDetailPanel panel)
    {

        this.url = url;
        this.detailPanel = panel;
    }


    private IEnumerator DownloadImage(string url) {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
            canvas.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
    }
}
