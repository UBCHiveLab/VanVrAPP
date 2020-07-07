using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ContentAudio : MonoBehaviour, IAnnotationContentBlock
{
    public BlockType type => BlockType.AUDIO;
    public Button playButton;
    public Button pauseButton;
    public TextMeshProUGUI label;
    public string src;
    public AudioClip clip;
    public Slider progress;
    public TextMeshProUGUI timeLabel;
    public AnnotationDetailPanel panel;
    private bool _scrubbing;
    public bool richMedia => true;
    public string title { get; set; }

    public void Populate(ContentBlockData data, AnnotationDetailPanel panel)
    {
        if (data.type != BlockType.AUDIO)
        {
            throw new Exception("Must be audio block to render audio data");
        }

        label.text = data.title;
        src = data.content;
        this.panel = panel;
        title = data.title;


    }

    void Start() {
        playButton.onClick.AddListener(Play);
        pauseButton.onClick.AddListener(Pause);
        if (src != null) {
            StartCoroutine(DownloadAudio(src));
        } else {
            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (panel.currentAVSource == this && panel.audioSource.isPlaying && !_scrubbing)
        {
            timeLabel.text = $"{toTime(panel.audioSource.time)} / {toTime(panel.audioSource.clip.length)}";
            progress.value = (float)(panel.audioSource.time / panel.audioSource.clip.length);
        }
    }

    void Play()
    {
        panel.Play(this);
    }

    void Pause()
    {
        panel.Pause(this);
    }

    public void StartScrub() {
        _scrubbing = true;
    }

    public void EndScrub() {
        _scrubbing = false;
        panel.Scrub(this, progress.value);
    }

    private IEnumerator DownloadAudio(string url)
    {

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(src, AudioType.UNKNOWN))
        {
            UnityWebRequestAsyncOperation request = www.SendWebRequest();
            timeLabel.text = "Loading" + string.Concat(Enumerable.Repeat(".", (int)Time.time % 3));
            yield return request;

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogWarning(www.error);
                timeLabel.text = "Error!";
            }
            else
            {
                clip = DownloadHandlerAudioClip.GetContent(www);
                timeLabel.text = $"0:00 / {toTime(clip.length)}";
            }
        }
    }

    private string toTime(float input)
    {
        string mins = Mathf.FloorToInt(input / 60f).ToString();
        string secs = Mathf.FloorToInt(input % 60).ToString();
        if (secs.Length == 1) secs = "0" + secs;
        
        return $"{mins}:{secs}";
    }
}
