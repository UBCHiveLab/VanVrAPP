using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class ModeChangeToSpecimen : MonoBehaviour
{
    public CoursesPage coursesPage; 
    public SelectorMenu selectorMenu; 
    public GameObject panel; 
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => {coursesPage.LastEntry(); });
        GetComponent<Button>().onClick.AddListener(() => {selectorMenu.ToggleToLabs(); });
        GetComponent<Button>().onClick.AddListener(() => {panel.SetActive(false); });
        GetComponent<Button>().onClick.AddListener(() => {coursesPage.SidePanelPreviewOff(); });
    }


}
