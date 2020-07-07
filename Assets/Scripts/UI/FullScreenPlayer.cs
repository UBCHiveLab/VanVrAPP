using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FullScreenPlayer : MonoBehaviour
{

    public AnnotationDetailPanel detailPanel;

    [Header("Internal Structure")]
    public RawImage canvas;
    public TextMeshProUGUI title;
    public Button next;
    public Button prev;
    public Button fullScreenToggle;


    [Header("Controls")]
    public GameObject mediaControls;
    public Button fullScreen;
    public Button play;
    public Button pause;
    public Slider scrub;
    public TextMeshProUGUI time;

    private IAnnotationContentBlock _currentBlock;
    private bool _scrubbing;

    void Start()
    {
        next.onClick.AddListener(Next);
        prev.onClick.AddListener(Prev);

        fullScreen.onClick.AddListener(EndFullScreen);
        fullScreenToggle.onClick.AddListener(EndFullScreen);
        play.onClick.AddListener(Play);
        pause.onClick.AddListener(Pause);

    }

    public void Receive(IAnnotationContentBlock block)
    {

        title.text = block.title;
        _currentBlock = block;
        switch (block.type)
        {
            case BlockType.VIDEO:
                canvas.color = Color.white;
                canvas.texture = detailPanel.videoPlayer.targetTexture;
                mediaControls.gameObject.SetActive(true);
                break;

            case BlockType.IMAGE:
                canvas.color = Color.white;
                ContentImage img = block as ContentImage;
                if (img == null) return;
                canvas.texture = img.image;
                canvas.rectTransform.sizeDelta = img.GetConstrainedRect(-1, canvas.rectTransform.rect.height);
                mediaControls.gameObject.SetActive(false);
                break;

            case BlockType.AUDIO:
                canvas.color = Color.clear;
                mediaControls.gameObject.SetActive(true);
                break;

            default:
                Debug.LogWarning($"Unexpected block type! {block.type}");
                return;
        }
    }

    void Prev()
    {
        detailPanel.TurnFullScreenPage(-1);
    }

    void Next()
    {
        detailPanel.TurnFullScreenPage(1);
    }

    void EndFullScreen()
    {
        detailPanel.ToggleFullScreen(null);
    }

    void Play()
    {
        detailPanel.Play(_currentBlock);
    }

    void Pause()
    {
        detailPanel.Pause(_currentBlock);
    }


    /**
     * Scrubbing toggles called onDrag of the progress
     */
    public void StartScrub() {
        _scrubbing = true;
    }

    public void EndScrub() {
        _scrubbing = false;
        detailPanel.Scrub(_currentBlock, scrub.value);
    }
}
