using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnnotationSelector : MonoBehaviour
{
    [Header("Internal Structure")]
    public TextMeshProUGUI label;
    public Button left;
    public Button right;

    [Header("External Structure")]
    public AnnotationDisplay display;

    void Start()
    {
        UpdateIndex();
        left.onClick.AddListener(() =>
        {
            ChangeAnnotation(-1);
        });
        right.onClick.AddListener(() =>
        {
            ChangeAnnotation(1);
        });
    }

    void OnEnable()
    {
        UpdateIndex();
    }

    /**
     * Checks what the display's current index is and sets its UI accordingly
     */
    public void UpdateIndex()
    {
        if (display.activeIndicators.Count == 0) {
            label.text = "No annotations found.";
            left.gameObject.SetActive(false);
            right.gameObject.SetActive(false);
            return;
        }

        int idx = display.selectedSpecimenIndex;
        if (idx == -1)
        {
            label.text = "Select an annotation.";
            left.gameObject.SetActive(true);
            right.gameObject.SetActive(true);
            
        }
        else
        {
            label.text = idx + ". " + display.activeIndicators[idx].data.title;
        }
    }


    /**
     * Asks display to change its current annotation
     */
    public void ChangeAnnotation(int delta)
    {
        display.IncrementAnnotationIndex(delta);
        UpdateIndex();
    }

}
