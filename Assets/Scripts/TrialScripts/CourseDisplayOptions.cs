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
    private CoursesPage _coursesPage;
    private SelectorMenu _selectorMenu;


    public void Populate(CourseData data, CoursesPage coursesPage,SelectorMenu selectorMenu)
    {
        idFrame.text = $"{data.courseId}";
       // nameFrame.text = data.courseCode;
        _coursesPage = coursesPage;
        _selectorMenu = selectorMenu;
        
        button.onClick.AddListener(() => _coursesPage.CourseSelected(data.courseId));
        button.onClick.AddListener(() => _selectorMenu.CourseSelected(data.courseId));
    }

    
}
