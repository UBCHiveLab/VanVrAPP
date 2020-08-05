using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class ContentAudio : MultimediaContent, IAnnotationContentBlock
{
    public BlockType type => BlockType.AUDIO;

    public string src;
    public AudioClip clip;
    public string title { get; set; }
    public bool loaded;

    public TextMeshProUGUI citationLabel;
    public TextMeshProUGUI titleLabel;

    public void Populate(ContentBlockData data, AnnotationDetailPanel panel)
    {
        if (data.type != BlockType.AUDIO)
        {
            throw new Exception("Must be audioToggle block to render audioToggle data");
        }

        contentBlock = this;
        src = data.content;
        detailPanel = panel;
        title = data.title;


        if (data.cite == "") {
            citationLabel.gameObject.SetActive(false);
        } else {
            citationLabel.text = data.cite;
        }

        if (data.title == "") {
            titleLabel.gameObject.SetActive(false);
        } else {
            titleLabel.text = data.title;
        }
    }

    protected override void PrepareContent() {
        if (src != null) {
            StartCoroutine(DownloadAudio(src));
        } else {
            gameObject.SetActive(false);
        }
    }

    protected override void UpdateTime()
    {
        if (detailPanel.audioSource.clip != null && detailPanel.currentAVSource == this && detailPanel.audioSource.isPlaying && !scrubbing)
        {
            timeLabel.text = $"{toTime(detailPanel.audioSource.time)} / {toTime(detailPanel.audioSource.clip.length)}";
            progress.value = (float)(detailPanel.audioSource.time / detailPanel.audioSource.clip.length);
        }
    }

    private IEnumerator DownloadAudio(string url)
    {
        playHover.Disable();
        play.interactable = false;
        loaded = false;
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(src, AudioType.UNKNOWN))
        {
            UnityWebRequestAsyncOperation request = www.SendWebRequest();
            timeLabel.text = "Loading" + string.Concat(Enumerable.Repeat(".", (int)Time.time % 3));
            yield return request;

            if (www.isNetworkError || www.isHttpError)
            {
                detailPanel.SendError(www.error);
                timeLabel.text = "Error!";
            }
            else
            {
                try
                {
                    clip = DownloadHandlerAudioClip.GetContent(www);
                    timeLabel.text = $"0:00 / {toTime(clip.length)}";
                    playHover.Enable();
                    loaded = true;
                    play.interactable = true;

                    detailPanel.fullScreenPlayer.ContentDownloaded(this);

                }
                catch (Exception e)
                {
                    timeLabel.text = "Error" + e;
                }

            }
        }
    }

}
