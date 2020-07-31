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

    public void Populate(Vector2Int res, bool active, GeneralSettings st)
    {
        resolution = res;
        settings = st;
        label.text = res.ToString();
        button.onClick.AddListener(OnSelect);
        
        button.interactable = !active;  
        
    }

    public void OnSelect()
    {
        settings.OnChangeResolution(resolution, this);
    }
}
