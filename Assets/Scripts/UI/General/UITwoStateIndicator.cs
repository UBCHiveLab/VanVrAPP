using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITwoStateIndicator : MonoBehaviour
{
    public Sprite imageTrue;
    public Sprite imageFalse;
    public string textTrue;
    public string textFalse;
    public bool state;
    public Image frame;
    public TextMeshProUGUI label;


    public void UpdateState(bool newState)
    {
        state = newState;
        if (frame != null)
        {
            frame.sprite = state ? imageTrue : imageFalse;
        }

        if (label != null)
        {
            label.text = state ? textTrue : textFalse;
        }
    }
}
