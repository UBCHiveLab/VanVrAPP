using UnityEngine;
using UnityEngine.UI;

public class UITwoStateIndicator : MonoBehaviour
{
    public Sprite imageTrue;
    public Sprite imageFalse;
    public bool state;
    public Image frame;

    public void UpdateState(bool newState)
    {
        state = newState;
        frame.sprite = state ? imageTrue : imageFalse;
    }
}
