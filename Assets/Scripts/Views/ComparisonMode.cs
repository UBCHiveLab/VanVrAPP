using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComparisonMode : MonoBehaviour, IPage
{
    [Header("State")]
    public GameObject comparisonCanvas;
    public GameObject analysisLeftContainer;
    public bool isCompared = false;

    [Header("button")]
    public Button comparisonCloseButton;

    public AnalysisPage analysisPage;


    public void Activate() { }

    public void Deactivate() { }

    // Start is called before the first frame update
    void Start()
    {
        //addlistener
        comparisonCloseButton.onClick.AddListener(ToggleClose);

        //
        analysisPage = GameObject.Find("UIManager").GetComponent<AnalysisPage>();
    }

    // Update is called once per frame
    void Update()
    {
        ComparisonState();
    }

    void ComparisonState()
    {
        comparisonCanvas.SetActive(isCompared);
        analysisLeftContainer.SetActive(!isCompared);
    }

    void ToggleClose()
    {
        analysisPage.ToggleCompare();
    }
}
