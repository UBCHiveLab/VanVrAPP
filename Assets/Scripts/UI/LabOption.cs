using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LabOption : MonoBehaviour
{
    [Header("Internal Structure")]
    public Button button;
    public RawImage imageFrame;
    public TextMeshProUGUI idFrame;
    public TextMeshProUGUI nameFrame;

    [Header("Data")]
    public LabData data;

    private SelectorMenu _selectorMenu;

    public void Populate(LabData data, SelectorMenu menu)
    {
        StartCoroutine(DownloadImage(data.imgUrl));
        idFrame.text = data.labId;
        nameFrame.text = data.labName;
        _selectorMenu = menu;
        button.onClick.AddListener(() => { _selectorMenu.LabSelected(data.labId); });
    }


    private IEnumerator DownloadImage(string url) {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
            imageFrame.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
    }

}
