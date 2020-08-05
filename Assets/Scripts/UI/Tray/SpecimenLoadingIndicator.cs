using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpecimenLoadingIndicator : MonoBehaviour
{
    public TextMeshProUGUI label;
    public SpecimenStore store;


    void Start()
    {
        if (store == null)
        {
            Debug.LogWarning("No store for specimen indicator");
            return;
        }


    }

    void Update()
    {
        if (!store.Loading()) {
            Deactivate();
            return;
        }

        string txt = store.GetStatus();
        if (txt == "")
        {
            Deactivate();
        }
        else
        {
            label.text = txt;
        }
    }


    void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
