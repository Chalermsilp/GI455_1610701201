const { CONNECTING } = require('ws');
const sqlite3 = require('sqlite3').verbose();
var websocket = require('ws');
   
var callbackInitServer = ()=>
{
    console.log("server is running.");
}

var wss = new websocket.Server({port:5500} , callbackInitServer)

let db = new sqlite3.Database('./database/chatDB.db', sqlite3.OPEN_CREATE | sqlite3.OPEN_READWRITE, (err)=>{
    if(err) throw err;

    console.log('Connected to database.')

});


var roomList = [];
/*
{
    roomName: ""
    wsList: []
}
*/
var wsList = [];

wss.on("connection", (ws)=>{
    
    //Lobby
    console.log("client connected.");
    //Reception
    ws.on("message", (data)=>{
        console.log("send from client :" + data);

                //========== Convert jsonStr into jsonObj =======

        //toJsonObj = JSON.parse(data);

        // I change to line below for prevent confusion
        var toJsonObj = { 
            roomName:"",
            data:""
        }
        toJsonObj = JSON.parse(data);

        //===============================================

        if(toJsonObj.eventName == "Login")
        {
        var splitStr = toJsonObj.data.split('#');
        var userID = splitStr[0];
        var password = splitStr[1];
        var sqlSelect = "SELECT * FROM UserData WHERE UserID='"+userID+"' AND Password='"+password+"'";
            db.all(sqlSelect,(err,rows)=>
            {
                if(err)
                {
                    console.log(err);
                }
                else
                {
                    if(rows.length >0)
                    {
                        var callbackMsg =
                        {
                            eventName:"Login",
                            data:rows[0].Name
                        }
                        var toJsonStr = JSON.stringify(callbackMsg);
                        console.log(toJsonStr);
                        ws.send(toJsonStr);
                    }
                    else
                    {
                        var callbackMsg =
                        {
                            eventName:"Login",
                            data:"fail"
                        }
                        var toJsonStr = JSON.stringify(callbackMsg);
                        console.log(toJsonStr);
                        ws.send(toJsonStr);
                    }
                }
            });
        }

        else if(toJsonObj.eventName == "Register")
        {
        var splitStr = toJsonObj.data.split('#');
        var userID = splitStr[0];
        var password = splitStr[1];
        var name = splitStr[2];
        var sqlInsert = "INSERT INTO UserData (UserID,Password,Name) VALUES ('"+userID+"' , '"+password+"' , '"+name+"')";
            db.all(sqlInsert,(err,rows)=>
            {
                if(err)
                {
                    console.log("[0]" + err);
                    var callbackMsg =
                        {
                            eventName:"Register",
                            data:"fail"
                        }
                        var toJsonStr = JSON.stringify(callbackMsg);
                        console.log(toJsonStr);
                        ws.send(toJsonStr);
                }
                else
                {
                    var callbackMsg =
                        {
                            eventName:"Register",
                            data:"success"
                        }
                        var toJsonStr = JSON.stringify(callbackMsg);
                        console.log(toJsonStr);
                        ws.send(toJsonStr);
                }
            });
        }

        else if(toJsonObj.eventName == "SendMessage")
        {
            var selectionRoomIndex = -1;

            for(var i = 0; i < roomList.length; i++)
            {
                for(var j = 0; j < roomList[i].wsList.length; j++)
                {
                    if(ws == roomList[i].wsList[j])
                    {
                        selectionRoomIndex = i;
                        break;
                    }
                }
            }

            for(var i = 0; i < roomList[selectionRoomIndex].wsList.length; i++)
            {
                var splitStr = toJsonObj.data.split('#');
                var username = splitStr[0];
                var data = splitStr[1];

                var callbackMsg = {
                    eventName:"SendMessage",
                    data:username + " : " + data
                }
                /*var toJsonStr = JSON.stringify(callbackMsg);
                ws.send(toJsonStr);*/

                roomList[selectionRoomIndex].wsList[i].send(JSON.stringify(callbackMsg));
            }
        }
    
    
        //===============================================

        if(toJsonObj.eventName == "CreateRoom")//CreateRoom
        {
            //============= Find room with roomName from Client =========
            var isFoundRoom = false;
            for(var i = 0; i < roomList.length; i++)
            {
                if(roomList[i].roomName == toJsonObj.data)
                {
                    isFoundRoom = true;
                    break;
                }
            }
            //===========================================================

            if(isFoundRoom == true)// Found room
            {
                //Can't create room because roomName is exist.
                //========== Send callback message to Client ============

                //ws.send("CreateRoomFail"); 

                //I will change to json string like a client side. Please see below
                var callbackMsg = {
                    eventName:"CreateRoom",
                    data:"fail"
                }
                var toJsonStr = JSON.stringify(callbackMsg);
                ws.send(toJsonStr);
                //=======================================================

                console.log("client create room fail.");
            }
            else
            {
                //============ Create room and Add to roomList ==========
                var newRoom = {
                    roomName: toJsonObj.data,
                    wsList: []
                }

                newRoom.wsList.push(ws);

                roomList.push(newRoom);
                //=======================================================

                //========== Send callback message to Client ============

                //ws.send("CreateRoomSuccess");

                //I need to send roomName into client too. I will change to json string like a client side. Please see below
                var callbackMsg = {
                    eventName:"CreateRoom",
                    data:toJsonObj.data
                }
                var toJsonStr = JSON.stringify(callbackMsg);
                ws.send(toJsonStr);
                //=======================================================
                console.log("client create room success.");
            }

            //console.log("client request CreateRoom ["+toJsonObj.data+"]");
            
        }
        else if(toJsonObj.eventName == "JoinRoom")//JoinRoom
        {
            //============= Home work ================
            // Implementation JoinRoom event when have request from client.
            
            //================= Hint =================
            for(var i=0;i<roomList.length;i++)
            {
                if(toJsonObj.data == roomList[i].roomName) //found
                {
                    roomList[i].wsList.push(ws);
                    var callbackMsg = {
                        eventName:"JoinRoom",
                        data:toJsonObj.data
                    }
                    var toJsonStr = JSON.stringify(callbackMsg);
                    ws.send(toJsonStr);
                    console.log("Join Success");
                    break;
                }
                else if(i == roomList.length-1)
                {                         
                    var callbackMsg = {
                        eventName:"JoinRoom",
                        data:"fail"
                    }                                      
                    var toJsonStr = JSON.stringify(callbackMsg);
                    ws.send(toJsonStr);
                    console.log("Join Failed");
                }
            }
            console.log("client request JoinRoom");
            //========================================
        }
        else if(toJsonObj.eventName == "LeaveRoom")//LeaveRoom
        {
            //============ Find client in room for remove client out of room ================
            var isLeaveSuccess = false;//Set false to default.
            for(var i = 0; i < roomList.length; i++)//Loop in roomList
            {
                for(var j = 0; j < roomList[i].wsList.length; j++)//Loop in wsList in roomList
                {
                    if(ws == roomList[i].wsList[j])//If founded client.
                    {
                        roomList[i].wsList.splice(j, 1);//Remove at index one time. When found client.

                        if(roomList[i].wsList.length <= 0)//If no one left in room remove this room now.
                        {
                            roomList.splice(i, 1);//Remove at index one time. When room is no one left.                        
                        }
                        isLeaveSuccess = true;
                        break;
                    }
                }
            }
            
            //===============================================================================

            if(isLeaveSuccess)
            {
                //========== Send callback message to Client ============

                //ws.send("LeaveRoomSuccess");

                //I will change to json string like a client side. Please see below
                var callbackMsg = {
                    eventName:"LeaveRoom",
                    data:"success"
                }
                var toJsonStr = JSON.stringify(callbackMsg);
                ws.send(toJsonStr);
                //=======================================================

                console.log("leave room success");
            }
            else
            {
                //========== Send callback message to Client ============

                //ws.send("LeaveRoomFail");

                //I will change to json string like a client side. Please see below
                var callbackMsg = {
                    eventName:"LeaveRoom",
                    data:"fail"
                }
                var toJsonStr = JSON.stringify(callbackMsg);
                ws.send(toJsonStr);
                //=======================================================

                console.log("leave room fail");
            }
        }
    });


    /*wsList.push(ws);
    
    ws.on("message", (data)=>{
        console.log("send from client :"+ data);
        Boardcast(data);
    });
    */
    ws.on("close", ()=>{
        console.log("client disconnected.");

        //============ Find client in room for remove client out of room ================
        for(var i = 0; i < roomList.length; i++)//Loop in roomList
        {
            for(var j = 0; j < roomList[i].wsList.length; j++)//Loop in wsList in roomList
            {
                if(ws == roomList[i].wsList[j])//If founded client.
                {
                    roomList[i].wsList.splice(j, 1);//Remove at index one time. When found client.

                    if(roomList[i].wsList.length <= 0)//If no one left in room remove this room now.
                    {
                        roomList.splice(i, 1);//Remove at index one time. When room is no one left.
                    }
                    break;
                }
            }
        }
        //===============================================================================
    });
});

function Boardcast(data)
{
    /*for(var i = 0; i < wsList.length; i++)
    {
        wsList[i].send(data);
    }*/
}