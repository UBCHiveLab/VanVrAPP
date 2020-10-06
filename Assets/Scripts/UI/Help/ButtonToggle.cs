using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonToggle : MonoBehaviour
{
    [SerializeField]
    private Sprite onImage, offImage;
    [SerializeField]
    private Color32 onColor, offColor;
    [SerializeField]
    //private TextMeshProUGUI title;
    TMP_Text tmp;
    [SerializeField]
    private Image image;
    // Start is called before the first frame update
    void Start()
    {
        //image = GetComponent<Image>();
        
    }

    public void OnToggleClick(bool status)
    {
        //swap the sprite of toggle and change text color
        image.sprite = status ? onImage : offImage;
        tmp.color = status ? onColor : offColor;
    }
}
