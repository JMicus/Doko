"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/dokoHub").build();

connection.on("PlayerNo", function (event) {

});

connection.start()
    .then(function () {
        console.info("connection started");
    })
    .catch(function (error) {
        console.error(error.message);
        alert(error.message);
    });

connection.on("Initialized", function (gameName, playerNo, playerToken) {

    window.location.href = 'game?gameName=' + gameName + '&playerNo=' + playerNo + '&playerToken=' + playerToken;
});

document.getElementById("loginButton").addEventListener("click", function (event) {

    //// show game

    //document.getElementById("gameDiv").style.visibility = 'visible';

    var gameName = document.getElementById("gameNameInput").value;
    var playerNo = document.getElementById("playerNoSelect").value;
    var playerName = document.getElementById("playerNameInput").value;

    //document.getElementById("headLabel").innerHTML = gameName + " - " + playerName + " (SpielerIn " + playerNo + ")";

    //// remove login
    //var loginDiv = document.getElementById("loginDiv");
    //loginDiv.parentElement.removeChild(loginDiv);

    //// say hello
    connection.invoke("Init", gameName, playerNo, playerName);

    

});