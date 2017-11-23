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

function getBase64Image(img, width, height)
{
	var canvas = document.createElement("canvas");
	canvas.width = img.width;
	canvas.height = img.height;
	var ctx = canvas.getContext("2d");
	ctx.drawImage(img, 0, 0, width, height);
	var dataURL = canvas.toDataURL("image/png");
	return dataURL.replace(/^data:image\/(png|jpg);base64,/, "");
}

function DisableDiv(div)
{
    var loader = document.createElement("div");
    loader.setAttribute("class", "loader");
    loader.setAttribute("id", "loader");
    var div2 = document.createElement("div");
    div2.setAttribute("id", "divLoader");
    div2.setAttribute("style", "position:absolute; top:0; left:0; width:200px; height:100vh; visibility:visible; opacity: 0.7; background-color:#808080");
    loader.setAttribute("style", "position:absolute; top:calc(50vh - 25px); left:75px; height:50px; width:50px; visibility:visible");
    div2.appendChild(loader);
    div.appendChild(div2);
}

function EnableDiv(div)
{
    document.getElementById("loader").remove();
    document.getElementById("divLoader").remove();
}
