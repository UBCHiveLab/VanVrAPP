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

    // Start is called before the first frame update
    void Start()
    {
        if (controller == null) controller = FindObjectOfType<StateController>();
        GetComponent<Button>().onClick.AddListener(() => { controller.mode = mode; });
    }

}
