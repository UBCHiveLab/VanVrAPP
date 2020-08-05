using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum DisplayResolutionButtonState
{
    AVAILABLE,
    ACTIVE,
    UNAVAILABLE
}

public class DisplayResolutionButton : MonoBehaviour {


    [Header("Internal Structure")]
    public TextMeshProUGUI label;
    public Button button;
    public UITwoStateIndicator indicator;
    public Image buttonFrame;

    [Header("Other")]
    public Vector2Int resolution;
    public GeneralSettingsPanel settingsPanel;
    public DisplayResolutionButtonState state;

    public void Populate(Vector2Int res, DisplayResolutionButtonState state, GeneralSettingsPanel st)
    {
        resolution = res;
        settingsPanel = st;
        label.text = res.ToString();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnSelect);
        this.state = state;

        switch (state)
        {
            case DisplayResolutionButtonState.UNAVAILABLE:
                indicator.UpdateState(false);
                button.interactable = false;
                buttonFrame.color = Color.gray;
                break;
            case DisplayResolutionButtonState.AVAILABLE:
                indicator.UpdateState(true);
                button.interactable = true;
                break;

            case DisplayResolutionButtonState.ACTIVE:
                indicator.UpdateState(false);
                button.interactable = false;
                buttonFrame.color = new Color(0.3490196f, 0.3333333f, 1f);
                break;
        }

    }

    public void OnSelect()
    {
        settingsPanel.OnChangeResolution(this);
    }
}
