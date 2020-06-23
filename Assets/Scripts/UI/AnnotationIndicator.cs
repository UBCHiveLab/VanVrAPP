using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnnotationIndicator : MonoBehaviour
{
    private AnnotationData _data;
    private SpecimenData _specData;
    private GameObject _specObject;
    public TextMeshProUGUI label;
    public TextMeshProUGUI content;
    public Button button;


    public void Populate(AnnotationData data, SpecimenData specData, GameObject obj)
    {
        _data = data;
        _specObject = obj;
        _specData = specData;



    }

    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener(Clicked);
    }

    void Clicked()
    {
        Debug.Log(_data.annotationId);
    }

    // Update is called once per frame
    void Update()
    {
        if (_specObject == null) return;
        UpdatePosition();
    }

    void UpdatePosition()
    {
        transform.position = _specObject.transform.position + _data.positionVector3 * _specData.scale;
        /*
        Transform parent = transform.parent;
        transform.SetParent(_specObject.transform);
        transform.localPosition = _data.positionVector3;
        transform.SetParent(parent);*/
    }
}
