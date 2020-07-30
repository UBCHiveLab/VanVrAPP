using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FullScreenPlayer : MultimediaContent
{

    [Header("Internal Structure")]
    public TextMeshProUGUI title;
    public Button next;
    public Button prev;
    public Button fullScreenToggle;


    protected override void PrepareContent()
    {
        next.onClick.AddListener(Next);
        prev.onClick.AddListener(Prev);
        fullScreenToggle.onClick.AddListener(ToggleFullScreen);
        MaximizeScreenHeight();
    }

    protected override void UpdateTime()
    {
        if (contentBlock == null)
        {
            ToggleFullScreen();
            return;
        }
        if (detailPanel.currentAVSource == contentBlock)
        {
            if (contentBlock.type == BlockType.VIDEO && detailPanel.videoPlayer.isPlaying && !scrubbing && detailPanel.videoPlayer.length > 0)
            {
                progress.value = (float)(detailPanel.videoPlayer.time / detailPanel.videoPlayer.length);
                timeLabel.text = $"{toTime((float)detailPanel.videoPlayer.time)} / {toTime((float)detailPanel.videoPlayer.length)}";
            } else if (contentBlock.type == BlockType.AUDIO && detailPanel.audioSource.isPlaying && !scrubbing && detailPanel.audioSource.clip != null && detailPanel.audioSource.clip.length > 0)
            {
                progress.value = (float)(detailPanel.audioSource.time / detailPanel.audioSource.clip.length);
                timeLabel.text = $"{toTime((float)detailPanel.audioSource.time)} / {toTime((float)detailPanel.audioSource.clip.length)}";
            }
        }
    }

    public void Receive(IAnnotationContentBlock block)
    {
        MaximizeScreenHeight();
        title.text = block.title;
        contentBlock = block;
        switch (block.type)
        {
            case BlockType.VIDEO:
                playHover.Enable();
                play.interactable = true;
                canvas.color = Color.white;
                ContentVideo vid = block as ContentVideo;
                canvas.texture = vid.thumbnail;
                canvas.rectTransform.sizeDelta = vid.GetConstrainedRec(-1, canvas.rectTransform.rect.height);
                controls.gameObject.SetActive(true);
                timeLabel.text = $"Ready";
                playPauseIndicator.UpdateState(false);
                Play();
                break;

            case BlockType.IMAGE:
                canvas.color = Color.white;
                ContentImage img = block as ContentImage;
                if (img == null) return;
                canvas.texture = img.image;
                canvas.rectTransform.sizeDelta = img.GetConstrainedRec(-1, canvas.rectTransform.rect.height);
                controls.gameObject.SetActive(false);
                break;

            case BlockType.AUDIO:
                ContentAudio aud = block as ContentAudio;
                if (!aud.loaded)
                {
                    play.interactable = false;
                    playHover.Disable();
                    timeLabel.text = $"Loading";
                } else
                {
                    playHover.Enable();
                    play.interactable = true;
                    detailPanel.Play(block);
                    canvas.color = Color.clear;
                    controls.gameObject.SetActive(true);
                    progress.value = 0;
                    timeLabel.text = $"Ready";
                    playPauseIndicator.UpdateState(false);
                }

                break;

            default:
                Debug.LogWarning($"Unexpected block type! {block.type}");
                return;
        }

    }

    void MaximizeScreenHeight()
    {
        float h = canvas.transform.parent.GetComponent<RectTransform>().rect.height;
        canvas.rectTransform.sizeDelta = new Vector2(canvas.rectTransform.sizeDelta.x, h);
    }

    void Prev()
    {
        detailPanel.TurnFullScreenPage(-1);
        playPauseIndicator.UpdateState(!detailPanel.IsPlaying());
    }

    void Next() {
        detailPanel.TurnFullScreenPage(1);
        playPauseIndicator.UpdateState(!detailPanel.IsPlaying());
    }

    protected override void Play()
    {
        if (contentBlock.type == BlockType.VIDEO)
        {
            canvas.texture = detailPanel.videoPlayer.targetTexture;
        }
        playPauseIndicator.UpdateState(false);
        MaximizeScreenHeight();
        base.Play();
    }
}
