using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MultimediaContent : MonoBehaviour
{
    public bool richMedia => true;
    public Vector2 sizeRect;
    public AnnotationDetailPanel detailPanel;
    public RawImage canvas;
    public GameObject controls;
    public TextMeshProUGUI label;
    public Button play;
    public Button pause;
    public Slider progress;
    public TextMeshProUGUI timeLabel;
    public Button fullScreen;
    public IAnnotationContentBlock contentBlock;


    protected bool scrubbing;


    private void Start() {
        fullScreen?.onClick.AddListener(ToggleFullScreen);
        if (controls != null)
        {
            play?.onClick.AddListener(Play);
            pause?.onClick.AddListener(Pause);
            progress?.onValueChanged.AddListener(Scrub);
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

        protected virtual void Play() {

            detailPanel.Play(contentBlock);
        }

        private void Pause() {
            detailPanel.Pause(contentBlock);
        }

        private void Scrub(float val) {
            if (scrubbing) {
                detailPanel.Scrub(contentBlock, val);
                progress.value = val;
            }
        }

        protected void ToggleFullScreen() {
            detailPanel.ToggleFullScreen(contentBlock);
        }
}
