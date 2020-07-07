using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnnotationIndicator : MonoBehaviour
{
    [Header("Data")] public AnnotationData data;
    public SpecimenData specData;
    private GameObject _specObject;
    public int index;

    [Header("External Structure")] private AnnotationDisplay _display;

    [Header("Internal Structure")] public TextMeshProUGUI number;
    public TextMeshProUGUI label;
    public TextMeshProUGUI content;
    public Button button;

    private void Start()
    {
        button.onClick.AddListener(Clicked);
    }

    private void Update()
    {
        if (_specObject == null) return;
        UpdatePosition();
    }

    /**
     * Populates the indicator with the annotation data
     */
    public void Populate(int index, AnnotationData data, SpecimenData specData, GameObject obj,
        AnnotationDisplay display)
    {
        this.data = data;
        _specObject = obj;
        this.specData = specData;
        this.index = index;
        _display = display;
        number.text = index.ToString();
        UpdatePosition();
    }

    private void Clicked()
    {
        _display.ShowDetail(this);
    }

    /**
     * Updates the screen location of the annotation indicator
     */
    private void UpdatePosition()
    {
        transform.position =
            Camera.main.WorldToScreenPoint(_specObject.transform.position + data.positionVector3 * specData.scale);
    }
}