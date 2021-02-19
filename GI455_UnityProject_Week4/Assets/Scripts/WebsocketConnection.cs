using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using System;
using UnityEngine.UI;

namespace ChatWebSocket
{
    public class WebsocketConnection : MonoBehaviour
    {
        public struct SocketEvent
        {
            public string eventName;
            public string data;

            public SocketEvent(string eventName, string data)
            {
                this.eventName = eventName;
                this.data = data;
            }
        }
        
        private WebSocket ws;

        public GameObject RootConnection;
        public GameObject RootLobby;
        public GameObject RootMessager;
        public GameObject createRoom;
        public GameObject joinRoom;
        public GameObject RootError;
        public GameObject backLobbyButton;

        private string inputUsername;
        public Text RoomNameTextCreate;
        public Text RoomNameTextJoin;
        public Text RoomNameText;


        private string tempMessageString;

        public delegate void DelegateHandle(SocketEvent result);
        public DelegateHandle OnCreateRoom;
        public DelegateHandle OnJoinRoom;
        public DelegateHandle OnLeaveRoom;
        
        private void Update()
        {
            UpdateNotifyMessage();
        }

        public void Connect()
        {
            string url = "ws://127.0.0.1:5500/";

            ws = new WebSocket(url);

            ws.OnMessage += OnMessage;

            ws.Connect();

            RootConnection.SetActive(false);
            RootLobby.SetActive(true);
        }

        public void UsernameInput(string s)
        {
            inputUsername = s;
        }

        public void CreateButton()
        {
            RootLobby.SetActive(false);
            createRoom.SetActive(true);
            backLobbyButton.SetActive(true);
        }

        public void JoinButton()
        {
            RootLobby.SetActive(false);
            joinRoom.SetActive(true);
            backLobbyButton.SetActive(true);
        }

        public void CreateRoom(string roomName)
        {
            roomName = RoomNameTextCreate.text;

            SocketEvent socketEvent = new SocketEvent("CreateRoom", roomName);

            string toJsonStr = JsonUtility.ToJson(socketEvent);

            ws.Send(toJsonStr);

            //RootMessager.SetActive(true);
            //createRoom.SetActive(false);
            RoomNameText.text = "Room : " + roomName;

        }

        public void JoinRoom(string roomName)
        {
            roomName = RoomNameTextJoin.text;

            SocketEvent socketEvent = new SocketEvent("JoinRoom", roomName);

            string toJsonStr = JsonUtility.ToJson(socketEvent);

            ws.Send(toJsonStr);

            //RootMessager.SetActive(true);
            //joinRoom.SetActive(false);
            RoomNameText.text = "Room : " + roomName;

        }

        public void LeaveRoom()
        {
            SocketEvent socketEvent = new SocketEvent("LeaveRoom", "");

            string toJsonStr = JsonUtility.ToJson(socketEvent);

            ws.Send(toJsonStr);

            RootMessager.SetActive(false);
            RootLobby.SetActive(true);
        }

        public void Disconnect()
        {
            if (ws != null)
                ws.Close();
        }
        
        public void SendMessage(string message)
        {
            
        }

        private void OnDestroy()
        {
            Disconnect();
        }

        private void UpdateNotifyMessage()
        {
            if (string.IsNullOrEmpty(tempMessageString) == false)
            {
                SocketEvent receiveMessageData = JsonUtility.FromJson<SocketEvent>(tempMessageString);

                if (receiveMessageData.eventName == "CreateRoom")
                {
                    if (OnCreateRoom != null)
                        OnCreateRoom(receiveMessageData);
                    if (receiveMessageData.data == "fail")
                    {
                        print("Error");
                        RootError.SetActive(true);
                    }
                    else
                    {
                        RootMessager.SetActive(true);
                        createRoom.SetActive(false);
                    }
                }
                else if (receiveMessageData.eventName == "JoinRoom")
                {
                    if (OnJoinRoom != null)
                        OnJoinRoom(receiveMessageData);
                    if (receiveMessageData.data == "fail")
                    {
                        print("Error");
                        RootError.SetActive(true);
                    }
                    else
                    {
                        RootMessager.SetActive(true);
                        joinRoom.SetActive(false);
                    }
                }
                else if(receiveMessageData.eventName == "LeaveRoom")
                {
                    if (OnLeaveRoom != null)
                        OnLeaveRoom(receiveMessageData);
                }

                tempMessageString = "";
            }
        }

        private void OnMessage(object sender, MessageEventArgs messageEventArgs)
        {
            Debug.Log(messageEventArgs.Data);

            tempMessageString = messageEventArgs.Data;
        }

        public void BackErrorButton()
        {
            RootError.SetActive(false);
        }

        public void BackLobbyButton()
        {
            createRoom.SetActive(false);
            joinRoom.SetActive(false);
            RootLobby.SetActive(true);
            backLobbyButton.SetActive(false);
        }
    }
}


