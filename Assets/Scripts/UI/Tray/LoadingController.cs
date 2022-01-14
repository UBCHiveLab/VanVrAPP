using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.State;
using UnityEngine;
using UnityEngine.UI;

public class LoadingController : MonoBehaviour
{
    public GameObject loadingLargeSpinner;
    public GameObject skyler; 
    public GameObject actionButtons;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnLoadingProcess(bool loading)
    {
        loadingLargeSpinner.SetActive(!loading);
        skyler.SetActive(loading); 
        Debug.Log(loading); 
     //   actionButtons.SetActive(loading);
    }
}
