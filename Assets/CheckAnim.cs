using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckAnim : MonoBehaviour
{
    public Animator animator; 
    public TrayPage trayPage; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    // public IEnumerator CheckAnimationCompleted(string CurrentAnim)
    // {
    //     while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
    //     yield return null; 
    //     trayPage.SetAnalyzeOn(); 
    //     Debug.Log("completed"); 
    // }
}
