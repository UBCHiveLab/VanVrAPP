using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ContentVideo : MonoBehaviour, IAnnotationContentBlock
{
    public AnnotationDetailPanel detailPanel;
    public RawImage canvas;
    public TextMeshProUGUI label;
    public string url;
    public string title;
    public Transform homeParent { get; private set; }

    public Button Play;
    public Button Pause;
    public Slider Progress;
    public Button FullScreen;

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

    void Click()
    {
        detailPanel.VideoClicked(this);
    }

    void PlayVideo()
    {
        detailPanel.VideoClicked(this);
    }

    void PauseVideo()
    {
        detailPanel.VideoPaused(this);
    }

    void ScrubVideo(float val)
    {
        if (_scrubbing)
        {
            detailPanel.VideoScrubbed(this, val);
            Progress.value = val;
        }
    }

    void ToggleFullScreen()
    {
        detailPanel.ToggleFullScreen(this);
    }

    public void StartScrub()
    {
        _scrubbing = true;
    }

    public void EndScrub()
    {
        _scrubbing = false;
        Debug.Log(Progress.value);
        ScrubVideo(Progress.value);
    }

    public void Populate(string url, string title, AnnotationDetailPanel panel)
    {
        this.url = url;
        this.title = title;
        detailPanel = panel;
        homeParent = detailPanel.contentTransform;
        label.text = title;

    }
}
