using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayResolutionButton : MonoBehaviour
{
    [Header("Internal Structure")]
    public TextMeshProUGUI label;
    public Button button;

    [Header("Other")]
    public Vector2Int resolution;
    public GeneralSettings settings;

    public void Populate(Vector2Int res, bool active, bool disabled, GeneralSettings st)
    {
        resolution = res;
        settings = st;
        label.text = res.ToString();
        button.onClick.AddListener(OnSelect);

        button.interactable = !active && !disabled;
        if (active)
        {
            label.color = Color.green;
        }
        else
        {
            label.color = Color.black;
        }

    }

    public void OnSelect()
    {
        settings.OnChangeResolution(resolution, this);
    }
}
