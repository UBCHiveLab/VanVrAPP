using UnityEngine;
using TMPro;

public class SchoolSelectionController : MonoBehaviour
{
    // Reference to the TextMesh Pro Dropdown UI component
    public TMP_Dropdown dropdown;

    // The global value that can be accessed from other scripts
    public string selectedSchool;

    private void Start()
    {
        // Subscribe to the dropdown's onValueChanged event
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);

        // Initialize the selectedSchool with the default value
        selectedSchool = dropdown.options[0].text;
    }

    // This method is called when the dropdown's value changes
    private void OnDropdownValueChanged(int index)
    {
        // Update the selectedSchool with the new dropdown value
        selectedSchool = dropdown.options[index].text;
    }
}