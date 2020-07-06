using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;

public class ContentVideo : MonoBehaviour, IAnnotationContentBlock
{
    public BlockType type => BlockType.VIDEO;

    [Header("External Structure")]
    public AnnotationDetailPanel detailPanel;

    [Header("Internal Structure")]
    public RawImage canvas;
    public TextMeshProUGUI label;
    public Button play;
    public Button pause;
    public Slider progress;
    public TextMeshProUGUI timeLabel;
    public Button fullScreen;
    public YoutubePlayer youtubePlayer;

    [Header("Data")]
    public string url;
    public string title;
    public bool youtube;
    public Texture2D thumbnail;

    private bool _scrubbing;
    

    void Start()
    {
        play.onClick.AddListener(PlayVideo);
        pause.onClick.AddListener(PauseVideo);
        progress.onValueChanged.AddListener(ScrubVideo);
        fullScreen.onClick.AddListener(ToggleFullScreen);
        timeLabel.text = "";
        if (youtube)
        {
            StartCoroutine(LoadThumbnail(ExtractVideoId(url)));
        }
    }
    
    void Update()
    {
        if (detailPanel.currentAVSource == this && detailPanel.videoPlayer.isPlaying && !_scrubbing)
        {
            progress.value = (float) (detailPanel.videoPlayer.time / detailPanel.videoPlayer.length);
            timeLabel.text = $"{toTime((float) detailPanel.videoPlayer.time)} / {toTime((float) detailPanel.videoPlayer.length)}";

        }
    }

    public void Populate(ContentBlockData data, AnnotationDetailPanel panel) {
        if (data.type != BlockType.VIDEO) {
            throw new Exception("Must be video block to render video data");
        }


        url = data.content;
        if (url.Contains("youtube.com")) {
            youtube = true;
            youtubePlayer.youtubeUrl = url;
            youtubePlayer.videoPlayer = panel.videoPlayer;
        }
        title = data.title;
        detailPanel = panel;
        label.text = title;
    }


    /**
     * Scrubbing toggles called onDrag of the progress
     */
    public void StartScrub() {
        _scrubbing = true;
    }

    public void EndScrub() {
        _scrubbing = false;
        ScrubVideo(progress.value);
    }

    private void PlayVideo()
    {
        detailPanel.Play(this);
    }

    private void PauseVideo()
    {
        detailPanel.Pause(this);
    }

    private void ScrubVideo(float val)
    {
        if (_scrubbing)
        {
            detailPanel.Scrub(this, val);
            progress.value = val;
        }
    }

    private void ToggleFullScreen()
    {
        detailPanel.ToggleFullScreen(this);
    }

    private IEnumerator LoadThumbnail(string id)
    {
        UnityWebRequest req = UnityWebRequestTexture.GetTexture("https://img.youtube.com/vi/" + id + "/0.jpg");
        yield return req.SendWebRequest();
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

    private string toTime(float input) {
        string mins = Mathf.FloorToInt(input / 60f).ToString();
        string secs = Mathf.FloorToInt(input % 60).ToString();
        if (secs.Length == 1) secs = "0" + secs;

        return $"{mins}:{secs}";
    }



}