using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectorButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI text;
    public Button button;
    public int indexValue;
    public Image icon;
    public GameObject loadingSpinner;

    public void Populate(string label, int index, Sprite sprite)
    {

        SetLoading(false);
        text.text = label;
        indexValue = index;

        if (sprite != null)
        {
            icon.sprite = sprite;
        }
    }

    public void SetLoading(bool loading)
    {
        if (loadingSpinner != null)
        {
            loadingSpinner.gameObject.SetActive(loading);
        }
    }

    public void SetLoadingUntil(Func<bool> condition)
    {
        StartCoroutine(LoadingUntil(condition));
    }

    private IEnumerator LoadingUntil(Func<bool> condition)
    {
        SetLoading(true);
        while (!condition()) yield return null;
        SetLoading(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        button.OnSelect(null);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        button.OnDeselect(null);
    }
}
