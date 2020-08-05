using TMPro;
using UnityEngine;

public class ErrorPanel : MonoBehaviour
{
    public TextMeshProUGUI label;

    public void Populate(string errorText)
    {
        gameObject.SetActive(true);
        label.text = errorText;
    }
}
