using System;
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
    public GameObject iconActive;
    public GameObject iconInactive;

    [Header("AV Controls")]
    public IAnnotationContentBlock currentAVSource;
    public RawImage currentVideoCanvas;
    public VideoPlayer videoPlayer;
    public AudioSource audioSource;
    public YoutubePlayer youtubePlayer;


    [Header("Full Screen Controls")]
    public IAnnotationContentBlock fullScreenObject;
    public FullScreenPlayer fullScreenPlayer;

    [Header("Prefabs")]
    public ContentText textPrefab;
    public ContentVideo videoPrefab;
    public ContentImage imagePrefab;
    public ContentAudio audioPrefab;
    public ContentSeparator seperatorPrefab;

    [Header("Internal State")]
    private bool _detailOpen;
    private AnnotationIndicator _displayedIndicator;
    private List<ContentBlockData> _blockData;
    private List<IAnnotationContentBlock> _blocks = new List<IAnnotationContentBlock>();

    private Dictionary<BlockType, Func<AnnotationDetailPanel, IAnnotationContentBlock>> blockTypeToBuilder =
    new Dictionary<BlockType, Func<AnnotationDetailPanel, IAnnotationContentBlock>>
    {
        {
            BlockType.SEPARATOR,
            p => Instantiate(p.seperatorPrefab, p.contentTransform)
        },
        {
            BlockType.AUDIO,
            p => Instantiate(p.audioPrefab, p.contentTransform)
        },
        {
            BlockType.VIDEO,
            p => Instantiate(p.videoPrefab, p.contentTransform)
        },
        {
            BlockType.TEXT,
            p => Instantiate(p.textPrefab, p.contentTransform)
        },
        {
            BlockType.IMAGE,
            p => Instantiate(p.imagePrefab, p.contentTransform)
        }
    };


    void Start()
    {
        _detailOpen = scrollView.IsActive();
        UpdateIcon();
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
        AnnotationParser parser = new AnnotationParser();
        _blockData = parser.ParseAndAddContentBlocks(data.content);
        _blocks = SpawnBlocks();
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
        UpdateIcon();
    }

    private void UpdateIcon()
    {
        if (_detailOpen) {
            iconActive.SetActive(true);
            iconInactive.SetActive(false);
        } else {
            iconActive.SetActive(false);
            iconInactive.SetActive(true);
        }
    }

    /**
     * Scroll view must be set to top, and window must be resized for different annotations.
     */
    private IEnumerator ResetScrollView()
    {
        yield return new WaitForEndOfFrame();   // Needs to draw content to know what size to make container; waits until that content is drawn at the end of next frame
        yield return new WaitForEndOfFrame();
        scrollView.verticalScrollbar.value = 1f;
        float h = Mathf.Clamp(scrollView.content.sizeDelta.y, minDetailHeight, maxDetailHeight);
        scrollView.GetComponent<RectTransform>().sizeDelta = new Vector2(128f, h);
        selfTransform.anchoredPosition = new Vector3(selfTransform.anchoredPosition.x, h/2f);
    }

    /**
     * Destroys all current content.
     */
    private void Clear()
    {
        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }

        _blocks = new List<IAnnotationContentBlock>();
    }

    // VIDEO CONTROLS

    public void Play(IAnnotationContentBlock ab)
    {
        if (currentAVSource == ab)
        {
            if (ab.type == BlockType.AUDIO)
            {
                audioSource.Play();
            } else if (ab.type == BlockType.VIDEO)
            {
                videoPlayer.Play();
            }
        }
        else
        {
            currentAVSource = ab;

            if (ab.type == BlockType.VIDEO)
            {

                ContentVideo vc = ab as ContentVideo;
                currentVideoCanvas = vc.canvas;
                videoPlayer.targetTexture = RenderTexture.GetTemporary(640, 480);
                currentVideoCanvas.texture = videoPlayer.targetTexture;
                if (vc.youtube) {
                    vc.youtubePlayer.Play(vc.url);
                }
                else
                {

                    videoPlayer.url = vc.url;
                    videoPlayer.Play();
                }

            }
            else if (ab.type == BlockType.AUDIO) 
            {
                ContentAudio ac = ab as ContentAudio;
                audioSource.clip = ac.clip;
                audioSource.Play();
            }

        }
    }

    public void Pause(IAnnotationContentBlock ab)
    {
        audioSource.Pause();
        videoPlayer.Pause();
    }

    public void Scrub(IAnnotationContentBlock ab, float val)
    {
        if (ab.type == BlockType.AUDIO)
        {
            audioSource.time = val * audioSource.clip.length;
        } else if (ab.type == BlockType.VIDEO)
        {
            videoPlayer.time = val * videoPlayer.length;
        }
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
    private void UpdateTargetLine()
    {
        if (_displayedIndicator == null) return;
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

    private List<IAnnotationContentBlock> SpawnBlocks()
    {
        List<IAnnotationContentBlock> blocks = new List<IAnnotationContentBlock>();

        foreach (ContentBlockData data in _blockData)
        {
            IAnnotationContentBlock block = blockTypeToBuilder[data.type].Invoke(this);
            block.Populate(data, this);
            blocks.Add(block);
        }
        return blocks;
    }
    
}