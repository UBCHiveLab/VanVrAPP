using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/**
 * Implements the hold-to-enter button on Landing view.
 */
public class StartButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Action buttonAction;

    public Image fillImage;
    public float totalTouchTime = 1f;

    private float currentTouchTime;
    private bool touching;
    
    public void bindAction(Action action)
    {
        buttonAction = action;
    }

    public void OnCompleteTouch()
    {
        buttonAction();

    }

    void Update()
    {
        if (Input.GetButton("Fire1") && touching)
        {
            currentTouchTime += Time.deltaTime;
            if (currentTouchTime >= totalTouchTime)
            {
                buttonAction();
            }
        }
        else
        {
            currentTouchTime = Mathf.Max(0, currentTouchTime - Time.deltaTime);
        }

        fillImage.fillAmount = currentTouchTime / totalTouchTime;
    }



    public void OnPointerEnter(PointerEventData eventData)
    {
        touching = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        touching = false;
    }
}
