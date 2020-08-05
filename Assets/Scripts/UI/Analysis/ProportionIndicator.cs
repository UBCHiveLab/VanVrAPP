using Assets.Scripts.State;
using UnityEngine;

/**
 * Manages the UI element proportion indicator, highlighting organs based on selected specimen types.
 */
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
            if (proportionIndicatorBody.transform.GetChild(i).transform.name == stateController.CurrentSpecimenData.organ.ToLower())
            {
                proportionIndicatorBody.transform.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                proportionIndicatorBody.transform.GetChild(i).gameObject.SetActive(false);
            }

            if(stateController.CompareSpecimenData != null)
            {
                if(proportionIndicatorBody.transform.GetChild(i).transform.name == stateController.CompareSpecimenData.organ.ToLower())
                {
                    proportionIndicatorBody.transform.GetChild(i).gameObject.SetActive(true);
                }
            } 
        }
    }

    public void ResetProportionIndicator(){
        // Hide
        if (stateController.CompareSpecimenData == null) return;
        for (int i = 0; i < proportionIndicatorBody.transform.childCount - 1; i++) 
        {
            if (proportionIndicatorBody.transform.GetChild(i).transform.name == stateController.CurrentSpecimenData.organ.ToLower()) {
                proportionIndicatorBody.transform.GetChild(i).gameObject.SetActive(false);
            } 

            if(stateController.CompareSpecimenData != null)
            {
                if(proportionIndicatorBody.transform.GetChild(i).transform.name == stateController.CompareSpecimenData.organ.ToLower())
                {
                    proportionIndicatorBody.transform.GetChild(i).gameObject.SetActive(false);
                }
            } 
        }
    }
}
