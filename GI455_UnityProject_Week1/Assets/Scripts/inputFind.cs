using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class inputFind : MonoBehaviour
{
    public string input;
    public Text text;
    
    inputData Data;

    public void ReadAndInput(string s)
    {
        //check user input
        input = s;
        print(input);
    }
   
    public void FindButton()
    {
        //get inputData script
        Data = GameObject.FindObjectOfType<inputData>();
        
        //check user input with data board
        for(int i = 0; i < Data.data.Length; i++)
         {
             if(Data.data[i] == input)
             {
                 text.text = "[ " + "<color=green>" + Data.data[i] + "</color>" + " ] is found.";
                 break;
             }
             else
             {
                 text.text = "[ " + "<color=red>" + input + "</color>" + " ] is not found.";
             }
         }
    }

}
