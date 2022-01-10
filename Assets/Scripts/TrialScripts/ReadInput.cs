using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ReadInput : MonoBehaviour
{
    public TextMeshProUGUI input;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ReadStringInput(string message)
    {
        input.text = message;
        Debug.Log(input);
    }

    public bool CheckString(string message)
    {
        if (input.text == message)
        {
            return true; 
        }
        else 
        {
            return false;
        }
    }
    
}
