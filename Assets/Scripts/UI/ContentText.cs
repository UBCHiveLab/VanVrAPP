﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ContentText : MonoBehaviour, IAnnotationContentBlock
{
    public TextMeshProUGUI tmp;

    public void Populate(string text, AnnotationDetailPanel panel)
    {
        tmp.text = text;
    }
}
