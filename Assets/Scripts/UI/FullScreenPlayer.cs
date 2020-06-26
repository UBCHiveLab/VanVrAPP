using UnityEngine;

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
        // TODO: Image full screen
        Debug.Log("Not implemented yet");
    }
}
