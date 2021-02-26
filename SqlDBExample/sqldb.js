const sqlite3 = require('sqlite3').verbose();

let db = new sqlite3.Database('./database/chatDB.db', sqlite3.OPEN_CREATE | sqlite3.OPEN_READWRITE, (err)=>{
    if(err) throw err;

    console.log('Connected to database.');

    var dataFromClient = 
    {
        eventName:"Login",
        data:"test03#100"
    }
    
    var spliteStr = dataFromClient.data.split('#');

    var userID = spliteStr[0];
    var addedMoney = parseInt(spliteStr[1]);
    //var password = spliteStr[1];
    //var name = spliteStr[2];

    //var sqlSelect = "SELECT * FROM UserData WHERE UserID ='"+userID+"' AND Password ='"+password+"'";//login
    //var sqlInsert = "INSERT INTO UserData (UserID, Password, Name, Money) VALUES('"+userID+"', '"+password+"', '"+name+"', '0')";//Register
    //var sqlUpdate = "UPDATE UserData SET Money ='200' WHERE UserID ='"+userID+"'";//Update

    db.all("SELECT Money FROM UserData WHERE UserID = '"+userID+"'", (err, rows)=>{
        if(err)
        {
            var callbackMsg = 
            {
                eventName:"AddMoney",
                data:"fail"
            }
            var toJsonStr = JSON.stringify(callbackMsg);
            console.log(toJsonStr);
        }
        else
        {
            console.log(rows);
            if(rows.length >0)
            {
                var currentMoney = rows[0].Money;
                currentMoney += addedMoney;
                
                db.all("UPDATE UserData SET Money = '"+currentMoney+"' WHERE UserID = '"+userID+"'", (err, rows)=>
                {
                    if(err)
                    {
                        var callbackMsg = 
                        {
                            eventName:"AddMoney",
                            data:"fail"
                        }
                        var toJsonStr = JSON.stringify(callbackMsg);
                        console.log(toJsonStr);                        
                    }
                    else
                    {
                        var callbackMsg = 
                        {
                            eventName:"AddMoney",
                            data:currentMoney.toString()
                        }
                        var toJsonStr = JSON.stringify(callbackMsg);
                        console.log(toJsonStr);
                    }
                });
            }
            else
            {
                var callbackMsg = 
                {
                    eventName:"AddMoney",
                    data:currentMoney
                }
                var toJsonStr = JSON.stringify(callbackMsg);
                console.log(toJsonStr);
            }
        }
    });

    /*db.all(sqlInsert, (err, rows)=>{
        if(err)
        {
            var callbackMsg = 
                {
                    eventName:"Register",
                    data:"fail"
                }
                var toJsonStr = JSON.stringify(callbackMsg);
                console.log(toJsonStr);
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
        }
    });*/
    
    /*db.all(sqlSelect, (err, rows)=>{
        if(err)
        {
            console.log(err);
        }
        else
        {
            if(rows.length > 0)
            {
                console.log(rows);
                var callbackMsg = 
                {
                    eventName:"Login",
                    data:rows[0].Name
                }

                var toJsonStr = JSON.stringify(callbackMsg);
                console.log(toJsonStr);
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
            }
            
        }
    });*/

});