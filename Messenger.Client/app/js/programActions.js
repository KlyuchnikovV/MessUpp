// Функция показа всплывающих сообщений. //
function PopUp(message, type, isAutoClose)
{
    var popUp = document.getElementById("popUp");
    var popUpMessage = document.getElementById("popUpMessage");
    popUpMessage.innerHTML = message;
    switch(type)
    {
        case 0:
        {
            popUp.setAttribute("style",
                "background:#808080;z-index:99;visibility:visible;color:#0f7702; height:40px; " +
                "width:50vw; position:absolute; left:25vw;padding-top:20px; top:-60px; " +
                "-moz-border-radius: 10px; -webkit-border-radius: 9px; text-align:center");
            break;
        }
        case 1:
        {
            popUp.setAttribute("style",
                "background:#808080;z-index:99;visibility:visible;color:#a30101; height:40px; " +
                "width:50vw; position:absolute; left:25vw;padding-top:20px; top:-60px; " +
                "-moz-border-radius: 10px; -webkit-border-radius: 9px; text-align:center");
            break;
        }
    }
    $("div.popUp").animate({top:'0px'},500);
    var img = document.createElement("img");
    img.setAttribute("src", "./img/close.png");
    img.setAttribute("height", "25px");
    img.setAttribute("width", "25px");
    img.setAttribute("vspace", "5px");
    img.setAttribute("hspace", "5px");
    img.setAttribute("style", "position:absolute; right:0px; top:0px");
    img.setAttribute("onclick", "ClosePopUp()");
    img.setAttribute("class", "divButton");
    popUp.appendChild(img);
    if(isAutoClose)
    {
        setTimeout( function()
        {
            $("div.popUp").animate({top:'-60px'},500);
            setTimeout( function()
            {
                popUp.setAttribute("style",
                    "background:#808080;z-index:99;visibility:visible;height:40px; " +
                    "width:50vw; position:absolute; left:25vw;padding-top:20px; top:-60px; " +
                    "-moz-border-radius: 10px; -webkit-border-radius: 9px; text-align:center");
            }, 500);
        }, 5000);
    }
}

// Функция закрытия всплывающего сообщения. //
function ClosePopUp()
{
    $("div.popUp").animate({top:'-60px'},500);
}