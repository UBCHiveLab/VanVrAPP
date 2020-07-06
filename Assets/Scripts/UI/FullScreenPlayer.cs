using UnityEngine;
using UnityEngine.UI;

public class FullScreenPlayer : MonoBehaviour
{

    public AnnotationDetailPanel detailPanel;
    public RawImage canvas;
    public Button next;
    public Button prev;

    public void Receive(IAnnotationContentBlock block)
    {
        canvas.color = Color.white;
        canvas.texture = detailPanel.videoPlayer.targetTexture;
    }

}
