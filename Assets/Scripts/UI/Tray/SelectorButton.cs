using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/**
 * Generic button class for all selectable content in the SelectorMenu
 */
public class SelectorButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI text;
    public Button button;
    public int indexValue;
    public Image icon;
    public GameObject loadingSpinner;
    public Transform children;
    public Image background;

    public void Populate(string label, int index, Sprite sprite)
    {
        ShowBackground(false);
        SetLoading(false);
        text.text = label;
        indexValue = index;

        if (sprite != null)
        {
            icon.sprite = sprite;
        }

        Vector2 vals = text.GetPreferredValues();
        if (vals.y < 64f) vals.y = 64f;
        gameObject.GetComponent<RectTransform>().sizeDelta = vals;
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
        while (!condition()) yield return new WaitForSeconds(0.2f);
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

    public void ShowBackground(bool show)
    {
        if (background)
        {
            background.enabled = show;
        }
    }
}
