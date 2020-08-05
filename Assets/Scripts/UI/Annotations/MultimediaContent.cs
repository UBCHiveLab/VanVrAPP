using TMPro;
using UnityEngine;
using UnityEngine.UI;

/**
 * Parent class for rich media content classes; implements common controls and queries.
 */
public class MultimediaContent : MonoBehaviour
{
    public bool richMedia => true;

    [Header("Internal Structure")]
    public Vector2 sizeRect;
    public RawImage canvas;
    public GameObject controls;
    public TextMeshProUGUI label;
    public Button play;
    public Button audioToggle;
    public UITwoStateIndicator playPauseIndicator;
    public UITwoStateIndicator audioOnIndicator;
    public HoverButton playHover;
    public Slider progress;
    public TextMeshProUGUI timeLabel;
    public Button fullScreen;
    public IAnnotationContentBlock contentBlock;

    [Header("Services")]
    public AnnotationDetailPanel detailPanel;


    protected bool scrubbing;

    private void Start() {
        fullScreen?.onClick.AddListener(ToggleFullScreen);
        if (controls != null)
        {
            play?.onClick.AddListener(TogglePlay);
            progress?.onValueChanged.AddListener(Scrub);
            audioToggle.onClick?.AddListener(ToggleAudio);
            timeLabel.text = "";
        }
        PrepareContent();
    }

    private void Update()
    {
        UpdateTime();
    }

    protected virtual void UpdateTime()
    {
        // Override
    }

    protected virtual void PrepareContent()
    {
        // Override
    }

    protected virtual void ContentLoaded()
    {
        // Override
    }

    public Vector2 GetConstrainedRec(float cWidth, float cHeight) {

        if (cHeight <= 0) {
            float tw = sizeRect.x;
            float whRatio = cWidth / tw;
            float height = sizeRect.y * whRatio;
            return new Vector2(cWidth, height);
        }

        if (cWidth <= 0) {
            float th = sizeRect.y;
            float hwRatio = cHeight / th;
            float width = sizeRect.x * hwRatio;
            return new Vector2(width, cHeight);
        }

        //TODO: allow for maxs that aren't -1
        return new Vector2(cWidth, cHeight);
    }

    protected string toTime(float input) {
        string mins = Mathf.FloorToInt(input / 60f).ToString();
        string secs = Mathf.FloorToInt(input % 60).ToString();
        if (secs.Length == 1) secs = "0" + secs;

        return $"{mins}:{secs}";
    }

    /**
     * Scrubbing toggles called onDrag of the progress
     */
        public void StartScrub() {
            scrubbing = true;
        }

        public void EndScrub() {
            scrubbing = false;
            Scrub(progress.value);
        }

        public void TogglePlay()
        {
            if (detailPanel.IsPlaying())
            {
                Pause();
            }
            else
            {
                Play();
            }
        }

        protected virtual void Play()
        {
            playPauseIndicator.UpdateState(false);
            detailPanel.Play(contentBlock);
        }

        private void Pause()
        {
            playPauseIndicator.UpdateState(true);
            detailPanel.Pause(contentBlock);
        }

        private void Scrub(float val) {
            if (scrubbing) {
                detailPanel.Scrub(contentBlock, val);
                progress.value = val;
                if (detailPanel.IsPlaying()) {
                    Play();
                } else {
                    Pause();
                }
        }

            
        }

    private void ToggleAudio()
        {
            bool on = detailPanel.AudioIsOn();
            detailPanel.ToggleAudio(on);
            audioOnIndicator.UpdateState(on);
        }

        protected void ToggleFullScreen() {
            detailPanel.ToggleFullScreen(contentBlock);
        }
}
