using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContentVideo : MonoBehaviour, IAnnotationContentBlock
{
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
        if (detailPanel.currentVideo == this && detailPanel.videoPlayer.isPlaying && !_scrubbing)
        {
            Progress.value = (float) (detailPanel.videoPlayer.time / detailPanel.videoPlayer.length);
        }
    }

    public void Populate(string url, string title, AnnotationDetailPanel panel) {
        this.url = url;
        this.title = title;
        detailPanel = panel;
        label.text = title;
    }

    /**
     * Scrubbing toggles called onDrag of the slider
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
        detailPanel.VideoPlayed(this);
    }

    private void PauseVideo()
    {
        detailPanel.VideoPaused(this);
    }

    private void ScrubVideo(float val)
    {
        if (_scrubbing)
        {
            detailPanel.VideoScrubbed(this, val);
            Progress.value = val;
        }
    }

    private void ToggleFullScreen()
    {
        detailPanel.ToggleFullScreen(this);
    }

}