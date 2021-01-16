"use strict";

var DEBUG = false;

var url = "http://janisdoppelkopf.azurewebsites.net/";
if (DEBUG) {
    var url = "http://localhost:5000/";
}

var data = document.getElementById("data");

var gameName = data.getAttribute("data-gameName");
var playerNo = data.getAttribute("data-playerNo");
var playerToken = data.getAttribute("data-playerToken");

var layoutDict = {
    "test": "test"
}

document.getElementById("headLabel").innerHTML = gameName + " - (Spieler*in " + playerNo + ")";

var connection = new signalR.HubConnectionBuilder().withUrl("/dokoHub").withAutomaticReconnect().build();

connection.onreconnecting((error) => {
    log("Reconnect");
    connection.invoke("SayHello", gameName, playerNo);
});

connection.on("Unauthorized", function (gameName, playerNo, playerName) {
    log("Unauthorized");
    alert("Spieler " + playerNo + " (" + playerName + ") ist bereits im Spiel " + gameName + " angemeldet.");
    window.location.href = 'login';
});

connection.on("Info", function (msg) {
    log("Info: " + msg);
    //alert(msg);
    info(msg);
});


connection.on("PlayerJoined", function (no, name) {
    log("PLayerJoined " + name + "(" + no + ")");
    var panel = document.getElementById("playerName" + playerPosition(no) + "Panel");
    panel.setAttribute("data-playerNo", no);
    panel.innerHTML = name;
});

connection.on("Hand", function (cards) {
    log("Hand");
    try {
        var handDiv = document.getElementById("handDiv");
        handDiv.innerHTML = "";

        //var width = handDiv.getBoundingClientRect().width;

        

        var cardArray = cards.split(".");

        //var margin = 0;

        //var no = 15;

        //if (no * 96 > width) {
        //    margin = parseInt((width - (no * 96)) / (no - 1));
        //}


        for (var i = 0; i < cardArray.length; i++)
        {
            var card = cardArray[i];
            var img = document.createElement("img");

            //if (i == cardArray.length - 1) {
            //    margin = 0;
            //}

            img.src = cardLink(card);
            img.className = "lift";
            //img.style = "cursor: pointer; margin-right: " + margin + "px";
            img.style = "cursor: pointer; position: absolute; left: " + ((i) * 100 / (cardArray.length)) + "%";
            setCardImgLayout(img);
            img.setAttribute("data-cardCode", card);
            img.draggable = true;
            img.addEventListener("click", function (event) {
                connection.invoke("PutCard", gameName, playerNo, event.target.getAttribute("data-cardCode"));
                //img.parentElement.removeChild(img);
            });

            img.onmouseover = function (event) {
                event.target.style.marginTop = "-15px";
            };
            img.onmouseout = function (event) {
                event.target.style.marginTop = "0px";
            };

            img.ondragstart = function (event) {
                //document.getElementById("tableCenterDiv").style.background = "rgba(150, 150, 150, 0.5)";
                for (var i = 2; i <= 4; i++) {
                    var namePanel = document.getElementById("playerName" + i + "Panel");
                    namePanel.style.background = "rgba(150, 150, 150, 0.7)";
                }
                event.dataTransfer.setData("card", event.target.getAttribute("data-cardCode"));
                event.dataTransfer.setData("player", playerNo);
            };
            img.ondragend = function (event) {
                //document.getElementById("tableCenterDiv").style.background = "transparent";
                for (var i = 2; i <= 4; i++) {
                    var namePanel = document.getElementById("playerName" + i + "Panel");
                    namePanel.style.background = "transparent";
                }
            }

            //width = img.width;

            handDiv.appendChild(img);
        }

        document.getElementById("handDiv").style.maxWidth = (layoutDict["cardWidth"] * (cardArray.length)) + "px";
        //document.getElementById("handDiv").style.marginRight = layoutDict["cardWidth"] + "px";
        //document.getElementById("handDivRight").style.height = layoutDict["cardHeight"] + "px";


    }
    catch (error) {
        alert(error.message);
    }
    //infoLabel.innerHTML = cards;
});

connection.on("Points", function (points) {
    log("Points");
    var tableCenterDiv = document.getElementById("tableCenterDiv");
    tableCenterDiv.innerHTML = "";

    points.split(".").forEach(entry => {

        var row = document.createElement("div");
        row.className = "row";

        var entryA = entry.split("-");
        row.innerHTML = '<div class="col" ><b>' + entryA[0] + ':</b></div> <div class="col"><b>' + entryA[1] + '</b></div>';

        tableCenterDiv.appendChild(row);
    });
});

connection.on("Trick", function (startPlayerNo, cards) {
    log("Trick");
    var cardLst = cards.split(".");

    for (var i = 1; i <= 4; i++) {
        var img = document.getElementById("trick" + playerPosition(i) + "Img");
        img.src = cardLink(cardLst[i - 1]);
        img.parentElement.style.zIndex = (i + 4 - startPlayerNo) % 4;
    }
});

connection.on("LastTrick", function (startPlayerNo, cards) {
    log("LastTrick");
    var cardLst = cards.split(".");

    for (var i = 1; i <= 4; i++) {
        document.getElementById("lastTrick" + playerPosition(i) + "Img").src = cardLink(cardLst[i - 1]);
    }
});

connection.on("ExternalPage", function (link) {
    log("ExternalPage");
    var externalPageDiv = document.getElementById("externalPageDiv");

    externalPageDiv.innerHTML = "";

    var page = document.createElement("object");
    page.type = "text/html";
    page.data = link;
    page.style = "width:100%;height:1500px;";

    externalPageDiv.appendChild(page);
});

connection.on("Symbols", function (symbols) {
    log("Symbols");
    var playerSymbols = ("." + symbols).split(".");
    for (var i = 1; i <= 4; i++) {
        var symbolsDiv = document.getElementById("player" + playerPosition(i) + "SymbolsDiv");
        symbolsDiv.innerHTML = "";
        playerSymbols[i].split("-").forEach(s => {
            var imgName = s.split("+")[0];
            var hint = s.split("+")[1];
            if (s.length > 0) {
                /*
                 * <a data-toggle="tooltip" title="Stich nehmen">
                    <img class="circularButton" id="takeTrickButton" src="~/Images/takeTrickButton.png" alt="Stich nehmen" />
                </a>
                */

                var hintA = document.createElement("a");
                hintA.setAttribute("data-toggle", "tooltip");
                hintA.setAttribute("title", hint);

                var img = document.createElement("img");
                img.src = "Images/" + imgName + ".png";
                img.style = "max-height: 30px;";
                hintA.appendChild(img);

                symbolsDiv.appendChild(hintA);
            }
        });

        var symbolsDict = {
            "fox": "Fuchs",
            "doppelkopf": "Doppelkopf",
            "charlie": "Charlie",
            "jens": "Jens",
            "hearth": "Herzstich",
            "clear": "LÖSCHEN"
        }
        var addSymbolDiv = document.createElement("div");
        addSymbolDiv.className = "popupMenu";
        addSymbolDiv.style = "display: inline";
        symbolsDiv.appendChild(addSymbolDiv);

        var addSymbolImg = document.createElement("img");
        addSymbolImg.src = "Images/plusButton.png";
        addSymbolImg.style = "max-height: 20px;";
        addSymbolDiv.appendChild(addSymbolImg);

        var addSymbolPopup = document.createElement("div");
        for (var key in symbolsDict) {
            var item = document.createElement("div");
            //item.style = "align: left";
            item.innerHTML = symbolsDict[key];

            item.className = "popupMenuItem";
            item.setAttribute("data-msg", i + ";" + key + ";" + symbolsDict[key]);

            item.addEventListener("click", function (event) {
                var data = event.target.getAttribute("data-msg").split(";");
                connection.invoke("AddSymbol", gameName, data[0], data[1], data[2]);
            });

            addSymbolPopup.appendChild(item);
        }
        addSymbolDiv.appendChild(addSymbolPopup);
        
    }
});

connection.on("DealQuestion", function () {
    log("DealQuestion");
    var retVal = confirm("Soll wirklich neu gegeben werden?");
    if (retVal == true) {
        connection.invoke("Deal", gameName, playerNo, true);
    }
});

connection.on("Messages", function (messages) {
    log("Messages");
    var playerMessages = ("###" + messages).split("###");
    for (var i = 1; i <= 4; i++) {
        var msgDiv = document.getElementById("player" + playerPosition(i) + "MsgDiv");
        msgDiv.innerHTML = "";
        playerMessages[i].split("---").forEach(s => {
            if (s.length > 0) {
                var p = document.createElement("p");
                p.className = "box";
                p.innerHTML = s;

                if (s.startsWith("<!--nobackground-->")) {
                    p.style.backgroundColor = "transparent";
                    p.style.border = "none";
                }

                msgDiv.appendChild(p);
            }
        });
    }
});

connection.on("Rules", function (rules) {
    log("Rules");
    rules.split(".").forEach(rule => {
        var pair = rule.split("-");
        document.getElementById("rules" + pair[0] + "Select").value = pair[1];
    });
});

connection.on("Center", function (cards) {
    log("Center");
    try {
        var tableCenterDiv = document.getElementById("tableCenterDiv");
        tableCenterDiv.innerHTML = "";

        var cardArray = cards.split(".")
                             .filter(function (el) {
                                 return el != ""
                             });

        for (var i = 0; i < cardArray.length; i++)
        {
            var card = cardArray[i];
            var img = document.createElement("img");

            img.src = cardLink();
            img.style = "cursor: pointer; margin-right: -75px; padding-left: -43px";
            img.setAttribute("data-cardCode", card);
            img.draggable = true;
            img.addEventListener("click", function (event) {
                alert("Ziehe die Karte auf deine Hand um sie zu nehmen.");
            });
            img.ondragstart = function (event) {
                var cardCode = event.target.getAttribute("data-cardCode");
                event.target.src = cardLink(cardCode);
                event.dataTransfer.setData("player", "center");
                event.dataTransfer.setData("card", cardCode);
            };
            img.ondragend = function (event) {
                event.target.src = cardLink();
            }
            tableCenterDiv.appendChild(img);
        }
    }
    catch (error) {
        alert(error.message);
    }
});

connection.on("Stat", function (statMsg) {
    log("Stat");
    try {
        var statDiv = document.getElementById("statDiv");
        statDiv.innerHTML = "";

        var index = 0;
        var newStatMsg = "";
        while (index < statMsg.length){
            if (statMsg.substring(index, index + 3) == "##C"){
                newStatMsg += "<img style=\"height: 70px; margin-right: -31px\" src =\"" + cardLink(statMsg.substring(index + 3, index + 6)) + "\"/>";
                index += 6;
            }
            else{
                newStatMsg += statMsg[index++];
            }
        }
        statMsg = newStatMsg;

        statMsg.split("##;").forEach(entry => {

            if (entry != "") {
                var row = document.createElement("div");
                row.className = "row";

                var entryA = entry.split("##,");
                row.innerHTML = '<div class="col" ><b>' + entryA[0] + '</b></div> <div class="col"><b>' + entryA[1] + '</b></div>';

                statDiv.appendChild(row);
            }
        });
    }
    catch (error) {
        alert(error.message);
    }
});

connection.on("Layout", function (layout) {
    log("Layout");

    layout.split(".").forEach(l => {
        var key = l.split(":")[0];
        var val = l.split(":")[1];

        layoutDict[key] = val;
    });

    document.body.style.backgroundImage = "Images/" + layoutDict["background"];

    document.getElementById("handDiv").style.height = layoutDict["cardHeight"] + "px";
    //document.getElementById("handDiv").style.paddingTop = "20px";
    

    for (var i = 1; i <= 4; i++) {
        //var margin = (110 - parseInt(layoutDict["cardHeight"])) + "px";

        var img = document.getElementById("trick" + playerPosition(i) + "Img");
        setCardImgLayout(img);
    }

    var img1 = document.getElementById("trick1Img");

    var space = 20;

    var w = img1.width;
    var h = img1.height;
    var hwgap = (h - w) / 2;

    var center = document.getElementById("tableCenterDiv").parentElement;
    center.style.width = (w * 3 + space + space) + "px";
    center.style.height = (h + h + space + space) + "px";

    for (var i = 1; i <= 4; i++) {
        var pp = i;
        var img = document.getElementById("trick" + i + "Img");

        // left
        if (pp == 2) {
            img.style.left = (-1.5 * w - space - hwgap) + "px";
        }
        else if (pp == 1 || pp == 3) {
            img.style.left = (-w / 2) + "px";
        }
        else if (pp == 4) {
            img.style.left = (w / 2 + space + hwgap) + "px";
        }

        // top
        if (pp == 3) {
            img.style.top = "0px";
        }
        else if (pp == 2 || pp == 4) {
            img.style.top = (h - w / 2 - hwgap + space) + "px";
        }
        else if (pp == 1) {
            img.style.top = (h + space + space) + "px";
        }

        var trickImg = document.getElementById("lastTrick" + i + "Img");
        setCardImgLayoutSized(trickImg, 0.5);
    }
});

connection.start()
    .then(function () {
        console.info("connection started");
        
        connection.invoke("SayHello", gameName, playerNo, playerToken);
    })
    .catch(function (error) {
        console.error(error.message);
        alert(error.message);
    });

for (var i = 1; i <= 4; i++) {
    var pos = playerPosition(i);
    if (pos != 1) {
        var namePanel = document.getElementById("playerName" + playerPosition(i) + "Panel");
        namePanel.ondragover = function (event) {
            event.preventDefault();
        };
        namePanel.ondrop = function (event) {
            event.preventDefault();
            if (event.dataTransfer.getData("player") != "center") {
                connection.invoke("GiveCardToPlayer", gameName, event.dataTransfer.getData("player"), event.dataTransfer.getData("card"), event.target.getAttribute("data-playerNo"))
            }
            //alert("Um Karten an einen anderen Spieler zu geben, lege die Karten zuerst verdeckt in die Mitte (per drag&drop). Sie können dann von einerm anderen Spieler aufgenommen werden.")
        }
    }
}

/*var tableCenterDiv = document.getElementById("tableCenterDiv");
tableCenterDiv.ondragover = function(event) {
    event.preventDefault();
};
tableCenterDiv.ondrop = function(event) {
    event.preventDefault();
    if (event.dataTransfer.getData("player") != "center") {
        connection.invoke("CardToCenter", gameName, playerNo, event.dataTransfer.getData("card"));
    }
}*/

var handDiv = document.getElementById("handDiv");
handDiv.ondragover = function(event) {
    event.preventDefault();
}
handDiv.ondrop = function(event) {
    event.preventDefault();
    if (event.dataTransfer.getData("player") == "center") {
        connection.invoke("CardFromCenter", gameName, playerNo, event.dataTransfer.getData("card"));
    }
};

document.getElementById("dealButton").addEventListener("click", function (event) {
    connection.invoke("Deal", gameName, playerNo, false);
});

document.getElementById("trick1Img").addEventListener("click", function (event) {
    connection.invoke("TakeCardBack", gameName, playerNo);
});

document.getElementById("takeTrickButton").addEventListener("click", function (event) {
    connection.invoke("TakeTrick", gameName, playerNo);
});

document.getElementById("externalPageButton").addEventListener("click", function (event) {
    connection.invoke("SetExternalPage", gameName, document.getElementById("externalPageInput").value);
});

document.getElementById("lastTrickBackButton").addEventListener("click", function (event) {
    connection.invoke("LastTrickBack", gameName);
});

// announce buttons
var announceDict = {
    "Frei": "",
    "Vorbehalt": "",
    "-": "",
    "Re!": "",
    "Kontra!": "",
    "Keine&nbsp;90!": "",
    "Keine&nbsp;60!": "",
    "Keine&nbsp;30!": "",
    "Schwarz!": ""
}
var announceDiv = document.getElementById("announceDiv");
for (var key in announceDict) {
    var item = document.createElement("div");

    item.innerHTML = key;
    var msg = announceDict[key];

    if (msg == "") {
        msg = key;
    }

    if (msg != "-") {
        item.className = "popupMenuItem";
        item.setAttribute("data-msg", msg);

        item.addEventListener("click", function (event) {
            connection.invoke("PlayerMsg", gameName, playerNo, "<b>" + event.target.getAttribute("data-msg") + "</b>");
        });
    }

    announceDiv.appendChild(item);
}

var msgButton = document.getElementById("msgButton");
msgButton.addEventListener("click", function (event) {
    connection.invoke("PlayerMsg", gameName, playerNo, document.getElementById("msgInput").value);
    document.getElementById("msgInput").value = "";
});

document.getElementById("msgInput").addEventListener("keypress", function (event) {
    if (event.keyCode == 13) {
        msgButton.click();
    }
});

// change card order
var orderDict = {
    "Normalspiel": "Regular",
    "Damensolo": "SoloD",
    "Bubensolo": "SoloB",
    "Kreuz-Solo": "SoloK",
    "Pik-Solo": "SoloP",
    "Herz-Solo": "SoloH",
    "Karo-Solo": "SoloC",
    "Fleischloser": "SoloF"
}
var soloDiv = document.getElementById("soloDiv");
for (var key in orderDict) {
   var item = document.createElement("div");
    item.className = "popupMenuItem";
    item.innerHTML = key;
    item.setAttribute("data-msg", orderDict[key]);

    item.addEventListener("click", function (event) {
        connection.invoke("ChangeCardOrder", gameName, event.target.getAttribute("data-msg"));
    });

    soloDiv.appendChild(item);
}


document.getElementById("rulesButton").addEventListener("click", function (event) {
    var rulesDiv = document.getElementById("rulesDiv");

    //if (rulesDiv.style.visibility == "visible") {
    //    rulesDiv.style.visibility = "hidden";
    //}
    //else {
        rulesDiv.style.visibility = "visible";
    //}
});

document.getElementById("closeRulesButton").addEventListener("click", function (event) {
    document.getElementById("rulesDiv").style.visibility = "hidden";

    var ruleCode = "";

    ["Pigs", "Nines"].forEach(rule => {
        ruleCode += rule + "-" + document.getElementById("rules" + rule + "Select").value + ".";
    });
    connection.invoke("SetRules", gameName, ruleCode);
});

// functions ///////////////////////
function playerPosition(no) {
    return (parseInt(no) - parseInt(playerNo) + 4) % 4 + 1;
}

function positionPlayer(no) {
    for (var i = 1; i <= 4; i++) {
        if (playerPosition(i) == parseInt(no)) {
            return i;
        }
    }
    return -1;
}

function cardLink(cardCode = "b") {
    var short = "c0";
    if (cardCode.length >= 3) {
        short = cardCode.substring(0, 3);
    }
    //info("load img: " + url + "Images/" + short + ".png")
    return "Images/Cards" + layoutDict["cardLayout"] + "/" + short + "." + layoutDict["cardImageType"];
}

function setCardImgLayout(img) {
    setCardImgLayoutSized(img, 1);
}

function setCardImgLayoutSized(img, size) {
    var h = parseInt(layoutDict["cardHeight"]) * size;

    img.style.height = h + "px";

    if (layoutDict["cardBorder"] == "true") {
        img.style.border = "1px solid #000";
        img.style.borderRadius = (h / 15) + "px";
    }
    else {
        img.style.border = "none";
        img.style.borderRadius = "0px";
    }
}

function info(msg) {
    if (false) {
        var li = document.createElement("li");
        li.textContent = msg;
        document.getElementById("infoList").appendChild(li);
    }
}

var lastLogDate = "";
function log(msg) {
    var d = new Date();
    var dateStr = d.getHours() + ":" + d.getMinutes() + ":" + twoDigits(d.getSeconds());
    if (lastLogDate != dateStr) {
        console.log("--- " + dateStr + "-----------------------");
        lastLogDate = dateStr;
    }
    console.log("    " + msg);
}

function twoDigits(x) {
    var y = x + "";
    if (y.length == 2) {
        return y;
    }
    return "0" + y;
}