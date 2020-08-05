using TMPro;
using UnityEngine;
using UnityEngine.UI;

/**
 * Clickable entry for dropup menu in AnnotationSelector
 */
public class DropUpAnnotationEntry : MonoBehaviour
{
    public Button button;
    public TextMeshProUGUI label;

    public void Populate(AnnotationSelector selector, int index, AnnotationData annotation)
    {
        label.text = $"{index}. {annotation.title}";
        button.onClick.AddListener(() =>
        {
            selector.SelectAnnotation(index);
        });
    }

    public void SetSelected(bool selected)
    {
        ColorBlock block = button.colors;
        if (selected)
        {
            block.normalColor = Color.white;
        }
        else
        {
            block.normalColor = Color.clear;
        }
        button.colors = block;
        button.OnDeselect(null);
    }
}

