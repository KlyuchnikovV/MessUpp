var $=require('jquery');
function OnLoad() 
{
    var paramValue = window.location.href.split("?")[1].split("=")[1];
    document.getElementById("profileId").value = paramValue;
    GetChats();
}

function OnRedirect(whereTo)
{
    document.location = whereTo + "?id=" + document.getElementById("profileId").value;
}

function ResetPanels(element)
{
    if(document.getElementById("chatsHidden").value != "false" && element != document.getElementById("chatsHidden"))
    {
        $("div.chatsPanel").animate({left:0},500);
        document.getElementById("chatsHidden").value = "false";
    }
    if(document.getElementById("createHidden").value != "false" && element != document.getElementById("createHidden"))
    {
        $("div.createPanel").animate({left:0},500);
        document.getElementById("createHidden").value = "false";
    }
    if(document.getElementById("findHidden").value != "false" && element != document.getElementById("findHidden"))
    {
        $("div.findPanel").animate({left:0},500);
        document.getElementById("findHidden").value = "false";
    }
    if(document.getElementById("settingsHidden").value != "false" && element != document.getElementById("settingsHidden"))
    {
        $("div.settingsPanel").animate({left:0},500);
        document.getElementById("settingsHidden").value = "false";
    }
    if(document.getElementById("peopleHidden").value != "false" && element != document.getElementById("peopleHidden"))
    {
        $("div.peoplePanel").animate({left:0},500);
        document.getElementById("peopleHidden").value = "false";
    }
    if(document.getElementById("logoutHidden").value != "false" && element != document.getElementById("logoutHidden"))
    {
        $("div.logoutPanel").animate({left:0},500);
        document.getElementById("logoutHidden").value = "false";
    }
}

$(document).ready(function(){	
    $("div.divButtonChats").click(
        function()
        {
            ResetPanels(document.getElementById("chatsHidden"));

            if(document.getElementById("chatsHidden").value == "false")
            {
                document.getElementById("chatsHidden").value = "true";
                $("div.chatsPanel").animate({left:'235px'},500);
                $('div.dialogPanel').css(
                    {
                        'width' : 'calc(100vw - 236px)'
                    }
                );
                $("div.dialogPanel").animate({left:'235px'},500);
            }
            else
            {
                $("div.chatsPanel").animate({left:-35},500);
                $("div.dialogPanel").animate({left:35},500);
                $('div.dialogPanel').css(
                    {
                        'width' : 'calc(100vw - 36px)',
                    }
                );
                document.getElementById("chatsHidden").value = "false";
            }
        }
    );

    $("div.divButtonCreate").click(
        function()
        {
            ResetPanels(document.getElementById("createHidden"));

            if(document.getElementById("createHidden").value == "false")
            {
                document.getElementById("createHidden").value = "true";
                $("div.createPanel").animate({left:'200px'},500);
                $('div.dialogPanel').css(
                    {
                        'width' : 'calc(100vw - 236px)'
                    }
                );
                $("div.dialogPanel").animate({left:'235px'},500);
            }
            else
            {
                $("div.createPanel").animate({left:0},500);
                $("div.dialogPanel").animate({left:35},500);
                $('div.dialogPanel').css(
                    {
                        'width' : 'calc(100vw - 36px)',
                    }
                );
                document.getElementById("createHidden").value = "false";
            }
        }
    );

    $("div.divButtonFind").click(
        function()
        {
            ResetPanels(document.getElementById("findHidden"));

            if(document.getElementById("findHidden").value == "false")
            {
                document.getElementById("findHidden").value = "true";
                $("div.findPanel").animate({left:'200px'},500);
                $('div.dialogPanel').css(
                    {
                        'width' : 'calc(100vw - 236px)'
                    }
                );
                $("div.dialogPanel").animate({left:'235px'},500);
            }
            else
            {
                $("div.findPanel").animate({left:0},500);
                $("div.dialogPanel").animate({left:35},500);
                $('div.dialogPanel').css(
                    {
                        'width' : 'calc(100vw - 36px)',
                    }
                );
                document.getElementById("findHidden").value = "false";
            }
        }
    );

    $("div.divButtonSettings").click(
        function()
        {
            ResetPanels(document.getElementById("settingsHidden"));

            if(document.getElementById("settingsHidden").value == "false")
            {
                document.getElementById("settingsHidden").value = "true";
                $("div.settingsPanel").animate({left:'200px'},500);
                $('div.dialogPanel').css(
                    {
                        'width' : 'calc(100vw - 236px)',

                    }
                );
                $("div.dialogPanel").animate({left:'235px'},500);
            }
            else
            {
                $("div.settingsPanel").animate({left:0},500);
                $("div.dialogPanel").animate({left:35},500);
                $('div.dialogPanel').css(
                    {
                        'width' : 'calc(100vw - 36px)',
                    }
                );
                document.getElementById("settingsHidden").value = "false";
            }
        }
    );

    $("div.divButtonLogout").click(
        function()
        {
            ResetPanels(document.getElementById("logoutHidden"));

            if(document.getElementById("logoutHidden").value == "false")
            {
                document.getElementById("logoutHidden").value = "true";
                $("div.logoutPanel").animate({left:'200px'},500);
                $('div.dialogPanel').css(
                    {
                        'width' : 'calc(100vw - 236px)',

                    }
                );
                $("div.dialogPanel").animate({left:'235px'},500);
            }
            else
            {
                $("div.logoutPanel").animate({left:0},500);
                $("div.dialogPanel").animate({left:35},500);
                $('div.dialogPanel').css(
                    {
                        'width' : 'calc(100vw - 36px)',
                    }
                );
                document.getElementById("logoutHidden").value = "false";
            }
        }
    );

    $("div.divButtonPeople").click(
        function()
        {
            ResetPanels(document.getElementById("peopleHidden"));

            if(document.getElementById("peopleHidden").value == "false")
            {
                document.getElementById("peopleHidden").value = "true";
                $("div.peoplePanel").animate({left:'200px'},500);
                $('div.dialogPanel').css(
                    {
                        'width' : 'calc(100vw - 236px)',

                    }
                );
                $("div.dialogPanel").animate({left:'235px'},500);
            }
            else
            {
                $("div.peoplePanel").animate({left:0},500);
                $("div.dialogPanel").animate({left:35},500);
                $('div.dialogPanel').css(
                    {
                        'width' : 'calc(100vw - 36px)',
                    }
                );
                document.getElementById("peopleHidden").value = "false";
            }
        }
    );

    var id = document.getElementById("profileId").value;
    var profile = GetProfile(id);
    //PopUp("Добро пожаловать, Admin!", 0, true);
    PopUp("Добро пожаловать, " + profile.Surname + " " + profile.Name + "!", 0, true);

});	

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
                "z-index:99;visibility:visible;color:#FFFFFF; height:40px; width:50vw; position:absolute; left:25vw;padding-top:20px; top:-60px; background-color:#53472A;-moz-border-radius: 10px; -webkit-border-radius: 9px; text-align:center");
            break;
        }
        case 1:
        {
            popUp.setAttribute("style", 
                "z-index:99;visibility:visible;color:#A60000; height:40px; width:50vw; position:absolute; left:25vw;padding-top:20px; top:-60px; background-color:#53472A;-moz-border-radius: 10px; -webkit-border-radius: 9px; text-align:center");
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
                    "color:#FFFFFF; height:40px; width:50vw; position:absolute;padding-top:20px; left:25vw;-moz-border-radius: 10px; -webkit-border-radius: 9px; top:-60px;background-color:#53472A;");
            }, 500);
        }, 5000);
    }
}

function ClosePopUp()
{
    $("div.popUp").animate({top:'-60px'},500);
}