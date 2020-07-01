using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectorButton : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Button button;
    public int indexValue;
    public Image icon;

    public void Populate(string label, int index, Sprite sprite)
    {
        text.text = label;
        indexValue = index;

        if (sprite != null)
        {
            icon.sprite = sprite;
        }
    }
}
