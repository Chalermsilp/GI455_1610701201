using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JsonFormat;

public class JsonFormat : MonoBehaviour
{
    class MessageJsonData
    {
        public string username;
        public string message;
        public string color;

    }
    class CharacterStatus
    {
        public float hp;

    }

    // Start is called before the first frame update
    void Start()
    {
        /*string jsonStr = "{\"username\":\"noob\", \"message\":\"hi\", \"color\":\"blue\"}"

        MessageJsonData messageJsonData = JsonUtility.fromJson<MessageJsonData>(jsonStr);

        Debug.log(messageJsonData.username);*/
        
        CharacterStatus myCharacterStatus = new CharacterStatus();
        myCharacterStatus.hp = 100;
        string toJsonStr = JsonUtility.ToJson();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
