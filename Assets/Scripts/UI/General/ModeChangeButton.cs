using Assets.Scripts.State;
using UnityEngine;
using UnityEngine.UI;

/**
 * Binds a simple mode-change event to a button.
 */

public class ModeChangeButton : MonoBehaviour
{
    public ViewMode mode;
    private StateController controller;
    public CoursesPage coursesPage;
    public MainCameraEvents cameraEvents;
    public TrayPage trayPage;

    // Start is called before the first frame update
    void Start()
    {
        if (controller == null) controller = FindObjectOfType<StateController>();
        GetComponent<Button>().onClick.AddListener(() => {cameraEvents.SwitchCamera(); });
        GetComponent<Button>().onClick.AddListener(() => {trayPage.SetAnalyzeOff(); });
        GetComponent<Button>().onClick.AddListener(() => { controller.mode = mode; });
        GetComponent<Button>().onClick.AddListener(() => {coursesPage.ShowHomeInfo(); });

    }

}
