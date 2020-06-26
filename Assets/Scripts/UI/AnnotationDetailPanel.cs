using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Button = UnityEngine.UI.Button;

public class AnnotationDetailPanel : MonoBehaviour
{
    [Header("Layout Parameters")]
    public float lineWeight = 1f;
    public float discRadius = 0.1f;
    public float maxDetailHeight;
    public float minDetailHeight;

    [Header("Internal Structure")]
    public TextMeshProUGUI title;
    public Transform contentTransform;
    public ScrollRect scrollView;
    public RectTransform selfTransform;
    public Button detailOpenToggle;
    public RectTransform line;

    [Header("Video Controls")]
    public ContentVideo currentVideo;
    public RawImage currentVideoCanvas;
    public VideoPlayer videoPlayer;

    [Header("Full Screen Controls")]
    public IAnnotationContentBlock fullScreenObject;
    public FullScreenPlayer fullScreenPlayer;

    [Header("Prefabs")]
    public ContentText textPrefab;
    public ContentVideo videoPrefab;
    public ContentImage imagePrefab;

    [Header("Internal State")]
    private bool _detailOpen;
    private AnnotationIndicator _displayedIndicator;
    private List<IAnnotationContentBlock> _blocks = new List<IAnnotationContentBlock>();


    void Start()
    {
        _detailOpen = scrollView.IsActive(); 
        detailOpenToggle.onClick.AddListener(ToggleDetailView);
        StartCoroutine(ResetScrollView());
    }

    void Update() {
        UpdateTargetLine();
    }

    void OnEnable()
    {
        StartCoroutine(ResetScrollView());
    }

    /**
     * Fills the detail view with appropriate blocks
     */
    public void Populate(AnnotationData data, AnnotationIndicator ind)
    {
        Clear();
        title.text = data.title;
        _blocks = ParseAndAddContentBlocks(data.content);
        _displayedIndicator = ind;

        if (gameObject.activeSelf) // If not active, will be called on Start()
        {
            StartCoroutine(ResetScrollView());
        }
    }

    /**
     * Toggles the detail panel for the active annotation.
     */
    public void ToggleDetailView() {
        _detailOpen = !_detailOpen;
        scrollView.gameObject.SetActive(_detailOpen);
        StartCoroutine(ResetScrollView());
    }

    /**
     * Scroll view must be set to top, and window must be resized for different annotations.
     */
    private IEnumerator ResetScrollView()
    {
        yield return new WaitForEndOfFrame();   // Needs to draw content to know what size to make container; waits until that content is drawn at the end of next frame
        yield return new WaitForEndOfFrame();
        scrollView.verticalScrollbar.value = 0f;
        float h = Mathf.Clamp(scrollView.content.sizeDelta.y, minDetailHeight, maxDetailHeight);
        scrollView.GetComponent<RectTransform>().sizeDelta = new Vector2(128f, h);
        selfTransform.anchoredPosition = new Vector3(selfTransform.anchoredPosition.x, h/2f);
    }

    private void GenerateThumbnail(ContentVideo vc)
    {
        // TODO: Need to add something for video thumbnails
        vc.canvas.texture = Texture2D.grayTexture;
    }

    /**
     * Destroys all current content.
     */
    private void Clear()
    {
        foreach (IAnnotationContentBlock block in _blocks)
        {
            Destroy(block.gameObject);
        }
        _blocks = new List<IAnnotationContentBlock>();
    }

    // VIDEO CONTROLS

    public void VideoPlayed(ContentVideo vc)
    {
        if (currentVideo == vc && videoPlayer.isPaused)
        {
            videoPlayer.Play();
        }
        else
        {
            currentVideo = vc;
            currentVideoCanvas = vc.canvas;
            videoPlayer.targetTexture = RenderTexture.GetTemporary(640, 480);
            videoPlayer.url = vc.url;
            currentVideoCanvas.texture = videoPlayer.targetTexture;
            videoPlayer.Play();
        }
    }

    public void VideoPaused(ContentVideo vc)
    {
        videoPlayer.Pause();
    }

    public void VideoScrubbed(ContentVideo vc, float val)
    {
        videoPlayer.time = val * videoPlayer.length;
    }

    public void ToggleFullScreen(IAnnotationContentBlock block)
    {
        if (fullScreenPlayer.gameObject.activeSelf)
        {
            videoPlayer.Pause();
            fullScreenPlayer.gameObject.SetActive(false);
            return;
        }
        
        ContentVideo vidBlock = block as ContentVideo;

        if (vidBlock != null)
        {
            fullScreenPlayer.ReceiveVideo(vidBlock);
            fullScreenPlayer.gameObject.SetActive(true);
        }

        ContentImage imgBlock = block as ContentImage;

        if (imgBlock != null)
        {
            fullScreenPlayer.ReceiveImage(imgBlock);
            fullScreenPlayer.gameObject.SetActive(true);
        }
    }

    public void ImageClicked(ContentImage ic) {
        // TODO: Show image in full screen
    }



    /**
     * Updates the position and angle of the targeting line; must be called through update.
     */
    private void UpdateTargetLine() {
        Vector3 target = _displayedIndicator.transform.position; // The center point of the target indicator
        Vector3 pivot = line.position; // The pivot of the line to be drawn
        float dist = Vector3.Distance(target, pivot); // Distance between pivot and target
        Vector3 diff = target - pivot;
        float angle = Mathf.Atan2(diff.x, -diff.y) *
                      Mathf.Rad2Deg; // Find the angle made by a line from pivot to target (in degrees)
        line.rotation = Quaternion.Euler(new Vector3(0, 0, angle)); // Rotate by given angle
        line.sizeDelta =
            new Vector2(lineWeight,
                dist / 2f -
                discRadius); // Set line rect; note, height must be divided by two then offset by the disc radius so it doesn't intersect the indicator
    }


    /**
     * Helper method that splits the given content string to find rich media and split text blocks.
     */
    private List<string> TokenizeContent(string content) {
        return Regex.Split(content, "\\[|\\]").ToList();
    }

    /**
     * Parses through tokenized content and creates blocks from prefabs.
     */
    private List<IAnnotationContentBlock> ParseAndAddContentBlocks(string parseableString) {

        List<IAnnotationContentBlock> blocks = new List<IAnnotationContentBlock>();

        List<string> contents = TokenizeContent(parseableString);

        foreach (string content in contents) {
            if (content == "[" || content == "]") {
                // ignore
            } else if (content.StartsWith("vid") || content.StartsWith("video")) {
                ContentVideo vid = Instantiate(videoPrefab, contentTransform);
                Match match = Regex.Match(content, "src=[\'|\"](.*)[\'|\"]");
                string src = match.Groups[1].Value;
                match = Regex.Match(content, "title=[\'|\"](.*?)[\'|\"]");
                string title = src;
                if (match.Groups.Count > 0) {
                    title = match.Groups[1].Value;
                }
                vid.Populate(src, title, this);
                GenerateThumbnail(vid);
                blocks.Add(vid);
            } else if (content.StartsWith("img") || content.StartsWith("image")) {
                ContentImage img = Instantiate(imagePrefab, contentTransform);
                Match match = Regex.Match(content, "src=[\'|\"](.*)[\'|\"]");
                string src = match.Groups[1].Value;
                img.Populate(src, this);
                blocks.Add(img);

            } else {
                ContentText text = Instantiate(textPrefab, contentTransform);
                text.Populate(content, this);
                blocks.Add(text);
            }
        }

        return blocks;
    }
}