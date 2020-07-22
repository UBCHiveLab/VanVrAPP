using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnnotationSelector : MonoBehaviour
{
    [Header("Internal Structure")]
    public TextMeshProUGUI label;
    public Button left;
    public Button right;
    public GameObject dropup;
    public Transform dropupContent;
    public Button self;

    public DropUpAnnotationEntry dropUpEntryPrefab;
    public List<DropUpAnnotationEntry> currentDropUpEntries;

    [Header("External Structure")]
    public AnnotationDisplay display;

    void Start()
    {
        UpdateIndex();
        left.onClick.AddListener(() =>
        {
            ChangeAnnotation(-1);
        });
        right.onClick.AddListener(() =>
        {
            ChangeAnnotation(1);
        });

        self.onClick.AddListener(() =>
        {
            if (display.currentSpecimenData.annotations.Count > 0)
            {
                dropup.SetActive(!dropup.activeSelf);
            }
        });
    }

    void OnEnable()
    {
        UpdateIndex();
        PopulateDropUp();
    }

    /**
     * Checks what the display's current index is and sets its UI accordingly
     */
    public void UpdateIndex()
    {
        if (display.activeIndicators.Count == 0) {
            label.text = "No annotations found.";
            left.gameObject.SetActive(false);
            right.gameObject.SetActive(false);
            return;
        }

        int idx = display.selectedSpecimenIndex;
        if (idx == -1)
        {
            label.text = "Select an annotation.";
            left.gameObject.SetActive(true);
            right.gameObject.SetActive(true);
            
        }
        else
        {
            label.text = idx + ". " + display.activeIndicators[idx].data.title;
        }

        for (int i = 0; i < currentDropUpEntries.Count; i++)
        {
            currentDropUpEntries[i].SetSelected(i == idx);
        }
    }


    /**
     * Asks display to change its current annotation
     */
    public void ChangeAnnotation(int delta)
    {
        display.IncrementAnnotationIndex(delta);
        UpdateIndex();
    }


    public void SelectAnnotation(int index)
    {
        display.SelectAnnotationIndex(index);
        UpdateIndex();
    }

    public void PopulateDropUp()
    {
        currentDropUpEntries = new List<DropUpAnnotationEntry>();
        foreach (Transform child in dropupContent)
        {
            Destroy(child.gameObject);
        }

        if (display.currentSpecimenData == null || display.currentSpecimenData.annotations.Count == 0)
        {
            dropup.SetActive(false);
            return;
        }

        for (int i = 0; i < display.currentSpecimenData.annotations.Count; i++)
        {
            DropUpAnnotationEntry entry = Instantiate(dropUpEntryPrefab, dropupContent);
            entry.Populate(this, i, display.currentSpecimenData.annotations[i]);
            currentDropUpEntries.Add(entry);
        }
    }

}
