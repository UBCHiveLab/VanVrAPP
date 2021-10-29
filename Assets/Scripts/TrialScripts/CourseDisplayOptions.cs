using TMPro;
using UnityEngine;
using UnityEngine.UI;

/*
 * The representation of a single lab course for the SelectorMenu
 */
public class CourseDisplayOptions : MonoBehaviour
{
    [Header("Internal Structure")]
    public Button button;
    public Text courseNameFrame2;
    

    private CoursesPage _coursesPage;

    public void Populate(string courseId, CoursesPage coursesPage)
    {
        courseNameFrame2.text = courseId;
        _coursesPage = coursesPage;
        button.onClick.AddListener(() => _coursesPage.CourseSelected(courseId,"Description"));
        Debug.Log("button can be clicked");
    }
}
