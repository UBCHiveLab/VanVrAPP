using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToolTip : MonoBehaviour
{
    private float tipDelay = 0.2f;
    private float activeMax = 2f;
    public GameObject tip;
    public Animator anim;
    public bool left;

    private EventTrigger trigger;

    private bool _entered;
    private float _time = 0f;
    private float _activeTime;

    // Start is called before the first frame update
    void Start()
    {
        tip.SetActive(false);

        trigger = transform.parent.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = transform.parent.gameObject.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry enter = new EventTrigger.Entry();
        enter.eventID = EventTriggerType.PointerEnter;
        enter.callback.AddListener(HandleEnter);
        trigger.triggers.Add(enter);

        EventTrigger.Entry click = new EventTrigger.Entry();
        click.eventID = EventTriggerType.PointerClick;
        click.callback.AddListener(HandleExit);
        trigger.triggers.Add(click);

        EventTrigger.Entry exit = new EventTrigger.Entry();
        exit.eventID = EventTriggerType.PointerExit;
        exit.callback.AddListener(HandleExit);
        trigger.triggers.Add(exit);

    }


    void HandleEnter(BaseEventData data)
    {
        _entered = true;
    }

    void HandleExit(BaseEventData data)
    {
        _entered = false;

        Deactivate();
    }

    // Update is called once per frame
    void Update()
    {
        if (_entered)
        {
            _time += Time.deltaTime;
            if (_time > tipDelay)
            {
                Activate();
            }
        } else if (tip.activeSelf)
        {
            _activeTime += Time.deltaTime;
            if (_activeTime > activeMax)
            {
                Deactivate();
            }
        }
    }

    void Activate()
    {
        _entered = false;
        anim.SetBool("Left", left);
        anim.SetBool("Active", true);
    }

    void Deactivate()
    {
        _time = 0f;
        _activeTime = 0f;
        anim.SetBool("Active", false);
    }
}
