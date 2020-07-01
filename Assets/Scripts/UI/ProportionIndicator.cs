using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Controller;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class ProportionIndicator : MonoBehaviour
{
    [Header("Proportion Indicator")]

    public StateController stateController;
    public GameObject proportionIndicatorBody;

    void OnEnable()
    {
        HighlightProportionIndicator();
    }

    void OnDisable()
    {
        ResetProportionIndicator();
    }

    public void HighlightProportionIndicator() {
        // Find specimen / body part that was selected in analyze view
        // Find the same part on the proportion

        if (stateController.CurrentSpecimenData == null) return;
        for (int i = 0; i < proportionIndicatorBody.transform.childCount - 1; i++) 
        {
            if (proportionIndicatorBody.transform.GetChild(i).transform.name == stateController.CurrentSpecimenData.organ)
            {
                proportionIndicatorBody.transform.GetChild(i).gameObject.SetActive(true);
                //print(stateController.CurrentSpecimenData.organ + "Added to proportion indicator");
            }
            else
            {
                proportionIndicatorBody.transform.GetChild(i).gameObject.SetActive(false);
            }

            if(stateController.CompareSpecimenData != null)
            {
                if(proportionIndicatorBody.transform.GetChild(i).transform.name == stateController.CompareSpecimenData.organ)
                {
                    proportionIndicatorBody.transform.GetChild(i).gameObject.SetActive(true);
                    //print(stateController.CompareSpecimenData.organ + "Added to proportion indicator");
                }
            } 
        }
    }

    public void ResetProportionIndicator(){
        // Hide
        if (stateController.CompareSpecimenData == null) return;
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
                    //print(stateController.CompareSpecimenData.organ + "Added to proportion indicator");
                }
            } 
        }
    }
}
