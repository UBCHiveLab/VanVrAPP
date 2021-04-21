using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/**
 * A UI representation of a single lab for the SelectorMenu
 */
public class LabOption : MonoBehaviour
{
    [Header("Internal Structure")]
    public Button button;
    public RawImage imageFrame;
    public TextMeshProUGUI idFrame;
    public TextMeshProUGUI nameFrame;

    private SelectorMenu _selectorMenu;

    public void Populate(LabData data, SelectorMenu menu)
    {
        StartCoroutine(DownloadImage(data.imgUrl));
        idFrame.text = $"Lab {data.labId}";
        nameFrame.text = data.labName;
        _selectorMenu = menu;
      
        button.onClick.AddListener(() => { 
            _selectorMenu.LabSelected(data.labId, data.labName);
            
        });
    }


    private IEnumerator DownloadImage(string url) {
        url = url.Trim();
        if (url.Length > 0)
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
            yield return request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
                Debug.Log(request.error);
            else
                imageFrame.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        }
    }

}
