using TMPro;
using UnityEngine;
using UnityEngine.UI;

/**
 * A single instance of a numbered UI indicator, clickable to summon the attached annotation.
 */
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

    private Camera mainCam;
    private Image indicatorImage;
    private bool isTransparencyChanged=false;
    private AnnotationDisplayController annotationDisplayController;

    private void Start()
    {
        button.onClick.AddListener(Clicked);
        mainCam = Camera.main;
        indicatorImage = GetComponent<Image>();
        
    }

    private void Update()
    {
        if (_specObject == null) return;
        UpdatePosition();
        ControlTransparency();
    }
    private void FixedUpdate()
    {
        if (_specObject == null) return;
        UpdateAppearence();
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
        UpdateAppearence();
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
        if (data.positionVector3 != null)
        {
            if (mainCam == null)
                mainCam = Camera.main;
            transform.position = 
                mainCam.WorldToScreenPoint(_specObject.transform.TransformPoint((Vector3)data.positionVector3)); 
        } 
        else
        {
            // TODO: where does the non-local point indicator show up?
            gameObject.SetActive(false);
        }
    }
    /// <summary>
    ///  Updates the transparency of the annotation indicator based on where the camera looks at
    /// </summary>
    private void UpdateAppearence()
    {
        if (mainCam == null)
            mainCam = Camera.main;
        if (indicatorImage==null)
        indicatorImage = GetComponent<Image>();

        if (data.positionVector3 != null)
        {
            Vector3 _annotationScreenPoint = _specObject.transform.TransformPoint((Vector3)data.positionVector3);
            Vector3 _mainCamPosition = mainCam.transform.position;
            Vector3 _dir = (_annotationScreenPoint - _mainCamPosition).normalized;
            float distance = Vector3.Distance(mainCam.transform.position, _annotationScreenPoint);
            Debug.DrawLine(_mainCamPosition, _annotationScreenPoint, Color.red);
            if (Physics.Raycast(_mainCamPosition, _dir, distance))
            {
                ChangeTransparency(indicatorImage, true);
            }
            else
            {
                ChangeTransparency(indicatorImage, false);
            }
        }
        
    }
    private void ChangeTransparency(Image indicatorImage, bool isChanged)
    {
        if (isTransparencyChanged != isChanged)
        {
            switch (isChanged)
            {
                case true:
                    indicatorImage.color = new Color(1, 1, 1, 0.3f);

                    break;
                    
                case false:
                    indicatorImage.color = Color.white;
                    break;
            }
            isTransparencyChanged = isChanged;
        }
        
    }

    void ControlTransparency()
    {
        indicatorImage = GetComponent<Image>();
        annotationDisplayController = GameObject.Find("Annotations_Button").GetComponent<AnnotationDisplayController>();
        if(isTransparencyChanged != true)
        {
            indicatorImage.color = Color.white;
            number.color = new Color(1, 1, 1, 1f);
        }
        else
        {
            if(annotationDisplayController.isButtonOn == true)
            {
                indicatorImage.color = new Color(1, 1, 1, 0.3f);
                number.color = new Color(1, 1, 1, 1f);
            }
            else
            {
                indicatorImage.color = new Color(1, 1, 1, 0f);
                number.color = new Color(1, 1, 1, 0f);
            }
        }
    }
}