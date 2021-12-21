using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class CloseButton : MonoBehaviour
{
    public GameObject panel;
    // Start is called before the first frame update
    

    public void ClosePanel()
    {
        panel.SetActive(false);
        Debug.Log("panel should not be active now");
    }
}
