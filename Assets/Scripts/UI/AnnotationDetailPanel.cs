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

    public float lineWeight = 1f;
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
        Vector3 target = _ind.transform.position; // The center point of the target indicator
        Vector3 pivot = line.position; // The pivot of the line to be drawn
        float dist = Vector3.Distance(target, pivot); // Distance between pivot and target
        Vector3 diff = target - pivot;
        float angle = Mathf.Atan2(diff.x, -diff.y) * Mathf.Rad2Deg; // Find the angle made by a line from pivot to target (in degrees)
        line.rotation = Quaternion.Euler(new Vector3(0, 0, angle)); // Rotate by given angle
        line.sizeDelta = new Vector2(lineWeight, dist/2f - discRadius); // Set line rect; note, height must be divided by two then offset by the disc radius so it doesn't intersect the indicator
    }
}
