using System;
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
    public TextMeshProUGUI idFrame;
  //  public TextMeshProUGUI nameFrame;
    private CoursesPage _coursesPage;

    public void Populate(CourseData data, CoursesPage coursesPage)
    {
        idFrame.text = $"{data.courseId}";
       // nameFrame.text = data.courseCode;
        _coursesPage = coursesPage;
        button.onClick.AddListener(() => _coursesPage.CourseSelected(data.courseId));
    }

    
}
