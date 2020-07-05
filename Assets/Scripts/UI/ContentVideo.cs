using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContentVideo : MonoBehaviour, IAnnotationContentBlock
{
    public BlockType type => BlockType.VIDEO;

    [Header("External Structure")]
    public AnnotationDetailPanel detailPanel;

    [Header("Internal Structure")]
    public RawImage canvas;
    public TextMeshProUGUI label;
    public Button Play;
    public Button Pause;
    public Slider Progress;
    public Button FullScreen;

    [Header("Data")]
    public string url;
    public string title;
    public bool youtube;

    private bool _scrubbing;
    

    void Start()
    {
        Play.onClick.AddListener(PlayVideo);
        Pause.onClick.AddListener(PauseVideo);
        Progress.onValueChanged.AddListener(ScrubVideo);
        FullScreen.onClick.AddListener(ToggleFullScreen);
    }
    
    void Update()
    {
        if (detailPanel.currentAVSource == this && detailPanel.videoPlayer.isPlaying && !_scrubbing)
        {
            Progress.value = (float) (detailPanel.videoPlayer.time / detailPanel.videoPlayer.length);
        }
    }

    public void Populate(ContentBlockData data, AnnotationDetailPanel panel) {
        if (data.type != BlockType.VIDEO) {
            throw new Exception("Must be video block to render video data");
        }


        url = data.content;
        if (url.Contains("youtube.com")) {
            youtube = true;
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
        ScrubVideo(Progress.value);
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
            Progress.value = val;
        }
    }

    private void ToggleFullScreen()
    {
        detailPanel.ToggleFullScreen(this);
    }

}