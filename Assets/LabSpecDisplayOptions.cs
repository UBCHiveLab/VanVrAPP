using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI; 
using TMPro;


public class LabSpecDisplayOptions : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMeshProUGUI labName; 
    public List<SpecimenData> _loadedCourseSpecimens; 
    private CoursesPage _coursesPage; 
    private SelectorMenu _selectorMenu; 
    public Transform children; 
    


    public void Populate (Tuple<int, List<SpecimenData>> tuple, CoursesPage coursesPage)
    {
        labName.text = $"Lab {tuple.Item1.ToString()}"; 
        Debug.Log(labName.text);
        _loadedCourseSpecimens = tuple.Item2; 
        
        _coursesPage = coursesPage; 
     //   _coursesPage.RenderSpecimenInfo();
        
    }
}
