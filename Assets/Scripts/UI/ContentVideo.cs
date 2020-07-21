using System;
using System.Collections;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ContentVideo : MultimediaContent, IAnnotationContentBlock
{
    public readonly int DEFAULT_WIDTH = 1600;
    public readonly int DEFAULT_HEIGHT = 900;

    public BlockType type => BlockType.VIDEO;
    

    [Header("Internal Structure")]
    public YoutubePlayer youtubePlayer;

    public RectTransform transform;
    public LayoutElement layoutEl;
    public TextMeshProUGUI citation;
    public GameObject loadingSpinner;


    [Header("Data")]
    public string url;
    public string title { get; set; }
    public bool youtube;
    public Texture2D thumbnail;

    protected override  void UpdateTime()
    {
        if (detailPanel.currentAVSource == this && detailPanel.videoPlayer.isPlaying && !scrubbing)
        {
            progress.value = (float) (detailPanel.videoPlayer.time / detailPanel.videoPlayer.length);
            timeLabel.text = $"{toTime((float) detailPanel.videoPlayer.time)} / {toTime((float) detailPanel.videoPlayer.length)}";
        }
        else
        {
            timeLabel.text = "Ready";
        }
    }

    protected override void ContentLoaded() {
        loadingSpinner.SetActive(false);
        canvas.gameObject.SetActive(true);
    }

    public void Populate(ContentBlockData data, AnnotationDetailPanel panel) {
        if (data.type != BlockType.VIDEO) {
            throw new Exception("Must be video block to render video data");
        }

        contentBlock = this;
        title = data.title;
        detailPanel = panel;
        label.text = title;
        
        if (data.widthHeight == new Vector2(-1, -1))
        {
            sizeRect = new Vector2(DEFAULT_WIDTH, DEFAULT_HEIGHT);
        } else
        {
            sizeRect = data.widthHeight;
        }

        if (data.cite == "")
        {
            citation.gameObject.SetActive(false);
        }
        else
        {
            citation.text = data.cite;
        }

        url = data.content;
        youtubePlayer.videoPlayer = panel.videoPlayer;

        if (url.Contains("youtube.com")) {
            youtube = true;
            youtubePlayer.youtubeUrl = url;
            youtubePlayer.videoPlayer = panel.videoPlayer;
        }

        if (youtube && gameObject.activeSelf) {
            youtubePlayer.enabled = true;
            StartCoroutine(LoadThumbnail(ExtractVideoId(url)));
        }
        else
        {
            ContentLoaded();
        }

        Vector2 rect = GetConstrainedRec(148, -1);
        canvas.rectTransform.sizeDelta = rect;
        transform.sizeDelta = new Vector2(transform.sizeDelta.x, rect.y + 50f);
        layoutEl.minHeight = rect.y;

    }

    private IEnumerator LoadThumbnail(string id)
    {
        loadingSpinner.SetActive(true);
        canvas.gameObject.SetActive(false);

        UnityWebRequest req = UnityWebRequestTexture.GetTexture("https://img.youtube.com/vi/" + id + "/0.jpg");
        yield return req.SendWebRequest();

        ContentLoaded();

        thumbnail = DownloadHandlerTexture.GetContent(req);
        canvas.texture = thumbnail;
    }

    private string ExtractVideoId(string url) {

        Match match = Regex.Match(url, "https:\\/\\/www\\.youtube\\.com\\/watch\\?v=([^&=\\s]*)");
        if (match.Groups.Count < 1)
        {
            Debug.LogWarning($"Can't find id in youtube url {url}");
            return null;
        }
        string id = match.Groups[1].Value;

        return id;
    }

}