using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class inputData : MonoBehaviour
{
    public Text text;

    public string[] data;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();

        //show data on board
        text.text = data[0] + "\n" + data[1] + "\n" + data[2] + "\n" + data[3] + "\n" + data[4];
    }
}
