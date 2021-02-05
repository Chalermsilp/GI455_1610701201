const { CONNECTING } = require('ws');
var websocket = require('ws');

var callbackInitServer = ()=>{
    console.log("Server is running.");
}

var wss = new websocket.Server({port:5500}, callbackInitServer);

var wsList = [];

wss.on("connection", (ws)=>{

    wsList.push(ws);
    User(ws);

    ws.on("message", (data)=>{
        for(var i = 0; i < wsList.length; i++)
        {
            if(wsList[i] == ws)
            {
                wsList[i].send(data);
                console.log("send from " + i + " " + data);
            }
            if(wsList[i] != ws)
            {
                wsList[i].send(data + "                             ");
            }
        }
    });

    ws.on("close", ()=>{
                console.log("client" + i + " disconnected.");
                wsList = ArrayRemove(wsList, ws);
    });
});

function ArrayRemove(arr, value)
{
    return arr.fillter((element)=>{
        return element != value;
    });
}

//function Boardcast(data)
//{
    //for(var i = 0; i < wsList.length; i++)
    //{
        //wsList[i].send(data);
    //}
//}

function User(ws)
{
    for(var i = 0; i < wsList.length; i++)
    {
        if(wsList[i] == ws)
        {
            console.log(i + " connected.")
            break;
        }
    }
}