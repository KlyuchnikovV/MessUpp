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
            if(isAutoClose)
            {
                setTimeout( function()
                {
                    $("div.popUp").animate({top:'-60px'},500);
                    setTimeout( function()
                    {
                        popUp.setAttribute("style",
                            "background:#808080;z-index:99;visibility:visible;height:40px; " +
                            "width:50vw; position:absolute; left:25vw;padding-top:20px; top:-61px; " +
                            "-moz-border-radius: 10px; -webkit-border-radius: 9px; text-align:center;color:#0f7702");
                    }, 500);
                }, 5000);
            }
            break;
        }
        case 1:
        {
            popUp.setAttribute("style",
                "background:#808080;z-index:99;visibility:visible;color:#a30101; height:40px; " +
                "width:50vw; position:absolute; left:25vw;padding-top:20px; top:-60px; " +
                "-moz-border-radius: 10px; -webkit-border-radius: 9px; text-align:center");
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
                            "-moz-border-radius: 10px; -webkit-border-radius: 9px; text-align:center;color:#a30101");
                    }, 500);
                }, 5000);
            }
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

function DisableDiv(div, width, widthType, height, heightType)
{
    var loader = document.createElement("div");
    loader.setAttribute("class", "loader");
    loader.setAttribute("id", "loader");

    var div2 = document.createElement("div");
    div2.setAttribute("id", "divLoader");

    div2.setAttribute("style", "position:absolute; top:0; left:0; width:" + width + widthType + "; height:" + height + heightType + "; visibility:visible; opacity: 0.5; background-color:#555555");
    loader.setAttribute("style", "position:absolute; top:calc(" + (height/2) + heightType + " - 15px); left:calc(" + (width / 2) + widthType + " - 15px); height:30px; width:30px; visibility:visible");

    div2.appendChild(loader);
    div.appendChild(div2);
}

function EnableDiv(div)
{
    document.getElementById("loader").remove();
    document.getElementById("divLoader").remove();
}

function NameValidate(input)
{
    var value = input.value;
    var rep = /^[a-zA-Zа-яА-Я]+$/;
    if(!rep.test(value))
    {
        input.setAttribute("style", "border:1px solid #CD853F;color:red;-moz-border-radius: 10px;-webkit-border-radius: 9px;padding-left: 5px;font-weight: bold;");
        document.getElementById("registerPassed").value = false;
    }
    else
    {
        document.getElementById("registerPassed").value = true;
        input.setAttribute("style", "border:1px solid #CD853F;color:green;-moz-border-radius: 10px;-webkit-border-radius: 9px;padding-left: 5px;font-weight: bold;");

    }
}

function SurnameValidate(input)
{
    var value = input.value;
    var rep = /^[a-zA-Zа-яА-Я]+$/;
    if(!rep.test(value))
    {
        //input.value = "";
        input.setAttribute("style", "border:1px solid #CD853F;color:red;-moz-border-radius: 10px;-webkit-border-radius: 9px;padding-left: 5px;font-weight: bold;");
        document.getElementById("registerPassed").value = false;
    }
    else
    {
        document.getElementById("registerPassed").value = true;
        input.setAttribute("style", "border:1px solid #CD853F;color:green;-moz-border-radius: 10px;-webkit-border-radius: 9px;padding-left: 5px;font-weight: bold;");

    }
}

function LoginValidate(input)
{
    var value = input.value;
    var rep = /^[a-zA-Z0-9]+$/;
    if(!rep.test(value))
    {
        //input.value = "";
        input.setAttribute("style", "border:1px solid #CD853F;color:red;-moz-border-radius: 10px;-webkit-border-radius: 9px;padding-left: 5px;font-weight: bold;");
        document.getElementById("registerPassed").value = false;
    }
    else
    {
        document.getElementById("registerPassed").value = true;
        input.setAttribute("style", "border:1px solid #CD853F;color:green;-moz-border-radius: 10px;-webkit-border-radius: 9px;padding-left: 5px;font-weight: bold;");

    }
}

function PassValidate(input)
{
    var value = input.value;
    var rep = /^[a-zA-Z0-9]+$/;
    if(!rep.test(value))
    {
        //input.value = "";
        input.setAttribute("style", "border:1px solid #CD853F;color:red;-moz-border-radius: 10px;-webkit-border-radius: 9px;padding-left: 5px;font-weight: bold;");
        document.getElementById("registerPassed").value = false;
    }
    else
    {
        document.getElementById("registerPassed").value = true;
        input.setAttribute("style", "border:1px solid #CD853F;color:green;-moz-border-radius: 10px;-webkit-border-radius: 9px;padding-left: 5px;font-weight: bold;");

    }
}

function ValidateAll()
{
    NameValidate(document.getElementById("txtName"));
    if(document.getElementById("registerPassed").value == 'true')
    {
        SurnameValidate(document.getElementById("txtSurname"));
    }

    if(document.getElementById("registerPassed").value == 'true')
    {
        LoginValidate(document.getElementById("txtLogin"));
    }

    if(document.getElementById("registerPassed").value == 'true')
    {
        PassValidate(document.getElementById("txtPassword"));
    }
}

function ValidateLoginPass()
{
    var input = document.getElementById("txtLog");
    var value = input.value;
    var rep = /^[a-zA-Z0-9]+$/;
    if(!rep.test(value))
    {
        //input.value = "";
        input.setAttribute("style", "border:1px solid #CD853F;color:red;-moz-border-radius: 10px;-webkit-border-radius: 9px;padding-left: 5px;font-weight: bold;");
        document.getElementById("loginPassed").value = false;
    }
    else
    {
        document.getElementById("loginPassed").value = true;
        input.setAttribute("style", "border:1px solid #CD853F;color:green;-moz-border-radius: 10px;-webkit-border-radius: 9px;padding-left: 5px;font-weight: bold;");

    }
    if(document.getElementById("loginPassed").value == 'true')
    {
        var pass = document.getElementById("txtPass");
        var value = pass.value;
        var rep = /^[a-zA-Z0-9]+$/;
        if(!rep.test(value))
        {
            //input.value = "";
            pass.setAttribute("style", "border:1px solid #CD853F;color:red;-moz-border-radius: 10px;-webkit-border-radius: 9px;padding-left: 5px;font-weight: bold;");
            document.getElementById("loginPassed").value = false;
        }
        else
        {
            document.getElementById("loginPassed").value = true;
            pass.setAttribute("style", "border:1px solid #CD853F;color:green;-moz-border-radius: 10px;-webkit-border-radius: 9px;padding-left: 5px;font-weight: bold;");
    
        }
    }
}