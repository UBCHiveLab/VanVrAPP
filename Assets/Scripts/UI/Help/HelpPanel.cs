using Assets.Scripts.State;
using UnityEngine;
/**
 * TO IMPLEMENT; currently just knows what view mode it is in when enabled. Intention is to give context-specific help to  users.
 */
public class HelpPanel : MonoBehaviour
{
    public StateController controller;

    void OnEnable()
    {
        Debug.Log($"Opening help panel in {controller.mode} mode");
    }
}
