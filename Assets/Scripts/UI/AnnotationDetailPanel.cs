using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.Video;

public class AnnotationDetailPanel : MonoBehaviour
{
    public TextMeshProUGUI title;
    public TextMeshProUGUI content;
    public RectTransform line;

    private AnnotationData _data;
    private AnnotationIndicator _ind;

    private List<IAnnotationContentBlock> _blocks = new List<IAnnotationContentBlock>();

    public float lineWeight = 1f;
    public float discRadius = 0.1f;

    public ContentVideo currentVideo;
    public RawImage currentVideoCanvas;
    public VideoPlayer videoPlayer;
    public Transform contentTransform;

    public ContentText textPrefab;
    public ContentVideo videoPrefab;
    public ContentImage imagePrefab;

    public ScrollRect scrollView;
    public float maxDetailHeight;
    public float minDetailHeight;
    public RectTransform selfTransform;


    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        StartCoroutine(ResetScrollView());

    }



    public void Populate(AnnotationData data, AnnotationIndicator ind)
    {
        Clear();


        title.text = data.title;
        _blocks = new List<IAnnotationContentBlock>();

        // TODO: break up data.content
        List<string> contents = TokenizeContent(data.content);

        foreach (string content in contents)
        {
            if (content == "[" || content == "]")
            {
                // ignore
            }
            else if (content.StartsWith("vid") || content.StartsWith("video"))
            {
                ContentVideo vid = Instantiate(videoPrefab, contentTransform);
                Match match = Regex.Match(content, "src=[\'|\"](.*)[\'|\"]");
                string src = match.Groups[1].Value;
                vid.Populate(src, this);
                GenerateThumbnail(vid);
                _blocks.Add(vid);
            }
            else if (content.StartsWith("img") || content.StartsWith("image"))
            {
                ContentImage img = Instantiate(imagePrefab, contentTransform);
                Match match = Regex.Match(content, "src=[\'|\"](.*)[\'|\"]");
                string src = match.Groups[1].Value;
                img.Populate(src, this);
                _blocks.Add(img);

            } else
            {
                ContentText text = Instantiate(textPrefab, contentTransform);
                text.Populate(content, this);
                _blocks.Add(text);
            }
        }

        _data = data;
        _ind = ind;

        if (gameObject.activeSelf) // If not active, will be called on Start()
        {
            StartCoroutine(ResetScrollView());
        }

    }

    private IEnumerator ResetScrollView()
    {
        yield return new WaitForEndOfFrame();
        scrollView.verticalScrollbar.value = 0f;
        float h = Mathf.Clamp(scrollView.content.sizeDelta.y, minDetailHeight, maxDetailHeight);
        scrollView.GetComponent<RectTransform>().sizeDelta = new Vector2(128f, h);
        selfTransform.anchoredPosition = new Vector3(selfTransform.anchoredPosition.x, h/2f);
    }

    List<string> TokenizeContent(string content)
    {
        return Regex.Split(content, "\\[|\\]").ToList();
    }

    void GenerateThumbnail(ContentVideo vc)
    {
        // TODO
        vc.canvas.texture = Texture2D.grayTexture;
    }

    void Clear()
    {
        foreach (IAnnotationContentBlock block in _blocks)
        {
            Destroy(block.gameObject);
        }

        _blocks = new List<IAnnotationContentBlock>();
    }

    void Update()
    {
        UpdateTargetLine();


    }


    private void UpdateTargetLine()
    {
        Vector3 target = _ind.transform.position; // The center point of the target indicator
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

    public void VideoClicked(ContentVideo vc)
    {
        if (currentVideo == vc)
        {
            if (videoPlayer.isPaused)
            {
                videoPlayer.Play();
            }
            else
            {
                videoPlayer.Pause();
            }
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

    public void ImageClicked(ContentImage ic) {
        
    }
}