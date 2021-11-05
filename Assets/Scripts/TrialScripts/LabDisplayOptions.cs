using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/**
 * A UI representation of a single lab for the CoursesPage
 */
public class LabDisplayOptions : MonoBehaviour
{
    [Header("Internal Structure")]
    public Button button;
    public RawImage imageFrame;
    public TextMeshProUGUI idFrame;
    public TextMeshProUGUI nameFrame;

    private CoursesPage _coursesPage;

    public void Populate(LabData data, CoursesPage courses)
    {
      //  StartCoroutine(DownloadImage(data.imgUrl));
        idFrame.text = $"Lab {data.labId}";
        nameFrame.text = data.labName;
        _coursesPage = courses;
        Debug.Log("At Labs Populate function");
        button.onClick.AddListener(() => { _coursesPage.LabSelected(data.labId, data.labName, data.imgUrl); });
        Debug.Log("labs review button pressed");
    }


    private IEnumerator DownloadImage(string url)
    {
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
