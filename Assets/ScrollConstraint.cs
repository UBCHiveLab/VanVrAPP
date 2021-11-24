using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollConstraint : MonoBehaviour
{
    public ScrollRect myScrollRect;
    public Scrollbar newScrollBar;

    // Start is called before the first frame update
    void Start()
    {
        myScrollRect.verticalNormalizedPosition = 1.5f;
    }

    // Update is called once per frame
    void Update()
    {
      
    }
}
