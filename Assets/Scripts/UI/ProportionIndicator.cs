using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Controller;
using UnityEngine;
using UnityEngine.UI;

public class ProportionIndicator : MonoBehaviour
{
    [Header("Proportion Indicator")]

    public StateController stateController;
    public GameObject proportionIndicatorCanvas;
    public GameObject proportionIndicatorBody;
    public Button backB;
    // Start is called before the first frame update
    void Start()
    {
        Button backButton = backB.GetComponent<Button>();
        backButton.onClick.AddListener(ResetProportionIndicator);
    }

    // PROPORTION INDICATOR

    public void HighlightProportionIndicator() {
        // Find specimen / body part that was selected in analyze view
        // Find the same part on the proportion
        for (int i = 0; i < proportionIndicatorBody.transform.childCount - 1; i++) 
        {
            if (proportionIndicatorBody.transform.GetChild(i).transform.name == stateController.CurrentSpecimenData.organ)
            {
            proportionIndicatorBody.transform.GetChild(i).gameObject.SetActive(true);
            print(stateController.CurrentSpecimenData.organ + "Added to proportion indicator");
            } 

            if(stateController.CompareSpecimenData != null)
            {
                if(proportionIndicatorBody.transform.GetChild(i).transform.name == stateController.CompareSpecimenData.organ)
                {
                    proportionIndicatorBody.transform.GetChild(i).gameObject.SetActive(true);
                    print(stateController.CompareSpecimenData.organ + "Added to proportion indicator");
                }
            } 
        }
    }

    public void ResetProportionIndicator(){
        // Hide
         for (int i = 0; i < proportionIndicatorBody.transform.childCount - 1; i++) 
        {
            if (proportionIndicatorBody.transform.GetChild(i).transform.name == stateController.CurrentSpecimenData.organ){
            proportionIndicatorBody.transform.GetChild(i).gameObject.SetActive(false);
            } 

            if(stateController.CompareSpecimenData != null)
            {
                if(proportionIndicatorBody.transform.GetChild(i).transform.name == stateController.CompareSpecimenData.organ)
                {
                    proportionIndicatorBody.transform.GetChild(i).gameObject.SetActive(false);
                    print(stateController.CompareSpecimenData.organ + "Added to proportion indicator");
                }
            } 
        }
    }
}
