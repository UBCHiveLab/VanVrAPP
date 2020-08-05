﻿using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

/**
 * For displaying depth-of-field effects on specimens. (Currently not used).
 */
public class FocusDistanceFinder : MonoBehaviour
{
    Ray raycast;
    RaycastHit hit;
    //bool isHit;
    float hitDistance;

    public PostProcessVolume volume;
    DepthOfField depthOfField;

    private void Start()
    {
        volume.profile.TryGetSettings(out depthOfField);
    }
    void Update()
    {
        raycast = new Ray(transform.position, transform.forward * 100);

        //isHit = false;

        int layer_mask = LayerMask.GetMask("Specimens");

        if (Physics.Raycast(raycast, out hit, 100f, layer_mask))
        {
            //isHit = true;
            hitDistance = Vector3.Distance(transform.position, hit.point);
        }
        else
        {
            if (hitDistance < 100f)
            {
                hitDistance++;
            }
        }
        SetFocus();
    }

    void SetFocus() 
    {
        depthOfField.focusDistance.value = hitDistance;
    }
}
