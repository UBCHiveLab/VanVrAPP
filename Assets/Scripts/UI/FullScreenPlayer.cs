using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class FullScreenPlayer : MonoBehaviour
{
    public ContentVideo FullScreenVideo;
    public ContentImage FullScreenImage;
    public AnnotationDetailPanel detailPanel;

    public void ReceiveVideo(ContentVideo vidBlock)
    {
        FullScreenVideo.Populate(vidBlock.url, vidBlock.title, detailPanel);
    }

    public void ReceiveImage(ContentImage imgBlock)
    {
        // TODO
        Debug.Log("Not implemented yet");
    }
}
