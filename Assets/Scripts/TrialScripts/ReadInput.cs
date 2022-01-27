using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI; 
public class ReadInput : MonoBehaviour
{
    private string message;
    public GameObject inputField; 
    public Button button; 
    // Start is called before the first frame update
    void Start()
    {
        message = inputField.GetComponent<TextMeshProUGUI>().text;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // public void ReadStringInput(string message)
    // {
    //     input.text = message;
    //     Debug.Log(input);
    // }

    public void CheckString()
    {
        button.onClick.AddListener(() => ReturnString()); 
        
    }
    public string ReturnString()
    {
        return message; 
        Debug.Log(message); 
    }
    
}
