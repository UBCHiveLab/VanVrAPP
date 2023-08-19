using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/**
 * UI functionality for the disclaimer/policies panel that appears before entrance.
 */
public class DisclaimerPanel : MonoBehaviour
{
    [Header("Internal Structure")]
    public UITwoStateIndicator understoodIndicator;
    public HoverButton understoodHover;
    public Image understoodIcon;
    public TextMeshProUGUI understoodLabel;
    public UITwoStateIndicator enterIndicator;
    public GameObject arrowFrame;
    public Button enterButton;
    public GameObject warning;
    public SchoolSelectionController schoolSelectionController;

    [Header("Actions")]
    public Action enterAction;
    public SpecimenStore specimenStore;
    public bool understood;

    void Start()
    {
        enterButton.onClick.AddListener(TryEnter);
    }

    void OnEnable()
    {
        warning.SetActive(false);
    }

    public void SetUnderstood(bool value)
    {
        warning.SetActive(false);
        understood = value;

        understoodHover.StopAllCoroutines();
        understoodHover.enabled = !understood;
        enterIndicator.UpdateState(understood);
        understoodIndicator.UpdateState(understood);

        if (value)
        {
            understoodIcon.color = Color.white;
            understoodLabel.color = understoodHover.baseColor;
        }
        else
        {
            understoodIcon.color = understoodHover.hoverColor;
            understoodLabel.color = understoodHover.hoverColor;
        }

        arrowFrame.SetActive(understood);
    }

    public void TryEnter()
    {
        if (!understood)
        {
            warning.SetActive(true);
        }
        else
        {
            if (schoolSelectionController.selectedSchool.Length > 0)
            {

                Debug.Log(schoolSelectionController.selectedSchool);
                //StartCoroutine(specimenStore.LoadData(schoolSelectionController.selectedSchool));
                specimenStore.Start();
            }
            else
            {
                //StartCoroutine(specimenStore.LoadData(""));
            }

            enterAction();    
        }
        enterButton.OnDeselect(null);
    }

    public void OnCompleteFade()
    {
        gameObject.SetActive(false);
    }
}
