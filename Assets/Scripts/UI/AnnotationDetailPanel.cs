using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using TMPro;
using UnityEngine;

public class AnnotationDetailPanel : MonoBehaviour
{
    public TextMeshProUGUI title;
    public TextMeshProUGUI content;
    public RectTransform line;

    private AnnotationData _data;
    private AnnotationIndicator _ind;

    public float lineWeight = 0.66f;
    public float discRadius = 0.1f;

    public void Populate(AnnotationData data, AnnotationIndicator ind)
    {
        title.text = data.title;
        content.text = data.content;
        _data = data;
        _ind = ind;
    }

    void Update()
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(_ind.transform.position);
        Vector3 piv = line.position;
        float dist = Vector3.Distance(pos, piv);
        Vector3 diff = pos - piv;
        float angle = Mathf.Atan2(diff.x, -diff.y) * Mathf.Rad2Deg;
        line.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        line.sizeDelta = new Vector2(lineWeight, dist/2f - discRadius);
    }
}
