using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SpecimenListing : MonoBehaviour
{
    public TextMeshProUGUI label;
    public SpecimenData specimen;
    public Button button;

    public void Populate(SpecimenData spd, UnityAction action)
    {
        button.onClick.AddListener(action);
        specimen = spd;
        label.text = specimen.id;
        
    }
}
