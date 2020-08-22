using TMPro;
using UnityEngine;
using UnityEngine.UI;

/*
 * The representation of a single lab course for the SelectorMenu
 */
public class CourseOption : MonoBehaviour
{
    [Header("Internal Structure")]
    public Button button;
    public TextMeshProUGUI courseNameFrame;

    private SelectorMenu _selectorMenu;

    public void Populate(string courseId, SelectorMenu selectorMenu)
    {
        courseNameFrame.text = courseId;
        _selectorMenu = selectorMenu;
        button.onClick.AddListener(() => _selectorMenu.CourseSelected(courseId));
    }
}
