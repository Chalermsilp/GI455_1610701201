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
        public GameObject RootLogin;
        public GameObject RootRegister;
        public GameObject RootLobby;
        public GameObject RootMessager;
        public GameObject createRoom;
        public GameObject joinRoom;
        public GameObject RootError;
        public GameObject backLobbyButton;

        private string inputUsername;
        public string userNameFromServer;
        private string allMessage;

        public Text RoomNameTextCreate;
        public Text RoomNameTextJoin;
        public Text RoomNameText;
        public Text UserIDText;
        public Text PasswordText;
        public Text RePasswordText;
        public Text NameText;
        public Text UserIDLoginText;
        public Text PasswordLoginText;
        public Text messageBox;
        public Text sendText;
        public Text receiveText;


        private string tempMessageString;

        public delegate void DelegateHandle(SocketEvent result);
        public DelegateHandle OnCreateRoom;
        public DelegateHandle OnJoinRoom;
        public DelegateHandle OnLeaveRoom;
        public DelegateHandle OnRegister;
        public DelegateHandle OnLogin;
        public DelegateHandle OnSendMessage;

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
            RootLogin.SetActive(true);
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
        
        public void SendMessage()
        {
            allMessage = userNameFromServer + "#" + messageBox.text;

            SocketEvent socketEvent = new SocketEvent("SendMessage", allMessage);

            string toJsonStr = JsonUtility.ToJson(socketEvent);

            ws.Send(toJsonStr);
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
                else if (receiveMessageData.eventName == "Register")
                {
                    if (OnRegister != null)
                        OnRegister(receiveMessageData);
                    if (receiveMessageData.data == "fail")
                    {
                        print("Error");
                        RootError.SetActive(true);
                    }
                    else if (receiveMessageData.data == "success")
                    {
                        RootRegister.SetActive(false);
                        RootLogin.SetActive(true);
                    }
                }
                else if (receiveMessageData.eventName == "Login")
                {
                    if (OnLogin != null)
                        OnLogin(receiveMessageData);
                    if (receiveMessageData.data == "fail")
                    {
                        print("Error");
                        RootError.SetActive(true);
                    }
                    else
                    {
                        userNameFromServer = receiveMessageData.data;
                        RootLogin.SetActive(false);
                        RootLobby.SetActive(true);
                    }
                }
                else if (receiveMessageData.eventName == "SendMessage")
                {
                    if (OnSendMessage != null)
                        OnSendMessage(receiveMessageData);
                    if (receiveMessageData.data == "fail")
                    {
                        print("Error");
                    }
                    else
                    {
                        if (tempMessageString != "")
                        {
                            if (receiveMessageData.data == userNameFromServer + " : " + messageBox.text)
                            {
                                sendText.text += "\n" + receiveMessageData.data;
                                receiveText.text += "\n";
                            }
                            else if (receiveMessageData.data != userNameFromServer + " : " + messageBox.text)
                            {
                                receiveText.text += "\n" + receiveMessageData.data;
                                sendText.text += "\n";
                            }
                        }
                    }
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

        public void toRegister()
        {
            RootLogin.SetActive(false);
            RootRegister.SetActive(true);
        }

        public void registerButton()
        {
            if(PasswordText.text == RePasswordText.text)
            {
                string dataRegister = UserIDText.text + "#" + PasswordText.text + "#" + NameText.text;

                SocketEvent socketEvent = new SocketEvent("Register", dataRegister);

                string toJsonStr = JsonUtility.ToJson(socketEvent);

                ws.Send(toJsonStr);
            }
            else
            {
                RootError.SetActive(true);
            }
        }

        public void loginButton()
        {
            string dataLogin = UserIDLoginText.text + "#" + PasswordLoginText.text;

            SocketEvent socketEvent = new SocketEvent("Login", dataLogin);

            string toJsonStr = JsonUtility.ToJson(socketEvent);

            ws.Send(toJsonStr);
        }

    }
}


