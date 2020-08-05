using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/**
 * Binds simple text/icon color animations to PointerEnter/Exit events
 */
public class HoverButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float transitionTime = 0.25f;
    public Color baseColor;
    public Color hoverColor;
    public Color disabledColor;
    public bool disabled;

    public Image icon;
    public Image icon2;
    public TextMeshProUGUI label;
    public bool hoverBoldText;

    private float current;



    void Start()
    {
        if (transitionTime <= 0)
        {
            transitionTime = 0.1f;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (disabled) return;
        StopAllCoroutines();
        StartCoroutine(HoverOn());

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (disabled) return;
        StopAllCoroutines();
        StartCoroutine(HoverOff());
    }

    public void Disable()
    {
        disabled = true;
        if (label != null)
        {
            label.color = disabledColor;
        }

        if (icon != null) {
            icon.color = disabledColor;
        }

        if (icon2 != null) {
            icon2.color = disabledColor;
        }
    }

    public void Enable()
    {
        disabled = false;
        if (label != null) {
            label.color = baseColor;
        }

        if (icon != null) {
            icon.color = baseColor;
        }

        if (icon2 != null) {
            icon2.color = baseColor;
        }
    }



    private IEnumerator HoverOn()
    {
        while (current < 1f)
        {
            current += Time.deltaTime / transitionTime;
            Color color = Color.Lerp(baseColor, hoverColor, current);

            if (label != null) {
                label.color = color;
                if (hoverBoldText)
                {
                    label.fontStyle = FontStyles.Bold;
                }
            }

            if (icon != null) {
                icon.color = color;
            }

            if (icon2 != null)
            {
                icon2.color = color;
            }

            yield return null;
        }

        current = 1f;
    }

    private IEnumerator HoverOff()
    {
        while (current > 0f) {
            current -= Time.deltaTime / transitionTime;
            Color color = Color.Lerp(baseColor, hoverColor, current);

            if (label != null) {
                label.color = color;
                if (hoverBoldText) {
                    label.fontStyle = FontStyles.Normal;
                }
            }

            if (icon != null) {
                icon.color = color;
            }

            if (icon2 != null) {
                icon2.color = color;
            }

            yield return null;
        }

        current = 0f;
    }
}
