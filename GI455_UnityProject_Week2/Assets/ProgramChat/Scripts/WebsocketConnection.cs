using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace ProgramChat
{
    public class WebsocketConnection : MonoBehaviour
    {
        private WebSocket websocket;

        private string localHost;
        private string portID;
        private string userInput;

        public Text textBox;

        public GameObject ChatRoom;


        private void OnDestroy()
        {
            if(websocket != null)
            {
                websocket.Close();
            }
        }

        public void OnMessage(object sender, MessageEventArgs MessageEventArgs)
        {
            Debug.Log("Receive msg : " + MessageEventArgs.Data);
            textBox.text += MessageEventArgs.Data + "\n";
        }

        public void LocalHostInput(string s)
        {
            localHost = s;
        }
        public void PortInput(string s)
        {
            portID = s;
        }

        public void ConnectChatButton()
        {
            if(localHost == "127.0.0.1" && portID == "5500")
            {
                websocket = new WebSocket("ws://127.0.0.1:5500/");

                ChatRoom.SetActive(true);

                websocket.OnMessage += OnMessage;

                 websocket.Connect();
                 //websocket.Send("I'm coming here.");
            }
        }

        public void InputUser(string s)
        {
            userInput = s;
        }

        public void SendButton()
        {
             websocket.Send(userInput);
        }
    }
}
