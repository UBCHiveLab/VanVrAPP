using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessingBlender : MonoBehaviour
{
    public PostProcessVolume volume;
    private DepthOfField depthOfField;
    private BoolParameter activeDof = new BoolParameter();
    private FloatParameter focusDist = new FloatParameter();

    public float transitionTimeToGo;
    public float transitionTimeTotal = 1f;

    public float _focusFrom;
    public float _focusTo;

    void Start()
    {
        if (depthOfField == null) {
            volume.profile.TryGetSettings(out depthOfField);
        }
        depthOfField.enabled = activeDof;
        depthOfField.focusDistance = focusDist;
    }

    public void SetFocus(float to, float time)
    {
        if (time <= 0)
        {
            depthOfField.focusDistance.value = to;
            return;
        } 
        _focusFrom = depthOfField.focusDistance;
        _focusTo = to;
        transitionTimeToGo = transitionTimeTotal = time;
    }

    public void ActivateDepthOfField(bool active)
    {
        activeDof.value = active;
        Debug.Log(depthOfField.active);
    }

    void Update()
    {
        if (transitionTimeToGo > 0)
        {
            transitionTimeToGo -= Time.deltaTime;
            float val = Mathf.Lerp(_focusTo, _focusFrom, transitionTimeToGo / transitionTimeTotal);
            focusDist.value = val;

            if (transitionTimeToGo <= 0)
            {
                transitionTimeToGo = 0;
            }
        }
    }
}
